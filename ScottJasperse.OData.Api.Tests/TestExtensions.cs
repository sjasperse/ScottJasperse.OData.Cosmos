using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScottJasperse.ODataTest.Tests
{
    public static class TestExtensions
    {
        public static Task<JToken> ReadAsJsonAsync(this HttpContent responseContent)
            => ReadAsJsonAsync<JToken>(responseContent);

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent responseContent)
        {
            var json = await responseContent.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json)!;
        }
    }
}