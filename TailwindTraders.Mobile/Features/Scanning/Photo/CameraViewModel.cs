﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Localization;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Framework;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public class CameraViewModel : BaseViewModel
    {
        private readonly string mediaPath;

        private readonly PhotoService photoService;
        private readonly IPlatformService platformService;
        private readonly IVisionService visionService;

        public const string ReloadGalleryMessage = nameof(ReloadGalleryMessage);

        public bool GotRecommendedProducts { get; private set; }

        public ICommand CloseCommand => new AsyncCommand(App.NavigateModallyBackAsync);

        protected async Task AddProductToCartAsync()
        {
            var product = RecommendedProducts.First();

            if (product != null)
            {
                await TryExecuteWithLoadingIndicatorsAsync(
                    RestPoolService.ProductCartAPI.AddProductAsync(product));

                await Application.Current.MainPage.DisplayAlert(
                    product.Name,
                    Resources.Alert_Added_To_Cart,
                    Resources.Alert_OK);
            }
            else
            {
                XSnackService.ShowMessage(Resources.Snack_Message_AddedToCart_Error);
            }
        }

        public ICommand AddToCartCommand => new AsyncCommand(AddProductToCartAsync);

        public ICommand TakePhotoCommand => new AsyncCommand(App.NavigateModallyBackAsync);

        private string cameraImage;

        public string CameraImage
        {
            get => cameraImage;
            set => SetAndRaisePropertyChanged(ref cameraImage, value);
        }

        private List<ProductDTO> recommendedProducts;

        public List<ProductDTO> RecommendedProducts
        {
            get => recommendedProducts;
            set => SetAndRaisePropertyChanged(ref recommendedProducts, value);
        }

        public CameraViewModel(string mediaPath)
        {
            this.mediaPath = mediaPath;

            photoService = DependencyService.Get<PhotoService>();
            platformService = DependencyService.Get<IPlatformService>();
            visionService = DependencyService.Get<IVisionService>();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var resized = platformService.ResizeImage(this.mediaPath, PhotoSize.Small, quality: 70);

            // TODO: add error msg
            if (!resized)
            {
                return;
            }

            CameraImage = this.mediaPath;

            var visionResult = await TryExecuteWithLoadingIndicatorsAsync(
                visionService.GetRecommendedProductsFromPhotoAsync(CameraImage));

            GotRecommendedProducts = visionResult && visionResult.Value != default(IEnumerable<ProductDTO>);

            RecommendedProducts = GotRecommendedProducts ? 
                new List<ProductDTO>(visionResult.Value) : 
                new List<ProductDTO>();           
        }

        public override async Task UninitializeAsync()
        {
            await base.UninitializeAsync();

            MessagingCenter.Send(this, ReloadGalleryMessage);
        }
    }
}
