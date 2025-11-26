using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace QR;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
		string email = EmailEntry.Text;
		string password = PasswordEntry.Text;
		string displayName = DisplayNameEntry.Text;

		//בודק אם זה רייק
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(displayName)) {
			await DisplayAlert("error", "please fill in all fields", "OK");
			return; }
		
		try {
            //
            var api = ApiClientService.Instance;

            var data = new { email = email, password = password, displayName = displayName };
            var response = await api.PostAsync("Auth/register", data);


            if (response.IsSuccessStatusCode)
            {
                //If the registration was successful, we will try to read the returned code.
                string responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                string accessToken = jsonResponse.GetProperty("accessToken").GetString();
                string refreshToken = jsonResponse.GetProperty("refreshToken").GetString();

                await SecureStorage.SetAsync("access_token", accessToken);
                await SecureStorage.SetAsync("refresh_token", refreshToken);


                await DisplayAlert("Success", $"Registered!\n {accessToken}", "OK");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("error", error, "OK");
            }

        }
		catch (Exception ex) {
            await DisplayAlert("error", ex.Message, "OK");
        }
		
       
  
        await Navigation.PopAsync();


    }
    private async void OnGoToLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }

}