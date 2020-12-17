using MTYD.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Stripe;

using PayPalHttp;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;

using MTYD.Constants;

using Item = MTYD.Model.Item;
using System.Diagnostics;
using Application = Xamarin.Forms.Application;

namespace MTYD.ViewModel
{
    public partial class VerifyInfo : ContentPage
    {
        public ObservableCollection<Plans> NewDeliveryInfo = new ObservableCollection<Plans>();
        public string AptEntry, FNameEntry, LNameEntry, emailEntry, PhoneEntry, AddressEntry, CityEntry, StateEntry, ZipEntry, DeliveryEntry, CCEntry, CVVEntry, ZipCCEntry;
        public bool isSocialLogin;

        // CREDENTIALS CLASS
        public class Credentials
        {
            public string key { get; set; }
        }

        // PAYPAL CREDENTIALS
        private static string clientId = Constant.TestClientId;
        private static string secret = Constant.TestSecret;
        private string payPalOrderId = "";
        public static string mode = "";

        public VerifyInfo(string AptEntry1, string FNameEntry1, string LNameEntry1, string emailEntry1, string PhoneEntry1, string AddressEntry1, string CityEntry1, string StateEntry1, string ZipEntry1, string DeliveryEntry1, string CCEntry1, string CVVEntry1, string ZipCCEntry1, string salt1)
        {
            InitializeComponent();
            if (salt1 == "")
            {
                isSocialLogin = true;
            }
            else
            {
                isSocialLogin = false;
            }

            AptEntry = AptEntry1; FNameEntry = FNameEntry1; LNameEntry = LNameEntry1; emailEntry = emailEntry1; PhoneEntry = PhoneEntry1; AddressEntry = AddressEntry1; CityEntry = CityEntry1; StateEntry = StateEntry1; ZipEntry = ZipEntry1; DeliveryEntry = DeliveryEntry1; CCEntry = CCEntry1; CVVEntry = CVVEntry1; ZipCCEntry = ZipCCEntry1;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            //cardNum.Text = "**************" + CCEntry1.Substring(CCEntry1.Length - 2);
            name.Text = FNameEntry1 + " " + LNameEntry1;
            if (AptEntry1 == "NULL" || AptEntry1 == "")
            {
                apt.IsVisible = false;
                street.Text = AddressEntry1;
                cityStateZip.Text = CityEntry1 + ", " + StateEntry1 + " " + ZipEntry1;
            }
            else
            {
                apt.Text = "Apt #" + AptEntry1;
                street.Text = AddressEntry1;
                cityStateZip.Text = CityEntry1 + ", " + StateEntry1 + " " + ZipEntry1;
            }


            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.CornerRadius = 35;
                pfp.CornerRadius = 20;
                //password.CornerRadius = 22;
                checkoutButton.CornerRadius = 24;
            }
            SetPayPalCredentials();
        }

        protected async Task setPaymentInfo()
        {
            Console.WriteLine("SetPaymentInfo Func Started!");
            PaymentInfo newPayment = new PaymentInfo();
            //need to add item_business_id
            Item item1 = new Model.Item();
            item1.name = Preferences.Get("item_name", "");
            item1.price = Preferences.Get("price", "00.00");
            item1.qty = "1";
            item1.item_uid = Preferences.Get("item_uid", "");
            item1.itm_business_uid = "200-000001";
            List<Item> itemsList = new List<Item> { item1 };
            Preferences.Set("unitNum", AptEntry);

            string userID = (string)Application.Current.Properties["user_id"];
            Console.WriteLine("YOUR userID is " + userID);
            newPayment.customer_uid = userID;
            //newPayment.customer_uid = "100-000082";
            //newPayment.business_uid = "200-000002";
            newPayment.items = itemsList;
            //newPayment.salt = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71";
            //newPayment.salt = "cec35d4fc0c5e83527f462aeff579b0c6f098e45b01c8b82e311f87dc6361d752c30293e27027653adbb251dff5d03242c8bec68a3af1abd4e91c5adb799a01b";
            //newPayment.salt = "2020-09-22 21:55:17";
            newPayment.salt = "";
            newPayment.delivery_first_name = FNameEntry;
            newPayment.delivery_last_name = LNameEntry;
            newPayment.delivery_email = emailEntry;
            newPayment.delivery_phone = PhoneEntry;
            newPayment.delivery_address = AddressEntry;
            newPayment.delivery_unit = Preferences.Get("unitNum", "");
            newPayment.delivery_city = CityEntry;
            newPayment.delivery_state = StateEntry;
            newPayment.delivery_zip = ZipEntry;
            newPayment.delivery_instructions = DeliveryEntry;
            newPayment.delivery_longitude = "";
            newPayment.delivery_latitude = "";
            newPayment.order_instructions = "slow";
            newPayment.purchase_notes = "new purch";
            newPayment.amount_due = Preferences.Get("price", "00.00");
            newPayment.amount_discount = "00.00";
            newPayment.amount_paid = Preferences.Get("price", "00.00");
            newPayment.cc_num = "4242424242424242";
            newPayment.cc_exp_year = "2022";
            newPayment.cc_exp_month = "08";
            newPayment.cc_cvv = "123";
            newPayment.cc_zip = "12345";
            // OLD IMPLEMENTATION
            //==================================
            //newPayment.cc_num = CCEntry;
            //newPayment.cc_exp_year = YearPicker.Items[YearPicker.SelectedIndex];
            //newPayment.cc_exp_year = "2022";
            //newPayment.cc_exp_month = MonthPicker.Items[MonthPicker.SelectedIndex];
            //newPayment.cc_exp_month = "11";
            //newPayment.cc_cvv = CVVEntry;
            //newPayment.cc_zip = ZipCCEntry;
            //==================================

            //itemsList.Add("1"); //{ "1", "5 Meal Plan", "59.99" };
            var newPaymentJSONString = JsonConvert.SerializeObject(newPayment);
            // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
            var content = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
            Console.WriteLine("Content: " + content);
            /*var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
            request.Method = HttpMethod.Post;
            request.Content = content;*/
            var client = new System.Net.Http.HttpClient();
            var response = await client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout", content);
            // HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("RESPONSE TO CHECKOUT           : " + response.IsSuccessStatusCode);
                Debug.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                Debug.WriteLine("SetPaymentInfo Func ENDED!");
            }
            else
            {
                await DisplayAlert("Ooops", "Our system is down. We were able to process your request. We are currently working to solve this issue", "OK");
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
            await Navigation.PushAsync(new Menu("", ""));
        }

        // CHECKOUT FUNCTION MOVES TO SELECT PAGE IN SUCCESSFUL PAYMENTS ONLY
        private async void checkoutButton_Clicked(object sender, EventArgs e)
        {
            if (checkoutButton.Text == "CONTINUE")
            {
                //if (true)
                //{
                _ = setPaymentInfo();
                _ = Navigation.PushAsync(new Select("", "", ""));
            }
            else
            {
                await DisplayAlert("Oops", "Our records show that you still have to process your payment before moving on the meals selection. Please complete your payment via PayPal or Stripe.", "OK");
            }
        }

        // STRIPE FUNCTIONS

        // FUNCTION  1:
        public async void CheckouWithStripe(System.Object sender, System.EventArgs e)
        {
            var total = Preferences.Get("price", "00.00");
            Debug.WriteLine("STRIPE AMOUNT TO PAY: " + total);
            if (total != "00.00")
            {
                PaymentScreen.HeightRequest = this.Height;
                PaymentScreen.Margin = new Thickness(0, -PaymentScreen.HeightRequest / 2, 0, 0);
                PayPalScreen.Height = this.Height - (this.Height / 8);
                StripeScreen.Height = 0;
            }
            else
            {
                await DisplayAlert("Ooops", "The amount to pay is zero. It must be greater than zero to process a payment", "OK");
            }
        }
        // FUNCTION  2:
        public async void PayViaStripe(System.Object sender, System.EventArgs e)
        {
            try
            {
                var total = Preferences.Get("price", "00.00");
                var clientHttp = new System.Net.Http.HttpClient();
                var stripe = new Credentials();
                    stripe.key = Constant.LivePK;

                var stripeObj = JsonConvert.SerializeObject(stripe);
                var stripeContent = new StringContent(stripeObj, Encoding.UTF8, "application/json");
                var RDSResponse = await clientHttp.PostAsync(Constant.StripeModeUrl, stripeContent);
                var content = await RDSResponse.Content.ReadAsStringAsync();

                Debug.WriteLine("key to send JSON: " + stripeObj);
                Debug.WriteLine("Response from key: " + content);

                if (RDSResponse.IsSuccessStatusCode)
                {
                    if (content != "200")
                    {
                        string SK = "";
                        string mode = "";

                        if (content.Contains("Test"))
                        {
                            mode = "TEST";
                            SK = Constant.TestSK;
                        }
                        else if (content.Contains("Live"))
                        {
                            mode = "LIVE";
                            SK = Constant.LiveSK;
                        }

                        Debug.WriteLine("MODE          : " + mode);
                        Debug.WriteLine("STRIPE SECRET : " + SK);
                       
                        //Debug.WriteLine("SK" + SK);
                        StripeConfiguration.ApiKey = SK;

                        string CardNo = cardHolderNumber.Text.Trim();
                        string expMonth = cardExpMonth.Text.Trim();
                        string expYear = cardExpYear.Text.Trim();
                        string cardCvv = cardCVV.Text.Trim();

                        // Step 1: Create Card
                        TokenCardOptions stripeOption = new TokenCardOptions();
                        stripeOption.Number = CardNo;
                        stripeOption.ExpMonth = Convert.ToInt64(expMonth);
                        stripeOption.ExpYear = Convert.ToInt64(expYear);
                        stripeOption.Cvc = cardCvv;

                        // Step 2: Assign card to token object
                        TokenCreateOptions stripeCard = new TokenCreateOptions();
                        stripeCard.Card = stripeOption;

                        TokenService service = new TokenService();
                        Stripe.Token newToken = service.Create(stripeCard);

                        // Step 3: Assign the token to the soruce 
                        var option = new SourceCreateOptions();
                        option.Type = SourceType.Card;
                        option.Currency = "usd";
                        option.Token = newToken.Id;

                        var sourceService = new SourceService();
                        Source source = sourceService.Create(option);

                        // Step 4: Create customer
                        CustomerCreateOptions customer = new CustomerCreateOptions();
                        customer.Name = cardHolderName.Text.Trim();
                        customer.Email = cardHolderEmail.Text.ToLower().Trim();
                        customer.Description = cardDescription.Text.Trim();
                        if (cardHolderUnit.Text == null)
                        {
                            cardHolderUnit.Text = "";
                        }
                        customer.Address = new AddressOptions { City = cardCity.Text.Trim(), Country = Constant.Contry, Line1 = cardHolderAddress.Text.Trim(), Line2 = cardHolderUnit.Text.Trim(), PostalCode = cardZip.Text.Trim(), State = cardState.Text.Trim() };

                        var customerService = new CustomerService();
                        var cust = customerService.Create(customer);

                        // Step 5: Charge option
                        var chargeOption = new ChargeCreateOptions();
                        chargeOption.Amount = (long)RemoveDecimalFromTotalAmount(total);
                        chargeOption.Currency = "usd";
                        chargeOption.ReceiptEmail = cardHolderEmail.Text.ToLower().Trim();
                        chargeOption.Customer = cust.Id;
                        chargeOption.Source = source.Id;
                        chargeOption.Description = cardDescription.Text.Trim();

                        // Step 6: charge the customer
                        var chargeService = new ChargeService();
                        Charge charge = chargeService.Create(chargeOption);
                        if (charge.Status == "succeeded")
                        {
                            PaymentScreen.HeightRequest = 0;
                            PaymentScreen.Margin = new Thickness(0, 0, 0, 0);
                            StripeScreen.Height = 0;
                            PayPalScreen.Height = 0;
                            Debug.WriteLine("STRIPE PAYMENT WAS SUCCESSFUL");
                            Preferences.Set("price", "00.00");
                            await DisplayAlert("Payment Completed", "Your payment was successful. Press 'CONTINUE' to select your meals!", "OK");
                            checkoutButton.Text = "CONTINUE";
                        }
                        else
                        {
                            // Fail
                            await DisplayAlert("Ooops", "Payment was not succesfull. Please try again", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert!", ex.Message, "OK");
            }
        }

        // FUNCTION  3:
        public int RemoveDecimalFromTotalAmount(string amount)
        {
            var stringAmount = "";
            var arrayAmount = amount.ToCharArray();
            for (int i = 0; i < arrayAmount.Length; i++)
            {
                if ((int)arrayAmount[i] != (int)'.')
                {
                    stringAmount += arrayAmount[i];
                }
            }
            System.Diagnostics.Debug.WriteLine(stringAmount);
            return Int32.Parse(stringAmount);
        }

        // FUNCTION  4:
        public void CancelViaStripe(System.Object sender, System.EventArgs e)
        {
            PaymentScreen.HeightRequest = 0;
            PaymentScreen.Margin = new Thickness(0, 0, 0, 0);
            StripeScreen.Height = 0;
            PayPalScreen.Height = 0;
        }

        // PAYPAL FUNCTIONS

        // FUNCTION  1: SET BROWSER AND PAYPAL SCREEN TO PROCESS PAYMENT
        public async void CheckouWithPayPayl(System.Object sender, System.EventArgs e)
        {
            var total = Preferences.Get("price", "00.00");
            Debug.WriteLine("PAYPAL AMOUNT TO PAY: " + total);
            if (total != "00.00")
            {
                PaymentScreen.HeightRequest = this.Height;
                PaymentScreen.Margin = new Thickness(0, -PaymentScreen.HeightRequest / 2, 0, 0);
                PayPalScreen.Height = 0;
                StripeScreen.Height = this.Height;
                Browser.HeightRequest = this.Height - (this.Height / 8);
                PayViaPayPal(sender, e);
            }
            else
            {
                await DisplayAlert("Ooops", "The amount to pay is zero. It must be greater than zero to process a payment", "OK");
            }
        }

        // FUNCTION  2: CREATES A PAYMENT REQUEST
        public async void PayViaPayPal(System.Object sender, System.EventArgs e)
        {
            
            var response = await createOrder(Preferences.Get("price", "00.00"));
            var content = response.Result<PayPalCheckoutSdk.Orders.Order>();
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

            Debug.WriteLine("Status: {0}", result.Status);
            Debug.WriteLine("Order Id: {0}", result.Id);
            Debug.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
            Debug.WriteLine("Links:");

            foreach (PayPalCheckoutSdk.Orders.LinkDescription link in result.Links)
            {
                Debug.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                if (link.Rel == "approve")
                {
                    Browser.Source = link.Href;
                }
            }

            Browser.Navigated += Browser_Navigated;
            payPalOrderId = result.Id;
        }

        // FUNCTION  3: SET BROWSER SOURCE WITH PROPER URL TO PROCESS PAYMENT
        private void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var source = Browser.Source as UrlWebViewSource;
            Debug.WriteLine("BROWSER CURRENT SOURCE: " + source.Url);
            if (source.Url == "https://servingfresh.me/")
            {
                PaymentScreen.HeightRequest = 0;
                PaymentScreen.Margin = new Thickness(0, 0, 0, 0);
                PayPalScreen.Height = 0;
                StripeScreen.Height = 0;

                _ = captureOrder(payPalOrderId);
            }
        }

        // FUNCTION  4: PAYPAL CLIENT
        public static PayPalHttp.HttpClient client()
        {
            
            Debug.WriteLine("PAYPAL CLIENT ID MTYD: " + clientId);
            Debug.WriteLine("PAYPAL SECRET MTYD   : " + secret);

            if (mode == "TEST")
            {
                PayPalEnvironment enviroment = new SandboxEnvironment(clientId, secret);
                PayPalHttpClient payPalClient = new PayPalHttpClient(enviroment);
                return payPalClient;
            }
            else if (mode == "LIVE")
            {
                PayPalEnvironment enviroment = new LiveEnvironment(clientId, secret);
                PayPalHttpClient payPalClient = new PayPalHttpClient(enviroment);
                return payPalClient;
            }
            return null;
        }

        // FUNCTION  5: SET PAYPAL CREDENTIALS
        public async void SetPayPalCredentials()
        {
            var clientHttp = new System.Net.Http.HttpClient();
            var paypal = new Credentials();
                paypal.key = Constant.LiveClientId;

            var stripeObj = JsonConvert.SerializeObject(paypal);
            var stripeContent = new StringContent(stripeObj, Encoding.UTF8, "application/json");
            var RDSResponse = await clientHttp.PostAsync(Constant.PayPalModeUrl, stripeContent);
            var content = await RDSResponse.Content.ReadAsStringAsync();

            Debug.WriteLine("CREDENTIALS JSON OBJECT TO SEND: " + stripeObj);
            Debug.WriteLine("RESPONE FROM PAYPAL ENDPOINT   : " + content);

            if (RDSResponse.IsSuccessStatusCode)
            {
                if (!content.Contains("200"))
                {
                    if (content.Contains("Test"))
                    {
                        mode = "TEST";
                        clientId = Constant.TestClientId;
                        secret = Constant.TestSecret;
                    }
                    else if (content.Contains("Live"))
                    {
                        mode = "LIVE";
                        clientId = Constant.LiveClientId;
                        secret = Constant.LiveSecret;
                    }
                    Debug.WriteLine("MODE:             " + mode);
                    Debug.WriteLine("PAYPAL CLIENT ID: " + clientId);
                    Debug.WriteLine("PAYPAL SECRENT:   " + secret);
                }
                else
                {
                    Debug.WriteLine("ERROR");
                    await DisplayAlert("Oops", "We can't not process your request at this moment.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Oops", "We can't not process your request at this moment.", "OK");
            }
        }

        // FUNCTION  6: CREATE ORDER REQUEST
        public async static Task<HttpResponse> createOrder(string amount)
        {
            HttpResponse response;
            // Construct a request object and set desired parameters
            // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
            var order = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = "USD",
                            Value = amount
                        }
                    }
                },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = "https://servingfresh.me",
                    CancelUrl = "https://servingfresh.me"
                }
            };


            // Call API with your client and get a response for your call
            var request = new OrdersCreateRequest();
                request.Prefer("return=representation");
                request.RequestBody(order);
                response = await client().Execute(request);
            return response;
        }

        // FUNCTION  7: CAPTURE ORDER
        public async Task<HttpResponse> captureOrder(string id)
        {
            // Construct a request object and set desired parameters
            // Replace ORDER-ID with the approved order id from create order
            var request = new OrdersCaptureRequest(id);
                request.RequestBody(new OrderActionRequest());

            var response = await client().Execute(request);
            var statusCode = response.StatusCode;
            var code = statusCode.ToString();
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

            Debug.WriteLine("REQUEST STATUS CODE: " + code);
            Debug.WriteLine("PAYPAL STATUS      : " + result.Status);
            Debug.WriteLine("ORDER ID           : " + result.Id);
            Debug.WriteLine("ID                 : " + id);

            if (result.Status == "COMPLETED")
            {
                Debug.WriteLine("PAYPAL PAYMENT WAS SUCCESSFUL");
                Preferences.Set("price", "00.00");
                await DisplayAlert("Payment Completed","Your payment was successful. Press 'CONTINUE' to select your meals!","OK");
                checkoutButton.Text = "CONTINUE";
            }
            else
            {
                await DisplayAlert("Ooops", "You payment was cancel or not sucessful. Please try again", "OK");
            }

            return response;
        }
    }
}
