﻿using System;
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
                grid1.Margin = new Thickness(0, height / 14, 0, 0);

                grid2.Margin = new Thickness(width / 13, height / 80, width / 13, 0);
                userFrame.HeightRequest = height / 180;
                userFrame.CornerRadius = 27;
                loginUsername.Margin = new Thickness(0, height / (-120), 0, height / (-120));
                passFrame.HeightRequest = height / 180;
                passFrame.CornerRadius = 27;
                loginPassword.Margin = new Thickness(0, height / (-120), 0, height / (-120));


                loginButton.HeightRequest = height / 35;
                signUpButton.HeightRequest = height / 35;
                loginButton.WidthRequest = width / 7;
                signUpButton.WidthRequest = width / 7;
                loginButton.CornerRadius = (int)(height / 80);
                signUpButton.CornerRadius = (int)(height / 90);

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
                loginButton.HeightRequest = height / 100;
                signUpButton.HeightRequest = height / 120;
                loginButton.WidthRequest = width / 15;
                signUpButton.WidthRequest = width / 14;
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



        async void clickedSignUp(object sender, EventArgs e)
        {
            //temporary change for testing
            await Navigation.PushAsync(new MainPageExperiment());

            //await Navigation.PushAsync(new SignUp(), false);
        }

        // handles when the login button is clicked
        private async void clickedLogin(object sender, EventArgs e)
        {
            //For testing purposes
            await Navigation.PushAsync(new SubscriptionPage());
            //loginButton.IsEnabled = false;
            if (String.IsNullOrEmpty(this.loginUsername.Text) || String.IsNullOrEmpty(this.loginPassword.Text))
            { // check if all fields are filled out
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                loginButton.IsEnabled = true;
            }
            else
            {
                await Navigation.PushAsync(new SubscriptionPage());
                /*
                var accountSalt = await retrieveAccountSalt(this.loginUsername.Text); // retrieve user's account salt
                Console.WriteLine("after acct salt 84");
                //System.Diagnostics.Debug.WriteLine("account salt count: " + accountSalt.result.Count);

                //if (accountSalt != null && accountSalt.result.Count != 0)
                //{ // make sure the account salt exists 
                //var loginAttempt = await login(this.loginEmail.Text, this.loginPassword.Text, accountSalt);
                login(this.loginUsername.Text, this.loginPassword.Text, accountSalt);
                Console.WriteLine("login executed");
                //System.Diagnostics.Debug.WriteLine("login attempt: " + loginAttempt.GetType());
                /*
                if (loginAttempt != null && loginAttempt.Message != "Request failed, wrong password.")
                { // make sure the login attempt was successful
                    captureLoginSession(loginAttempt);
                    await Navigation.PopAsync();

                }
                else
                {
                    await DisplayAlert("Error", "Wrong password was entered", "OK");
                    loginButton.IsEnabled = true;
                }
                
                //}
                //else
                if (accountSalt == null)
                {
                    await DisplayAlert("Error", "An account with that email does not exist", "OK");
                    loginButton.IsEnabled = true;

                }
                */
            }
        }
    }
}
