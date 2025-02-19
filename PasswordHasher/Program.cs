using PasswordHasher;
string? plainText = "268%*WCag!JhMG";
if (args.Length != 1)
{
    Console.Write("password: ");
    while (true)
    {
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Enter)
            break;
        plainText += key.KeyChar;
    }
}
else
{
    plainText = args[0];
}
var hashed = PasswordHelper.HashPassword(plainText);
Console.WriteLine(hashed);


