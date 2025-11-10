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
            var client = new HttpClient();
            var data = new { email = email, password = password, displayName = displayName };
            string json = JsonSerializer.Serialize(data);
            //
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://10.0.2.2:5027/api/Auth/register", content);
            await DisplayAlert("Debug", response.StatusCode.ToString(), "OK");

            if (response.IsSuccessStatusCode)
            {
                //אם ההרשמה הצליחה ננסה לקראואת הקוד שחזר
                string responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                string token = jsonResponse.GetProperty("token").GetString();



                await DisplayAlert("Success", $"Registered!\n {token}", "OK");
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
		
       
        //מחזיר לעמוד הקודם
        await Navigation.PopAsync();


    }
}