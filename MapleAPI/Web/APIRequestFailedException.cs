namespace MapleAPI.Web;

public class APIRequestFailedException : Exception
{
    public APIRequestFailedException(APIResponse error, ArgBuilder args) : base($"Nexon API Request Failed. [Error Code: {error.ResponseType}] [Args: {args}]")
    {
        
    }
}