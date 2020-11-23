using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class Menu : ContentPage
    {

        public Menu()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.CornerRadius = 35;
                pfp.CornerRadius = 20;
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

        async void clickedPlan(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SubscriptionPage(), false);
            Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedDelivery(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new DeliveryBilling(), false);
            Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedMealSelect(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Select(), false);
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
    }
}
