namespace Sales.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Sales.Helpers;
    using Sales.Services;
    using Sales.Views;
    using Xamarin.Forms;

    public class ProductsItemViewModel : Product
    {
        #region Attributes
        private ApiService apiService;
        #endregion

        #region Constructors
        public ProductsItemViewModel()
        {
            this.apiService = new ApiService();
        }
        #endregion

        #region Command

        public ICommand EditProductCommand
        {
            get
            {
                return new RelayCommand(EditProduct);
            }
        }

        private async void EditProduct()
        {
            MainViewModel.GetInstance().EditProduct = new EditProductViewModel(this);
            await Application.Current.MainPage.Navigation.PushAsync(new EditProductPage());
        }

        public ICommand DeleteProductCommand
        {
            get
            {
                return new RelayCommand(DeleteProduct);
            }
        }

        private async void DeleteProduct()
        {
            var answer = await Application.Current.MainPage.DisplayAlert(
                Languages.Confirm,
                Languages.DeleteConfirmation,
                Languages.Yes,
                Languages.No);

            if (!answer)
            {
                return;
            }

            var conecction = await this.apiService.CheckConecction();
            if (!conecction.IsSucces)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, conecction.Message, Languages.Accept);
                return;
            }

            var url = Application.Current.Resources["UrlApi"].ToString();
            var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
            var urlController = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Delete(url, urlPrefix, urlController, this.ProductId);
            if (!response.IsSucces)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Error);
                return;
            }

            var productsViewModel = ProductsViewModel.GetInstance();
            var deleteProduct = productsViewModel.Products.Where(p => p.ProductId == this.ProductId).FirstOrDefault();
            if (deleteProduct != null)
            {
                productsViewModel.MyProducts.Remove(deleteProduct);
            }

            productsViewModel.RefresList();
        }
        #endregion
    }
}
