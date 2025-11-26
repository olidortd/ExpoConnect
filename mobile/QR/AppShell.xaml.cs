namespace QR
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");

            Application.Current.MainPage = new GuestShell();
        }
    }
}
