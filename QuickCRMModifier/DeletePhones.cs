using Microsoft.Xrm.Tooling.Connector;
using System;

namespace CRMLib
{
    public class DeletePhones
    {
        public static CrmServiceClient Service = new CrmServiceClient(CRM.Prod);
        public static void Main(string[] args, string PurposeUserID)
        {
            AssignEntity(args, new Guid(PurposeUserID), "phonecall");
        }


        public static void AssignEntity(string[] args, Guid newOwnerGuid, string entityType)
        {
            try
            {
                if (!Service.IsReady)
                {
                    throw new Exception(Service.LastCrmError);
                }
                else
                {
                    foreach (string GUID in args)
                    {
                        Service.AssignEntityToUser(newOwnerGuid, entityType, new Guid(GUID));

                        //Console.WriteLine("Assigned entity {0} with GUID {1} to user {2}", entityType, GUID, newOwnerGuid);
                    }


                }

            }

            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось установить соединение с CRM.\r\n\r\n{ex.Message}");
            }

        }
    }
}
