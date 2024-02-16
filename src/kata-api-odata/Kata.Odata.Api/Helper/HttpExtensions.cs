namespace Kata.Odata.Api.Helper
{
    public static class HttpRequestExtensions
    {

        public static Dictionary<string, string> GetODataOption(this HttpRequest Request)
        {

            Request.Query.TryGetValue("$select", out var select);
            Request.Query.TryGetValue("$filter", out var filter);
            Request.Query.TryGetValue("$orderby", out var orderby);
            Request.Query.TryGetValue("$skip", out var skip);
            bool hasTopLimit = Request.Query.TryGetValue("$top", out var top);
            if (string.IsNullOrEmpty(skip)) skip = "0";

            Dictionary<string, string> options = new()
            {
                                                   { "select", select.ToString() },
                                                   { "filter", filter.ToString() },
                                                   { "orderby", orderby.ToString() },
                                                   { "skip", skip.ToString() }
                                                 };

            if (hasTopLimit) options.Add("top", top.ToString());

            return options;

        }
    }
}
