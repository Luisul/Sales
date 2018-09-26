using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Sales
{
    using Newtonsoft.Json;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Sales.ViewModels;
    using Views;
    public partial class App : Application
    {
        public static NavigationPage Navigator { get; internal set; }

        public App()
        {
            InitializeComponent();

            var mainViewModel = MainViewModel.GetInstance();

            if (Settings.IsRemembered)
            {

                if (!string.IsNullOrEmpty(Settings.UserAsp))
                {
                    mainViewModel.UserAsp = JsonConvert.DeserializeObject<MyUserASP>(Settings.UserAsp);
                }

                mainViewModel.Products = new ProductsViewModel();
                this.MainPage = new MasterPage();
            }
            else
            {
                mainViewModel.Login = new LoginViewModel();
                this.MainPage = new NavigationPage(new LoginPage());
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
