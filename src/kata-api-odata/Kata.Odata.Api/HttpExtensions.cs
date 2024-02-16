namespace Kata.Odata.Api
{
    public static class HttpRequestExtensions
    {

        public static Dictionary<string, string> GetODataOption(this HttpRequest Request)
        {

            Request.Query.TryGetValue("$select", out var select);
            Request.Query.TryGetValue("$filter", out var filter);
            Request.Query.TryGetValue("$orderby", out var orderby);
            Request.Query.TryGetValue("$skip", out var skip);
            bool bTop = Request.Query.TryGetValue("$top", out var top);

            //if (string.IsNullOrEmpty(top)) top = "10";
            if (string.IsNullOrEmpty(skip)) skip = "0";

            Dictionary<string, string> options = new Dictionary<string, string>
                                                 {
                                                   { "select", select.ToString() },
                                                   { "filter", filter.ToString() },
                                                   { "orderby", orderby.ToString() },
                                                   { "skip", skip.ToString() }
                                                 };

            if (bTop) options.Add("top", top.ToString());

            return options;

        }
    }
}
