using System.Text;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Azure.Cosmos;
using Microsoft.OData.UriParser;

namespace ScottJasperse.OData.Cosmos
{
    public static class ODataQueryOptionsCosmosExtensions
    {
        public static OneOf<QueryDefinition, BadRequestError> CreateCosmosQuery(this ODataQueryOptions query)
        {
            var cosmosQueryBuilder = new CosmosQueryBuilder();
            return cosmosQueryBuilder.CreateCosmosQuery(query);
        }
    }

    class CosmosQueryBuilder
    {
        private bool ValidateFieldName(string fieldName)
            => true;

        public OneOf<QueryDefinition, BadRequestError> CreateCosmosQuery(ODataQueryOptions query)
        {
            var queryBuilder = new StringBuilder();
            var queryParameters = new Dictionary<string, object>();
            Func<object, string> createQueryParameter = value => {
                var paramName = $"@{queryParameters.Count() + 1}";
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

            var cosmosQuery = new QueryDefinition(queryBuilder.ToString().Trim());
            foreach (var p in queryParameters)
                cosmosQuery = cosmosQuery.WithParameter(p.Key, p.Value);

            return cosmosQuery;
        }

        private string CreateSelect(SelectExpandQueryOption selectOption)
        {
            if (selectOption.SelectExpandClause.AllSelected) return "SELECT * ";

            var fieldList = selectOption.SelectExpandClause.SelectedItems.Select(GetFieldName).ToArray();

            return $"SELECT {string.Join(", ", fieldList)} ";
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
            // var strBldr = new StringBuilder();
            // var strWriter = new StringWriter(strBldr);
            // var visitor = new DiagnosticVisitor(strWriter);

            // filterOption.FilterClause.Expression.Accept(visitor);

            // Console.WriteLine($"Filter: \n{strBldr}");

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


    public record BadRequestError(IEnumerable<string> Messages);
}