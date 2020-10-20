using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using MTYD.Model;
using MTYD.Model.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MTYD.ViewModel
{
    public partial class Select : ContentPage
    {
        public ObservableCollection<PaymentInfo> NewPlan = new ObservableCollection<PaymentInfo>();

        public string text1;
        public int weekNumber;
        public Color orange = Color.FromHex("#f59a28");
        public Color green = Color.FromHex("#006633");
        public Color beige = Color.FromHex("#f3f2dc");
        private const string purchaseId = "400-000388";
        //NEW PURCHASE ID: 400-000010
        private static string jsonMeals;
        public static ObservableCollection<MealInfo> Meals1 = new ObservableCollection<MealInfo>();
        private const string postUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selection?customer_uid=100-000001";
        private const string menuUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/upcoming_menu";
        private const string userMeals = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=100-000001";
        private static Dictionary<string, string> qtyDict = new Dictionary<string, string>();
        private static List<MealInformation> mealsSaved = new List<MealInformation>();
        private static int mealsAllowed;
        public int count;
        ArrayList itemsArray = new ArrayList();
        ArrayList purchIdArray = new ArrayList();

        WebClient client = new WebClient();
        public Select()
        {
            InitializeComponent();
            Preferences.Set("origMax", 0);
            GetMealPlans();
            setDates();
            getUserMeals();
            setMenu();

        }

        /*
        protected async Task SetMealSelect()
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

            //itemsList.Add("1"); //{ "1", "5 Meal Plan", "59.99" };
            var newPaymentJSONString = JsonConvert.SerializeObject(newPayment);
            Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
            var content = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
            request.Method = HttpMethod.Post;
            request.Content = content;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            Console.WriteLine("SetPaymentInfo Func ENDED!");
        }*/

        private void setMenu()
        {
            try
            {
                Meals1 = new ObservableCollection<MealInfo>();
                int mealQty;
                var content = client.DownloadString(menuUrl);
                var obj = JsonConvert.DeserializeObject<UpcomingMenu>(content);

                // Convert dates to json date format 2020-09-13
                var convertDay1 = String.Format("{0:yyyy-MM-dd}", text1);

                System.Diagnostics.Debug.WriteLine("Here " + convertDay1.ToString());


                for (int i = 0; i < obj.Result.Length; i++)
                {
                    if (!obj.Result[i].MealCat.Equals("Add-on") && obj.Result[i].MenuDate.Equals(convertDay1))
                    {
                        if (qtyDict.ContainsKey(obj.Result[i].MenuUid.ToString()))
                        {
                            System.Diagnostics.Debug.WriteLine("Made it here item dict " + qtyDict[obj.Result[i].MenuUid.ToString()]);
                        }
                        System.Diagnostics.Debug.WriteLine("Made it here item " + obj.Result[i].MenuUid.ToString());

                        if (qtyDict.ContainsKey(obj.Result[i].MealUid.ToString()))
                        {
                            mealQty = Int32.Parse(qtyDict[obj.Result[i].MealUid.ToString()]);
                            System.Diagnostics.Debug.WriteLine("Made it here 11 " + mealQty);

                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Made it here2");
                            mealQty = 0;
                        }

                        Meals1.Add(new MealInfo
                        {
                            MealName = obj.Result[i].MealName,
                            MealCalories = "Cal: " + obj.Result[i].MealCalories.ToString(),
                            MealImage = obj.Result[i].MealPhotoUrl,
                            MealQuantity = mealQty,
                            MealPrice = obj.Result[i].MealPrice,
                            ItemUid = obj.Result[i].MealUid,
                        });

                    }
                }
                weekOneMenu.ItemsSource = Meals1;
                BindingContext = this;
            }
            catch
            {
                Console.WriteLine("SET MENU IS CRASHING!");
            }
        }

        // Set Dates of Each Label
        private void setDates()
        {
            try
            {
                var content = client.DownloadString(menuUrl);
                var obj = JsonConvert.DeserializeObject<UpcomingMenu>(content);
                string[] dateArray = new string[4];
                string dayOfWeekString = String.Format("{0:dddd}", DateTime.Now);
                DateTime today = DateTime.Now;
                Dictionary<string, int> hm = new Dictionary<string, int>();

                for (int i = 0; i < obj.Result.Length; i++)
                {
                    if (hm.ContainsKey(obj.Result[i].MenuDate))
                        hm.Remove(obj.Result[i].MenuDate);
                    hm.Add(obj.Result[i].MenuDate, i);
                }

                foreach (var i in hm)
                {
                    datePicker.Items.Add(i.Key);
                    //String.Format("MMMM dd, yyyy", i.Key);
                }

                datePicker.SelectedIndex = 0;
                text1 = datePicker.SelectedItem.ToString();
                Preferences.Set("dateSelected", text1.Substring(0, 11));
                Console.WriteLine("dateSet: " + Preferences.Get("dateSelected",""));
            }
            catch
            {
                Console.WriteLine("SET DATA IS CRASHING");
            }

        }


        // Date Picker Selection Changes
        private void dateChange(object sender, EventArgs e)
        {
            Console.WriteLine("Setting now");
            text1 = datePicker.SelectedItem.ToString();
            getUserMeals();
            setMenu();
            weekOneProgress.Progress = 0;

            int orig = Preferences.Get("origMax", 0);
            if (orig != 0)
            {
                totalCount.Text = orig.ToString();

            }
            else
            {
                totalCount.Text = "Count";
            }
            Preferences.Set("total", orig);
            Preferences.Set("dateSelected", text1.Substring(0, 11));
            Console.WriteLine("dateSelected: " + Preferences.Get("dateSelected", ""));
            mealsSaved.Clear();   //New Addition SV
        }

        private void planChange(object sender, EventArgs e)
        {
            if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("5 "))
            {
                mealsAllowed = 5;
            }
            else if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("10"))
            {
                mealsAllowed = 10;
            }
            if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("15"))
            {
                mealsAllowed = 15;
            }
            Console.WriteLine("meals allowed " + mealsAllowed);

            int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
            Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
            Console.WriteLine("Purch Id: " + Preferences.Get("purchId",""));

            string s = SubscriptionPicker.SelectedItem.ToString();
            s = s.Substring(0, 2);
            Preferences.Set("total", int.Parse(s));
            totalCount.Text = Preferences.Get("total", 0).ToString();
            Preferences.Set("origMax", int.Parse(s));
           // Button b = (Button)sender;
           // MealInfo ms = b.BindingContext as MealInfo;
           // ms.MealQuantity = 0;
            mealsSaved.Clear(); //New Addition SV
            resetAll(); //New Addition SV
            GetRecentSelection();
        }

        // Navigation Bar
        private async void onNavClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Equals(SubscribeNav))
            {
                await Navigation.PushAsync(new SubscriptionPage());
            }
            else if (button.Equals(ProfileNav))
            {
                await Navigation.PushAsync(new Profile());
            }
            else if (button.Equals(SelectNav))
            {
                await Navigation.PushAsync(new Select());
            }
        }

        // Navigation Bar Icons
        private async void onNavIconClick(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;

            if (button.Equals(SubscribeIconNav))
            {
                await Navigation.PushAsync(new SubscriptionPage());
            }
            else if (button.Equals(ProfileIconNav))
            {
                await Navigation.PushAsync(new Profile());
            }
            else if (button.Equals(SelectIconNav))
            {
                await Navigation.PushAsync(new Select());
            }

        }

        // Favorite BUtton
        private async void clickedFavorite(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;
            if (b.Source.ToString().Equals("File: heartoutline.png"))
            {
                b.Source = "heart.png";
            }
            else
            {
                b.Source = "heartoutline.png";

            }
        }

        private async void clickIncrease(object sender, EventArgs e)
        {
            int count = Preferences.Get("total", 0);
            if (count != 0)
            {

                totalCount.Text = (--count).ToString();
                Preferences.Set("total", count);

                Button b = (Button)sender;
                MealInfo ms = b.BindingContext as MealInfo;
                ms.MealQuantity++;

                weekOneProgress.Progress += 0.1;

                if (weekOneProgress.Progress < 0.3)
                {
                    weekOneProgress.ProgressColor = Color.LightGoldenrodYellow;
                }
                else if (weekOneProgress.Progress >= 0.3 && weekOneProgress.Progress < 0.5)
                {
                    weekOneProgress.ProgressColor = Color.Orange;
                }
                else if (weekOneProgress.Progress >= 0.5 && weekOneProgress.Progress <= 0.7)
                {
                    weekOneProgress.ProgressColor = Color.LightGreen;
                }
                else if (weekOneProgress.Progress >= 0.8)
                {
                    weekOneProgress.ProgressColor = Color.DarkOliveGreen;
                }
            }
            else
            {
                DisplayAlert("Alert", "You have reached the maximum amount of meals that can be selected for this plan.", "OK");
            }
        }

        private async void clickDecrease(object sender, EventArgs e)
        {
            int count = Preferences.Get("total", 0);
            if (count != Preferences.Get("origMax", 0))
            {
                Button b = (Button)sender;
                MealInfo ms = b.BindingContext as MealInfo;
                if (ms.MealQuantity != 0)
                {
                    totalCount.Text = (++count).ToString();
                    Preferences.Set("total", count);
                    ms.MealQuantity--;

                    weekOneProgress.Progress -= 0.1;
                    if (weekOneProgress.Progress < 0.3)
                    {
                        weekOneProgress.ProgressColor = Color.LightGoldenrodYellow;
                    }
                    else if (weekOneProgress.Progress >= 0.3 && weekOneProgress.Progress < 0.5)
                    {
                        weekOneProgress.ProgressColor = Color.Orange;
                    }
                    else if (weekOneProgress.Progress >= 0.5 && weekOneProgress.Progress <= 0.7)
                    {
                        weekOneProgress.ProgressColor = Color.LightGreen;
                    }
                    else if (weekOneProgress.Progress >= 0.8)
                    {
                        weekOneProgress.ProgressColor = Color.DarkOliveGreen;
                    }
                }
                else { }

            }
            else { }
        }

        protected async Task GetMealPlans()
        {
            Console.WriteLine("ENTER GET MEAL PLANS FUNCTION");
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=100-000082");
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var userString = await content.ReadAsStringAsync();
                JObject mealPlan_obj = JObject.Parse(userString);
                this.NewPlan.Clear();

                //ArrayList itemsArray = new ArrayList();        //TESTING SV
                //ArrayList purchIdArray = new ArrayList();      //TESTING SV
                // List<Item> itemsArray = new List<Item>;
                ArrayList namesArray = new ArrayList();


                foreach (var m in mealPlan_obj["result"])
                {
                    itemsArray.Add((m["items"].ToString()));
                    purchIdArray.Add((m["purchase_id"].ToString()));

                }

                Console.WriteLine("itemsArray contents:" + itemsArray[0] + " " + itemsArray[1]);

                for (int i = 0; i < itemsArray.Count; i++)
                {
                    JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(itemsArray[i].ToString());

                    foreach (JObject config in newobj)
                    {
                        //string qty = (string)config["qty"];
                        string name = (string)config["name"];
                        //string price = (string)config["price"];
                        //string mealid = (string)config["item_uid"];

                        namesArray.Add(name);
                    }
                }

                //Find unique number of meals

                Console.WriteLine("namesArray contents:" + namesArray[0] + " " + namesArray[1] + " " + namesArray[2] + " ");
                SubscriptionPicker.ItemsSource = namesArray;
                //SubscriptionPicker.Title = namesArray[0];

                Console.WriteLine("END OF GET MEAL PLANS FUNCTION");
            }
        }

        private void getUserMeals()
        {
            try
            {
                MealInformation jsonobj;
                // UID = 100-000001 PID = 400-000001
                var content = client.DownloadString(userMeals);
                var obj = JsonConvert.DeserializeObject<MealsSelected>(content);

                for (int i = 0; i < obj.Result.Length; i++)
                {
                    // If meals selected matches menu date, get meals selected 
                    if (obj.Result[i].SelMenuDate.Equals(datePicker.SelectedItem))
                    {
                        string json = obj.Result[i].MealSelection;
                        JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(json);

                        foreach (JObject config in newobj)
                        {
                            string qty = (string)config["qty"];
                            string name = (string)config["name"];
                            string mealid = (string)config["item_uid"];


                            if (qty != null)
                            {
                                if (qtyDict.ContainsKey(mealid))
                                {
                                    qtyDict.Remove(mealid);
                                }
                                qtyDict.Add(mealid, qty);
                            }

                        }

                    }
                }
            }
            catch
            {
                Console.WriteLine("GET USER MEALS ERROR CATCHED");
            }
        }


        private async void saveUserMeals(object sender, EventArgs e)
        {
            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    mealsSaved.Add(new MealInformation
                    {
                        Qty = Meals1[i].MealQuantity.ToString(),
                        Name = Meals1[i].MealName,
                        Price = Meals1[i].MealPrice.ToString(),
                        ItemUid = Meals1[i].ItemUid,
                    }
                    );
                }

            }

            jsonMeals = JsonConvert.SerializeObject(mealsSaved);
            Console.WriteLine("line 302 " + jsonMeals);
            postData();
        }

        public async void postData()
        {
            HttpClient hclient = new HttpClient();

            var mealSelectInfoTosend = new PostMeals
            {
                IsAddon = false,
                // Need to create json formatting for this
                Items = mealsSaved,
                PurchaseId = Preferences.Get("purchId",""),
                MenuDate = datePicker.SelectedItem.ToString(),
                DeliveryDay = "Testday",
            };
            //PurchaseId = purchaseId //Inside above code
            string mealSelectInfoJson = JsonConvert.SerializeObject(mealSelectInfoTosend);
            Console.WriteLine("line 322 " + mealSelectInfoJson);

            try
            {
                var httpContent = new StringContent(mealSelectInfoJson, Encoding.UTF8, "application/json");
                var response = await hclient.PostAsync(postUrl, httpContent);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseContent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }   // Clicked Save function
        }

        private void resetAll()
        {
            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    Meals1[i].MealQuantity = 0;
                    /*
                    mealsSaved.Add(new MealInformation
                    {
                        Qty = Meals1[i].MealQuantity.ToString(),
                        Name = Meals1[i].MealName,
                        Price = Meals1[i].MealPrice.ToString(),
                        ItemUid = Meals1[i].ItemUid,
                    }
                    );*/
                }

            }
        }

        protected async Task GetRecentSelection()
        {
            var request = new HttpRequestMessage();
            string purchaseID = Preferences.Get("purchId", "");
            string date = Preferences.Get("dateSelected", "");
            string urlSent = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_specific?customer_uid=100-000082&purchase_id=" + purchaseID + "&menu_date=" + date;
            Console.WriteLine("URL ENDPOINT TRYING TO BE REACHED:" + urlSent);
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_specific?customer_uid=100-000082&purchase_id=" + purchaseID + "&menu_date=" + date);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var userString = await content.ReadAsStringAsync();
                JObject recentMeals = JObject.Parse(userString);
                this.NewPlan.Clear();

                ArrayList qtyList = new ArrayList();
                //ArrayList nameList = new ArrayList();
                //ArrayList itemUidList = new ArrayList();
                ArrayList namesArray = new ArrayList();
                ArrayList combinedArray = new ArrayList();

                foreach (var m in recentMeals["result"])
                {
                    //Console.WriteLine("PARSING DATA FROM DB: ITEM_UID: " + m["item_uid"].ToString());
                    //qtyList.Add(double.Parse(m["qty"].ToString()));
                    //nameList.Add(int.Parse(m["name"].ToString()));
                    combinedArray.Add((m["combined_selection"].ToString()));
                }

                for (int i = 0; i < combinedArray.Count; i++)
                {
                    JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(combinedArray[i].ToString());

                    foreach (JObject config in newobj)
                    {
                        string qty = (string)config["qty"];
                        string name = (string)config["name"];
                        //string price = (string)config["price"];
                        //string mealid = (string)config["item_uid"];

                        namesArray.Add(name);
                        qtyList.Add(qty);
                    }
                }

                for (int i = 0; i < Meals1.Count; i++)
                {

                    Meals1[i].MealQuantity = (int)qtyList[i];

                }
                setMenu();
                Console.WriteLine("END OF GET PLANS FUNCTION");
            }
        }
            
        }
}