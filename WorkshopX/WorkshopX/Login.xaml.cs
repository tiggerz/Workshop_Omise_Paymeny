using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.InputKit.Shared.Controls;
using WorkshopX.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkshopX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        private const string authServer = "https://x8ona2vu4c.execute-api.ap-southeast-1.amazonaws.com/Prod/api/auth";
        static HttpClient client = new HttpClient();
        MakeDataModel makeData = new MakeDataModel();

        public Login(MakeDataModel model)
        {
            InitializeComponent();
            //Application.Current.Properties.Clear();
            makeData = model;
        }
        public bool ChechInput(string model)
        {
            var userInput = model.ToUpper();
            if (userInput.Contains("DROP") || userInput.Contains("INSERT") || userInput.Contains("DELETE") || userInput.Contains("UPDATE") || userInput.Contains("EXECUTE") || userInput.Contains("CREATE"))
            {
                return false;
            }
            return true;
        }
        public async Task GetToken(MemberLoginModel model)
        {
            try
            {
                var objModel = new
                {
                    username = model.UserName,
                    userPassword = model.Password
                };

                StringContent requestMessage = new StringContent($"{JsonConvert.SerializeObject(objModel)}", Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(authServer, requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var bodyDeserialize = JsonConvert.DeserializeObject<TokenModel>(body);

                    Application.Current.Properties["AccessToken"] = bodyDeserialize.AccessToken;
                    await DisplayAlert("Done", "Authentication สำเร็จ", "OK");

                }
            }
            catch (Exception)
            {
                ////HttpContext.Session.Clear();
                //ViewBag.m = "Can't connect to Authentication Server";
                await DisplayAlert("Alert", "ไม่สามารถเชื่อมต่อกับ Authentication Server", "OK");
                //return RedirectToAction("Login");
                //// ถ้า connect token server ไม่ได้ จะ error ตรงนี้
            }
        }
        private async void Login_Clicked(object sender, EventArgs e)
        {
            try
            {
                var username = userName.Text;
                var password = loginPassword.Text;
                var firstname = firstName.Text;
                var lastname = lastName.Text;
                var regEmail = email.Text;
                var mobile = mobilePhone.Text;

                loading.IsVisible = true;
                btnLogin.IsVisible = false;

                //check input is null ?
                //if (username == null || password == null || username == "" || password == "")
                //{
                //    //do something if uername or password is null!
                //    await DisplayAlert("Alert", "กรุณากรอก ชื่อผู้ใช้งาน และรหัสผ่าน", "OK");
                //    //loading.IsVisible = false;
                //    //btnLogin.IsVisible = true;
                //}
                //if (firstName.Text =="")
                //{

                //}
                //if (userName.Text !="" && loginPassword.Text !="")
                //{

                //}
                if (firstName.Text != "" && lastName.Text != "" && email.Text != "" && mobilePhone.Text != "" && RegisterPassword.Text != "")
                {
                    username = email.Text;
                    password = RegisterPassword.Text;
                }
                //else
                //{
                SHA256 sha256has = SHA256.Create();
                //Hast
                byte[] bytes = sha256has.ComputeHash(Encoding.UTF8.GetBytes(password));
                // convert byte array to string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                string hashPass = builder.ToString();

                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://z9mviw2582.execute-api.ap-southeast-1.amazonaws.com/Prod/api/v1/Member/");
                        //client.BaseAddress = new Uri("http://192.168.42.51:5000/api/v1/Member/");

                        MemberLoginModel objModel = new MemberLoginModel
                        {
                            UserName = username,
                            Password = hashPass,
                            FirstName = firstname,
                            LastName = lastname,
                            Email = regEmail,
                            MobileNo = mobile
                        };

                        var login = JsonConvert.SerializeObject(objModel);
                        var content = new StringContent(login, Encoding.UTF8, "application/json");
                        //var response = client.PostAsync("MemberLogin", content).Result;
                        var response = await client.PostAsync("MemberLogin", content);


                        //if no connection ? "response" return ????
                        if (response.IsSuccessStatusCode)
                        {
                            await GetToken(objModel);

                            var responseString = response.Content.ReadAsStringAsync().Result;
                            MemberLoginModel loginlist = new MemberLoginModel();
                            loginlist = JsonConvert.DeserializeObject<MemberLoginModel>(responseString);

                            //await DisplayAlert("Alert", "userId : " + loginlist.MemberId + " UserName : " + loginlist.UserName, "OK");
                            await DisplayAlert("Done", "เข้าสู่ระบบสำเร็จ", "OK");
                            makeData.MemberId = loginlist.MemberId;
                            await Navigation.PushAsync(new Policy(makeData));
                            loading.IsVisible = false;
                            btnLogin.IsVisible = true;
                        }
                        else
                        {
                            var responseString = response.Content.ReadAsStringAsync().Result;
                            await DisplayAlert("WARRING", responseString, "OK");
                            ////if login fail ! do something here!
                            loading.IsVisible = false;
                            btnLogin.IsVisible = true;
                        }

                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("WARRING", "ไม่สามารถเชื่อมต่อกับระบบได้ " + ex.Message, "OK");
                    loading.IsVisible = false;
                    btnLogin.IsVisible = true;
                }
            }
            catch (Exception)
            {
                await DisplayAlert("WARRING", "กรุณากรอกข้อมูล", "OK");
                loading.IsVisible = false;
                btnLogin.IsVisible = true;
            }
            
            //}
        }

        private void userName_TextChanged(object sender, TextChangedEventArgs e)
        {
            firstName.Text = "";
            lastName.Text = "";
            email.Text = "";
            mobilePhone.Text = "";
            RegisterPassword.Text = "";
            Match checkRegex = Regex.Match(userName.Text, @"^((?!\.)[\w-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$");
            bool reservedWord = ChechInput(userName.Text);
            if (checkRegex.Success && reservedWord == true)
            {
                userName.BorderColor = Color.Gray;
                
            }
            else if (userName.Text == "")
            {
                userName.BorderColor = Color.Gray;
                userName.AnnotationMessage = "กรุณาระบุชื่อ";
            }
            else
            {
                userName.BorderColor = Color.Red;
                userName.AnnotationMessage = "รูปแบบตัวอักษรไม่ถูกต้อง";
            }
            
        }

        private void loginPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loginPassword.Text != "")
            {
                loginPassword.BorderColor = Color.Gray;
            }
            else if (loginPassword.Text == "")
            {
                loginPassword.BorderColor = Color.Gray;
                loginPassword.AnnotationMessage = "กรุณาระบุ รหัสผ่าน";
            }
            else
            {
                loginPassword.BorderColor = Color.Red;
                loginPassword.AnnotationMessage = "รูปแบบตัวอักษรไม่ถูกต้อง";
            }
        }

        private void firstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            userName.Text = "";
            loginPassword.Text = "";
            Match checkRegex = Regex.Match(firstName.Text, @"^[a-zA-Z,ก-๙]{1,20}$");
            bool reservedWord = ChechInput(firstName.Text);
            if (checkRegex.Success && reservedWord == true)
                {
                firstName.BorderColor = Color.Gray;
                userName.Text = "";
                loginPassword.Text = "";
            }
            else if (firstName.Text == "")
            {
                firstName.BorderColor = Color.Gray;
                firstName.AnnotationMessage = "กรุณาระบุชื่อ";
            }
            else
            {
                firstName.BorderColor = Color.Red;
                firstName.AnnotationMessage = "รูปแบบตัวอักษรไม่ถูกต้อง";
            }

        }

        private void lastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Match checkRegex = Regex.Match(lastName.Text, @"^[a-zA-Z,ก-๙]{1,20}$");
            bool reservedWord = ChechInput(lastName.Text);
            if (checkRegex.Success && reservedWord == true)
                {
                lastName.BorderColor = Color.Gray;
            }
            else if (lastName.Text == "")
            {
                lastName.BorderColor = Color.Gray;
                lastName.AnnotationMessage = "กรุณาระบุ นามสกุล";
            }
            else
            {
                lastName.BorderColor = Color.Red;
                lastName.AnnotationMessage = "รูปแบบตัวอักษรไม่ถูกต้อง";
            }
        }

        private void email_TextChanged(object sender, TextChangedEventArgs e)
        {
            Match checkRegex = Regex.Match(email.Text, @"^((?!\.)[\w-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$");
            bool reservedWord = ChechInput(email.Text);
            if (checkRegex.Success && reservedWord == true)
                {
                email.BorderColor = Color.Gray;
            }
            else if (email.Text == "")
            {
                email.BorderColor = Color.Gray;
                email.AnnotationMessage = "กรุณาระบุ อีเมลล์";
            }
            else
            {
                email.BorderColor = Color.Red;
                email.AnnotationMessage = "รูปแบบตัวอักษรไม่ถูกต้อง";
            }
        }

        private void mobilePhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            Match checkRegex = Regex.Match(mobilePhone.Text, @"^0[0-9]{9}$");
            if (checkRegex.Success)
            {
                mobilePhone.BorderColor = Color.Gray;
            }
            else if (mobilePhone.Text == "")
            {
                mobilePhone.BorderColor = Color.Gray;
                mobilePhone.AnnotationMessage = "กรุณาระบุ อีเมลล์";
            }
            else
            {
                mobilePhone.BorderColor = Color.Red;
                mobilePhone.AnnotationMessage = "รูปแบบตัวอักษรไม่ถูกต้อง";
            }

        }
        private void RegisterPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RegisterPassword.Text !="")
            {
                RegisterPassword.BorderColor = Color.Gray;
            }
            else if (RegisterPassword.Text == "")
            {
                RegisterPassword.BorderColor = Color.Gray;
                RegisterPassword.AnnotationMessage = "กรุณาระบุ รหัสผ่าน";
            }
            else
            {
                RegisterPassword.BorderColor = Color.Red;
            }
        }
    }
}