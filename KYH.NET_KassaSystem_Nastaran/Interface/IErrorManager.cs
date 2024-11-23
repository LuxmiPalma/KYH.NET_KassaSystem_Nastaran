using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYH.NET_KassaSystem_Nastaran.Interface
{
    public interface IErrorManager
    {
        void LogError(Exception ex);      // Loggar ett fel
        void DisplayError(string message); // Visar ett användarvänligt felmeddelande
        
    }

}


