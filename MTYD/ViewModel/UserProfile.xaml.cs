using MTYD.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Security.Cryptography;

namespace MTYD.ViewModel
{
    public partial class UserProfile : ContentPage
    {
        public ObservableCollection<Plans> customerProfileInfo = new ObservableCollection<Plans>();
        public ObservableCollection<PaymentInfo> NewPlan = new ObservableCollection<PaymentInfo>();
        PaymentInfo orderInfo;
        ArrayList itemsArray = new ArrayList();
        ArrayList purchIdArray = new ArrayList();
        ArrayList namesArray = new ArrayList();
        JObject info_obj;

        public UserProfile()
        {
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
            getInfo();
        }

        public void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                heading.FontSize = width / 32;
                heading.Margin = new Thickness(0, 0, 0, 30);
                pfp.HeightRequest = width / 20;
                pfp.WidthRequest = width / 20;
                pfp.CornerRadius = (int)(width / 40);
                pfp.Margin = new Thickness(0, 0, 23, 27);
                menu.HeightRequest = width / 25;
                menu.WidthRequest = width / 25;
                menu.Margin = new Thickness(25, 0, 0, 30);

                mainPfp.HeightRequest = width / 6;
                mainPfp.WidthRequest = width / 6;
                mainPfp.CornerRadius = (int)(width / 12);

                customerInfo.FontSize = width / 38;
                customerInfo.Margin = new Thickness(width / 50, 0);

                FName.CornerRadius = 21;
                FName.Margin = new Thickness(width / 50, 0.5, 0, 0.5);
                LName.CornerRadius = 21;
                LName.Margin = new Thickness(2, 0.5, width / 50, 0.5);

                emailAdd.CornerRadius = 21;
                emailAdd.Margin = new Thickness(width / 50, 0.5);
                confirmEmailAdd.CornerRadius = 21;
                confirmEmailAdd.Margin = new Thickness(width / 50, 0.5);

                street.CornerRadius = 21;
                street.Margin = new Thickness(width / 50, 0.5);
                unit.CornerRadius = 21;
                unit.Margin = new Thickness(width / 50, 0.5, 0, 0.5);
                city.CornerRadius = 21;
                city.Margin = new Thickness(2, 0.5, 2, 0.5);
                state.CornerRadius = 21;
                state.Margin = new Thickness(0, 0.5, width / 50, 0.5);

                zipCode.CornerRadius = 21;
                zipCode.Margin = new Thickness(width / 50, 0.5, 0, 0.5);
                phoneNum.CornerRadius = 21;
                phoneNum.Margin = new Thickness(2, 0.5, width / 50, 0.5);

                FNameEntry.FontSize = width / 45;
                LNameEntry.FontSize = width / 45;
                emailEntry.FontSize = width / 45;
                confirmEmailEntry.FontSize = width / 45;
                AddressEntry.FontSize = width / 45;
                AptEntry.FontSize = width / 45;
                CityEntry.FontSize = width / 45;
                StateEntry.FontSize = width / 45;
                ZipEntry.FontSize = width / 45;
                PhoneEntry.FontSize = width / 45;

                saveCustomerInfo.HeightRequest = height / 40;
                saveCustomerInfo.CornerRadius = (int)(height / 80);
                saveCustomerInfo.FontSize = width / 38;
                saveCustomerInfo.Margin = new Thickness(width / 50, 0);

                passwordHeading.FontSize = width / 38;
                passwordHeading.Margin = new Thickness(width / 50, 0);

                password.CornerRadius = 21;
                password.Margin = new Thickness(width / 50, 0.5);
                confirmPassword.CornerRadius = 21;
                confirmPassword.Margin = new Thickness(width / 50, 0.5);

                passwordEntry.FontSize = width / 45;
                confirmPasswordEntry.FontSize = width / 45;

                savePass.HeightRequest = height / 40;
                savePass.CornerRadius = (int)(height / 80);
                savePass.FontSize = width / 38;
                savePass.Margin = new Thickness(width / 50, 0);
            }
            else //android
            {

            }
        }

        async void getInfo()
        {
            var request = new HttpRequestMessage();
            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
            request.RequestUri = new Uri(url);
            //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                Console.WriteLine("content: " + content);
                var userString = await content.ReadAsStringAsync();
                info_obj = JObject.Parse(userString);
                this.customerProfileInfo.Clear();

                Console.WriteLine("user social media: " + (info_obj["result"])[0]["user_social_media"].ToString());

                if ((info_obj["result"])[0]["user_social_media"].ToString() != "NULL")
                {
                    Console.WriteLine("social media login");

                    passwordHeading.IsVisible = false;
                    divider6.IsVisible = false;
                    passwordGrid.IsVisible = false;
                    confirmPasswordGrid.IsVisible = false;
                    divider4.IsVisible = false;
                    savePass.IsVisible = false;
                    divider5.IsVisible = false;
                    Email.IsVisible = false;
                    confirmEmail.IsVisible = false;
                }
                else emailEntry.Text = (info_obj["result"])[0]["customer_email"].ToString();

                FNameEntry.Text = (info_obj["result"])[0]["customer_first_name"].ToString();
                LNameEntry.Text = (info_obj["result"])[0]["customer_last_name"].ToString();
                emailEntry.Text = (info_obj["result"])[0]["customer_email"].ToString();
                AddressEntry.Text = (info_obj["result"])[0]["customer_address"].ToString();
                AptEntry.Text = (info_obj["result"])[0]["customer_unit"].ToString();

                if (AptEntry.Text == "NULL")
                    AptEntry.Text = "";

                CityEntry.Text = (info_obj["result"])[0]["customer_city"].ToString();
                StateEntry.Text = (info_obj["result"])[0]["customer_state"].ToString();
                ZipEntry.Text = (info_obj["result"])[0]["customer_zip"].ToString();
                PhoneEntry.Text = (info_obj["result"])[0]["customer_phone_num"].ToString();

            }
        }


        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu("", ""));
        }

        async void clickedSave(System.Object sender, System.EventArgs e)
        {
            ProfileInfo profileUpdate = new ProfileInfo();

            //uid, first_name, last_name, phone, email, address, unit, city, state, zip, noti
            profileUpdate.uid = (info_obj["result"])[0]["customer_uid"].ToString();
            profileUpdate.first_name = FNameEntry.Text;
            profileUpdate.last_name = LNameEntry.Text;
            profileUpdate.phone = PhoneEntry.Text;
            profileUpdate.email = emailEntry.Text;
            profileUpdate.address = AddressEntry.Text;
            profileUpdate.unit = AptEntry.Text;
            profileUpdate.city = CityEntry.Text;
            profileUpdate.state = StateEntry.Text;
            profileUpdate.zip = ZipEntry.Text;
            profileUpdate.noti = "false";

            var newPaymentJSONString = JsonConvert.SerializeObject(profileUpdate);
            // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
            var content2 = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
            Console.WriteLine("Content: " + content2);
            var client = new HttpClient();
            var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/UpdateProfile", content2);
            Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
            Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
            Console.WriteLine("clickedSave Func ENDED!");
            DisplayAlert("Success", "information updated!", "close");
            //await Navigation.PushAsync(new UserProfile(), false);
        }

        async void clickedSavePassword(System.Object sender, System.EventArgs e)
        {
            PasswordInfo passwordUpdate = new PasswordInfo();

            //customer_uid, old_password, new_password
            passwordUpdate.customer_uid = (info_obj["result"])[0]["customer_uid"].ToString();

            SHA512 sHA512 = new SHA512Managed();
            byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry.Text + (info_obj["result"])[0]["password_salt"].ToString())); // take the password and account salt to generate hash
            string hashedPassword = BitConverter.ToString(data).Replace("-", string.Empty).ToLower(); // convert hash to hex
            //passwordUpdate.old_password = hashedPassword;

            if (passwordEntry.Text == confirmPasswordEntry.Text)
            {
                //passwordUpdate.old_password = Preferences.Get("hashed_password", "");
                passwordUpdate.old_password = Preferences.Get("user_password", "");
                //passwordUpdate.new_password = hashedPassword;
                passwordUpdate.new_password = passwordEntry.Text;
                var newPaymentJSONString = JsonConvert.SerializeObject(passwordUpdate);
                // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
                var content2 = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
                Console.WriteLine("Content: " + content2);
                var client = new HttpClient();
                var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_password", content2);
                DisplayAlert("Success", "password updated!", "close");
                Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
                Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                Console.WriteLine("clickedSave Func ENDED!");
            }
            else DisplayAlert("Error", "passwords don't match", "close");


            

            //await Navigation.PushAsync(new UserProfile(), false);
        }

    }
}
