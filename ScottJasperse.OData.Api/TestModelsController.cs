using Microsoft.AspNetCore.OData.Query;
using Microsoft.Azure.Cosmos;
using ScottJasperse.OData.Cosmos;

namespace ScottJasperse.OData.Api
{
    [Route("/[controller]")]
    public class TestModelsController : ControllerBase
    {
        private IQueryable<TestModel> models = Enumerable.Range(1, 10000)
                .Select(x => new TestModel() {
                    Id = x,
                    Name = $"Model #{x.ToString().PadLeft(4, '0')}",
                    Child = new ChildModel() { 
                        Text = $"Text {x}",
                        GrandChild = new GrandChildModel()
                        {
                            Text = $"Text {x}"
                        }
                    }
                }).ToArray().AsQueryable();

        [HttpGet]
        public IActionResult Get(ODataQueryOptions<TestModel> odataOptions)
        {
            var r = odataOptions.ApplyTo(models);

            var queryParts = odataOptions.CreateCosmosQuery();
            var query = queryParts.Query;
            var reqOpts = queryParts.RequestOptions;

            return this.Ok(new {
                values = r,
                queryText = query.QueryText,
                queryParameters = query.GetQueryParameters()
                    .ToDictionary(x => x.Name, x => x.Value),
                requestOptions = reqOpts
            });
        }

        [HttpGet("query")]
        public ActionResult<QueryDefinition> GetQuery(ODataQueryOptions<TestModel> odataOptions)
        {
            var queryParts = odataOptions.CreateCosmosQuery();
            var query = queryParts.Query;
            var reqOpts = queryParts.RequestOptions;

            return this.Ok(new {
                queryText = query.QueryText,
                queryParameters = query.GetQueryParameters()
                    .ToDictionary(x => x.Name, x => x.Value),
                requestOptions = reqOpts
            });
        }
    }

    public class TestModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public ChildModel? Child { get; set; }
    }

    public class ChildModel
    {
        public string? Text { get; set; }
        public GrandChildModel? GrandChild { get; set; }
    }

    public class GrandChildModel
    {
        public string? Text { get; set; }
    }
}
