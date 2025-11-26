using System.Text.Json;


namespace QR;

public partial class GetCatalogs : ContentPage
{
    
    public GetCatalogs()
    {

        InitializeComponent();

    }

    private async void OnGetNameClicked(object sender, EventArgs e)
    {
        var api = ApiClientService.Instance;
        try
        {
            var response = await api.GetAsync("catalogs");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                //  interpret the response as a list of objects
                var jsonArray = JsonSerializer.Deserialize<List<JsonElement>>(json);

                //  take the first object in the list
                var firstObject = jsonArray.FirstOrDefault();

                //  extract the name (or any other field)
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