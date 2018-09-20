namespace Sales.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Services;
    using Xamarin.Forms;

    public class ProductsViewModel : BaseViewModel
    {
        #region Attributes
        private string filter;

        private ApiService apiService;

        private DataService dataService;

        private bool isRefreshing;

        private ObservableCollection<ProductsItemViewModel> products;
        #endregion

        #region Properties    

        public List<Product> MyProducts { get; set; }

        public string Filter
        {
            get { return this.filter; }
            set
            {
                this.filter = value;
                this.RefresList();
            }
        }

        public ObservableCollection<ProductsItemViewModel> Products
        {
            get { return this.products; }
            set { this.SetValue(ref this.products, value); }
        }

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { this.SetValue(ref this.isRefreshing, value); }
        }
        #endregion

        #region Constructors
        public ProductsViewModel()
        {
            instance = this;
            this.apiService = new ApiService();
            this.dataService = new DataService();
            this.LoadProducts();
        }
        #endregion

        #region Singleton
        private static ProductsViewModel instance;
        public static ProductsViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ProductsViewModel();
            }
            return instance;
        }
        #endregion

        #region Methods
        private async void LoadProducts()
        {
            this.IsRefreshing = true;

            var conecction = await this.apiService.CheckConecction();
            if (!conecction.IsSucces)
            {
                var answer = await this.LoadProductsFromApi();
                if (answer)
                {
                    this.SaveProductsToDB();
                }
            }
            else
            {
                await this.LoadProductsFromDB();
            }

            if (this.MyProducts == null || this.MyProducts.Count.Equals(0))
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error, Languages.NoProductsMessage, Languages.Accept);
                return;
            }

            this.RefresList();
            this.IsRefreshing = false;
        }

        private async Task LoadProductsFromDB()
        {
            this.MyProducts = await this.dataService.GetAllProducts();
        }

        private async Task  SaveProductsToDB()
        {
            await dataService.DeleteAllProducts();
            dataService.Insert(this.MyProducts);
        }

        private async Task<bool> LoadProductsFromApi()
        {
            var url = Application.Current.Resources["UrlApi"].ToString();
            var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
            var urlController = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.GetList<Product>(url, urlPrefix, urlController, Settings.TonkenType, Settings.AccesToken);
            if (!response.IsSucces)
            {
                return false;
            }

            this.MyProducts = (List<Product>)response.Result;
            return true;
        }

        public void RefresList()
        {
            if (string.IsNullOrEmpty(this.Filter))
            {

                var myListProductsItemViewModel = this.MyProducts.Select(p => new ProductsItemViewModel
                {
                    Description = p.Description,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks
                });

                this.Products = new ObservableCollection<ProductsItemViewModel>(
                    myListProductsItemViewModel.OrderBy(p => p.Description));
                this.IsRefreshing = false;
            }
            else
            {
                var myListProductsItemViewModel = this.MyProducts.Select(p => new ProductsItemViewModel
                {
                    Description = p.Description,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks
                }).Where(p => p.Description.ToLower().Contains(this.Filter.ToLower())).ToList();

                this.Products = new ObservableCollection<ProductsItemViewModel>(
                    myListProductsItemViewModel.OrderBy(p => p.Description));
                this.IsRefreshing = false;
            }
        }



        #endregion

        #region Commands
        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(RefresList);
            }

        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadProducts);
            }

        }
        #endregion
    }
}

