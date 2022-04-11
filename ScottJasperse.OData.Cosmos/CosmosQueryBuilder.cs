using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Azure.Cosmos;
using Microsoft.OData.UriParser;

namespace ScottJasperse.OData.Cosmos
{
    class CosmosQueryBuilder
    {
        private bool ValidateFieldName(string fieldName)
            => true;

        public (QueryDefinition Query, QueryRequestOptions RequestOptions) CreateCosmosQuery(ODataQueryOptions query)
        {
            var queryBuilder = new StringBuilder();
            var queryParameters = new Dictionary<string, object>();
            Func<object, string> createQueryParameter = value => {
                var paramName = $"@p{queryParameters.Count() + 1}";
                queryParameters.Add(paramName, value);
                return paramName;
            };

            if (query.SelectExpand == null)
            {
                queryBuilder.Append("SELECT * ");
            }
            else
            {
                queryBuilder.Append(CreateSelect(query.SelectExpand));
            }

            queryBuilder.Append("FROM c ");

            if (query.Filter != null)
            {
                queryBuilder.Append(CreateWhere(query.Filter, createQueryParameter));
            }

            if (query.OrderBy != null)
            {
                queryBuilder.Append(CreateOrderBy(query.OrderBy));
            }

            var cosmosQuery = new QueryDefinition(queryBuilder.ToString().Trim());
            foreach (var p in queryParameters)
                cosmosQuery = cosmosQuery.WithParameter(p.Key, p.Value);

            var requestOptions = new QueryRequestOptions();
            if (query.Top != null)
            {
                requestOptions.MaxItemCount = query.Top.Value;
            }

            return (cosmosQuery, requestOptions);
        }

        private string CreateSelect(SelectExpandQueryOption selectOption)
        {
            if (selectOption.SelectExpandClause.AllSelected) return "SELECT * ";

            var fieldList = selectOption.SelectExpandClause.SelectedItems
                .Select(x => $"c.{GetFieldName(x)}")
                .ToArray();

            return $"SELECT {string.Join(", ", fieldList)} ";
        }

        private string CreateOrderBy(OrderByQueryOption orderByOption)
        {
            var nodesAsText = new List<string>();

            foreach (var node in orderByOption.OrderByNodes)
            {
                if (node is OrderByPropertyNode propNode) {
                    var nodeAsText = "c." + ToCamelCase(propNode.Property.Name);

                    if (node.Direction == OrderByDirection.Descending) {
                        nodeAsText += " DESC";
                    }
                    nodesAsText.Add(nodeAsText);
                }
                else
                {
                    throw new NotImplementedException($"Support for Order By node type '${typeof(OrderByPropertyNode).Name}' is not implmented");
                }
            }

            return $"ORDER BY {string.Join(", ", nodesAsText)} ";
        }

        private string GetFieldName(Microsoft.OData.UriParser.SelectItem item)
        {
            var asPathSelectItem = item as Microsoft.OData.UriParser.PathSelectItem;

            if (asPathSelectItem == null) throw new Exception("Failed to convert to PathSelectItem");

            var fieldName = asPathSelectItem.SelectedPath.FirstSegment.Identifier;

            return ToCamelCase(fieldName);
        }

        private string CreateWhere(FilterQueryOption filterOption, Func<object, string> createQueryParameter)
        {
            var filterVisitor = new FilterVisitor(
                createQueryParameter,
                ToCamelCase
            );

            filterOption.FilterClause.Expression.Accept(filterVisitor);

            return filterVisitor.ToString();
        }

        private string ToCamelCase(string source)
        {
            return source.Substring(0, 1).ToLower() + source.Substring(1);
        }
    }
}