﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using MTYD.Model;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class Menu : ContentPage
    {
        public ObservableCollection<Plans> NewMenu = new ObservableCollection<Plans>();
        string fullName; string email;

        async void fillEntries()
        {
            Console.WriteLine("fillEntries in Menu entered");
            var request2 = new HttpRequestMessage();
            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
            request2.RequestUri = new Uri(url);
            //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
            request2.Method = HttpMethod.Get;
            var client2 = new HttpClient();
            HttpResponseMessage response = await client2.SendAsync(request2);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                HttpContent content = response.Content;
                Console.WriteLine("content: " + content);
                var userString = await content.ReadAsStringAsync();
                //Console.WriteLine(userString);
                JObject info_obj = JObject.Parse(userString);
                this.NewMenu.Clear();

                fullName = (info_obj["result"])[0]["customer_first_name"].ToString() + " " + (info_obj["result"])[0]["customer_last_name"].ToString();
                email = (info_obj["result"])[0]["customer_email"].ToString();

                userName.Text = fullName;
                userEmail.Text = email;

            }
        }


        public Menu(string name, string emailAdd)
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            if (name != "" && name != null)
            {
                userName.Text = name;
                userEmail.Text = emailAdd;
                fullName = name;
                email = emailAdd;
            }
            else
            {
                fillEntries();
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                mainGrid.Margin = new Thickness(0, -100, 0, 0);
                mainGrid.Padding = new Thickness(0, 100, 0, 0);

                menu.Margin = new Thickness(25, 15, 0, 40);
                menu.HeightRequest = width / 25;
                menu.WidthRequest = width / 25;

                pfp.HeightRequest = width / 15;
                pfp.WidthRequest = width / 15;
                pfp.CornerRadius = (int)(width / 30);
                pfp.Margin = new Thickness(25, 0, 0, 0);
                //profileInfoStack.Margin = new Thickness(10, 0, 0, 0);

                divider1.Margin = new Thickness(24, 10);

                //subscription.Margin = new Thickness(0, -5);
                subscriptionButton.Margin = new Thickness(0, -5);
                moneyPic.HeightRequest = width / 15;
                moneyPic.WidthRequest = width / 15;
                moneyPic.Margin = new Thickness(25, 0, 0, 0);

                divider2.Margin = new Thickness(24, 10);

                //mealPlan.Margin = new Thickness(0, -5);
                mealPlanButton.Margin = new Thickness(0, -5);
                mealPic.HeightRequest = width / 15;
                mealPic.WidthRequest = width / 15;
                mealPic.Margin = new Thickness(25, 0, 0, 0);

                divider3.Margin = new Thickness(24, 10);

                //mealsAvai.Margin = new Thickness(0, -5);
                mealsAvailButton.Margin = new Thickness(0, -5);
                calendarPic.HeightRequest = width / 15;
                calendarPic.WidthRequest = width / 15;
                calendarPic.Margin = new Thickness(25, 0, 0, 0);

                divider4.Margin = new Thickness(24, 10);

                //profile.Margin = new Thickness(0, -5);
                placeholderButton.Margin = new Thickness(0, -5);
                placeholderPic.HeightRequest = width / 15;
                placeholderPic.WidthRequest = width / 15;
                placeholderPic.Margin = new Thickness(25, 0, 0, 0);


            }
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile());
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void clickedSubscription(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SubscriptionPage(), false);
            Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedMealPlan(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new UserProfile(), false);
            Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedMealSelect(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Select("", "", ""), false);
            Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedVerify(System.Object sender, System.EventArgs e)
        {
            string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "", s6 = "", s7 = "", s8 = "", s9 = "", s10 = "", s11 = "", s12 = "", s13 = "", salt = "";
            Navigation.PushAsync(new VerifyInfo(s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, salt), false);
            Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedSubscriptionTest(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SubscriptionExperiment(), false);
            Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}
