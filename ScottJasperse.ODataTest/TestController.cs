using Microsoft.AspNetCore.OData.Query;
using Microsoft.Azure.Cosmos;
using ScottJasperse.OData.Cosmos;

namespace ScottJasperse.ODataTest;

[Route("/")]
public class TestController : ControllerBase
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

        var cosmosQueryResult = odataOptions.CreateCosmosQuery();
        return cosmosQueryResult.Match<IActionResult>(
            cosmosQuery => {
                return this.Ok(new {
                    values = r,
                    query = cosmosQuery.QueryText
                });
            },
            badRequest => {
                foreach (var message in badRequest.Messages)
                {
                    ModelState.AddModelError("request", message);
                }

                return this.BadRequest(ModelState);
            }
        );
    }

    [HttpGet("query")]
    public ActionResult<QueryDefinition> GetQuery(ODataQueryOptions<TestModel> odataOptions)
    {
        var queryResult = odataOptions.CreateCosmosQuery();
        if (queryResult.IsT1) return this.BadRequest(queryResult.AsT0);

        var query = queryResult.AsT0;

        return this.Ok(new {
            queryText = query.QueryText,
            parameters = query.GetQueryParameters()
                .ToDictionary(x => x.Name, x => x.Value)
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
