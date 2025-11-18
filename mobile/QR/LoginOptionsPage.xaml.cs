namespace QR;

public partial class LoginOptionsPage : ContentPage
{
	public LoginOptionsPage()
	{
		InitializeComponent();
	}

    private async void OnGetCatalogClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GetCatalogs());
    }

    private async void OnAddNewItemClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddNewItemPage());
    }
}