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

namespace MTYD.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryBilling : ContentPage
    {
        protected async Task setPaymentInfo()
        {
            Console.WriteLine("SetPaymentInfo Func Started!");
            PaymentInfo newPayment = new PaymentInfo();

            Item item1 = new Item();
            item1.name = Preferences.Get("item_name", "");
            item1.price = Preferences.Get("price", "00.00");
            item1.qty = "1";
            item1.item_uid = Preferences.Get("item_uid", ""); ;
            List<Item> itemsList = new List<Item> { item1 };
            Preferences.Set("unitNum", AptEntry.Text);

            newPayment.customer_uid = "100-000082";
            newPayment.business_uid = "200-000001";
            newPayment.items = itemsList;
            //newPayment.salt = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71";
            newPayment.salt = "cec35d4fc0c5e83527f462aeff579b0c6f098e45b01c8b82e311f87dc6361d752c30293e27027653adbb251dff5d03242c8bec68a3af1abd4e91c5adb799a01b";
            //newPayment.salt = "2020-09-22 21:55:17";
            newPayment.delivery_first_name = FNameEntry.Text;
            newPayment.delivery_last_name = LNameEntry.Text;
            newPayment.delivery_email = emailEntry.Text;
            newPayment.delivery_phone = PhoneEntry.Text;
            newPayment.delivery_address = AddressEntry.Text;
            newPayment.delivery_unit = Preferences.Get("unitNum", "");
            newPayment.delivery_city = CityEntry.Text;
            newPayment.delivery_state = StateEntry.Text;
            newPayment.delivery_zip = ZipEntry.Text;
            newPayment.delivery_instructions = DeliveryEntry.Text;
            newPayment.delivery_longitude = "";
            newPayment.delivery_latitude = "";
            newPayment.order_instructions = "slow";
            newPayment.purchase_notes = "new purch";
            newPayment.amount_due = Preferences.Get("price", "00.00");
            newPayment.amount_discount = "00.00";
            newPayment.amount_paid = Preferences.Get("price", "00.00");
            newPayment.cc_num = CCEntry.Text;
            //newPayment.cc_exp_year = YearPicker.Items[YearPicker.SelectedIndex];
            newPayment.cc_exp_year = "2022";
            //newPayment.cc_exp_month = MonthPicker.Items[MonthPicker.SelectedIndex];
            newPayment.cc_exp_month = "11";
            newPayment.cc_cvv = CVVEntry.Text;
            newPayment.cc_zip = ZipCCEntry.Text;

            //itemsList.Add("1"); //{ "1", "5 Meal Plan", "59.99" };
            var newPaymentJSONString = JsonConvert.SerializeObject(newPayment);
            // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
            var content = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
            Console.WriteLine("Content: " + content);
            /*var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
            request.Method = HttpMethod.Post;
            request.Content = content;*/
            var client = new HttpClient();
            var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout", content);
            // HttpResponseMessage response = await client.SendAsync(request);
            Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
            Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
            Console.WriteLine("SetPaymentInfo Func ENDED!");
        }
        public DeliveryBilling()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.CornerRadius = 35;
                pfp.CornerRadius = 20;
                firstName.CornerRadius = 22;
                lastName.CornerRadius = 22;
                emailAdd.CornerRadius = 22;
                street.CornerRadius = 22;
                unit.CornerRadius = 22;
                city.CornerRadius = 22;
                state.CornerRadius = 22;
                zipCode.CornerRadius = 22;
                phoneNum.CornerRadius = 22;
                deliveryInstr.CornerRadius = 22;
                creditCard.CornerRadius = 22;
                cvv.CornerRadius = 22;
                zipCode2.CornerRadius = 22;
                month.CornerRadius = 22;
                year.CornerRadius = 22;
                SignUpButton.CornerRadius = 25;
            }
        }

        /*
        private void clickedMeals1(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.Green;
            meals2.BackgroundColor = Color.Transparent;
            meals3.BackgroundColor = Color.Transparent;
            meals4.BackgroundColor = Color.Transparent;
            // TotalPrice.Text = "$05.00";
        }
        private void clickedMeals2(object sender, EventArgs e)
        {
            meals2.BackgroundColor = Color.Green;
            meals1.BackgroundColor = Color.Transparent;
            meals3.BackgroundColor = Color.Transparent;
            meals4.BackgroundColor = Color.Transparent;
            // TotalPrice.Text = "$10.00";
        }

        private void clickedMeals3(object sender, EventArgs e)
        {
            meals3.BackgroundColor = Color.Green;
            meals1.BackgroundColor = Color.Transparent;
            meals2.BackgroundColor = Color.Transparent;
            meals4.BackgroundColor = Color.Transparent;
            //  TotalPrice.Text = "$15.00";
        }

        private void clickedMeals4(object sender, EventArgs e)
        {
            meals4.BackgroundColor = Color.Green;
            meals1.BackgroundColor = Color.Transparent;
            meals3.BackgroundColor = Color.Transparent;
            meals2.BackgroundColor = Color.Transparent;
            // TotalPrice.Text = "$20.00";
        }*/

        private async void clickedDone(object sender, EventArgs e)
        {
            setPaymentInfo();
            Navigation.PushAsync(new Select());
            //MainPage = PaymentPage();
        }

        async void clickedPfp(System.Object sender, System.EventArgs e) {
            await Navigation.PushAsync(new UserProfile()); 
        }

        async void clickedMenu(System.Object sender, System.EventArgs e) {
            await Navigation.PushAsync(new Menu()); 
        }

        void clickedNotDone(object sender, EventArgs e)
        {
            if (FNameEntry.Text == null || FNameEntry.Text == "")
            {
                DisplayAlert("Warning!", "first name required", "okay");
                return;
            }

            if (LNameEntry.Text == null || LNameEntry.ToString() == "")
            {
                DisplayAlert("Warning!", "last name required", "okay");
                return;
            }

            if (emailEntry.Text == null || emailEntry.Text == "")
            {
                DisplayAlert("Warning!", "email required", "okay");
                return;
            }

            if (AddressEntry.Text == null || AddressEntry.Text == "")
            {
                DisplayAlert("Warning!", "address required", "okay");
                return;
            }

            if (CityEntry.Text == null || CityEntry.Text == "")
            {
                DisplayAlert("Warning!", "city required", "okay");
                return;
            }

            if (StateEntry.Text == null || StateEntry.Text == "")
            {
                DisplayAlert("Warning!", "state required", "okay");
                return;
            }

            if (ZipEntry.Text == null || ZipEntry.Text == "")
            {
                DisplayAlert("Warning!", "address zip code required", "okay");
                return;
            }

            if (StateEntry.Text == null || StateEntry.Text == "")
            {
                DisplayAlert("Warning!", "state required", "okay");
                return;
            }

            if (ZipEntry.Text == null || ZipEntry.Text == "")
            {
                DisplayAlert("Warning!", "address zip code required", "okay");
                return;
            }

            if (PhoneEntry.Text == null || PhoneEntry.Text == "")
            {
                DisplayAlert("Warning!", "phone number required", "okay");
                return;
            }

            if (CCEntry.Text == null || CCEntry.Text == "")
            {
                DisplayAlert("Warning!", "credit card number required", "okay");
                return;
            }

            if (CVVEntry.Text == null || CVVEntry.Text == "")
            {
                DisplayAlert("Warning!", "CVV required", "okay");
                return;
            }

            if (ZipCCEntry.Text == null || ZipCCEntry.Text == "")
            {
                DisplayAlert("Warning!", "credit card zip code required", "okay");
                return;
            }

            if (MonthPicker.SelectedIndex == -1)
            {
                DisplayAlert("Warning!", "select a month", "okay");
                return;
            }

            if (YearPicker.SelectedIndex == -1)
            {
                DisplayAlert("Warning!", "select a year", "okay");
                return;
            }

            clickedDone(sender, e);
        }

        async void clickedBack(System.Object sender, System.EventArgs e) {
            await Navigation.PopAsync(false); 
        }

        /*//Saving CC and Address Info
        private void clickedSaveCC(object sender, EventArgs e)
        {
            if (SaveCC.BackgroundColor != Color.Green)
            {
                SaveCC.BackgroundColor = Color.Green;
            }
            else
            {
                SaveCC.BackgroundColor = Color.Transparent;
            }
        }

        private void clickedSaveAdd(object sender, EventArgs e)
        {
            if (SaveAdd.BackgroundColor != Color.Green)
            {
                SaveAdd.BackgroundColor = Color.Green;
            }
            else
            {
                SaveAdd.BackgroundColor = Color.Transparent;
            }
        }*/
    }
}