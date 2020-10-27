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

        // Initialize variables for Apple Login
        public const string LoggedInKey = "LoggedIn";
        public const string AppleUserIdKey = "AppleUserIdKey";
        string userId;


        public App()
        {
            InitializeComponent();
            //MainPage = new NavigationPage(new MainPage());

            // user_id and time_stamp are retrieved from local phone memory (written by Login View Model, Signup, Social Signup and MainPage.xaml.cs)
            // Question: What else is stored in local memory? (user_id, time_stamp, ??)
            if (Application.Current.Properties.ContainsKey("user_id"))

            {
                // Use System.Diagnostics.Debug.Writeline for comments to see output in in Blue in Application Output tab
                System.Diagnostics.Debug.WriteLine("UserID is:" + (string)Application.Current.Properties["user_id"])
                // Question: How do I find out what is contained within Application.Current.Properties?  Can you just add variable without pre-defining them?
                if (Application.Current.Properties.ContainsKey("time_stamp"))
                    
                {
                    System.Diagnostics.Debug.WriteLine("Time Stamp is:" + (DateTime)Application.Current.Properties["time_stamp"]);
                    DateTime today = DateTime.Now;
                    DateTime expTime = (DateTime)Application.Current.Properties["time_stamp"];
                    Console.WriteLine("today" + today.ToString());
                    Console.WriteLine("expTime" + expTime.ToString());

                    if (today <= expTime)
                    {
                        // MainPage = new CarlosHomePage();
                        Console.WriteLine("entered time check");
                        MainPage = new NavigationPage(new SubscriptionPage());
                    }
                    else
                    {
                        MainPage client = new MainPage();
                        MainPage = client;

                        if (Application.Current.Properties.ContainsKey("platform"))
                        {
                            System.Diagnostics.Debug.WriteLine("platform is:" + (string)Application.Current.Properties["platform"]);
                            string socialPlatform = (string)Application.Current.Properties["platform"];

                            if (socialPlatform.Equals("FACEBOOK"))
                            {
                                client.facebookLoginButtonClicked(new object(), new EventArgs());
                                // Goes to MainPage.xaml.cs
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
                                //MainPage = new MainPage();
                                MainPage = new NavigationPage(new MainPage());
                            }
                        }
                    }
                }
                else
                {
                    //MainPage = new MainPage();
                    MainPage = new NavigationPage(new MainPage());
                }
            }
            else
            {
                //MainPage = new MainPage();
                MainPage = new NavigationPage(new MainPage());
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
