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
    public partial class UserProfileExperiment : ContentPage
    {
        public ObservableCollection<Plans> userProfileInfo = new ObservableCollection<Plans>();

        public UserProfileExperiment()
        {
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
            fillEntries();
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
                selectMealPlan.Margin = new Thickness(10, 0, 0, 0);
                selectMealPlan.FontSize = width / 40;
                selectMealPlan.HeightRequest = height / 45;
                selectMealPlan.CornerRadius = (int) height / 90;
                changeMealPlan.Margin = new Thickness(10, 0, 0, 0);
                changeMealPlan.FontSize = width / 40;
                changeMealPlan.HeightRequest = height / 45;
                changeMealPlan.CornerRadius = (int)height / 90;

                mainGrid.Margin = new Thickness(width / 50);
                mainFrame.CornerRadius = 20;
                innerStack.Margin = new Thickness(width / 100);
                delivery.FontSize = width / 38;

                firstName.CornerRadius = 21;
                lastName.CornerRadius = 21;
                emailAdd.CornerRadius = 21;
                street.CornerRadius = 21;
                unit.CornerRadius = 21;
                city.CornerRadius = 21;
                state.CornerRadius = 21;
                zipCode.CornerRadius = 21;
                phoneNum.CornerRadius = 21;
                FNameEntry.FontSize = width / 43;
                LNameEntry.FontSize = width / 43;
                emailEntry.FontSize = width / 43;
                AddressEntry.FontSize = width / 43;
                AptEntry.FontSize = width / 43;
                CityEntry.FontSize = width / 43;
                StateEntry.FontSize = width / 43;

            }
            else //android
            {

            }
        }

        //auto-populate the delivery info if the user has already previously entered it
        public async void fillEntries()
        {
            Console.WriteLine("fillEntries entered");
            var request = new HttpRequestMessage();
            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
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
                JObject info_obj = JObject.Parse(userString);
                this.userProfileInfo.Clear();

                //ArrayList item_price = new ArrayList();
                //ArrayList num_items = new ArrayList();
                //ArrayList payment_frequency = new ArrayList();
                //ArrayList groupArray = new ArrayList();

                if ((info_obj["result"]).ToString() == "[]")
                {
                    Console.WriteLine("no info");

                    FNameEntry.Placeholder = "First Name*";
                    LNameEntry.Placeholder = "Last Name*";
                    emailEntry.Placeholder = "Email*";
                    AddressEntry.Placeholder = "Street*";
                    AptEntry.Placeholder = "Unit";
                    CityEntry.Placeholder = "City*";
                    StateEntry.Placeholder = "State*";
                    ZipEntry.Placeholder = "Zip*";
                    PhoneEntry.Placeholder = "Phone Number*";

                    return;
                }

                //Console.WriteLine("delivery first name: " + (info_obj["result"])[0]["selection_uid"]);
                FNameEntry.Text = (info_obj["result"])[0]["delivery_first_name"].ToString();
                if (FNameEntry.Text == "")
                    FNameEntry.Placeholder = "First Name*";

                LNameEntry.Text = (info_obj["result"])[0]["delivery_last_name"].ToString();
                if (LNameEntry.Text == "")
                    LNameEntry.Placeholder = "Last Name*";

                emailEntry.Text = (info_obj["result"])[0]["delivery_email"].ToString();
                if (emailEntry.Text == "")
                    emailEntry.Placeholder = "Email*";

                AddressEntry.Text = (info_obj["result"])[0]["delivery_address"].ToString();
                if (AddressEntry.Text == "")
                    AddressEntry.Placeholder = "Street*";

                AptEntry.Text = (info_obj["result"])[0]["delivery_unit"].ToString();
                if (AptEntry.Text == "")
                    AptEntry.Placeholder = "Unit";

                CityEntry.Text = (info_obj["result"])[0]["delivery_city"].ToString();
                if (CityEntry.Text == "")
                    CityEntry.Placeholder = "City*";

                StateEntry.Text = (info_obj["result"])[0]["delivery_state"].ToString();
                if (StateEntry.Text == "")
                    StateEntry.Placeholder = "State*";

                ZipEntry.Text = (info_obj["result"])[0]["delivery_zip"].ToString();
                if (ZipEntry.Text == "")
                    ZipEntry.Placeholder = "Zip*";

                PhoneEntry.Text = (info_obj["result"])[0]["delivery_phone_num"].ToString();
                if (PhoneEntry.Text == "")
                    PhoneEntry.Placeholder = "Phone Number*";
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

        void LogOutClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.Properties.Remove("user_id");
            Application.Current.Properties.Remove("time_stamp");
            Application.Current.Properties.Remove("platform");
            Application.Current.MainPage = new MainPage();
        }
    }
}
