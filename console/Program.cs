// See https://aka.ms/new-console-template for more information

using System.Text;

Console.WriteLine("Hello :--) Please enter the text you want to see encrypted");
string? enteredText = Console.ReadLine();


Console.WriteLine("toHexString");

byte[] binary = Encoding.UTF8.GetBytes(enteredText);

SoutEcrypted(binary);

string text = Encoding.UTF8.GetString(binary);

SoutDecrypted(text);




//--------------------

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