using System;
using WorkshopX.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkshopX
{
    public partial class App : Application
    {
        public App()
        {
            MakeDataModel makeData = new MakeDataModel
            {
                CarId = 1,
                PickUpDate = "2019-07-06 10:00:00",
                ReturnDate = "2019-07-08 12:00:00",
                InsurancePlanName = "Premium",
                InsurancePrice = 199,
                VatPercent = 7,
                //use in app only
                RentalPrice = 800
            };
            InitializeComponent();
            MainPage = new NavigationPage(new TempPage(null))
            {
                BarBackgroundColor = Color.White,
                BarTextColor = Color.Black
            };
            //MainPage = new NavigationPage(new PaymentPage())
            //{
            //    BarBackgroundColor = Color.FromHex("#f2c700"),
            //    //ti = Color.FromHex("#f2c700")
            //    BarTextColor = Color.Red

            //};
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
