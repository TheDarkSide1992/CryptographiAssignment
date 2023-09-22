// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

bool isRunnig = true;

while (isRunnig)
{
    Console.WriteLine("\n=============================================\n");
    Console.WriteLine("1. Encrypt a message");
    Console.WriteLine("2. read a message");
    Console.WriteLine("3. exit");

    string? mode = Console.ReadLine();
    switch (mode)
    {
        case "1":
            Console.WriteLine("\n=============================================\n");
            Encryption();
            break;
        case "2":
            Console.WriteLine("\n=============================================\n");
            readFromFile();
            break;
        case "3":
            Console.WriteLine("\n=============================================\n");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("goodbye have a nice day. \n");
            Console.ResetColor();
            isRunnig = false;
            break;
        default:
            Console.WriteLine("\n=============================================\n");
            Console.WriteLine("you didnt enter a number valid please try again");
            break;
    }
}


void Encryption()
{
    Console.WriteLine("Hello :--) Please enter the text you want to see encrypted");
    string? enteredText = Console.ReadLine();


    // generate a slat
    var salt = new byte[32];
    RandomNumberGenerator.Fill(salt);

    //make key
    var key = KeyDerivation.Pbkdf2(
        password: getPassword()!,
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

    Console.WriteLine("\n=============================================\n");

    SoutEcrypted(ciphertext); //this should be removed

    Console.WriteLine("\n=============================================\n");

    try
    {
        saveToFile(ciphertext, nonce, tag, salt);
    }
    catch (Exception e)
    {
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("an error acoured when writing to file");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=============================================");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=============================================");
            Console.ResetColor();
        }
    }
}


string Decrypt(byte[] ciphertext, byte[] nonce, byte[] tag, byte[] salt)
{
    var key = KeyDerivation.Pbkdf2(
        password: getPassword()!,
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

void saveToFile(byte[] ciphertext, byte[] nonce, byte[] tag, byte[] salt)
{
    try
    {
        string hexStringCipher = Convert.ToHexString(ciphertext);
        string hexStringNonce = Convert.ToHexString(nonce);
        string hexStringTag = Convert.ToHexString(tag);
        string hexStringSalt = Convert.ToHexString(salt);

        string content = hexStringCipher + "," + hexStringNonce + "," +
                         hexStringTag + "," + hexStringSalt;

        /*
        //on some systems the program may be unable to write to the file so the encrypted content can be printed.
        //if you cant write to file out comment this code and paste it into a Message.txt file in this dir
        //then run decrypt mehod
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(content);
        Console.ResetColor();
        */


        string filePath = @".\Message.txt";

        if (!File.Exists(filePath))
            File.Create(filePath);

        File.SetAttributes(filePath,
            (new FileInfo(filePath)).Attributes | FileAttributes.Normal);


        File.WriteAllText(filePath, content);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("=============================================");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Wrote successfully to file.");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("=============================================");
        Console.ResetColor();

    }
    catch (IOException e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("could not write content to file");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("=============================================");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("=============================================");
        Console.ResetColor();
    }
}

/**
 * Reads file and if theres data throughs data to the decrypt method.
 */
void readFromFile()
{
    try
    {
        string output = "";
        // Open the text file using a stream reader.
        using (var sr = new StreamReader(@"Message.txt"))
        {
            // Read the stream as a string, and write the string to the console.
            output = sr.ReadToEnd();
        }

        output.Replace("\r\n", "");


        string[] list = output.Split(",");

        byte[] ciphertext = new byte[] { };
        byte[] nonce = new byte[] { };
        byte[] tag = new byte[] { };
        byte[] salt = new byte[] { };

        for (int i = 0; i <= list.Length; i++)
        {
            switch (i)
            {
                case 0:
                    list[i].Replace(",", "");
                    ciphertext = Convert.FromHexString(list[i]);
                    break;
                case 1:
                    list[i].Replace(",", "");
                    nonce = Convert.FromHexString(list[i]);
                    break;
                case 2:
                    list[i].Replace(",", "");
                    tag = Convert.FromHexString(list[i]);
                    break;
                case 3:
                    list[i].Replace(",", "");
                    salt = Convert.FromHexString(list[i]);
                    break;
                default:
                    break;
            }
        }

        if (ciphertext.Length < 5)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No data to read");
            Console.ResetColor();
            return;
        }


        try
        {
            SoutDecrypted(Decrypt(ciphertext, nonce, tag, salt));
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=============================================");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("could not decrypt file");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=============================================");
            Console.ResetColor();
        }
        
    }
    catch (IOException e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("The file could not be read: \n" + e);
        Console.ResetColor();
    }
}

/**
 * gets the users password without showing it in the CLI.
 */
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
        }
        else
        {
            thePassword += character;
        }
    }

    return thePassword;
}

/**
 * prints byte array to Console
 */
void SoutEcrypted(byte[] encrypted)
{
    Console.Write("Text Encrypted: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    foreach (var item in encrypted)
    {
        Console.Write(item);
    }

    Console.WriteLine();
    Console.ResetColor();
}

/**
 * prints string to console
 */
void SoutDecrypted(string decrypted)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("=============================================");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Text Decrypted: ");
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine(decrypted);
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("=============================================");
    Console.ResetColor();
}