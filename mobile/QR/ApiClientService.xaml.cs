namespace QR;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

//מחלקה אחת שמרכזת את כל הטיפול בבקשות לשרת, כולל:
//שליחת בקשות ל־API(GET, POST וכו’)
//הוספת Header עם ה־Access Token
//טיפול אוטומטי במקרה של טוקן שפג תוקף
//חידוש הטוקן ע"י ה־Refresh Token


public partial class ApiClientService : ContentPage
{
    public static ApiClientService Instance { get; } = new ApiClientService();

    private readonly HttpClient _client;

    private ApiClientService()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:5027/api/")
        };
    }

    private async Task AddAuthorizationHeaderAsync()
    {
        var token = await SecureStorage.GetAsync("access_token");
        if (!string.IsNullOrEmpty(token)) {
           
            _client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        await AddAuthorizationHeaderAsync();
        var response = await _client.SendAsync(request);

        //If we received a 401 we will try to renew the patch.
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            bool refreshed = await TryRefreshTokenAsync();
            if (refreshed) {
                await AddAuthorizationHeaderAsync();
                response = await _client.SendAsync(request);
            }
        }
        return response;
    }

    private async Task<bool> TryRefreshTokenAsync()
    {
        var refreshToken = await SecureStorage.GetAsync("refresh_token");
        if (string.IsNullOrEmpty(refreshToken)) 
            return false;

        var json = JsonSerializer.Serialize(new { refreshToken });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //Request to server to renew token
        var refreshResponse = await _client.PostAsync("Auth/refresh",content);
        if (!refreshResponse.IsSuccessStatusCode)
            return false;
        
        var responseBody = await refreshResponse.Content.ReadAsStringAsync();
        var tokens = JsonSerializer.Deserialize<AuthResponse>(responseBody);
        //Checking that the server sent a new token
        if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
            return false;
       
    
        await SecureStorage.SetAsync("access_token",tokens.AccessToken);
        await SecureStorage.SetAsync("refresh_token", tokens.RefreshToken);

        return true;
    }

    //שלוח בקשת POST לשרת 
    public async Task<HttpResponseMessage> PostAsync(string endpoint ,object data)
    {
        //Converts the object to JSON format.
        var json = JsonSerializer.Serialize(data);
        //Create the request body
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        //Create a real request object for the server
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint) {Content = content };
        return await SendAsync(request);
    }
    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        return await SendAsync(request);
    }

    public class AuthResponse
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

}