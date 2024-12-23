using System;
using System.IO;

namespace CRMLib
{
    public static class GlobalVars
    {
        public static string LogsPath
        {
            get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString() + "\\logging\\logs.txt"; }
        }
    }

    public static class CRM
    {
        // URL для продакшн-среды
        public static string Prod
        {
            get
            {
                return "AuthType=AD;RequireNewInstance=true;Url=http://crm.gpbl.ru/GPBL;";
            }
        }

        // URL для предварительной среды
        public static string PreProd
        {
            get
            {
                return "AuthType=AD;RequireNewInstance=true;Url=http://preprod.crm.gpbl.ru/GPBL;";
            }
        }

        // URL для тестовой среды
        public static string Test
        {
            get
            {
                return "AuthType=AD;RequireNewInstance=true;Url=http://test.crm.gpbl.ru/GPBL;";
            }
        }

        // URL для dev среды
        public static string Dev
        {
            get
            {
                return "AuthType=AD;RequireNewInstance=true;Url=http://dev.crm.gpbl.ru/GPBL;";
            }
        }
    }
}
