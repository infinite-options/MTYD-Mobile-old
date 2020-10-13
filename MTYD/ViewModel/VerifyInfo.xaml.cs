using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class VerifyInfo : ContentPage
    {
        public VerifyInfo()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.CornerRadius = 35;
                pfp.CornerRadius = 20;
                password.CornerRadius = 22;
                checkoutButton.CornerRadius = 24;
            }
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
            await Navigation.PushAsync(new Menu());
        }

        private void checkoutButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Select());
        }
    }
}
