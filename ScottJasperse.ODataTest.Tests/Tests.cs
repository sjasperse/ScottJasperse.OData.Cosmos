using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using System.Net;
using Newtonsoft.Json.Linq;

namespace ScottJasperse.ODataTest.Tests;

public class Tests
{
    private readonly HttpClient _client;

    public Tests()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // ... Configure test services
            });

        _client = application.CreateClient();
    }

    private string UrlEncode(string value)
        => System.Net.WebUtility.UrlEncode(value);

    [Fact]
    public async Task NoParameters()
    {
        var response = await _client.GetAsync("/query");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await response.Content.ReadAsJsonAsync();
        data.Value<string>("queryText").Should().Be("SELECT * FROM c");
    }

    [Fact]
    public async Task FilterForNameEq()
    {
        var response = await _client.GetAsync($"/query?$filter=name eq 'test'");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await response.Content.ReadAsJsonAsync();
        data["queryText"]!.Value<string>().Should().Be("SELECT * FROM c WHERE c.name = @1");
        data["parameters"]!["@1"]!.Value<string>().Should().Be("test");
    }

    [Fact]
    public Task FilterForIdEq()
        => BaseFilterTest("$filter=id eq 1", "SELECT * FROM c WHERE c.id = @1", 1);

    [Fact]
    public Task FilterForIdGt()
        => BaseFilterTest("$filter=id gt 1", "SELECT * FROM c WHERE c.id > @1", 1);

    [Fact]
    public Task FilterForIdGte()
        => BaseFilterTest("$filter=id ge 1", "SELECT * FROM c WHERE c.id >= @1", 1);

    [Fact]
    public Task FilterForIdLt()
        => BaseFilterTest("$filter=id lt 1", "SELECT * FROM c WHERE c.id < @1", 1);

    [Fact]
    public Task FilterForIdLte()
        => BaseFilterTest("$filter=id le 1", "SELECT * FROM c WHERE c.id <= @1", 1);

    [Fact]
    public Task FilterForChildProp()
        => BaseFilterTest("$filter=child/text eq 'test'", "SELECT * FROM c WHERE c.child.text = @1", "test");

    [Fact]
    public Task FilterForGrandChildProp()
        => BaseFilterTest("$filter=child/grandChild/text eq 'test'", "SELECT * FROM c WHERE c.child.grandChild.text = @1", "test");

    private async Task BaseFilterTest(string query, string expectedSql, object expectedP1Value)
    {
        var response = await _client.GetAsync($"/query?{query}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await response.Content.ReadAsJsonAsync();
        data["queryText"]!.Value<string>().Should().Be(expectedSql);

        var p1Value = (object)"Unrecognized type";
        if (expectedP1Value is int)
            p1Value = data["parameters"]!["@1"]!.Value<int>();
        else if (expectedP1Value is string)
            p1Value = data["parameters"]!["@1"]!.Value<string>();

        p1Value.Should().Be(expectedP1Value);
    }
}