using System;
using ThaiNationalIDCard.NET.Models;

namespace ThaiNationalIDCard.NET.Example.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ThaiNationalIDCardReader cardReader = new ThaiNationalIDCardReader();
                PersonalPhoto personalPhoto = cardReader.GetPersonalPhoto();

                Console.WriteLine($"CitizenID: {personalPhoto.CitizenID}");
                Console.WriteLine($"ThaiPersonalInfo: {personalPhoto.ThaiPersonalInfo}");
                Console.WriteLine($"EnglishPersonalInfo: {personalPhoto.EnglishPersonalInfo}");
                Console.WriteLine($"Sex: {personalPhoto.Sex}");
                Console.WriteLine($"AddressInfo: {personalPhoto.AddressInfo}");
                Console.WriteLine($"IssueDate: {personalPhoto.IssueDate}");
                Console.WriteLine($"ExpireDate: {personalPhoto.ExpireDate}");
                Console.WriteLine($"Issuer: {personalPhoto.Issuer}");
                Console.WriteLine($"Photo: {personalPhoto.Photo}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Please any key to exit...");
            Console.ReadKey(true);
        }
    }
}
