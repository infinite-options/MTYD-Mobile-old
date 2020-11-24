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
        private const string purchaseId = "200-000010";
        private static string jsonMeals;
        public static ObservableCollection<MealInfo> Meals1 = new ObservableCollection<MealInfo>();
        public static string userId = (string)Application.Current.Properties["user_id"];
        private string postUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selection?customer_uid=" + userId;
        private const string menuUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/upcoming_menu";
        private string userMeals = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + userId;
        //private const string userMeals = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=100-000001";
        private static Dictionary<string, string> qtyDict = new Dictionary<string, string>();
        private static List<MealInformation> mealsSaved = new List<MealInformation>();
        private static int mealsAllowed;
        public int count;
        ArrayList itemsArray = new ArrayList();
        ArrayList purchIdArray = new ArrayList();
        string firstIndex = "";
        public int totalMealsCount = 0;
        public bool isAlreadySelected;
        public bool isSurprise = false;
        public bool isSkip = false;
        public int firstTotalCount;

        WebClient client = new WebClient();
        public Select()
        {
            InitializeComponent();
            Preferences.Set("origMax", 0);
            GetMealPlans();
            setDates();
            getUserMeals();
            setMenu();

            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);

            //mealsSaved.Clear();
            //resetAll();
            //GetRecentSelection();

            //firstTotalCount = Int32.Parse(totalCount.Text.ToString().Substring(0,2));
            //SubscriptionPicker.SelectedIndex = 0;
            // SubscriptionPicker.SelectedIndex = 0;
            //SubscriptionPicker.Title = firstIndex;
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

                selectPlanFrame.Margin = new Thickness(25, 7);
                selectPlanFrame.Padding = new Thickness(15, 5);
                selectPlanFrame.HeightRequest = height / 50;
                lunchPic.HeightRequest = height / 40;
                lunchPic.WidthRequest = height / 40;
                lunchPic.Margin = new Thickness(5, 1, 0, 1);
                SubscriptionPicker.FontSize = height / 95;
                SubscriptionPicker.VerticalOptions = LayoutOptions.Fill;
                SubscriptionPicker.HorizontalOptions = LayoutOptions.Fill;

                selectDateFrame.Margin = new Thickness(25, 3, 25, 7);
                selectDateFrame.Padding = new Thickness(15, 5);
                selectDateFrame.HeightRequest = height / 50;
                calendarPic.HeightRequest = height / 40;
                calendarPic.WidthRequest = height / 40;
                calendarPic.Margin = new Thickness(5, 1, 0, 1);
                datePicker.FontSize = height / 95;
                datePicker.VerticalOptions = LayoutOptions.Fill;
                datePicker.HorizontalOptions = LayoutOptions.Fill;
            }
            else //android
            {

            }
        }


        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu());
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
                Console.WriteLine("dateSet: " + Preferences.Get("dateSelected", ""));
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
            //reset the buttons
            //default to surprise if null
        }

        private async void planChange(object sender, EventArgs e)
        {
            if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("5 "))
            {
                mealsAllowed = 5;
            }
            else if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("10"))
            {
                mealsAllowed = 10;
            }
            else if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("15"))
            {
                mealsAllowed = 15;
            }
            else if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("20"))
            {
                mealsAllowed = 20;
            }
            Console.WriteLine("meals allowed " + mealsAllowed);

            isSkip = false;
            isSurprise = false;
            weekOneProgress.Progress = 0;
            //firstTotalCount = Int32.Parse(totalCount.Text.ToString().Substring(0, 2));

            /* 
             * SV COMMENT 11/17 Testing TotalCount.Text
            int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
            Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
            Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

            string s = SubscriptionPicker.SelectedItem.ToString();
            s = s.Substring(0, 2);
            Preferences.Set("total", int.Parse(s));
            //totalCount.Text = Preferences.Get("total", 0).ToString();
          //  Preferences.Set("origMax", int.Parse(s));
            */
            int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
            Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
            Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

            // Button b = (Button)sender;
            // MealInfo ms = b.BindingContext as MealInfo;
            // ms.MealQuantity = 0;
            mealsSaved.Clear(); //New Addition SV
            resetAll(); //New Addition SV

            await GetRecentSelection();
            GetRecentSelection2();

            Console.WriteLine("isAlreadySeleced in planchange" + isAlreadySelected);

            //bool isAlreadySelected = Preferences.Get("isAlreadySelected", true);
            if (isAlreadySelected == true)
            {
                totalCount.Text = "0";
                Preferences.Set("total", 0);
                //DisplayAlert("Alert", "Select reset button to change your meal selections", "OK");
                weekOneProgress.Progress = 1;
            }
            else if (isAlreadySelected == false)
            {
                string s = SubscriptionPicker.SelectedItem.ToString();
                s = s.Substring(0, 2);
                Preferences.Set("total", int.Parse(s));
                totalCount.Text = Preferences.Get("total", 0).ToString();
                Preferences.Set("origMax", int.Parse(s));
                weekOneProgress.Progress = 0;
            }

            if (isSkip)
            {
                skipBttn.BackgroundColor = Color.Black;
                skipFrame.BackgroundColor = Color.Black;
                skipBttn.TextColor = Color.White;
                surpriseBttn.BackgroundColor = Color.Transparent;
                surpriseFrame.BackgroundColor = Color.Transparent;
                surpriseBttn.TextColor = Color.Black;

                indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
                Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

                string s = SubscriptionPicker.SelectedItem.ToString();
                s = s.Substring(0, 2);
                Preferences.Set("total", int.Parse(s));
                totalCount.Text = Preferences.Get("total", 0).ToString();
                Preferences.Set("origMax", int.Parse(s));
                weekOneProgress.Progress = 0;
            }
            else if (isSurprise)
            {
                surpriseBttn.BackgroundColor = Color.Black;
                surpriseFrame.BackgroundColor = Color.Black;
                surpriseBttn.TextColor = Color.White;
                skipBttn.BackgroundColor = Color.Transparent;
                skipFrame.BackgroundColor = Color.Transparent;
                skipBttn.TextColor = Color.Black;

                indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
                Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

                string s = SubscriptionPicker.SelectedItem.ToString();
                s = s.Substring(0, 2);
                Preferences.Set("total", int.Parse(s));
                totalCount.Text = Preferences.Get("total", 0).ToString();
                Preferences.Set("origMax", int.Parse(s));
                weekOneProgress.Progress = 0;
            }
            else
            {
                //If neither skip or surprise (new plan), then initialize to surprise
                skipBttn.BackgroundColor = Color.Transparent;
                skipFrame.BackgroundColor = Color.Transparent;
                skipBttn.TextColor = Color.Black;
                surpriseBttn.BackgroundColor = Color.Transparent;
                surpriseFrame.BackgroundColor = Color.Transparent;
                surpriseBttn.TextColor = Color.Black;
                if (isAlreadySelected == false)
                    surprise();

            }
            //GetRecentSelection(); //11/17 10pm comment SV

            //calcTotal();
            /* //Testing 11/12 Total meals count
            int totalMealsCount = 110;
            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    totalMealsCount += Int32.Parse(Meals1[i].MealQuantity.ToString());
                }
            } */
            Console.WriteLine("Meals1 Count: " + totalMealsCount);
            //11/12
            //Preferences.Set("total", Meals1.Count);
            //totalCount.Text = Preferences.Get("total", 0).ToString();
            //Preferences.Set("origMax", int.Parse(s));

            //
            //GetMealPlans();
            //setDates();

            //commented out 11/11 for second merge
            //getUserMeals();
            //setMenu();
        }

        /*
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

        }*/

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
            int permCount = Preferences.Get("origMax", 0);
            if (count != 0)
            {

                totalCount.Text = (--count).ToString();
                Preferences.Set("total", count);

                Button b = (Button)sender;
                MealInfo ms = b.BindingContext as MealInfo;
                ms.MealQuantity++;

                //float adder = 0.0f;
                if (permCount == 5)
                {
                    //adder = 0.2f;
                    weekOneProgress.Progress += 0.2;
                }
                else if (permCount == 10)
                {
                    //adder = 0.1f;
                    weekOneProgress.Progress += 0.1;
                }
                else if (permCount == 15)
                {
                    //adder = 0.067f;
                    weekOneProgress.Progress += 0.067;
                }
                else if (permCount == 20)
                {
                    //adder = 0.05f;
                    weekOneProgress.Progress += 0.05;
                }

                //weekOneProgress.Progress -= 0.1;
                //weekOneProgress.Progress += adder;

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

                    int permCount = Preferences.Get("origMax", 0);
                    //float adder = 0.0f;
                    if (permCount == 5)
                    {
                        //adder = 0.2f;
                        weekOneProgress.Progress -= 0.2;
                    }
                    else if (permCount == 10)
                    {
                        //adder = 0.1f;
                        weekOneProgress.Progress -= 0.1;
                    }
                    else if (permCount == 15)
                    {
                        //adder = 0.067f;
                        weekOneProgress.Progress -= 0.067;
                    }
                    else if (permCount == 20)
                    {
                        //adder = 0.05f;
                        weekOneProgress.Progress -= 0.05;
                    }

                    //weekOneProgress.Progress -= 0.1;
                    // weekOneProgress.Progress -= adder;
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
                JObject mealPlan_obj = JObject.Parse(userString);
                this.NewPlan.Clear();

                ArrayList itemsArray = new ArrayList();
                // List<Item> itemsArray = new List<Item>;
                ArrayList namesArray = new ArrayList();

                Console.WriteLine("itemsArray contents:");

                foreach (var m in mealPlan_obj["result"])
                {
                    Console.WriteLine("In first foreach loop of getmeal plans func:");

                    itemsArray.Add((m["items"].ToString()));
                    purchIdArray.Add((m["purchase_id"].ToString()));
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
                SubscriptionPicker.ItemsSource = namesArray;
                SubscriptionPicker.SelectedItem = namesArray[0].ToString();
                Console.WriteLine("namesArray contents:" + namesArray[0].ToString());
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
            surpriseBttn.BackgroundColor = Color.Transparent;
            surpriseFrame.BackgroundColor = Color.Transparent;
            surpriseBttn.TextColor = Color.Black;
            skipBttn.BackgroundColor = Color.Transparent;
            skipFrame.BackgroundColor = Color.Transparent;
            skipBttn.TextColor = Color.Black;
            saveBttn.BackgroundColor = Color.Black;
            saveBttn.TextColor = Color.White;

            int count = Preferences.Get("total", 0);
            if (totalCount.Text == "0" || count == 0)
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
                saveBttn.BackgroundColor = Color.Transparent;
                saveBttn.TextColor = Color.Black;
            }
            else
            {
                DisplayAlert("Incomplete Selection", "Please select all the meals before saving.", "OK");
            }
        }

        private async void skipMealSelection(object sender, EventArgs e)
        {
            skipBttn.BackgroundColor = Color.Black;
            skipFrame.BackgroundColor = Color.Black;
            skipBttn.TextColor = Color.White;
            surpriseBttn.BackgroundColor = Color.Transparent;
            surpriseFrame.BackgroundColor = Color.Transparent;
            surpriseBttn.TextColor = Color.Black;
            resetAll();
            mealsSaved.Clear();
            int count = Preferences.Get("total", 0);
            totalCount.Text = "SKIPPED";
            //for (int i = 0; i < Meals1.Count; i++)
            //{
            //if (Meals1[i].MealQuantity > 0)
            //{
            mealsSaved.Add(new MealInformation
            {
                Qty = "",
                Name = "SKIP",
                Price = "",
                ItemUid = "",
            }
            );
            // }
            //}

            jsonMeals = JsonConvert.SerializeObject(mealsSaved);
            Console.WriteLine("line 302 " + jsonMeals);
            postData();
            DisplayAlert("Meal Skipped", "You have chosen to skip the meal selection for this week. If you want to select meals again for this meal plan then click the RESET button!", "OK");
            mealsSaved.Clear();
            int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
            Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
            Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

            string s = SubscriptionPicker.SelectedItem.ToString();
            s = s.Substring(0, 2);
            Preferences.Set("total", int.Parse(s));
            totalCount.Text = Preferences.Get("total", 0).ToString();
            Preferences.Set("origMax", int.Parse(s));
        }

        private void surprise()
        {
            weekOneProgress.Progress = 0;
            surpriseBttn.BackgroundColor = Color.Black;
            surpriseFrame.BackgroundColor = Color.Black;
            surpriseBttn.TextColor = Color.White;
            skipBttn.BackgroundColor = Color.Transparent;
            skipFrame.BackgroundColor = Color.Transparent;
            skipBttn.TextColor = Color.Black;
            resetAll();
            mealsSaved.Clear();
            int count = Preferences.Get("total", 0);
            totalCount.Text = "SURPRISE";
            //for (int i = 0; i < Meals1.Count; i++)
            //{
            //if (Meals1[i].MealQuantity > 0)
            //{
            mealsSaved.Add(new MealInformation
            {
                Qty = "",
                Name = "SURPRISE",
                Price = "",
                ItemUid = "",
            }
            );
            // }
            //}

            jsonMeals = JsonConvert.SerializeObject(mealsSaved);
            Console.WriteLine("line 302 " + jsonMeals);
            //postData();
            //DisplayAlert("SUPRISE", "You will be surprised with a randomized meal selection. If you want to select meals again for this meal plan then click the RESET button!", "OK");
            mealsSaved.Clear();
            int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
            Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
            Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

            string s = SubscriptionPicker.SelectedItem.ToString();
            s = s.Substring(0, 2);
            Preferences.Set("total", int.Parse(s));
            totalCount.Text = Preferences.Get("total", 0).ToString();
            Preferences.Set("origMax", int.Parse(s));
        }
        private async void surpriseMealSelection(object sender, EventArgs e)
        {
            surpriseBttn.BackgroundColor = Color.Black;
            surpriseFrame.BackgroundColor = Color.Black;
            surpriseBttn.TextColor = Color.White;
            skipBttn.BackgroundColor = Color.Transparent;
            skipFrame.BackgroundColor = Color.Transparent;
            skipBttn.TextColor = Color.Black;
            resetAll();
            mealsSaved.Clear();
            int count = Preferences.Get("total", 0);
            totalCount.Text = "SURPRISE";
            //for (int i = 0; i < Meals1.Count; i++)
            //{
            //if (Meals1[i].MealQuantity > 0)
            //{
            mealsSaved.Add(new MealInformation
            {
                Qty = "",
                Name = "SURPRISE",
                Price = "",
                ItemUid = "",
            }
            );
            // }
            //}

            jsonMeals = JsonConvert.SerializeObject(mealsSaved);
            Console.WriteLine("line 302 " + jsonMeals);
            postData();
            DisplayAlert("SUPRISE", "You will be surprised with a randomized meal selection. If you want to select meals again for this meal plan then click the RESET button!", "OK");
            mealsSaved.Clear();
            int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
            Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
            Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

            string s = SubscriptionPicker.SelectedItem.ToString();
            s = s.Substring(0, 2);
            Preferences.Set("total", int.Parse(s));
            totalCount.Text = Preferences.Get("total", 0).ToString();
            Preferences.Set("origMax", int.Parse(s));
        }

        public async void postData()
        {
            HttpClient hclient = new HttpClient();

            var mealSelectInfoTosend = new PostMeals
            {
                IsAddon = false,
                // Need to create json formatting for this
                Items = mealsSaved,
                PurchaseId = Preferences.Get("purchId", ""),
                MenuDate = datePicker.SelectedItem.ToString(),
                DeliveryDay = "Testday",
            };

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

        /*private bool isAlreadySelected()
        {
            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    return true;
                }

            }
            return false;
        }*/

        private void resetAll()
        {
            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    //totalMealsCount += Meals1[i].MealQuantity;
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

        private void calcTotal()
        {
            totalMealsCount = 0;
            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    totalMealsCount += Meals1[i].MealQuantity;
                }

            }
        }

        protected async Task GetRecentSelection()
        {
            Console.WriteLine("INSIDE GetRecentSelection #1");
            var request = new HttpRequestMessage();
            string purchaseID = Preferences.Get("purchId", "");
            string date = Preferences.Get("dateSelected", "");
            string userID = (string)Application.Current.Properties["user_id"];
            string halfUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_specific?customer_uid=" + userID;
            string urlSent = halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date;
            Console.WriteLine("URL ENDPOINT TRYING TO BE REACHED:" + urlSent);
            request.RequestUri = new Uri(halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            Console.WriteLine("Trying to enter if statement in Get Recent Selection");
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

                Console.WriteLine("Trying to enter foreach loop in Get Recent Meals");

                foreach (var m in recentMeals["result"])
                {
                    //Console.WriteLine("PARSING DATA FROM DB: ITEM_UID: " + m["item_uid"].ToString());
                    //qtyList.Add(double.Parse(m["qty"].ToString()));
                    //nameList.Add(int.Parse(m["name"].ToString()));
                    combinedArray.Add((m["combined_selection"].ToString()));
                }

                if (combinedArray.Count == 0)
                {
                    //Preferences.Set("isAlreadySelected", false);
                    isAlreadySelected = false;
                }
                else
                {
                    //Preferences.Set("isAlreadySelected", true);
                    isAlreadySelected = true;
                }
                //isAlreadySelected = Preferences.Get("isAlreadySelected", true);
                Console.WriteLine("isAlreadySelected" + isAlreadySelected);

                Console.WriteLine("Trying to enter for loop in Get Recent Selection");
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
                for (int i = 0; i < namesArray.Count; i++)
                {
                    if (namesArray[i].ToString() == "SURPRISE")
                    {
                        isSurprise = true;
                        break;
                    }
                    else if (namesArray[i].ToString() == "SKIP")
                    {
                        isSkip = true;
                        break;
                    }
                    else
                    {
                        isSkip = false;
                        isSurprise = false;
                    }
                }
                Console.WriteLine("isSurprise value: " + isSurprise + " isSkip value: " + isSkip);
                return;
                Console.WriteLine("Trying to enter second for loop in Get Recent Selection");
                totalMealsCount = 0;
                //resetAll();
                for (int i = 0; i < Meals1.Count; i++)
                {
                    Console.WriteLine("Inside second for loop in Get Recent Selection");

                    Meals1[i].MealQuantity = Int32.Parse(qtyList[i].ToString());
                    //totalMealsCount += Int32.Parse(qtyList[i].ToString());

                }
                Console.WriteLine("Before set menu call in Get Recent Seleciton");

                for (int i = 0; i < qtyList.Count; i++)
                {
                    Console.WriteLine("Inside third for loop in Get Recent Selection");
                    totalMealsCount += Int32.Parse(qtyList[i].ToString());
                }
                setMenu();
                Console.WriteLine("END OF GET RECENT SELECTION");

            }
        }

        protected async Task GetRecentSelection2()
        {
            Console.WriteLine("INSIDE GetRecentSelection #2");
            var request = new HttpRequestMessage();
            string purchaseID = Preferences.Get("purchId", "");
            string date = Preferences.Get("dateSelected", "");
            string userID = (string)Application.Current.Properties["user_id"];
            string halfUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_specific?customer_uid=" + userID;
            string urlSent = halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date;
            Console.WriteLine("URL ENDPOINT TRYING TO BE REACHED:" + urlSent);
            request.RequestUri = new Uri(halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            Console.WriteLine("Trying to enter if statement in Get Recent Selection");
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

                Console.WriteLine("Trying to enter foreach loop in Get Recent Meals");

                foreach (var m in recentMeals["result"])
                {
                    //Console.WriteLine("PARSING DATA FROM DB: ITEM_UID: " + m["item_uid"].ToString());
                    //qtyList.Add(double.Parse(m["qty"].ToString()));
                    //nameList.Add(int.Parse(m["name"].ToString()));
                    combinedArray.Add((m["combined_selection"].ToString()));
                }

                if (combinedArray.Count == 0)
                {
                    //Preferences.Set("isAlreadySelected", false);
                    isAlreadySelected = false;
                }
                else
                {
                    //Preferences.Set("isAlreadySelected", true);
                    isAlreadySelected = true;
                }
                //isAlreadySelected = Preferences.Get("isAlreadySelected", true);
                Console.WriteLine("isAlreadySelected" + isAlreadySelected);

                Console.WriteLine("Trying to enter for loop in Get Recent Selection");
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

                Console.WriteLine("Trying to enter second for loop in Get Recent Selection");
                totalMealsCount = 0;
                //resetAll();
                for (int i = 0; i < Meals1.Count; i++)
                {
                    Console.WriteLine("Inside second for loop in Get Recent Selection");

                    Meals1[i].MealQuantity = Int32.Parse(qtyList[i].ToString());
                    //totalMealsCount += Int32.Parse(qtyList[i].ToString());

                }
                Console.WriteLine("Before set menu call in Get Recent Seleciton");

                for (int i = 0; i < qtyList.Count; i++)
                {
                    Console.WriteLine("Inside third for loop in Get Recent Selection");
                    totalMealsCount += Int32.Parse(qtyList[i].ToString());
                }
                setMenu();
                Console.WriteLine("END OF GET RECENT SELECTION");

            }
        }

        private void resetBttn_Clicked(object sender, EventArgs e)
        {

            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    Meals1[i].MealQuantity = 0;
                }

            }

            int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
            Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
            Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

            string s = SubscriptionPicker.SelectedItem.ToString();
            s = s.Substring(0, 2);
            Preferences.Set("total", int.Parse(s));
            totalCount.Text = Preferences.Get("total", 0).ToString();
            Preferences.Set("origMax", int.Parse(s));
        }
    }
}
