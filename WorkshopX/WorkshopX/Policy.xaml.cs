using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopX.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkshopX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Policy : ContentPage
    {
        MakeDataModel makeData = new MakeDataModel();

        public Policy(MakeDataModel model)
        {
            makeData = model;

            InitializeComponent();

        }

        private void ok_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Payment(makeData));
        }
    }
}