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
            await Navigation.PushAsync(new SubscriptionPage());
        }

        async void clickedDelivery(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DeliveryBilling());
        }

        async void clickedMealSelect(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void clickedVerify(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new VerifyInfo());
        }
    }
}
