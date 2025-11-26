using System.Text.Json;

namespace QR;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}


    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());

    }

    private async void OnLoginClicked(object sender, EventArgs e)
	{
		string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please fill all fields", "OK");
            return;
        }

        try
        {
            var api = ApiClientService.Instance;

            var data = new { email = email, password = password };
            var response = await api.PostAsync("Auth/login", data);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                //Decodes (decodes) the text returned by the API server into a data structure that can be accessed in code.
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                string accessToken = jsonResponse.GetProperty("accessToken").GetString();
                string refreshToken = jsonResponse.GetProperty("refreshToken").GetString();

            
                await SecureStorage.SetAsync("access_token", accessToken);
                await SecureStorage.SetAsync("refresh_token", refreshToken);

                await DisplayAlert("Welcome", "Login successful!", "OK");
                await Navigation.PushAsync(new LoginOptionsPage());

            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"Login failed: {error}", "OK");
            }
        }
        catch (Exception ex) {
            await DisplayAlert("Error", ex.Message, "OK");
        }


    }
}