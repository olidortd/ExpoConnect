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
	private readonly HttpClient _client;

    public ApiClientService()
	{
        //הלקוח שאיתו מדברים עם  השרת
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:5027/api/")
        };
        InitializeComponent();

	}
    private async Task AddAuthorizationHeaderAsync()
    {
        var token = await SecureStorage.GetAsync("access_token");
        if (!string.IsNullOrEmpty(token)) {
            //מוסיף את ה־Access Token לכל בקשה שאת שולחת לשרת.
            _client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        await AddAuthorizationHeaderAsync();
        var response = await _client.SendAsync(request);

        //אם קיבלנו 401 ננסה לחדש את התוקן
        if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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

        //בקשה לשרת כדי לחדש את הטוקן
        var refreshResponse = await _client.PostAsync("Auth/refresh",content);
        if (!refreshResponse.IsSuccessStatusCode)
            return false;
        
        var responseBody = await refreshResponse.Content.ReadAsStringAsync();
        var tokens = JsonSerializer.Deserialize<AuthResponse>(responseBody);
        //בדיקה שהשרת  שלח טוקן חדש
        if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
            return false;
       
        //מחליפים את ה־Access Token
       // מחליפים גם את ה־Refresh Token
        await SecureStorage.SetAsync("access_token",tokens.AccessToken);
        await SecureStorage.SetAsync("refresh_token", tokens.RefreshToken);

        return true;
    }

    //שלוח בקשת POST לשרת 
    public async Task<HttpResponseMessage> PostAsync(string endpoint ,object data)
    {
        //ממירה את האובייקט  לפורמט JSON.
        var json = JsonSerializer.Serialize(data);
        //יוצרים את גוף הבקשה (body)
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        //יוצרים אובייקט בקשה אמיתי לשרת
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