﻿using MTYD.Model;
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
    public partial class MealPlans : ContentPage
    {
        public ObservableCollection<Plans> userProfileInfo = new ObservableCollection<Plans>();
        public ObservableCollection<PaymentInfo> NewPlan = new ObservableCollection<PaymentInfo>();
        PaymentInfo orderInfo;
        ArrayList itemsArray = new ArrayList();
        ArrayList purchIdArray = new ArrayList();
        ArrayList namesArray = new ArrayList();
        JObject info_obj;
        string frequency;
        int lastPickerIndex;
        int chosenIndex;

        public MealPlans()
        {
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
            getMealsSelected();
            GetMealPlans();
        }

        public async void getFrequency()
        {
            var request = new HttpRequestMessage();
            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/next_billing_date?customer_uid=" + (string)Application.Current.Properties["user_id"];
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
                var freq_obj = JObject.Parse(userString);
                this.userProfileInfo.Clear();

                frequency = (freq_obj["result"])[planPicker.SelectedIndex]["payment_frequency"].ToString();
                Console.WriteLine("frequency: " + (freq_obj["result"])[planPicker.SelectedIndex]["payment_frequency"].ToString());
                if (frequency == "2")
                {
                    freq.Text = "2 WEEKS";
                    ticketPic.Source = "Discount5.png";
                }
                else if (frequency == "4")
                {
                    freq.Text = "4 WEEKS";
                    ticketPic.Source = "Discount10.png";
                }
                else
                {
                    freq.Text = "WEEKLY";
                    ticketPic.Source = "noDiscount.png";
                }
            }

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

                mealPlanGrid.Margin = new Thickness(width / 40, 10, width / 40, 5);
                selectPlanFrame.Margin = new Thickness(10, 0, 0, 0);
                selectPlanFrame.Padding = new Thickness(15, 5);
                selectPlanFrame.HeightRequest = height / 55;
                planPicker.FontSize = width / 40;
                //planPicker.VerticalOptions = LayoutOptions.Fill;
                planPicker.HorizontalOptions = LayoutOptions.Fill;
                changeMealPlan.Margin = new Thickness(10, 0, 0, 0);
                changeMealPlan.FontSize = width / 40;
                changeMealPlan.HeightRequest = height / 45;
                changeMealPlan.CornerRadius = (int)height / 90;

                mainGrid.Margin = new Thickness(width / 50);
                mainFrame.CornerRadius = 20;
                innerStack.Margin = new Thickness(width / 100);
                delivery.FontSize = width / 38;
                saveInfo.CornerRadius = (int)(height / 80);
                saveInfo.FontSize = width / 38;

                FName.CornerRadius = 21;
                LName.CornerRadius = 21;
                emailAdd.CornerRadius = 21;
                street.CornerRadius = 21;
                unit.CornerRadius = 21;
                city.CornerRadius = 21;
                state.CornerRadius = 21;
                zipCode.CornerRadius = 21;
                phoneNum.CornerRadius = 21;
                FNameEntry.FontSize = width / 45;
                LNameEntry.FontSize = width / 45;
                emailEntry.FontSize = width / 45;
                AddressEntry.FontSize = width / 45;
                AptEntry.FontSize = width / 45;
                CityEntry.FontSize = width / 45;
                StateEntry.FontSize = width / 45;
                ZipEntry.FontSize = width / 45;
                PhoneEntry.FontSize = width / 45;
                //instructionsEntry.FontSize = width / 45;

                pay.FontSize = width / 38;

                card.FontSize = width / 55;
                cardPic.WidthRequest = width / 10;
                cardNum.FontSize = width / 70;

                freq.FontSize = width / 55;
                ticketPic.WidthRequest = width / 10;
                ticketPic.HeightRequest = width / 10;
            }
            else //android
            {

            }
        }

        //auto-populate the delivery info if the user has already previously entered it
        public async void getMealsSelected()
        {
            Console.WriteLine("fillEntries entered");
            var request = new HttpRequestMessage();
            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
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

                if (userString.ToString()[0] != '{')
                {
                    Console.WriteLine("no meal plans");
                    return;
                }

                info_obj = JObject.Parse(userString);
                this.userProfileInfo.Clear();
                //Console.WriteLine("info_obj: " + info_obj);

                ////ArrayList item_price = new ArrayList();
                ////ArrayList num_items = new ArrayList();
                ////ArrayList payment_frequency = new ArrayList();
                ////ArrayList groupArray = new ArrayList();

                //if ((info_obj["result"]).ToString() == "[]")
                //{
                //    Console.WriteLine("no info");

                //    FNameEntry.Placeholder = "First Name*";
                //    LNameEntry.Placeholder = "Last Name*";
                //    emailEntry.Placeholder = "Email*";
                //    AddressEntry.Placeholder = "Street*";
                //    AptEntry.Placeholder = "Unit";
                //    CityEntry.Placeholder = "City*";
                //    StateEntry.Placeholder = "State*";
                //    ZipEntry.Placeholder = "Zip*";
                //    PhoneEntry.Placeholder = "Phone Number*";


                //return;
                //}

                //Console.WriteLine("delivery first name: " + (info_obj["result"])[0]["selection_uid"]);
                //FNameEntry.Text = (info_obj["result"])[0]["delivery_first_name"].ToString();
                //if (FNameEntry.Text == "")
                //    FNameEntry.Text = "First Name*";

                //LNameEntry.Text = (info_obj["result"])[0]["delivery_last_name"].ToString();
                //if (LNameEntry.Text == "")
                //    LNameEntry.Text = "Last Name*";

                //emailEntry.Text = (info_obj["result"])[0]["delivery_email"].ToString();
                //if (emailEntry.Text == "")
                //    emailEntry.Text = "Email*";

                //AddressEntry.Text = (info_obj["result"])[0]["delivery_address"].ToString();
                //if (AddressEntry.Text == "")
                //    AddressEntry.Text = "Street*";

                //AptEntry.Text = (info_obj["result"])[0]["delivery_unit"].ToString();
                //if (AptEntry.Text == "")
                //    AptEntry.Text = "Unit";

                //CityEntry.Text = (info_obj["result"])[0]["delivery_city"].ToString();
                //if (CityEntry.Text == "")
                //    CityEntry.Text = "City*";

                //StateEntry.Text = (info_obj["result"])[0]["delivery_state"].ToString();
                //if (StateEntry.Text == "")
                //    StateEntry.Text = "State*";

                //ZipEntry.Text = (info_obj["result"])[0]["delivery_zip"].ToString();
                //if (ZipEntry.Text == "")
                //    ZipEntry.Text = "Zip*";

                //PhoneEntry.Text = (info_obj["result"])[0]["delivery_phone_num"].ToString();
                //if (PhoneEntry.Text == "")
                //    PhoneEntry.Text = "Phone Number*";
            }
        }

        private async void planChange(object sender, EventArgs e)
        {
            Console.WriteLine("planChange entered");
            selectPlanFrame.BackgroundColor = Color.FromHex("#FF6505");
            coverPickerBorder.BorderColor = Color.FromHex("#FF6505");
            planPicker.TextColor = Color.White;
            planPicker.BackgroundColor = Color.FromHex("#FF6505");

            Console.WriteLine("before frequency " + frequency);
            getFrequency();

            Console.WriteLine("after frequency " + frequency);

            if ((info_obj["result"]).ToString() == "[]")
            {
                return;
            }

            FNameEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_first_name"].ToString();

            LNameEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_last_name"].ToString();
            emailEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_email"].ToString();
            AddressEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_address"].ToString();
            AptEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_unit"].ToString();

            if (AptEntry.Text == "NULL")
            {
                AptEntry.Text = "";
            }

            CityEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_city"].ToString();
            StateEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_state"].ToString();
            ZipEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_zip"].ToString();
            PhoneEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_phone_num"].ToString();
            //instructionsEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_instructions"].ToString();

            string creditCardNum = (info_obj["result"])[planPicker.SelectedIndex]["cc_num"].ToString();
            cardNum.Text = creditCardNum.Substring(creditCardNum.Length - 2);
            cardNum.Text = "**************" + cardNum.Text;

            string itemsStr = (info_obj["result"])[planPicker.SelectedIndex]["items"].ToString();
            Console.WriteLine("items: " + itemsStr);
            Console.WriteLine("name: " + itemsStr.Substring(itemsStr.IndexOf("itm_business_uid") + 20, 10));
            Console.WriteLine("item_uid: " + itemsStr.Substring(itemsStr.IndexOf("item_uid") + 12, 10));
            //Console.WriteLine("name: " + )

            //orderInfo.customer_uid = (info_obj["result"])[planPicker.SelectedIndex]["customer_uid"].ToString();
            //orderInfo.business_uid = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //Console.WriteLine("items" + ((info_obj["result"])[planPicker.SelectedIndex]["items"]).ToString());
            //orderInfo.salt = (info_obj["result"])[planPicker.SelectedIndex]["salt"].ToString();
            //orderInfo.delivery_first_name = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //orderInfo.business_uid = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //orderInfo.business_uid = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //orderInfo.business_uid = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //    public string customer_uid { get; set; }
            //public string business_uid { get; set; }
            //public List<Item> items { get; set; }
            //public string salt { get; set; }
            //public string delivery_first_name { get; set; }
            //public string delivery_last_name { get; set; }
            //public string delivery_email { get; set; }
            //public string delivery_phone { get; set; }
            //public string delivery_address { get; set; }
            //public string delivery_unit { get; set; }
            //public string delivery_city { get; set; }
            //public string delivery_state { get; set; }
            //public string delivery_zip { get; set; }
            //public string delivery_instructions { get; set; }
            //public string delivery_longitude { get; set; }
            //public string delivery_latitude { get; set; }
            //public string order_instructions { get; set; }
            //public string purchase_notes { get; set; }
            //public string amount_due { get; set; }
            //public string amount_discount { get; set; }
            //public string amount_paid { get; set; }
            //public string cc_num { get; set; }
            //public string cc_exp_year { get; set; }
            //public string cc_exp_month { get; set; }
            //public string cc_cvv { get; set; }
            //public string cc_zip { get; set; }


        }

        protected async Task GetMealPlans()
        {
            Console.WriteLine("ENTER GET MEAL PLANS FUNCTION");
            var request = new HttpRequestMessage();
            string userID = (string)Application.Current.Properties["user_id"];
            Console.WriteLine("Inside GET MEAL PLANS: User ID:  " + userID);

            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + userID);
            Console.WriteLine("GET MEALS PLAN ENDPOINT TRYING TO BE REACHED: " + "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + userID);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var userString = await content.ReadAsStringAsync();

                if (userString.ToString()[0] != '{')
                {
                    Console.WriteLine("no meal plans");
                    return;
                }

                JObject mealPlan_obj = JObject.Parse(userString);
                this.NewPlan.Clear();

                Console.WriteLine("itemsArray contents:");

                foreach (var m in mealPlan_obj["result"])
                {
                    Console.WriteLine("In first foreach loop of getmeal plans func:");

                    itemsArray.Add((m["items"].ToString()));
                    purchIdArray.Add((m["purchase_id"].ToString()));
                }

                lastPickerIndex = purchIdArray.Count - 1;

                Console.WriteLine("size of purchIdArray: " + purchIdArray.Count.ToString());
                for (int i = 0; i < purchIdArray.Count; i++)
                {
                    Console.WriteLine("purchId " + i + ": " + purchIdArray[i]);
                }

                // Console.WriteLine("itemsArray contents:" + itemsArray[0]);

                for (int i = 0; i < itemsArray.Count; i++)
                {
                    JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(itemsArray[i].ToString());

                    Console.WriteLine("Inside forloop before foreach in GetmealsPlan func");

                    foreach (JObject config in newobj)
                    {
                        Console.WriteLine("Inside foreach loop in GetmealsPlan func");
                        //string qty = (string)config["qty"];
                        string name = (string)config["name"];
                        //string price = (string)config["price"];
                        //string mealid = (string)config["item_uid"];

                        namesArray.Add(name);
                    }
                }
                Console.WriteLine("Outside foreach in GetmealsPlan func");
                //Find unique number of meals
                //firstIndex = namesArray[0].ToString();
                //Console.WriteLine("namesArray contents:" + namesArray[0].ToString() + " " + namesArray[1].ToString() + " " + namesArray[2].ToString() + " ");
                planPicker.ItemsSource = namesArray;
                Console.WriteLine("namesArray contents:" + namesArray[0].ToString());
                //SubscriptionPicker.Title = namesArray[0];

                Console.WriteLine("END OF GET MEAL PLANS FUNCTION");
            }
        }

        async void clickedSub(System.Object sender, System.EventArgs e)
        {
            string itemsStr = (info_obj["result"])[planPicker.SelectedIndex]["items"].ToString();
            string expDate = (info_obj["result"])[planPicker.SelectedIndex]["cc_exp_date"].ToString();

            await Navigation.PushAsync(new SubscriptionModal("", (info_obj["result"])[planPicker.SelectedIndex]["mobile_refresh_token"].ToString(), (info_obj["result"])[planPicker.SelectedIndex]["cc_num"].ToString(),
                expDate.Substring(0, 4), expDate.Substring(5, 2),
                (info_obj["result"])[planPicker.SelectedIndex]["cc_cvv"].ToString(), (info_obj["result"])[planPicker.SelectedIndex]["cc_zip"].ToString(), (info_obj["result"])[planPicker.SelectedIndex]["purchase_uid"].ToString(), itemsStr.Substring(itemsStr.IndexOf("itm_business_uid") + 20, 10),
                itemsStr.Substring(itemsStr.IndexOf("item_uid") + 12, 10), (info_obj["result"])[planPicker.SelectedIndex]["pur_customer_uid"].ToString()), false);
        }

        async void clickedInfo(System.Object sender, System.EventArgs e)
        {
            string itemsStr = (info_obj["result"])[planPicker.SelectedIndex]["items"].ToString();
            string expDate = (info_obj["result"])[planPicker.SelectedIndex]["cc_exp_date"].ToString();
            Console.WriteLine("clickedInfo exp date: " + expDate);
            string mealPlan;
            int lengthOfPrice = itemsStr.IndexOf("item_uid") - itemsStr.IndexOf("price") - 13;

            if (Int32.Parse(itemsStr.Substring(itemsStr.IndexOf("name") + 8, 2)) == 5)
            {
                mealPlan = itemsStr.Substring(itemsStr.IndexOf("name") + 8, 11);
            }
            else mealPlan = itemsStr.Substring(itemsStr.IndexOf("name") + 8, 12);



            await Navigation.PushAsync(new OrderInfoModal("", (info_obj["result"])[planPicker.SelectedIndex]["mobile_refresh_token"].ToString(), (info_obj["result"])[planPicker.SelectedIndex]["cc_num"].ToString(),
                expDate.Substring(0, 4), expDate.Substring(5, 2),
                (info_obj["result"])[planPicker.SelectedIndex]["cc_cvv"].ToString(), (info_obj["result"])[planPicker.SelectedIndex]["cc_zip"].ToString(), (info_obj["result"])[planPicker.SelectedIndex]["purchase_uid"].ToString(), itemsStr.Substring(itemsStr.IndexOf("itm_business_uid") + 20, 10),
                mealPlan, itemsStr.Substring(itemsStr.IndexOf("price") + 9, lengthOfPrice), itemsStr.Substring(itemsStr.IndexOf("item_uid") + 12, 10), (info_obj["result"])[planPicker.SelectedIndex]["pur_customer_uid"].ToString()), false);
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu("", ""));
        }

        void LogOutClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.Properties.Remove("user_id");
            Application.Current.Properties.Remove("time_stamp");
            Application.Current.Properties.Remove("platform");
            Application.Current.MainPage = new MainPage();
        }

        async void clickedSave(System.Object sender, System.EventArgs e)
        {
            //    public string customer_uid { get; set; }
            //public string business_uid { get; set; }
            //public string salt { get; set; }
            //public string delivery_first_name { get; set; }
            //public string delivery_last_name { get; set; }
            //public string delivery_email { get; set; }
            //public string delivery_phone { get; set; }
            //public string delivery_address { get; set; }
            //public string delivery_unit { get; set; }
            //public string delivery_city { get; set; }
            //public string delivery_state { get; set; }
            //public string delivery_zip { get; set; }
            //public string delivery_instructions { get; set; }
            DeliveryInfo delivery = new DeliveryInfo();

            delivery.purchase_uid = (info_obj["result"])[planPicker.SelectedIndex]["purchase_uid"].ToString();
            delivery.first_name = FNameEntry.Text;
            delivery.last_name = LNameEntry.Text;
            delivery.email = emailEntry.Text;
            delivery.phone = PhoneEntry.Text;
            delivery.address = AddressEntry.Text;
            //delivery.unit = AptEntry.Text;
            delivery.unit = AptEntry.Text;
            delivery.city = CityEntry.Text;
            delivery.state = StateEntry.Text;
            delivery.zip = ZipEntry.Text;

            chosenIndex = planPicker.SelectedIndex;

            var newPaymentJSONString = JsonConvert.SerializeObject(delivery);
            // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
            var content2 = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
            Console.WriteLine("Content: " + content2);
            /*var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
            request.Method = HttpMethod.Post;
            request.Content = content;*/
            var client = new HttpClient();
            var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Update_Delivery_Info_Address", content2);
            // HttpResponseMessage response = await client.SendAsync(request);
            Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
            Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
            Console.WriteLine("clickedDone Func ENDED!");

            await Navigation.PushAsync(new UserProfile(), false);
        }
    }
}
