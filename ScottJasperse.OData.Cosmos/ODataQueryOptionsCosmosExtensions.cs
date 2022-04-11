using System.Text;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Azure.Cosmos;
using Microsoft.OData.UriParser;

namespace ScottJasperse.OData.Cosmos
{
    public static class ODataQueryOptionsCosmosExtensions
    {
        public static (QueryDefinition Query, QueryRequestOptions RequestOptions) CreateCosmosQuery(this ODataQueryOptions query)
        {
            var cosmosQueryBuilder = new CosmosQueryBuilder();
            return cosmosQueryBuilder.CreateCosmosQuery(query);
        }

        public static FeedIterator<T> GetItemQueryIteratorFromOData<T>(this Container cosmosContainer, ODataQueryOptions query, string continuationToken)
        {
            var cosmosQueryBuilder = new CosmosQueryBuilder();
            var results = cosmosQueryBuilder.CreateCosmosQuery(query);

            return cosmosContainer.GetItemQueryIterator<T>(results.Query, continuationToken, results.RequestOptions);
        }
    }
}