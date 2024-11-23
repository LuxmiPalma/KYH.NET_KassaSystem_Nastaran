using System;
using System.IO;
using KYH.NET_KassaSystem_Nastaran.Interface;

namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class ErrorManager : IErrorManager
    {
        private const string LogFilePath = "ErrorLog.txt"; 


        
        public void LogError(Exception ex)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                {
                    writer.WriteLine("----- Error -----");
                    writer.WriteLine($"Date and time: {DateTime.Now}");
                    writer.WriteLine($"Type of error: {ex.GetType()}");
                    writer.WriteLine($"Error message: {ex.Message}");
                    writer.WriteLine($"StackTrace: {ex.StackTrace}");
                    writer.WriteLine("----------------\n");
                }
            }
            catch (Exception logEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Could not fix the error: {logEx.Message}");
                Console.ResetColor();
            }
        }



        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occurred: {message}");
            Console.ResetColor();
        }
    }
}
