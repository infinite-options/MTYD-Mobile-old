using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace MTYD.ViewModel
{
    public partial class MainPageExperiment : ContentPage
    {
        void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                subHeading.Margin = new Thickness(0, height / -100, 0, 0);
                grid1.Margin = new Thickness(0, height/14, 0, 0);

                grid2.Margin = new Thickness(width / 13, height / 80, width / 13, 0);
                userFrame.HeightRequest = height / 180;
                userFrame.CornerRadius = 27;
                loginUsername.Margin = new Thickness(0, height / (-120), 0, height / (-120));
                passFrame.HeightRequest = height / 180;
                passFrame.CornerRadius = 27;
                loginPassword.Margin = new Thickness(0, height / (-120), 0, height / (-120));
                //userFrame.HeightRequest = height / 35;
                //grid3.WidthRequest = width;
                //grid2.Padding = new Thickness(0, height/15, 0, height/15);
                //userFrame.Margin = new Thickness(0, height / 20, 0, height / 20);
                //grid3.Margin = new Thickness(width / 13, height / 100, width / 13, 0);
                //grid3.HeightRequest = height / 35;
                //passFrame.Margin = new Thickness(0, height / 30, 0, height / 30);
                //loginUsername.HeightRequest = height/5;
                //loginUsername.WidthRequest = width / 2;
                //grid3.Margin = new Thickness(width / 5, 0, width / 5, 0);
            }
            else
            {
                subHeading.Margin = new Thickness(0, height / -100, 0, 0);
                grid1.Margin = new Thickness(0, height / 17, 0, 0);

                grid2.Margin = new Thickness(width / 20, height / 80, width / 25, 0);
                userFrame.HeightRequest = height / 180;
                userFrame.CornerRadius = 27;
                loginUsername.Margin = new Thickness(0, height / (-120), 0, height / (-120));
                passFrame.HeightRequest = height / 180;
                passFrame.CornerRadius = 27;
                loginPassword.Margin = new Thickness(0, height / (-120), 0, height / (-120));
                //grid1.Margin = new Thickness(0, height/17, 0, 0);
            }
        }


        public MainPageExperiment()
        {
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            InitializeComponent();
            checkPlatform(height, width);
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundImageSource = "landing2.jpg";


        }

        void clickedSeePassword(System.Object sender, System.EventArgs e)
        {
            if (loginPassword.IsPassword == true)
                loginPassword.IsPassword = false;
            else loginPassword.IsPassword = true;
        }

    }
}
