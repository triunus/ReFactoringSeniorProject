//using UnityEngine;
using System.Net;

using System.IO;
using System.Text;

public enum ServerRequestPath
{
    login_test,
    register_test,
    delete_test,
    RequestGameLobbyUserInformaion_test,

}

public interface IServerConnecter
{
    public string Request(string requestData, string url);
}

public class ServerConnecter : IServerConnecter
{
    private IServerInfo server = new ServerInfo();

    public string Request(string requestData, string url)
    {
        string requestURL = string.Format("{0}{1}", server.GetURL(), "/" + url);
        string responseFromServer;

        WebRequest request = WebRequest.Create(requestURL);

        request.Method = "POST";
        byte[] byteArray = Encoding.UTF8.GetBytes(requestData);

        request.ContentType = "application/json";
        request.ContentLength = byteArray.Length;

        Stream dataStream = request.GetRequestStream();

        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();

        WebResponse response = request.GetResponse();

//        Debug.Log(((HttpWebResponse)response).StatusDescription);

        using (dataStream = response.GetResponseStream())
        {
            StreamReader reader = new StreamReader(dataStream);

            responseFromServer = reader.ReadToEnd();

//            Debug.Log("responseFromServer : " + responseFromServer);
        }

        response.Close();

        return responseFromServer;
    }
}