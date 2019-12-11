using Newtonsoft.Json;
using Omise;
using Omise.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WorkshopX.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkshopX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Payment : ContentPage
    {
        MakeDataModel makeData = new MakeDataModel();

        private readonly string p_key = "pkey_test_5hlnmrvgrqrh746zaju";
        //private readonly string s_key = "skey_test_5hlnmrvh0vhaxw24cgz";
        public Payment(MakeDataModel model)
        {
            InitializeComponent();
            makeData = model;
            var pickUpDate = Convert.ToDateTime(model.PickUpDate);
            var returnDate = Convert.ToDateTime(model.ReturnDate);
            model.BookingDuration = returnDate.Day - pickUpDate.Day;
            var price = (model.BookingDuration * model.RentalPrice) + model.InsurancePrice;
            model.Vat = (price * model.VatPercent) / 100;
            model.TotalPrice = price + model.Vat;

            priceSum.Text = price.ToString("N", CultureInfo.InvariantCulture);
            vatPercent.Text = model.Vat.ToString("N", CultureInfo.InvariantCulture);
            totalPrice.Text = model.TotalPrice.ToString("N", CultureInfo.InvariantCulture);

        }

        private async void Ok_Clicked(object sender, EventArgs e)
        {

            //chech in put!
            var cardName = CardName.Text;
            var carNumber = CardNumber.Text;
            var exDate = ExDate.Text;
            var ccv = Ccv.Text;
            if (cardName != null && carNumber != null && carNumber.Length == 16 && exDate != null && ccv.Length == 3)
            {
                loading.IsVisible = true;
                bntOk.IsVisible = false;
                CardName.IsEnabled = false;
                CardNumber.IsEnabled = false;
                ExDate.IsEnabled = false;
                Ccv.IsEnabled = false;
                radioGroup.IsEnabled = false;
                var exMonth = Convert.ToInt32(exDate.Substring(0, 2));
                var exYear = Convert.ToInt32(exDate.Substring(3, 2));

                try
                {
                    var client = new Client(p_key);
                    var token = await client.Tokens.Create(new CreateTokenRequest
                    {
                        Name = cardName,
                        Number = carNumber,
                        ExpirationMonth = exMonth,
                        ExpirationYear = exYear,
                        SecurityCode = ccv
                    });

                    var tokenId = await client.Tokens.Get(token.Id);

                    using (var clientx = new HttpClient())
                    {
                        clientx.BaseAddress = new Uri("https://z9mviw2582.execute-api.ap-southeast-1.amazonaws.com/Prod/api/v1/Omise/");
                        //clientx.BaseAddress = new Uri("http://192.168.42.51:5000/api/v1/Omise/");

                        makeData.TokenId = tokenId.Id;
                        var data = JsonConvert.SerializeObject(makeData);

                        //Authorize api
                        var accessToken = Convert.ToString(Application.Current.Properties["AccessToken"]);
                        clientx.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = await clientx.PostAsync("CallOmise", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = await response.Content.ReadAsStringAsync();
                            //loginlist = JsonConvert.DeserializeObject<UserLoginModel>(responseString);
                            await DisplayAlert("Done", responseString, "OK");
                            loading.IsVisible = false;
                            bntOk.IsVisible = true;
                            CardName.IsEnabled = true;
                            CardNumber.IsEnabled = true;
                            ExDate.IsEnabled = true;
                            Ccv.IsEnabled = true;
                            radioGroup.IsEnabled = true;
                            //bntOk.Text = "ชำระเงินสำเร็จ";
                            //Navigation.PushAsync(new Policy(loginlist.UserName));
                            //return RedirectToAction("GetToken", loginParam);
                        }
                        else
                        {
                            var responseString = await response.Content.ReadAsStringAsync();
                            await DisplayAlert("WARNING", "เกิดข้อผิดพลาด : " + responseString, "OK");
                            loading.IsVisible = false;
                            bntOk.IsVisible = true;
                            CardName.IsEnabled = true;
                            CardNumber.IsEnabled = true;
                            ExDate.IsEnabled = true;
                            Ccv.IsEnabled = true;
                            radioGroup.IsEnabled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("WARNING", "เกิดข้อผิดพลาด : " + ex.Message, "OK");
                    loading.IsVisible = false;
                    bntOk.IsVisible = true;
                    CardName.IsEnabled = true;
                    CardNumber.IsEnabled = true;
                    ExDate.IsEnabled = true;
                    Ccv.IsEnabled = true;
                    radioGroup.IsEnabled = true;
                }
            }
            else
            {
                await DisplayAlert("WARNING", "กรุณากรอกข้อมูลให้ครบถ้วน", "OK");
                loading.IsVisible = false;
                bntOk.IsVisible = true;
                CardName.IsEnabled = true;
                CardNumber.IsEnabled = true;
                ExDate.IsEnabled = true;
                Ccv.IsEnabled = true;
                radioGroup.IsEnabled = true;
                //throw;

            }
        }

        private void masterCard_Clicked(object sender, EventArgs e)
        {
            //Successful Charge Master Card
            CardName.Text = "John Doe";
            CardNumber.Text = "5454545454545454";
            ExDate.Text = "1022";
            Ccv.Text = "123";
        }

        private void visa_Clicked(object sender, EventArgs e)
        {
            //Failed Charge  Visa
            CardName.Text = "Eminem";
            CardNumber.Text = "4111111111160001";
            ExDate.Text = "1022";
            Ccv.Text = "123";
        }

        private void jcb_Clicked(object sender, EventArgs e)
        {
            //Payment Rejected JCB
            CardName.Text = "Sasha Grey";
            CardNumber.Text = "3530111111180012";
            ExDate.Text = "0823";
            Ccv.Text = "858";
        }

        private void ExDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            var exDate = ExDate.Text;
            char[] x = ExDate.Text.ToCharArray();
            if (ExDate.Text.Length == 4 && x[2] != '/')
            {
                var exMonth = exDate.Substring(0, 2);
                var exYear = exDate.Substring(2, 2);
                ExDate.Text = exMonth + "/" + exYear;
                ExDate.BorderColor = Color.Gray;
            }
            else if (ExDate.Text.Length == 3 && x[2] == '/')
            {
                var exMonth = exDate.Substring(0, 2);
                ExDate.Text = exMonth;
                ExDate.BorderColor = Color.Red;
                ExDate.AnnotationMessage = "วันหมดอายุ (ดด/ปป)";

            }
        }

        private void CardName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Match checkRegex = Regex.Match(CardName.Text, @"^[a-zA-Z, ]{1,20}$");
            Match reservedWord = Regex.Match(CardName.Text, @"^(?i)DROP|INSERT|DELETE|UPDATE|EXECUTE|CREATE$");
            if (checkRegex.Success && reservedWord.Success != true)
            {
                CardName.BorderColor = Color.Gray;
            }
            else if (CardName.Text == "")
            {
                CardName.BorderColor = Color.Gray;
                CardName.AnnotationMessage = "กรุณาใส่ ชื่อบนบัตร";
            }
            else
            {
                CardName.BorderColor = Color.Red;
                CardName.AnnotationMessage = "รูปแบบตัวอักษรไม่ถูกต้อง";
            }
        }

        private void CardNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CardNumber.Text.Length == 16)
            {
                CardNumber.BorderColor = Color.Gray;
            }
            else
            {
                CardNumber.BorderColor = Color.Red;
                CardNumber.AnnotationMessage = "หมายเลขบนบัตร 16 หลัก";
            }
        }

        private void bntTest_Clicked(object sender, EventArgs e)
        {
            var cardNumber = CardNumber.Text;
            var inputTemp = tempInput.Text;
            //Navigation.PushAsync(new TempPage(cardNumber,inputTemp));

        }
    }
}