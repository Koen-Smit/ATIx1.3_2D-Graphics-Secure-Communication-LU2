using System;

namespace Assets.Scripts.Api.Models
{
    [Serializable]
    public class Account
    {
        public string userName;
        public string password;
    }

    [Serializable]
    public class LoginResponse
    {
        public string token;
    }

}
