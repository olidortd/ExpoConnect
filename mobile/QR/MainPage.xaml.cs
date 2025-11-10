using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using Npgsql;

namespace QR
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //ConnectToDatabase();
        }
        //קורא לבסיס נתונים
        private async void ConnectToDatabase(object sender, EventArgs e)
        {
            try
            {
                //אומר לאיפו להתחבר
                string connectionString = "Host = 192.168.10.189;Port=5432;Username=postgres;Password=goldi5863;Database=expo_connect_db";
               //הusing מבטיח שחיבור יסגר אוטמטית,  
              // עושה את החיבור
                using (var connection = new NpgsqlConnection(connectionString)) { 
                   //
                    connection.Open();
                    //connection.ConnectionString = connectionString;
                    DisplayAlert("Success", "Connected to database successfully!", "OK");


                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Connection error: {ex}");
               // DisplayAlert("Error", $"Connection failed:\n{ex.Message}", "OK");
                //
          
            }
        }


        
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());

        }
        private async void OnScanClicked(object sender, EventArgs e)
        {
            //יוצרים אובייקט סורק מצלמה 
            var scanner = new CameraBarcodeReaderView
            {
                Options = new BarcodeReaderOptions
                {
                    //  איזה סוגי ברקודים לזהות
                    Formats = BarcodeFormat.Code128 | BarcodeFormat.QrCode | BarcodeFormat.Code39
                }
            };
            //מחכה עד שמקבל תשובה
            var tcs = new TaskCompletionSource<string>();
            scanner.BarcodesDetected += (s, args) =>
            {
                if (args.Results?.FirstOrDefault() is { } result)
                {
                    tcs.TrySetResult(result.Value);
                }

            };
            //יוצרים דף חדש שבו מוצג המצלמה-סורק
            var scanPage = new ContentPage
            {
                Content = scanner,
            };
            //עוברים לדף החדש -המצלמה נפתחת
            await Navigation.PushAsync(scanPage);
            //מחכים עד שיסרק ברקוד ואז נכנס למשתנה
            var barcode = await tcs.Task;
            //סוגרים אצ הדף סריקה
            await Navigation.PopAsync();
            RsultLabel.Text = barcode;
        }
    }
}

