using Newtonsoft.Json;

namespace Kata.Odata.DataModel.KataQuery.Models
{
    public class ODataQueryResult
    {
        [JsonProperty("@odata.count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public IEnumerable<dynamic>? Value { get; set; }

    }
}
