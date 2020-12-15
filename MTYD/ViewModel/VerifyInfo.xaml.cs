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
    public partial class VerifyInfo : ContentPage
    {
        public ObservableCollection<Plans> NewDeliveryInfo = new ObservableCollection<Plans>();
        public string AptEntry, FNameEntry, LNameEntry, emailEntry, PhoneEntry, AddressEntry, CityEntry, StateEntry, ZipEntry, DeliveryEntry, CCEntry, CVVEntry, ZipCCEntry;
        public bool isSocialLogin;

        public VerifyInfo(string AptEntry1, string FNameEntry1, string LNameEntry1, string emailEntry1, string PhoneEntry1, string AddressEntry1, string CityEntry1, string StateEntry1, string ZipEntry1, string DeliveryEntry1, string CCEntry1, string CVVEntry1, string ZipCCEntry1, string salt1)
        {
            InitializeComponent();
            if (salt1 == "")
            {
                isSocialLogin = true;
            }
            else
            {
                isSocialLogin = false;
            }

            AptEntry = AptEntry1; FNameEntry = FNameEntry1; LNameEntry = LNameEntry1; emailEntry = emailEntry1; PhoneEntry = PhoneEntry1; AddressEntry = AddressEntry1; CityEntry = CityEntry1; StateEntry = StateEntry1; ZipEntry = ZipEntry1; DeliveryEntry = DeliveryEntry1; CCEntry = CCEntry1; CVVEntry = CVVEntry1; ZipCCEntry = ZipCCEntry1;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            //cardNum.Text = "**************" + CCEntry1.Substring(CCEntry1.Length - 2);
            name.Text = FNameEntry1 + " " + LNameEntry1;
            if (AptEntry1 == "NULL" || AptEntry1 == "")
            {
                apt.IsVisible = false;
                street.Text = AddressEntry1;
                cityStateZip.Text = CityEntry1 + ", " + StateEntry1 + " " + ZipEntry1;
            }
            else
            {
                apt.Text = "Apt #" + AptEntry1;
                street.Text = AddressEntry1;
                cityStateZip.Text = CityEntry1 + ", " + StateEntry1 + " " + ZipEntry1;
            }


            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.CornerRadius = 35;
                pfp.CornerRadius = 20;
                //password.CornerRadius = 22;
                checkoutButton.CornerRadius = 24;
            }
        }

        protected async Task setPaymentInfo()
        {
            Console.WriteLine("SetPaymentInfo Func Started!");
            PaymentInfo newPayment = new PaymentInfo();
            //need to add item_business_id
            Item item1 = new Item();
            item1.name = Preferences.Get("item_name", "");
            item1.price = Preferences.Get("price", "00.00");
            item1.qty = "1";
            item1.item_uid = Preferences.Get("item_uid", "");
            item1.itm_business_uid = "200-000001";
            List<Item> itemsList = new List<Item> { item1 };
            Preferences.Set("unitNum", AptEntry);

            string userID = (string)Application.Current.Properties["user_id"];
            Console.WriteLine("YOUR userID is " + userID);
            newPayment.customer_uid = userID;
            //newPayment.customer_uid = "100-000082";
            //newPayment.business_uid = "200-000002";
            newPayment.items = itemsList;
            //newPayment.salt = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71";
            //newPayment.salt = "cec35d4fc0c5e83527f462aeff579b0c6f098e45b01c8b82e311f87dc6361d752c30293e27027653adbb251dff5d03242c8bec68a3af1abd4e91c5adb799a01b";
            //newPayment.salt = "2020-09-22 21:55:17";
            newPayment.salt = "";
            newPayment.delivery_first_name = FNameEntry;
            newPayment.delivery_last_name = LNameEntry;
            newPayment.delivery_email = emailEntry;
            newPayment.delivery_phone = PhoneEntry;
            newPayment.delivery_address = AddressEntry;
            newPayment.delivery_unit = Preferences.Get("unitNum", "");
            newPayment.delivery_city = CityEntry;
            newPayment.delivery_state = StateEntry;
            newPayment.delivery_zip = ZipEntry;
            newPayment.delivery_instructions = DeliveryEntry;
            newPayment.delivery_longitude = "";
            newPayment.delivery_latitude = "";
            newPayment.order_instructions = "slow";
            newPayment.purchase_notes = "new purch";
            newPayment.amount_due = Preferences.Get("price", "00.00");
            newPayment.amount_discount = "00.00";
            newPayment.amount_paid = Preferences.Get("price", "00.00");
            newPayment.cc_num = CCEntry;
            //newPayment.cc_exp_year = YearPicker.Items[YearPicker.SelectedIndex];
            newPayment.cc_exp_year = "2022";
            //newPayment.cc_exp_month = MonthPicker.Items[MonthPicker.SelectedIndex];
            newPayment.cc_exp_month = "11";
            newPayment.cc_cvv = CVVEntry;
            newPayment.cc_zip = ZipCCEntry;

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
        
        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile());
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu("", ""));
        }

        private void checkoutButton_Clicked(object sender, EventArgs e)
        {
            setPaymentInfo();
            Navigation.PushAsync(new Select("", "", ""));
        }
    }
}
