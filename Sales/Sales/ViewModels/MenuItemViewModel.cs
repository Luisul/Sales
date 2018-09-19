using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Sales.Helpers;
using Sales.Views;
using Xamarin.Forms;

namespace Sales.ViewModels
{
    public class MenuItemViewModel
    {
        #region Properties
        public string Icon { get; set; }

        public string Title { get; set; }

        public string PageName { get; set; }
        #endregion

        #region Commands

        public ICommand GotoCommand
        {
            get
            {
                return new RelayCommand(GoTo);
            }
                
        }

        private void GoTo()
        {
            if (this.PageName.Equals("LoginPage"))
            {
                Settings.AccesToken = string.Empty;
                Settings.TonkenType = string.Empty;
                Settings.IsRemembered = false;

                MainViewModel.GetInstance().Login = new LoginViewModel();
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
        #endregion
    }
}
