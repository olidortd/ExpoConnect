using System.Text.Json;

namespace QR;

public partial class AddNewItemPage : ContentPage
{
	public AddNewItemPage()
	{
        InitializeComponent();
	}

    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        var api = ApiClientService.Instance;

        string catalogId = CatalogIdEntry.Text;
        string name = NameEntry.Text;
        string desc = DescriptionEntry.Text;
        string category = CategoryEntry.Text;
        string priceStr = PriceEntry.Text;
        string imageUrl = ImageUrlEntry.Text;
        string featuresStr = FeaturesEntry.Text;

        try
        {
            var response = await api.GetAsync("catalogs");
            /*
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                // נפרש את התגובה כרשימה של אובייקטים
                var jsonArray = JsonSerializer.Deserialize<List<JsonElement>>(json);

                // ניקח את האובייקט הראשון ברשימה
                var firstObject = jsonArray.FirstOrDefault();

                // נשלוף את id (או כל שדה אחר)
                catalogId = firstObject.GetProperty("catalogId").GetString();

                await DisplayAlert("catalog id", catalogId, "OK");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"Failed: {error}", "OK");
            }
       */

        if (string.IsNullOrWhiteSpace(catalogId) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(priceStr))
        {
            await DisplayAlert("Error", "Please fill all required fields.", "OK");
            return;
        }
        if (!decimal.TryParse(priceStr, out decimal price))
        {
            await DisplayAlert("Error", "Invalid price format.", "OK");
            return;
        }

            //  Converts text with commas to a list of Strings
            var features = featuresStr?
             .Split(',')
             .Select(f => f.Trim())
             .Where(f => !string.IsNullOrWhiteSpace(f))
             .ToList() ?? new List<string>();

        var newItem = new
        {
            catalogId = catalogId,
            name = name,
            description = desc,
            category = category,
            price = price,
            imageUrl = imageUrl,
            features = features
        };

             response = await api.PostAsync($"catalogs/{catalogId}/items", newItem);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Success", "Item added successfully!", "OK");
                await Navigation.PopAsync(); 
            else
            {
                string err = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", err, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }


    }
}