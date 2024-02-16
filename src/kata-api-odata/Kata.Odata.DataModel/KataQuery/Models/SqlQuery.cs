using Dapper;

namespace Kata.Odata.DataModel.KataQuery.Models
{
    public class SqlQuery
    {
        public string SqlCommand { get; set; }

        public DynamicParameters? Parameters { get; set; }
    }

}
