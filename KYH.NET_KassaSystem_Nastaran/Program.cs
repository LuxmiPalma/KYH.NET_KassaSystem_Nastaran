using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using KYH.NET_KassaSystem_Nastaran.Enum;
using KYH.NET_KassaSystem_Nastaran.Interface;
using KYH.NET_KassaSystem_Nastaran.Models;
using KYH.NET_KassaSystem_Nastaran.Services;

namespace KYH.NET_KassaSystem_Nastaran
{
    class Program
    {
        static void Main(string[] args)
        {

            Admin admin = new Admin(new AdminTool());
            var errorManager = new ErrorManager();
            var receipt = new Receipt(errorManager);
          

            while (true)
            {
                try
                {

                    Console.Clear();
                    Console.WriteLine("\n---** Main Menu **---\n");
                    Console.WriteLine("1. New Customer");
                    Console.WriteLine("2. Admin");
                    Console.WriteLine("0. Exit");
                    Console.Write("\nSelect an Option: ");

                    // Läsa in användarens val
                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                HandleCustomer(receipt);
                                break;
                            case 2:
                                admin.ShowAdminMenu();
                                break;
                            case 0:
                                Console.WriteLine("\nThe program is closing. Thank you for using our application!");
                                Environment.Exit(0);
                                break;
                            default:
                                Console.WriteLine("\nInvalid selection, Try again.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nIncorrect input. Enter a valid number.");
                    }
                }
                catch (Exception ex)
                {
                    // Hantera oväntade fel
                    errorManager.LogError(ex);
                    errorManager.DisplayError("An unexpected error occurred. Please try again.");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                
            }
        }

        static void HandleCustomer(Receipt receipt)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("---  Main Menu ---\n");
                Console.WriteLine("Here you can add products, manage receipts, etc.");


                var cashRegister = new CashRegister();
                cashRegister.StartNewTransactionTest();
                cashRegister.Start();
                

                receipt.PrintAndSaveReceipt();
               
            }
            catch (Exception ex)
            {

                Console.WriteLine("An error occurred while handling the client function: " + ex.Message);
            }
        }
    }
}
