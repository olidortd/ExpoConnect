using QR.Models;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace QR;

public partial class UsersCatalogsPage : ContentPage
{
    public ObservableCollection<CatalogResponse> Catalogs { get; } = new();

    public UsersCatalogsPage()
    {
        InitializeComponent();
        CatalogsCollection.ItemsSource = Catalogs;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var api = ApiClientService.Instance; 
            var response = await api.GetAsync("catalogs");
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var catalogsList =
                JsonSerializer.Deserialize<List<CatalogResponse>>(json, options)
                ?? new List<CatalogResponse>();

            Catalogs.Clear();
            foreach (var c in catalogsList)
                Catalogs.Add(c);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void CatalogSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is CatalogResponse catalog)
        {
            await Navigation.PushAsync(new CatalogItemsPage(catalog));

            CatalogsCollection.SelectedItem = null;
        }
    }
    

}