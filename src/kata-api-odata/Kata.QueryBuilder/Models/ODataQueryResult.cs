using Newtonsoft.Json;

namespace Kata.QueryBuilder.Models
{
    public class ODataQueryResult
    {
        [JsonProperty("@odata.count")]
        public int Count { get; set; }
        [JsonProperty("value")]
        public IEnumerable<dynamic> Value { get; set; }

    }
}
