using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using MTYD.Model.Login;
using System.Threading.Tasks;
using MTYD.Model.Database;
using System.Windows.Input;
using Xamarin.Auth;
using System.Diagnostics;
using System.Collections;
using MTYD.ViewModel;
using MTYD.Model.User;
using System.IO;

﻿using MTYD.ViewModel;

using System.Collections.Generic;
using System.ComponentModel;



namespace MTYD
{
    public partial class MainPage : ContentPage
    {
        const string socialUrl = "https://uavi7wugua.execute-api.us-west-1.amazonaws.com/dev/api/v2/social/"; // api to check if user has a social media account; need email at end of link
        const string socialLoginUrl = "https://uavi7wugua.execute-api.us-west-1.amazonaws.com/dev/api/v2/socialacc/"; // api to login the user with social account, need user id at end of link
        
        const string accountSaltUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/accountsalt?email=quang@gmail.com";
        const string loginUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/login"; 
        public HttpClient client = new HttpClient(); // client to handle all api calls

        Account account;
        [Obsolete]
        AccountStore store;

        [Obsolete]
        public MainPage()
        {
            InitializeComponent();
            store = AccountStore.Create();
            checkPlatform();
            forgotPass.CornerRadius = 0;
        }

        private void checkPlatform()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                foreach (var button in absLayout.Children)
                {
                    if (button is Button)
                    {
                        ((Button)button).CornerRadius = 25;
                    }
                }
                googleLoginButton.CornerRadius = 27;
                appleLoginButton.CornerRadius = 27;
                facebookLoginButton.CornerRadius = 27;
                passFrame.CornerRadius = 20;
                userFrame.CornerRadius = 20;
                seePassword.CornerRadius = 14;
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                foreach (var button in absLayout.Children)
                {
                    if (button is Button)
                    {
                        ((Button)button).CornerRadius = 20;
                    }
                }

                Heading.CharacterSpacing = 1;
            }
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

        public class UserInfo {
            public string email { get; set; }
            public string password { get; set; }
        }

        // logs the user into the app 
        // returns a LoginResponse if successful and null if unsuccessful 
        //public async Task<LoginResponse> login(string userEmail, string userPassword, AccountSalt accountSalt)
        public async void login(string userEmail, string userPassword, AccountSalt accountSalt)
        {
            Console.WriteLine("login email" + userEmail);
            Console.WriteLine("login pw" + userPassword);
            Console.WriteLine("login acct salt" + accountSalt);

            const string deviceBrowserType = "Mobile";
             var deviceIpAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();

            //var deviceIpAddress = "0.0.0.0";
            if (deviceIpAddress != null)
            {
                try
                {
                    /*
                    LoginPost loginPostContent = new LoginPost()
                    { // object that contains ip address and browser type; will be converted into a json object 
                        ipAddress = deviceIpAddress.ToString(),
                        browserType = deviceBrowserType
                    };

                    string loginPostContentJson = JsonConvert.SerializeObject(loginPostContent); // make orderContent into json

                    var httpContent = new StringContent(loginPostContentJson, Encoding.UTF8, "application/json"); // encode orderContentJson into format to send to database
                    */

                    /*
                    UserInfo ui = new UserInfo()
                    {
                        email = "quang@gmail.com",
                        password = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71"
                    };
                    */



                    SHA512 sHA512 = new SHA512Managed();
                    Console.WriteLine("sha " + sHA512);

                    byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(userPassword + accountSalt.result[0].password_salt)); // take the password and account salt to generate hash
                    Console.WriteLine("data " + data[0]);

                    string hashedPassword = BitConverter.ToString(data).Replace("-", string.Empty).ToLower(); // convert hash to hex

                    UserInfo ui = new UserInfo()
                    {
                        email = userEmail,
                        password = hashedPassword,

                    };

                    Console.WriteLine("hash pw " + hashedPassword);

                    var data2 = JsonConvert.SerializeObject(ui);
                    var content = new StringContent(data2, Encoding.UTF8, "application/json");
                    Console.WriteLine("data2 "  + data2 );
                    Console.WriteLine("after content 176");
                    Console.WriteLine("login url " + loginUrl);

                    using (var httpClient = new HttpClient())
                    {
                        Console.WriteLine("HTTPclient " + httpClient);

                        Console.WriteLine("inside using");

                        var request1 = new HttpRequestMessage();
                        Console.WriteLine("request " + request1);

                        request1.Method = HttpMethod.Post;
                        Console.WriteLine("rq method " + request1.Method);

                        request1.Content = content;
                        Console.WriteLine("request ctnt " + request1.Content);

                        var httpResponse = await httpClient.PostAsync(loginUrl, content);
                        //HttpResponseMessage response = await httpClient.SendAsync(request);
                        //Console.WriteLine("This is the response from request" + response);
                        /*
                        var endpointresponse = await httpClient.GetAsync(loginUrl);
                        string jsonobject = endpointresponse.Content.ReadAsStringAsync().Result;
                        var data3 = httpClient.GetStringAsync(loginUrl);
                        Console.WriteLine("data 3 " + httpResponse.RequestMessage.Content);
                        */
                    }
                    Console.WriteLine("after 208");
                    /*
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(loginUrl);
                    request.Method = HttpMethod.Post;
                    request.Content = content;

                    var client = new HttpClient();
                    HttpResponseMessage response = await client.SendAsync(request);
                    string items = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("items " + items);
                    */
                    //string uiString = JsonConvert.SerializeObject(ui);
                    //var httpContent = new StringContent(uiString, Encoding.UTF8, "application/json"); // encode orderContentJson into format to send to database

                    /*
                    SHA512 sHA512 = new SHA512Managed();
                    byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(userPassword + accountSalt.result[0].passwordSalt)); // take the password and account salt to generate hash
                    string hashedPassword = BitConverter.ToString(data).Replace("-", string.Empty).ToLower(); // convert hash to hex

                    */

                    //var respString = loginUrl + userEmail + "/" + hashedPassword;
                    //var respString = loginUrl;
                    //var response = await client.PostAsync(respString, httpContent); // try to post to database
                    //var response = await client.PostAsync(respString, httpContent); // try to post to database
                    //var answer = await client.GetStringAsync(loginUrl);
                    //Console.WriteLine("Answer " + answer);
                    /*
                    if (response.Content != null)
                    { // post was successful
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                        System.Diagnostics.Debug.WriteLine("URL: " + respString + "\n" + uiString + "\n " + loginResponse);

                        return loginResponse;

                    }
                    */

                }
                catch (Exception e)
                {
                    Console.WriteLine("catch 225");

                    System.Diagnostics.Debug.WriteLine("Exception message: " + e.Message);
                    //return null;

                }


            }
            //return null;

                }


        public async Task<LoginResponse> socialLogin(string userUid)
        {
            const string deviceBrowserType = "Mobile";
            const string deviceIpAddress = "0.0.0.0";

            LoginPost loginPostContent = new LoginPost()
            { // object that contains ip address and browser type; will be converted into a json object 
                ipAddress = deviceIpAddress.ToString(),
                browserType = deviceBrowserType
            };

            string loginPostContentJson = JsonConvert.SerializeObject(loginPostContent); // make orderContent into json

            var httpContent = new StringContent(loginPostContentJson, Encoding.UTF8, "application/json"); // encode orderContentJson into format to send to database

            var response = await client.PostAsync(socialLoginUrl + userUid, httpContent); // try to post to database
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                return loginResponse;
            }
            return null;
        }

        // uses account salt api to retrieve the user's account salt
        // account salt is used to find the user's hashed password
        public async Task<AccountSalt> retrieveAccountSalt(string userEmail)
        {

            try
            {
                /*
                var url = accountSaltUrl + userEmail;
                System.Diagnostics.Debug.WriteLine("url " + url);
                var content = await client.GetStringAsync(accountSaltUrl + userEmail); // get the requested account salt
                var accountSalt = JsonConvert.DeserializeObject<AccountSalt>(content);
                System.Diagnostics.Debug.WriteLine("try" + accountSalt);

                //System.Diagnostics.Debug.WriteLine("account salt good " + accountSalt.result[0].password_salt);
                //System.Diagnostics.Debug.WriteLine("account salt good " + accountSalt.result[0].password_algorithm);
                return accountSalt;
                */

                /*
                var request = new HttpRequestMessage();

                request.RequestUri = new Uri(accountSaltUrl);
                */
                UriBuilder builder = new UriBuilder("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/accountsalt");
                builder.Query = "email=quang@gmail.com";
                System.Diagnostics.Debug.WriteLine("builder " + builder);
                System.Diagnostics.Debug.WriteLine("builderq " + builder.Query);

                var result =  await client.GetStringAsync(builder.Uri);

                Console.WriteLine("result line 287 = " + result);
                /*
                using (StreamReader sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
                {
                    Console.WriteLine(sr.ReadToEnd());
                }
                */
                /*

                request.Method = HttpMethod.Get;

                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);

                string items = await response.Content.ReadAsStringAsync();
                */
                Console.WriteLine("line 303");
                AccountSalt data = new AccountSalt();
                Console.WriteLine("line 305");
                data = JsonConvert.DeserializeObject<AccountSalt>(result);
                Console.WriteLine("line 307 Data: " + data.result[0].password_salt.ToString());

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("line 313");
                return null;
            }
            //return null;
        }

        /*public async void captureLoginSession(LoginResponse loginResponse)
        {

            var userSessionInformation = new UserLoginSession
            { // object to send into local database
                UserUid = loginResponse.Result.Result[0].UserUid,
                FirstName = loginResponse.Result.Result[0].FirstName,
                SessionId = loginResponse.LoginAttemptLog.SessionId,
                LoginId = loginResponse.LoginAttemptLog.LoginId,
                Email = loginResponse.Result.Result[0].UserEmail
            };

            await App.Database.SaveItemAsync(userSessionInformation); // send login session to local database
            System.Diagnostics.Debug.WriteLine("user logged in: " + App.Database.GetLastItem().Email);
            App.setLoggedIn(true);
            MainPage mainPage = (MainPage)Navigation.NavigationStack[0];
            mainPage.updateLoginButton();
        }*/

        // handler for when google login button is clicked 
        [Obsolete]
        private async void googleLoginButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GoogleLogin(), false);
            /*
            string clientId = null;
            string redirectUri = null;

            // retrieve client id based on the platform
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = SocialMediaLoginConstants.GoogleiOSClientId;
                    redirectUri = SocialMediaLoginConstants.GoogleiOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = SocialMediaLoginConstants.GoogleAndroidClientId;
                    redirectUri = SocialMediaLoginConstants.GoogleAndroidRedirectUrl;
                    break;
            }

            account = store.FindAccountsForService(SocialMediaLoginConstants.AppName).FirstOrDefault();

            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                SocialMediaLoginConstants.GoogleScope,
                new Uri(SocialMediaLoginConstants.GoogleAuthorizeUrl),
                new Uri(redirectUri),
                new Uri(SocialMediaLoginConstants.GoogleAccessTokenUrl),
                null,
                true);

            //authenticator.Completed += OnAuthCompleted;
            //authenticator.Error += OnAuthError;

            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();

            presenter.Login(authenticator);
            */
        }

        // handler for when facebook login button is clicked
        [Obsolete]
        private async void facebookLoginButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FacebookLogin(), false);
            /*
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = SocialMediaLoginConstants.FacebookiOSClientId;
                    redirectUri = SocialMediaLoginConstants.FacebookiOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = SocialMediaLoginConstants.FacebookAndroidClientId;
                    redirectUri = SocialMediaLoginConstants.FacebookAndroidRedirectUrl;
                    break;
            }

            account = store.FindAccountsForService(SocialMediaLoginConstants.AppName).FirstOrDefault();

            var authenticator = new OAuth2Authenticator(
                clientId,
                SocialMediaLoginConstants.FacebookScope,
                new Uri(SocialMediaLoginConstants.FacebookAuthorizeUrl),
                new Uri(SocialMediaLoginConstants.FacebookAccessTokenUrl),
                null);

            //authenticator.Completed += OnAuthCompleted;
            //authenticator.Error += OnAuthError;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
            */

        }

        // handler for when apple login button is clicked (copied from fb login)
        [Obsolete]
        private async void appleLoginButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AppleLogin(), false);
            /*
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = SocialMediaLoginConstants.FacebookiOSClientId;
                    redirectUri = SocialMediaLoginConstants.FacebookiOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = SocialMediaLoginConstants.FacebookAndroidClientId;
                    redirectUri = SocialMediaLoginConstants.FacebookAndroidRedirectUrl;
                    break;
            }

            account = store.FindAccountsForService(SocialMediaLoginConstants.AppName).FirstOrDefault();

            var authenticator = new OAuth2Authenticator(
                clientId,
                SocialMediaLoginConstants.FacebookScope,
                new Uri(SocialMediaLoginConstants.FacebookAuthorizeUrl),
                new Uri(SocialMediaLoginConstants.FacebookAccessTokenUrl),
                null);

            //authenticator.Completed += OnAuthCompleted;
            //authenticator.Error += OnAuthError;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
            */

        }

        // function when the auth is completed without any errors
        /*[Obsolete]
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
            Debug.WriteLine("starting authentication");
            if (e.IsAuthenticated)
            {
                Debug.WriteLine("first authentication");
                if (authenticator.AuthorizeUrl.Host == "www.facebook.com")
                {
                    Debug.WriteLine("authenticated!!!");
                    FacebookEmail facebookEmail = null;

                    var json = await client.GetStringAsync($"https://graph.facebook.com/me?fields=id,name,first_name,last_name,email,picture.type(large)&access_token=" + e.Account.Properties["access_token"]);

                    facebookEmail = JsonConvert.DeserializeObject<FacebookEmail>(json);

                    await store.SaveAsync(account = e.Account, SocialMediaLoginConstants.AppName);

                    Application.Current.Properties.Remove("Id");
                    Application.Current.Properties.Remove("FirstName");
                    Application.Current.Properties.Remove("LastName");
                    Application.Current.Properties.Remove("DisplayName");
                    Application.Current.Properties.Remove("EmailAddress");
                    Application.Current.Properties.Remove("ProfilePicture");

                    Application.Current.Properties.Add("Id", facebookEmail.Id);
                    Application.Current.Properties.Add("FirstName", facebookEmail.First_Name);
                    Application.Current.Properties.Add("LastName", facebookEmail.Last_Name);
                    Application.Current.Properties.Add("DisplayName", facebookEmail.Name);
                    Application.Current.Properties.Add("EmailAddress", facebookEmail.Email);
                    Application.Current.Properties.Add("ProfilePicture", facebookEmail.Picture.Data.Url);

                    try
                    {
                        var socialAccountJson = await client.GetStringAsync(socialUrl + facebookEmail.Email); // get the user's account from the social accounts table

                        SocialAccountResponse socialAccountResponse = JsonConvert.DeserializeObject<SocialAccountResponse>(socialAccountJson);

                        if (socialAccountResponse.Result.Result.Length == 0)
                        { // if the social account doesn't exist, navigate to social sign up page
                            Debug.WriteLine("no social account found");
                            string accessToken = e.Account.Properties["access_token"]; // access token retrieved from facebook
                            // facebook doesn't provide refresh token!
                            string socialMedia = "Facebook";
                            SocialMediaSignUpPage socialSignUpPage = new SocialMediaSignUpPage(facebookEmail.First_Name, facebookEmail.Last_Name, facebookEmail.Email, socialMedia, accessToken, ""); // declare new social sign up page with user's name, email, and tokens

                            await Navigation.PushAsync(socialSignUpPage);
                        }
                        else
                        { // user's social account exists and login attempt is made
                            Debug.WriteLine("social account found, logging in");
                            LoginResponse socialLoginAttempt = await socialLogin(socialAccountResponse.Result.Result[0].UserUid);
                            captureLoginSession(socialLoginAttempt);
                            await Navigation.PopAsync(); // go back to home page

                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                }
                else
                {
                    User user = null;

                    // If the user is authenticated, request their basic user data from Google
                    // UserInfoUrl https://www.googleapis.com/oauth2/v2/userinfo

                    var request = new OAuth2Request("GET", new Uri(SocialMediaLoginConstants.GoogleUserInfoUrl), null, e.Account); // create the request to get the user's google info
                    var response = await request.GetResponseAsync();
                    if (response != null)
                    {
                        // Deserialize the data and store it in the account store
                        // The users email address will be used to identify data in SimpleDB
                        string userJson = await response.GetResponseTextAsync();
                        Debug.WriteLine("user json: " + userJson);
                        user = JsonConvert.DeserializeObject<User>(userJson);

                    }

                    if (account != null)
                    {
                        store.Delete(account, SocialMediaLoginConstants.AppName);
                    }

                    await store.SaveAsync(account = e.Account, SocialMediaLoginConstants.AppName);

                    Application.Current.Properties.Remove("Id");
                    Application.Current.Properties.Remove("FirstName");
                    Application.Current.Properties.Remove("LastName");
                    Application.Current.Properties.Remove("DisplayName");
                    Application.Current.Properties.Remove("EmailAddress");
                    Application.Current.Properties.Remove("ProfilePicture");

                    Application.Current.Properties.Add("Id", user.Id);
                    Application.Current.Properties.Add("FirstName", user.GivenName);
                    Application.Current.Properties.Add("LastName", user.FamilyName);
                    Application.Current.Properties.Add("DisplayName", user.Name);
                    Application.Current.Properties.Add("EmailAddress", user.Email);
                    Application.Current.Properties.Add("ProfilePicture", user.Picture);
                    try
                    {
                        var socialAccountJson = await client.GetStringAsync(socialUrl + user.Email); // get the user's account from the social accounts table

                        SocialAccountResponse socialAccountResponse = JsonConvert.DeserializeObject<SocialAccountResponse>(socialAccountJson);

                        if (socialAccountResponse.Result.Result.Length == 0)
                        { // if the social account doesn't exist, navigate to social sign up page
                            string accessToken = e.Account.Properties["access_token"]; // access token retrieved from google 
                            string refreshToken = e.Account.Properties["refresh_token"]; // refresh token retrieved from google 
                            string socialMedia = "Google";
                            SocialMediaSignUpPage socialSignUpPage = new SocialMediaSignUpPage(user.GivenName, user.FamilyName, user.Email, socialMedia, accessToken, refreshToken); // declare new social sign up page, maybe bind to certain info???

                            await Navigation.PushAsync(socialSignUpPage);
                        }
                        else
                        { // user's social account exists and login attempt is made
                            LoginResponse socialLoginAttempt = await socialLogin(socialAccountResponse.Result.Result[0].UserUid);
                            captureLoginSession(socialLoginAttempt);
                            await Navigation.PopAsync(); // go back to home page

                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    //await Navigation.PopAsync();
                }
            }
        }

        // function when authenticator gives an error
        [Obsolete]
        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            Debug.WriteLine("Authentication Error " + e.Message);
        }*/


        async void clickedSignUp(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUp(), false);
        }

        /*public void updateLoginButton()
        {
            if (!App.LoggedIn)
            {
                this.loginButton.Text = "Log in";
                signUpButton.SetValue(IsVisibleProperty, true);
                mainSubStack.IsVisible = false;

            }
            else
            {
                this.loginButton.Text = "Log out";
                signUpButton.SetValue(IsVisibleProperty, false);
                mainSubStack.IsVisible = true;

            }
        }

        // Navigation Bar
        private async void onNavClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Equals(SubscribeNav))
            {
                await Navigation.PushAsync(new SubscriptionPage());
            }
            else if (button.Equals(ProfileNav))
            {
                await Navigation.PushAsync(new Profile());
            }
            else if (button.Equals(SelectNav))
            {
                await Navigation.PushAsync(new Select());
            }
        }

        // Navigation Bar Icons
        private async void onNavIconClick(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;

            if (button.Equals(SubscribeIconNav))
            {
                await Navigation.PushAsync(new SubscriptionPage());
            }
            else if (button.Equals(ProfileIconNav))
            {
                await Navigation.PushAsync(new Profile());
            }
            else if (button.Equals(SelectIconNav))
            {
                await Navigation.PushAsync(new Select());
            }

        }*/

        void clickedForgotPass(System.Object sender, System.EventArgs e)
        {
            DisplayAlert("Title", "Message", "Nope");
        }

        void clickedSeePassword(System.Object sender, System.EventArgs e)
        {
            if (loginPassword.IsPassword == true)
                loginPassword.IsPassword = false;
            else loginPassword.IsPassword = true;
        }
    }
}
