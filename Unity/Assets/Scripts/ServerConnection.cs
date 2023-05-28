using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using Assets.Scripts.Models;

public class ServerConnection : MonoBehaviour
{
    public static ServerConnection Instance;
    private readonly HttpClient client = new HttpClient();
    private readonly string url = "https://localhost:44344/api/MarauderUser/";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task<User> GetUser(int id) {
        try
        {
            HttpResponseMessage response = await client.GetAsync(url + id);
            Debug.Log(response);
            var temp = await response.Content.ReadAsStringAsync();
            var user = new User(temp);
            return user;
        }
        catch (System.Exception e)
        { Debug.Log(e);
            return null;
        }
    }

    public async Task<HttpResponseMessage> UpdateUser(User user) {

        HttpResponseMessage response = null;

        try {
            response = await client.PutAsync(url + user.id, new StringContent(user.ToJson(), System.Text.Encoding.UTF8, "application/json"));
            Debug.Log(response);
            return response;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return response;
        }
    }

    public async Task<Information> GetInformation()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(url + "connect");
            var temp = await response.Content.ReadAsStringAsync();
            var info = new Information(temp);
            return info;
        } catch (System.Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }
}
