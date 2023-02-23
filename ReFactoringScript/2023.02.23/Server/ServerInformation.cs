public interface IServerInfo
{
    string GetURL();
}

public class ServerInfo : IServerInfo
{
    private string host;
    private int port;
    private string url;

    public ServerInfo(string host = "http://127.0.0.1", int port = 52273)
    {
        this.host = host;
        this.port = port;
        url = string.Format("{0}:{1}", host, port);
    }

    public string GetURL() { return url; }
}
