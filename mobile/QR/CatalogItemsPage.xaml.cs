
using System.Collections.ObjectModel;
using System.Text.Json;
using QR.Models;
namespace QR;

public partial class CatalogItemsPage : ContentPage
{
    private readonly CatalogResponse _catalog;
    public ObservableCollection<CatalogItemResponse> Items { get; set; } = new();

    public CatalogItemsPage(CatalogResponse catalog)
    {
        InitializeComponent();

        _catalog = catalog;
        Title = catalog.Name;  

        ItemsCollection.ItemsSource = Items;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Items.Clear();

        if (_catalog.Items != null)
        {
            foreach (var item in _catalog.Items)
                Items.Add(item);
        }
    }
}

