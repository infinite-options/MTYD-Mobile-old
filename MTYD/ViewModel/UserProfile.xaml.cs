using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class UserProfile : ContentPage
    {
        public UserProfile()
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

        async void UserProfTestClick(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfileExperiment());
        }
    }
}
