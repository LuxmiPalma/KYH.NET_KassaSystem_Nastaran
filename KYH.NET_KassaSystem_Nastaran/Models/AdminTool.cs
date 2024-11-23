using System;
using System.Collections.Generic;
using System.Linq;
using KYH.NET_KassaSystem_Nastaran.Enum;
using KYH.NET_KassaSystem_Nastaran.Services;



namespace KYH.NET_KassaSystem_Nastaran.Models
{
    public class AdminTool

    {

        private static readonly string _filePath = "../../../Files/Products.txt";
        public List<Product> Products { get; private set; } = new List<Product>();

        public AdminTool()
        {
            Products = LoadProductsFromFile();

            try
            {

                var banana = new Product(300, "Banana", 8m, "per styck");
                var apple = new Product(301, "Apple", 10m, "per styck");
                var coffee = new Product(302, "Coffee", 35m, "per styck");
                var milk = new Product(303, "Milk", 10m, "per styck");


                var appleCampaign = new Campaign(
                    CampaignType.PercentageDiscount, // Typ av kampanj
                    20m,                              // Rabatt i procent
                    new DateTime(2024, 11, 1),       // Startdatum
                    new DateTime(2024, 12, 25)       // Slutdatum
                );


                apple.AddCampaign(appleCampaign);



                AddProduct(banana);
                AddProduct(apple);
                AddProduct(coffee);
                AddProduct(milk);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Product initialization error: {ex.Message}");
            }
        }
        // Läs produkter från fil
        private List<Product> LoadProductsFromFile()
        {
            var products = new List<Product>();

            if (!File.Exists(_filePath))
            {
                File.Create(_filePath).Close();
                return products;
            }

            var lines = File.ReadAllLines(_filePath).Distinct();
            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length >= 4 &&
                    int.TryParse(parts[0], out int id) &&
                    decimal.TryParse(parts[2], out decimal price))
                {
                    var product = new Product(id, parts[1], price, parts[3]);
                    products.Add(product);
                }
            }

            return products;
        }

        // Spara produkter till fil
        public void SaveProductsToFile()
        {

            var lines = Products.Select(p =>
                $"{p.Id},{p.Name},{p.Price},{p.PriceType}");

            File.WriteAllLines(_filePath, lines);

        }

        public void AddProduct(Product product)
        {

            if (product == null)
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");

            Products.Add(product);
            SaveProductsToFile();
        }


    }
}