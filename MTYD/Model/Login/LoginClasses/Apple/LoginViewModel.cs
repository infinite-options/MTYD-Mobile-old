using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MTYD.Model.Login.Constants;
using MTYD.ViewModel;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTYD.Model.Login.LoginClasses.Apple
{
    public class LoginViewModel
    {
        public static string apple_token = null;
        public static string apple_email = null;

        public bool IsAppleSignInAvailable { get { return appleSignInService?.IsAvailable ?? false; } }
        public ICommand SignInWithAppleCommand { get; set; }

        public event EventHandler AppleError = delegate { };
        public event EventHandler PlatformError = delegate { };

        IAppleSignInService appleSignInService = null;
        public LoginViewModel()
        {
            appleSignInService = DependencyService.Get<IAppleSignInService>();
            SignInWithAppleCommand = new Command(OnAppleSignInRequest);
        }

        public async void OnAppleSignInRequest()
        {
            var account = await appleSignInService.SignInAsync();
            if (account != null)
            {
                Preferences.Set(App.LoggedInKey, true);
                await SecureStorage.SetAsync(App.AppleUserIdKey, account.UserId);

                if (account.Token == null) { account.Token = ""; }
                if (account.Email != null)
                {
                    if (Application.Current.Properties.ContainsKey(account.UserId.ToString()))
                    {
                        Application.Current.Properties[account.UserId.ToString()] = account.Email;
                    }
                    else
                    {
                        Application.Current.Properties[account.UserId.ToString()] = account.Email;
                    }
                }
                if (account.Email == null) { account.Email = ""; }
                if (account.Name == null) { account.Name = ""; }

                System.Diagnostics.Debug.WriteLine((string)Application.Current.Properties[account.UserId.ToString()]);
                AppleUserProfileAsync(account.UserId, account.Token, (string)Application.Current.Properties[account.UserId.ToString()], account.Name);
            }
            else
            {
                AppleError?.Invoke(this, default(EventArgs));
            }
        }

        public async void AppleUserProfileAsync(string appleId, string appleToken, string appleUserEmail, string userName)
        {
            System.Diagnostics.Debug.WriteLine("LINE 95");
            var client = new HttpClient();
            var socialLogInPost = new SocialLogInPost();

            socialLogInPost.email = appleUserEmail;
            socialLogInPost.password = "";
            socialLogInPost.social_id = appleId;
            socialLogInPost.signup_platform = "APPLE";

            var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);

            System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

            var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");
            var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);
            var responseContent = await RDSResponse.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine(responseContent);

            if (RDSResponse.IsSuccessStatusCode)
            {
                if (responseContent != null)
                {
                    if (responseContent.Contains(Constant.EmailNotFound))
                    {
                        var signUp = await Application.Current.MainPage.DisplayAlert("Message", "It looks like you don't have a MTYD account. Please sign up!", "OK", "Cancel");
                        if (signUp)
                        {
                            // HERE YOU NEED TO SUBSTITUTE MY SOCIAL SIGN UP PAGE WITH MTYD SOCIAL SIGN UP
                            // NOTE THAT THIS SOCIAL SIGN UP PAGE NEEDS A CONSTRUCTOR LIKE THE FOLLOWING ONE
                            // SocialSignUp(string socialId, string firstName, string lastName, string emailAddress, string accessToken, string refreshToken, string platform)
                            Application.Current.MainPage = new CarlosSocialSignUp(appleId, userName, "", appleUserEmail, appleToken, appleToken, "APPLE");
                        }
                    }
                    if (responseContent.Contains(Constant.AutheticatedSuccesful))
                    {
                        var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                        Application.Current.Properties["user_id"] = data.result[0].customer_uid;

                        UpdateTokensPost updateTokesPost = new UpdateTokensPost();
                        updateTokesPost.uid = data.result[0].customer_uid;
                        updateTokesPost.mobile_access_token = appleToken;
                        updateTokesPost.mobile_refresh_token = appleToken;

                        var updateTokesPostSerializedObject = JsonConvert.SerializeObject(updateTokesPost);
                        var updateTokesContent = new StringContent(updateTokesPostSerializedObject, Encoding.UTF8, "application/json");
                        var updateTokesResponse = await client.PostAsync(Constant.UpdateTokensUrl, updateTokesContent);
                        var updateTokenResponseContent = await updateTokesResponse.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine(updateTokenResponseContent);

                        if (updateTokesResponse.IsSuccessStatusCode)
                        {
                            DateTime today = DateTime.Now;
                            DateTime expDate = today.AddDays(Constant.days);

                            Application.Current.Properties["time_stamp"] = expDate;
                            Application.Current.Properties["platform"] = "APPLE";
                            // Application.Current.MainPage = new SubscriptionPage();
                            Application.Current.MainPage = new NavigationPage(new SubscriptionPage());

                            // THIS IS HOW YOU CAN ACCESS YOUR USER ID FROM THE APP
                            // string userID = (string)Application.Current.Properties["user_id"];
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Oops", "We are facing some problems with our internal system. We weren't able to update your credentials", "OK");
                        }
                    }
                    if (responseContent.Contains(Constant.ErrorPlatform))
                    {
                        var RDSCode = JsonConvert.DeserializeObject<RDSLogInMessage>(responseContent);
                        await Application.Current.MainPage.DisplayAlert("Message", RDSCode.message, "OK");
                    }

                    if (responseContent.Contains(Constant.ErrorUserDirectLogIn))
                    {
                        await Application.Current.MainPage.DisplayAlert("Oops!", "You have an existing MTYD account. Please use direct login", "OK");
                    }
                }
            }
        }
    }
}
