using System;
namespace MTYD.Model.Login.LoginClasses
{
    public class SocialLogInPost
    {
        public string email { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public string signup_platform { get; set; }
    }
}
