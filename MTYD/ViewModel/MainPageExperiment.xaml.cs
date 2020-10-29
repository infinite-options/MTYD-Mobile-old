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
                grid2.Margin = new Thickness(width / 13, height / 90, width / 13, 0);
                loginUsername.Margin = new Thickness(0, height / (-120), 0, height / (-120));
                loginPassword.Margin = new Thickness(0, height / (-120), width / 55, height / (-120));


                loginButton.HeightRequest = height / 35;
                signUpButton.HeightRequest = height / 35;
                loginButton.WidthRequest = width / 7;
                signUpButton.WidthRequest = width / 7;
                loginButton.CornerRadius = (int)(height / 70);
                signUpButton.CornerRadius = (int)(height / 70);
                grid4.Margin = new Thickness(width / 15, height / 80, width / 15, height / 100);

                googleLoginButton.HeightRequest = width / 12;
                googleLoginButton.WidthRequest = width / 12;
                googleLoginButton.CornerRadius = (int)(width / 24);
                facebookLoginButton.HeightRequest = width / 12;
                facebookLoginButton.WidthRequest = width / 12;
                facebookLoginButton.CornerRadius = (int)(width / 24);
                appleLoginButton.HeightRequest = width / 12;
                appleLoginButton.WidthRequest = width / 12;
                appleLoginButton.CornerRadius = (int)(width / 24);
            }
            else //android
            {
                grid2.Margin = new Thickness(width / 20, height / 80, width / 25, 0);
                loginUsername.Margin = new Thickness(0, height / (-120), 0, height / (-120));
                loginPassword.Margin = new Thickness(0, height / (-120), 0, height / (-120));
                loginButton.HeightRequest = height / 40;
                signUpButton.HeightRequest = height / 40;
                loginButton.WidthRequest = width / 10;
                signUpButton.WidthRequest = width / 10;
                forgotPass.Margin = new Thickness(0, -30, 10, 0);

                loginButton.CornerRadius = (int)(height / 80);
                signUpButton.CornerRadius = (int)(height / 80);
                grid4.Margin = new Thickness(width / 15, height / 80, width / 15, height / 120);

                googleLoginButton.HeightRequest = width / 18;
                googleLoginButton.WidthRequest = width / 18;
                googleLoginButton.CornerRadius = (int)(width / 36);
                facebookLoginButton.HeightRequest = width / 18;
                facebookLoginButton.WidthRequest = width / 18;
                facebookLoginButton.CornerRadius = (int)(width / 36);
                appleLoginButton.HeightRequest = width / 18;
                appleLoginButton.WidthRequest = width / 18;
                appleLoginButton.CornerRadius = (int)(width / 36);
            }

            //adjustments regardless of device
            //grid1.Margin = new Thickness(0, 0, 0, 0);
            grid5.Margin = new Thickness(0, height / 80, 0, 0);
            userFrame.HeightRequest = height / 180;
            passFrame.HeightRequest = height / 180;
            //userFrame.CornerRadius = 25;
            //passFrame.CornerRadius = 25;
        }


        public MainPageExperiment()
        {
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            Console.WriteLine("Width = " + width.ToString());
            Console.WriteLine("Height = " + height.ToString());
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
            Application.Current.MainPage = new CarlosSignUp();

            //await Navigation.PushAsync(new SignUp(), false);
        }

        // handles when the login button is clicked
        private async void clickedLogin(object sender, EventArgs e)
        {
            //For testing purposes
            Application.Current.MainPage = new MainPage();

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

        [Obsolete]
        private async void googleLoginButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GoogleLogin(), false);
        }

        [Obsolete]
        private async void facebookLoginButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FacebookLogin(), false);
        }

        [Obsolete]
        private async void appleLoginButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AppleLogin(), false);
        }
    }
}

