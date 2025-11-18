using System.Text.Json;


namespace QR;

public partial class GetCatalogs : ContentPage
{
    
    public GetCatalogs()
    {
        var api = new ApiClientService();
        InitializeComponent();

    }

    private async void OnGetNameClicked(object sender, EventArgs e)
    {
        var api = new ApiClientService();
        try
        {
            var response = await api.GetAsync("catalogs");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
              
                // נפרש את התגובה כרשימה של אובייקטים
                var jsonArray = JsonSerializer.Deserialize<List<JsonElement>>(json);

                // ניקח את האובייקט הראשון ברשימה
                var firstObject = jsonArray.FirstOrDefault();

                // נשלוף את השם (או כל שדה אחר)
                string name = firstObject.GetProperty("name").GetString();

                await DisplayAlert("User Name", name, "OK");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"Failed: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}