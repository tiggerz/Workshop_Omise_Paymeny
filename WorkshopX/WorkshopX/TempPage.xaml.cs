using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkshopX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TempPage : ContentPage
    {
        //public TempPage(string cardNumber,string inputTemp)
        public TempPage(string x)
        {
            InitializeComponent();
            try
            {
                xxx.Text = x.ToString();
            }
            catch (Exception)
            {
                Navigation.PushAsync(new Login(null));
            }

            string a = "1234567890123";
            
            //char[] xx = cardNumber.ToCharArray();

            //    xx[12] = 'X';
            //    xx[13] = 'X';
            //    xx[14] = 'X';
            //    xx[15] = 'X';

            //var a = "";
            //for (int i = 0; i < xx.Length; i++)
            //{
            //    a = a + xx[i].ToString();

            //}

            //CardNumber.Text = a;
            //InputTemp.Text = inputTemp;

        }
    }
}