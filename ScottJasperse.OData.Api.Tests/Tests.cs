using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using System.Net;
using Newtonsoft.Json.Linq;
using System;

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

    [Fact]
    public Task NoParameters()
        => BaseFilterTest("", "SELECT * FROM c");

    [Fact]
    public Task FilterForIdEq()
        => BaseFilterTest("$filter=id eq 1", "SELECT * FROM c WHERE c.id = @p1", 1);

    [Fact]
    public Task FilterForNameEq()
        => BaseFilterTest("$filter=name eq 'test'", "SELECT * FROM c WHERE c.name = @p1", "test");

    [Fact]
    public Task FilterForIdGt()
        => BaseFilterTest("$filter=id gt 1", "SELECT * FROM c WHERE c.id > @p1", 1);

    [Fact]
    public Task FilterForIdGte()
        => BaseFilterTest("$filter=id ge 1", "SELECT * FROM c WHERE c.id >= @p1", 1);

    [Fact]
    public Task FilterForIdLt()
        => BaseFilterTest("$filter=id lt 1", "SELECT * FROM c WHERE c.id < @p1", 1);

    [Fact]
    public Task FilterForIdLte()
        => BaseFilterTest("$filter=id le 1", "SELECT * FROM c WHERE c.id <= @p1", 1);

    [Fact]
    public Task FilterForChildProp()
        => BaseFilterTest("$filter=child/text eq 'test'", "SELECT * FROM c WHERE c.child.text = @p1", "test");

    [Fact]
    public Task FilterForGrandChildProp()
        => BaseFilterTest("$filter=child/grandChild/text eq 'test'", "SELECT * FROM c WHERE c.child.grandChild.text = @p1", "test");

    [Fact]
    public Task FilterForNameContains()
        => BaseFilterTest("$filter=contains(name, 'test')", "SELECT * FROM c WHERE CONTAINS(c.name, @p1)", "test");

    [Fact]
    public Task FilterForAnd()
        => BaseFilterTest("$filter=id eq 1 and name eq 'test'", "SELECT * FROM c WHERE c.id = @p1 AND c.name = @p2", 1, "test");


    private async Task BaseFilterTest(string query, string expectedSql, object? expectedP1Value = null, object? expectedP2Value = null)
    {
        var response = await _client.GetAsync($"/testmodels/query?{query}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await response.Content.ReadAsJsonAsync();
        data["queryText"]!.Value<string>().Should().Be(expectedSql);

        Action<string, object> assertExpectedParameter = (paramName, expectedValue) => {
            var value = (object)"Unrecognized type";

            if (expectedValue is int)
                value = data["queryParameters"]![paramName]!.Value<int>();
            else if (expectedValue is string)
                value = data["queryParameters"]![paramName]!.Value<string>();

            value.Should().Be(expectedValue);
        };

        if (expectedP1Value != null) assertExpectedParameter("@p1", expectedP1Value);
        if (expectedP2Value != null) assertExpectedParameter("@p2", expectedP2Value);
    }
}