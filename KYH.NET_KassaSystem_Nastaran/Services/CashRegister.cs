using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KYH.NET_KassaSystem_Nastaran.Interface;
using KYH.NET_KassaSystem_Nastaran.Models;
using KYH.NET_KassaSystem_Nastaran.Services;



namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class CashRegister
    {
        private Admin _admin;
        private Receipt currentReceipt;
        private IErrorManager errorManager = new ErrorManager();

        public CashRegister()
        {

            var adminTool = new AdminTool();
            _admin = new Admin(adminTool);

        }


        public void Start()
        {
            while (true)
            {

                Console.WriteLine();
                Console.WriteLine("1. New customer\n2. Admin\n0. Exit");
                var input = Console.ReadLine();
                if (input == "1")
                {
                    StartNewTransactionTest();
                }
                else if (input == "2")
                {
                    _admin.ShowAdminMenu();
                }
                else if (input == "0")
                {
                    break;
                }
            }

        }

        // Starta en ny transaktion för en kund
        public void StartNewTransactionTest()
        {

            currentReceipt = new Receipt(errorManager);
            Console.WriteLine("CASH");
            Console.WriteLine($"RECEIPT:\t\t{DateTime.Now:yyyy-MM-dd\t HH:mm:ss}");

            while (true)
            {

                Console.WriteLine("Commands: <product ID> <number> or PAY");
                var command = Console.ReadLine()?.Split(' ');

                if (command.Length > 0 && string.Equals(command[0], "PAY", StringComparison.OrdinalIgnoreCase))
                {

                    currentReceipt.PrintAndSaveReceipt();
                    break;

                }
                else if (command.Length > 1 && int.TryParse(command[0], out int productId) && int.TryParse(command[1], out int quantity))
                {

                    try
                    {

                        var product = _admin.Products.FirstOrDefault(p => p.Id == productId);
                        if (product != null)
                        {
                            decimal effectivePrice = product.GetEffectivePrice(DateTime.Now);
                            currentReceipt.AddItem(product, quantity);

                        }
                        else
                        {
                            Console.WriteLine("The product with the specified id was not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        errorManager.LogError(ex); // Logga fel
                        errorManager.DisplayError("An error occurred while adding product.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid command. Enter product-ID and quantity, or PAY to end the transaction.");
                }
            }
        }
    }
}
