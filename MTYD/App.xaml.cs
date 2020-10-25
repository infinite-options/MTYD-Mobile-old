using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using MTYD.Model.Login.LoginClasses.Apple;
using MTYD.Model.Login.Constants;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Xaml;

using MTYD.Model.Database;

using MTYD.ViewModel;
using System.Net.Http;
using MTYD.Model.Login.LoginClasses;
using Newtonsoft.Json;
using System.Text;

[assembly: ExportFont("PlatNomor-WyVnn.ttf", Alias = "ButtonFont")]


namespace MTYD
{
    public partial class App : Application
    {

        static UserLoginDatabase database;
        static Boolean loggedIn = false;

        public const string LoggedInKey = "LoggedIn";
        public const string AppleUserIdKey = "AppleUserIdKey";
        string userId;
        DateTime time_stamp;

        // MTYP ORIGINAL APP CONSTRUCTOR
        /*
        public App()
        {
            InitializeComponent();

            if (database == null)
            {
                database = new UserLoginDatabase();
            }




            MainPage = new NavigationPage(new MainPage());

            // MainPage = new Login();
            // MainPage = new NavigationPage(new MealSchedule());
            // MainPage = new PaymentPage();
            // MainPage = new Profile();
        }*/

        //// OLD MTYP APP CONSTRUCTOR 
        //public App()
        //{
        //    InitializeComponent();

        //    if (Preferences.Get(LoggedInKey, false))
        //    {
        //        MainPage = new CarlosHomePage();
        //    }
        //    else
        //    {
        //        DateTime today = DateTime.Now;
        //        DateTime expTime = today;

        //        if (this.Properties.ContainsKey("time_stamp"))
        //        {
        //            expTime = (DateTime)this.Properties["time_stamp"];
        //        }

        //        if (this.Properties.ContainsKey("access_token")
        //            && this.Properties.ContainsKey("refresh_token")
        //            && this.Properties.ContainsKey("time_stamp") && today <= expTime)
        //        {

        //            MainPage = new CarlosHomePage();

        //        }
        //        else if (this.Properties.ContainsKey("access_token")
        //           && this.Properties.ContainsKey("refresh_token")
        //           && this.Properties.ContainsKey("time_stamp") && today > expTime)
        //        {

        //            MainPage myPage = new MainPage();

        //            System.Object sender = new System.Object();
        //            System.EventArgs e = new System.EventArgs();

        //            if (this.Properties["platform"].Equals(Constant.Google))
        //            {
        //                myPage.googleLoginButtonClicked(sender, e);
        //            }
        //            if (this.Properties["platform"].Equals(Constant.Facebook))
        //            {
        //                myPage.facebookLoginButtonClicked(sender, e);
        //            }
        //        }
        //        else if (this.Properties.ContainsKey("social"))
        //        {
        //            MainPage = new CarlosHomePage();
        //        }
        //        else
        //        {
        //            MainPage = new MainPage();
        //        }
        //    }
        //}

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());

            if (Application.Current.Properties.ContainsKey("user_id"))
            {
                if (Application.Current.Properties.ContainsKey("time_stamp"))
                {
                    DateTime today = DateTime.Now;
                    DateTime expTime = (DateTime)Application.Current.Properties["time_stamp"];

                    if (today <= expTime)
                    {
                        //original carlos code
                        //MainPage = new CarlosHomePage();

                        Console.WriteLine("REMEMBERED*************");
                        MainPage = new SubscriptionPage();
                        //MainPage = new NavigationPage(new MainPage());
                        //Navigation.PushAsync(new SubscriptionPage());
                    }
                    else
                    {
                        MainPage client = new MainPage();
                        MainPage = client;

                        if (Application.Current.Properties.ContainsKey("time_stamp"))
                        {
                            string socialPlatform = (string)Application.Current.Properties["platform"];

                            if (socialPlatform.Equals("FACEBOOK"))
                            {
                                client.facebookLoginButtonClicked(new object(), new EventArgs());
                            }
                            else if (socialPlatform.Equals("GOOGLE"))
                            {
                                client.googleLoginButtonClicked(new object(), new EventArgs());
                            }
                            else if (socialPlatform.Equals("APPLE"))
                            {
                                client.appleLoginButtonClicked(new object(), new EventArgs());
                            }
                            else
                            {
                                //original carlos code
                                //MainPage = new MainPage();

                                //MainPage = new NavigationPage(new MainPage());
                            }
                        }
                    }
                }
                else
                {
                    //original carlos code
                    //MainPage = new MainPage();

                    //MainPage = new NavigationPage(new MainPage());
                }
            }
            else
            {
                //original carlos code
                //MainPage = new MainPage();

                //MainPage = new NavigationPage(new MainPage());
            }
        
        }

        protected override async void OnStart()
        {
            var appleSignInService = DependencyService.Get<IAppleSignInService>();

            if (appleSignInService != null)
            {
                userId = await SecureStorage.GetAsync(AppleUserIdKey);

                if (appleSignInService.IsAvailable && !string.IsNullOrEmpty(userId))
                {
                    var credentialState = await appleSignInService.GetCredentialStateAsync(userId);
                    switch (credentialState)
                    {
                        case AppleSignInCredentialState.Authorized:
                            break;
                        case AppleSignInCredentialState.NotFound:
                        case AppleSignInCredentialState.Revoked:
                            //Logout;
                            SecureStorage.Remove(AppleUserIdKey);
                            Preferences.Set(LoggedInKey, false);
                            MainPage = new MainPage();
                            break;
                    }
                }
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static UserLoginDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new UserLoginDatabase();
                }
                return database;
            }
        }


        public static Boolean LoggedIn
        {
            get
            {
                return loggedIn;
            }
        }

        public static void setLoggedIn(Boolean loggedIn)
        {
            App.loggedIn = loggedIn;
        }
    }
}
