namespace Sales.ViewModels
{
    using System;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Sales.Common.Models;
    using Sales.Services;
    using Xamarin.Forms;

    public class AddProductViewModel : BaseViewModel
    {
        #region Attributes
        private MediaFile file;

        private ImageSource imageSource;

        private ApiService apiService;

        private bool isRunning;

        private bool isEnabled;
        #endregion


        #region Porperties
        public string Description { get; set; }

        public string Price { get; set; }

        public string Remarks { get; set; }


        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.SetValue(ref this.isRunning, value); }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.SetValue(ref this.isEnabled, value); }
        }

        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set { this.SetValue(ref this.imageSource, value); }
        }

        #endregion


        #region Constructors
        public AddProductViewModel()
        {
            this.apiService = new ApiService();
            this.isEnabled = true;
            this.ImageSource = "noproduct";
        }
        #endregion


        #region Commands

        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }

        }

        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                Languages.ImageSource,
                Languages.Cancel,
                null,
                Languages.FromGallery,
                Languages.NewPicture);

            if (source == Languages.Cancel)
            {
                this.file = null;
                return;
            }

            if (source == Languages.NewPicture)
            {
                this.file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                this.file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (this.file != null)
            {
                this.ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = this.file.GetStream();
                    return stream;
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }

        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Description))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.DescriptionError,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.Price))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PriceError,
                    Languages.Accept);
                return;
            }

            var price = decimal.Parse(this.Price);

            if (price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PriceError,
                    Languages.Accept);
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var conecction = await this.apiService.CheckConecction();
            if (!conecction.IsSucces)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    conecction.Message,
                    Languages.Accept);
                return;
            }

            byte[] imageArray = null;
            if (this.file != null)
            {
                imageArray = FileHelper.ReadFully(this.file.GetStream());
            }


            var product = new Product
            {
                Description = this.Description,
                Price = price,
                Remarks = this.Remarks,
                ImageArray = imageArray
            };
            var url = Application.Current.Resources["UrlApi"].ToString();
            var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
            var urlController = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Post(url, urlPrefix, urlController, product);

            if (!response.IsSucces)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }

            var newProduct = (Product)response.Result;
            var viewModel = ProductsViewModel.GetInstance();
            viewModel.Products.Add(newProduct);

            this.IsRunning = false;
            this.IsEnabled = true;

            await Application.Current.MainPage.Navigation.PopAsync();

        }
        #endregion
    }
}
