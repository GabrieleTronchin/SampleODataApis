namespace Kata.Odata.DataModel.KataQuery.Models
{
    public class BuilderQueryResult
    {
        public required string TSqlQuery { get; set; }

        public bool IsCountQuery { get; set; }

        public required IDictionary<string, object> Parameters { get; set; }
    }

}
