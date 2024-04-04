namespace MapleAPI;

public class APIRequest
{
    private static string GetUrlFromRequestType(APIRequestType type)
    {
        return $"https://open.api.nexon.com/maplestory/{type}";
    }

    protected internal static async Task<APIResponse> RequestAsync(APIRequestType requestType, Dictionary<string, string>? args = null)
    {
        using HttpClient request = new HttpClient();
        string url = GetUrlFromRequestType(requestType);
        if (args is {Count: > 0})
        {
            url += '?';
            foreach (var pair in args) url += $"{pair.Key}={pair.Value}&";
            url = url[..^1];
        }

        HttpRequestMessage header = new HttpRequestMessage(HttpMethod.Get, url);
        header.Headers.Add("x-nxopen-api-key", MapleAPI.APIKey);

        HttpResponseMessage response = await request.SendAsync(header);
        string resBody = await response.Content.ReadAsStringAsync();
        return APIResponse.Parse(resBody);
    }
    
    protected internal static APIResponse Request(APIRequestType requestType, Dictionary<string, string>? args = null)
    {
        using HttpClient request = new HttpClient();
        string url = GetUrlFromRequestType(requestType);
        if (args is {Count: > 0})
        {
            url += '?';
            foreach (var pair in args) url += $"{pair.Key}={pair.Value}&";
            url = url[..^1];
        }
        
        // Console.WriteLine($"Request {url}");
        HttpRequestMessage header = new HttpRequestMessage(HttpMethod.Get, url);
        header.Headers.Add("x-nxopen-api-key", MapleAPI.APIKey);

        HttpResponseMessage response = request.Send(header);
        string resBody = response.Content.ReadAsStringAsync().Result;
        return APIResponse.Parse(resBody);
    }
}