
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KYH.NET_KassaSystem_Nastaran.Models;
using KYH.NET_KassaSystem_Nastaran.Interface;
using System.Text.RegularExpressions;

namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class Receipt
    {
        private readonly IErrorManager _errorManager;
        private const decimal VatRate = 0.25m; // VAT (25%)

        private static int receiptCounter = LoadReceiptCounter();
        public int ReceiptNumber { get; private set; }
        public List<(Product product, int quantity)> Items { get; private set; } = new List<(Product, int)>();
        public DateTime Date { get; private set; } = DateTime.Now;


        private static string receiptCounterFilePath = "ReceiptCounter.txt";
        private static string receiptFilePath = "../../../Files/Receipts";

        public Receipt(IErrorManager errorManager)
        {
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            ReceiptNumber = receiptCounter;

        }


        public void AddItem(Product product, int quantity)
        {

            if (!ValidateProduct(product))
                return;

            if (!ValidateQuantity(quantity))
                return;

            ValidateCampaigns(product);

            decimal effectivePrice = product.GetEffectivePrice(DateTime.Now);
            Items.Add((product, quantity));
            Console.WriteLine($"Produkt: {product.Name}\nAntal: {quantity}\nPris: {effectivePrice:C}");

        }

        private bool ValidateProduct(Product product)
        {
            if (product == null)
            {
                _errorManager.DisplayError("The product is invalid.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(product.Name) || !Regex.IsMatch(product.Name, @"^[a-zA-Z\s]+$"))
            {
                _errorManager.DisplayError("The product name is invalid. Only alphabetic characters are allowed.");
                return false;
            }

            if (!Regex.IsMatch(product.Price.ToString(), @"^\d+(\.\d{1,2})?$") || product.Price <= 0)
            {
                _errorManager.DisplayError($"The product '{product.Name}' has an invalid price: {product.Price:C}. Only positive numbers are allowed.");
                return false;
            }

            return true;
        }
        private bool ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                _errorManager.DisplayError("Quantity must be greater than 0.");
                return false;
            }

            return true;
        }

        private void ValidateCampaigns(Product product)
        {
            foreach (var campaign in product.Campaigns)
            {
                if (!ValidateCampaignDates(campaign.StartDate, campaign.EndDate))
                {
                    _errorManager.DisplayError($"The campaign '{campaign.Type}' for '{product.Name}' has invalid dates: Start date {campaign.StartDate:yyyy-MM-dd}, End date {campaign.EndDate:yyyy-MM-dd}.");
                    continue;
                }

                Console.WriteLine($"Campaign: {campaign.Type}, Start: {campaign.StartDate:yyyy-MM-dd}, End: {campaign.EndDate:yyyy-MM-dd}");
            }
        }

        private bool ValidateCampaignDates(DateTime startDate, DateTime endDate)
        {
            var currentYear = DateTime.Now.Year;

            if (startDate.Year != currentYear || endDate.Year != currentYear)
            {
                _errorManager.DisplayError($"The campaign start and end dates must be in the current year ({currentYear}).");
                return false;
            }

            if (startDate > endDate)
            {
                _errorManager.DisplayError("The start date cannot be after the end date.");
                return false;
            }

            if (endDate < DateTime.Now)
            {
                _errorManager.DisplayError("The campaign has already expired.");
                return false;
            }

            return true;
        }
        private decimal CalculateTotalExcludingVat()
        {
            return Items.Sum(item => item.product.GetEffectivePrice(Date) * item.quantity);
        }

        private decimal CalculateVat()
        {
            return CalculateTotalExcludingVat() * VatRate;
        }

        public void PrintAndSaveReceipt()
        {
            ReceiptNumber = receiptCounter++;

            if (Date.Date != DateTime.Now.Date)
            {
                Date = DateTime.Now;
                Items.Clear(); 
            }
            decimal totalExclVat = CalculateTotalExcludingVat();
            decimal vat = CalculateVat();
            decimal totalInclVat = totalExclVat + vat;

            if (!Directory.Exists(receiptFilePath))
                Directory.CreateDirectory(receiptFilePath);

            string filePath = Path.Combine(receiptFilePath, $"Receipt_{Date:yyyy-MM-dd}.txt");

            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {

                writer.WriteLine("*===================================================*");
                writer.WriteLine("\t\t** MAX SUPERMARKET SOLNA **\n");
                writer.WriteLine(" Opening hrs:\t\t\t\tCentralvägen 16\n Mon-Fri   07:00-22:00\t\t\t171 42, SOLNA\n Sat-Sun   08:00-22:00");
                writer.WriteLine("-----------------------------------------------------");
                writer.WriteLine($"Cashier: 1214\t\t\t\tRECEIPT: #{ReceiptNumber}");
                writer.WriteLine($"Date: {Date:yyyy-MM-dd}\t\t\tTime: {Date:HH:mm:ss}");
                writer.WriteLine("-----------------------------------------------------");
                foreach (var item in Items)

                {
                    decimal itemTotal = item.product.GetEffectivePrice(Date) * item.quantity;
                    writer.WriteLine($"{item.product.Name}\t{item.quantity} x {item.product.Price:C} = {itemTotal:C}");
                }
                writer.WriteLine("-----------------------------------------------------");
                writer.WriteLine($"Total (excl. VAT): {totalExclVat:C}");
                writer.WriteLine($"VAT (25%):         {vat:C}");
                writer.WriteLine($"Total (incl. VAT): {totalInclVat:C}");
                writer.WriteLine("-----------------------------------------------------");
                writer.WriteLine("\t\t** Thank you, Welcome back! **\n\n\n\n\n\n\n\n");
            }

            Console.WriteLine("\n--- Receipt ---");
            foreach (var item in Items)
            {

                decimal itemTotal = item.product.GetEffectivePrice(Date) * item.quantity;
                Console.WriteLine($"{item.product.Name}\t{item.quantity} x {item.product.Price:C} = {itemTotal:C}");
            }
            Console.WriteLine("------------------------");
            Console.WriteLine($"Total (excl. VAT): {totalExclVat:C}");
            Console.WriteLine($"VAT (25%):         {vat:C}");
            Console.WriteLine($"Total (incl. VAT): {totalInclVat:C}");
            Console.WriteLine("------------------------");

            SaveReceiptCounter();

        }


        private static int LoadReceiptCounter()

        {
            string path = "ReceiptCounter.txt";
            if (File.Exists(path))
                return int.Parse(File.ReadAllText(path));
            return 0;
        }

        private static void SaveReceiptCounter()

        {
            File.WriteAllText("ReceiptCounter.txt", receiptCounter.ToString());
        }

    }
}
