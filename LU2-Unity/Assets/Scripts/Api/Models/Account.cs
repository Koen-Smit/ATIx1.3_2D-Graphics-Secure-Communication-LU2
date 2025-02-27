using System;


[Serializable]
public class Account
{
    public string UserName;
    public string Password;
}

[Serializable]
public class LoginResponse
{
    public string token;
}

