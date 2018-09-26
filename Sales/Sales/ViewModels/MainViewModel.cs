namespace Sales.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Sales.Views;
    using Xamarin.Forms;


    public class MainViewModel
    {
        #region Properties      
        public RegisterViewModel Register { get; set; }

        public LoginViewModel Login { get; set; }

        public EditProductViewModel EditProduct { get; set; }

        public ProductsViewModel Products { get; set; }

        public AddProductViewModel AddProduct { get; set; }

        public ObservableCollection<MenuItemViewModel> Menu { get; set; }

        public MyUserASP UserAsp { get; set; }

        public string UserFullName
        {
            get
            {
                if (this.UserAsp != null && this.UserAsp.Claims != null && this.UserAsp.Claims.Count > 1)
                {
                    return $"{this.UserAsp.Claims[0].ClaimValue} {this.UserAsp.Claims[1].ClaimValue}";
                }

                return null;
            }
        }

        public string UserImageFullPath
        {
            get
            {
                foreach (var claim in this.UserAsp.Claims)
                {
                    if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/uri")
                    {
                        if (claim.ClaimValue.StartsWith("~"))
                        {
                            return $"https://salesapiservices.azurewebsites.net{claim.ClaimValue.Substring(1)}";
                        }

                        return claim.ClaimValue;
                    }
                }

                return null;
            }
        }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            instance = this;
            this.LoadMenu();
            
        }

        #endregion

        #region Methods

        private void LoadMenu()
        {
            this.Menu = new ObservableCollection<MenuItemViewModel>();

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_info",
                PageName = "AboutPage",
                Title = Languages.About,
            });

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_phonelink_setup",
                PageName = "SetupPage",
                Title = Languages.Setup,
            });

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_exit_to_app",
                PageName = "LoginPage",
                Title = Languages.Exit,
            });
        }
        #endregion

        #region Singleton
        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }
            return instance;
        }
        #endregion

        #region Command
        public ICommand AddProductCommand
        {
            get
            {
                return new RelayCommand(GoToAddProduct);
            }

        }

        private async void GoToAddProduct()
        {
            this.AddProduct = new AddProductViewModel();
            await App.Navigator.PushAsync(new AddProductPage());
        }
        #endregion
    }
}
