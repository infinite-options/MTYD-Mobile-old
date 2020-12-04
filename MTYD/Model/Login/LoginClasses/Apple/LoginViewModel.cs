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
//using ServingFresh.Config;
using MTYD.Model.Login;
using MTYD.Model.SignUp;
using MTYD.Model.User;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
//using ServingFresh.Views;

namespace MTYD.Model.Login.LoginClasses.Apple
{
    public class LoginViewModel
    {
        public ObservableCollection<Plans> NewLogin = new ObservableCollection<Plans>();
        public static string apple_token = null;
        public static string apple_email = null;

        public bool IsAppleSignInAvailable { get { return appleSignInService?.IsAvailable ?? false; } }
        public ICommand SignInWithAppleCommand { get; set; }

        public event EventHandler AppleError = delegate { };
        public event EventHandler PlatformError = delegate { };

        IAppleSignInService appleSignInService = null;
        public LoginViewModel()
        {
            Console.WriteLine("in LoginViewModel, entered LoginViewModel()");

            appleSignInService = DependencyService.Get<IAppleSignInService>();
            Console.WriteLine("after appleSignInService reached");
            SignInWithAppleCommand = new Command(OnAppleSignInRequest);
            Console.WriteLine("after SignInWithAppleCommand reached");
        }

        public async void OnAppleSignInRequest()
        {
            Console.WriteLine("in LoginViewModel, entered OnAppleSignInRequest");

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
                        Console.WriteLine("updateTokesPostSerializedObject: " + updateTokesPostSerializedObject.ToString());
                        var updateTokesContent = new StringContent(updateTokesPostSerializedObject, Encoding.UTF8, "application/json");
                        Console.WriteLine("updateTokesContent: " + updateTokesContent.ToString());
                        var updateTokesResponse = await client.PostAsync(Constant.UpdateTokensUrl, updateTokesContent);
                        Console.WriteLine("updateTokesResponse: " + updateTokesResponse.ToString());
                        var updateTokenResponseContent = await updateTokesResponse.Content.ReadAsStringAsync();
                        Console.WriteLine("updateTokenResponseContent: " + updateTokenResponseContent.ToString());
                        System.Diagnostics.Debug.WriteLine(updateTokenResponseContent);

                        if (updateTokesResponse.IsSuccessStatusCode)
                        {
                            DateTime today = DateTime.Now;
                            DateTime expDate = today.AddDays(Constant.days);

                            Application.Current.Properties["time_stamp"] = expDate;
                            Application.Current.Properties["platform"] = "APPLE";

                            var request = new HttpRequestMessage();
                            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                            Console.WriteLine("url: " + url);
                            request.RequestUri = new Uri(url);
                            //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                            request.Method = HttpMethod.Get;
                            var client2 = new HttpClient();
                            HttpResponseMessage response = await client2.SendAsync(request);

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {

                                HttpContent content = response.Content;
                                Console.WriteLine("content: " + content);
                                var userString = await content.ReadAsStringAsync();
                                Console.WriteLine(userString);

                                if (userString.ToString()[0] != '{')
                                {
                                    Console.WriteLine("go to SubscriptionPage");
                                    Application.Current.MainPage = new NavigationPage(new SubscriptionPage());
                                    return;
                                }

                                JObject info_obj2 = JObject.Parse(userString);
                                this.NewLogin.Clear();

                                //ArrayList item_price = new ArrayList();
                                //ArrayList num_items = new ArrayList();
                                //ArrayList payment_frequency = new ArrayList();
                                //ArrayList groupArray = new ArrayList();

                                //int counter = 0;
                                //while (((info_obj2["result"])[0]).ToString() != "{}")
                                //{
                                //    Console.WriteLine("worked" + counter);
                                //    counter++;
                                //}

                                Console.WriteLine("string: " + (info_obj2["result"]).ToString());
                                //check if the user hasn't entered any info before, if so put in the placeholders
                                if ((info_obj2["result"]).ToString() == "[]" || (info_obj2["result"]).ToString() == "204")
                                {
                                    Console.WriteLine("go to SubscriptionPage");
                                    Application.Current.MainPage = new NavigationPage(new SubscriptionPage());
                                }
                                else Application.Current.MainPage = new NavigationPage(new Select((info_obj2["result"])[0]["delivery_first_name"].ToString(), (info_obj2["result"])[0]["delivery_last_name"].ToString(), (info_obj2["result"])[0]["delivery_email"].ToString()));
                            }

                            // Application.Current.MainPage = new SubscriptionPage();
                            //Application.Current.MainPage = new NavigationPage(new SubscriptionPage());

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
