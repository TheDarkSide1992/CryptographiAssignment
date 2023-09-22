// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

bool isRunnig = false;

Encryption();

while (isRunnig)
{
    Console.WriteLine("\n \n-----------------------------------------\n");
    Console.WriteLine("1. Encrypt a message");
    Console.WriteLine("2. read a message");
    Console.WriteLine("3. exit");
    
    string? mode = Console.ReadLine();
    switch (mode)
    {
        case "1":
            hexEncoding();
            break;
        case "2":
            Console.WriteLine("reading \n");
            break;
        case "3":
            Console.WriteLine("goodbye have a nice day. \n");
            isRunnig = false;
            break;
        default:
            Console.WriteLine("you didnt enter a number please try again");
            break;
    }
}


//--------------------
void hexEncoding()
{
    Console.WriteLine("\n-----------------------------------------------------------\n");
    Console.WriteLine("Hello :--) Please enter the text you want to see encoded");
    string? enteredText = Console.ReadLine();


    Console.WriteLine("toHexString");

    byte[] binary = Encoding.UTF8.GetBytes(enteredText);

    SoutEcrypted(binary);

    string text = Encoding.UTF8.GetString(binary);

    SoutDecrypted(text);

}

void Encryption()
{
    Console.WriteLine("\n-----------------------------------------------------------\n");
    Console.WriteLine("Hello :--) Please enter the text you want to see encrypted");
    string? enteredText = Console.ReadLine();
    
    
    // generate a key
    var salt = new byte[32];
    RandomNumberGenerator.Fill(salt);

    String password = getPassword();
    
    var key = KeyDerivation.Pbkdf2(
        password: password!,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 600000,
        numBytesRequested: 256 / 8);
    
    using var aes = new AesGcm(key);
    
    var nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // MaxSize = 12
    RandomNumberGenerator.Fill(nonce);

    var plaintextBytes = Encoding.UTF8.GetBytes(enteredText);
    var ciphertext = new byte[plaintextBytes.Length];
    var tag = new byte[AesGcm.TagByteSizes.MaxSize]; // MaxSize = 16
    
    aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
    
    SoutEcrypted(ciphertext);  //this should be removed
    
    SoutDecrypted(Decrypt(ciphertext, nonce, tag, salt)); //this should be moved
}

void Decrtypt()
{
    String password = getPassword();
}

string Decrypt(byte[] ciphertext, byte[] nonce, byte[] tag, byte[] salt)
{
    String password = getPassword();
    
    var key = KeyDerivation.Pbkdf2(
        password: password!,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 600000,
        numBytesRequested: 256 / 8);
    
    using (var aes = new AesGcm(key))
    {
        var plaintextBytes = new byte[ciphertext.Length];

        aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}

string getPassword()
{
    Console.WriteLine("Enter Password:");
    bool isGettingPassword = true;
    bool isEnter = false;
    String thePassword = "";
    
    while (isGettingPassword)
    {
        ConsoleKeyInfo key = Console.ReadKey(intercept: true);
        isEnter = key.Key == ConsoleKey.Enter;
        // Get the character that was pressed
        char character = key.KeyChar;

        if (isEnter == true)
        {
            isGettingPassword = false;
            break;
        } else
        {
            thePassword += character;
        }
    }
    
    return thePassword;
}


void SoutEcrypted(byte[] encrypted)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Text Encrypted: ");
    foreach (var item in encrypted) {
        Console.Write(item);
    }
    Console.WriteLine();
    Console.ResetColor();
}

void SoutDecrypted(string decrypted)
{
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("Text Decrypted: " + decrypted);
    Console.ResetColor();

}