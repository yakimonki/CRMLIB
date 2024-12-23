using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;


namespace CRMLib
{
    public class CreateCampaignsPlusPhonecalls
    {
        
        public static CrmServiceClient Service = new CrmServiceClient(CRM.Prod);

        public static  Dictionary<string, string> entyty_ref = new Dictionary<string, string>() {
                    { "gpbl_clientbase_id", "leasing_clientbase" }, // база клиентов "Аренда и лизинг":"A271D1B0-64A2-EB11-81AF-005056019243"...,
                    { "gpbl_leadsourceid_for_call", "gpbl_leadsource" }, // Источник у нас там всегда холодный поиск --Источник привлечения C49EE696-7930-EB11-A997-005056011895
                    { "regardingobjectid", "bulkoperation" }, // быстрая компания 
                    { "ownerid", "" }, // отвественный тут Антоха // systemuser dailer Anton - 86145F6F-B198-E911-A84D-005056010A7B / contact center - D3BD185E-B2C3-E711-90F2-0050568769C6
                };
        public static List<string> entyty_array = new List<string> { "getcrm_inn", "gpbl_lastname", "gpbl_firstname", "gpbl_middlename", "subject", "phonenumber", "gpbl_description", "gpbl_phonenumbers" };


        public static void Main(Dictionary<string, string> campaigns_parametrs, List<Dictionary<string, string>> phonecalls, Dictionary<string, string> staicfields)
        {
            Guid campaign_id = CreateCampaign(campaigns_parametrs);
            if (campaign_id.Equals(Guid.Empty))
            {
                Console.WriteLine($"Guid is Empty, capmpaign could not be init");
            }
            else
            {
                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < phonecalls.Count; i++)
                    {

                        CreatePhonecall(phonecalls[i], staicfields, campaign_id, campaigns_parametrs["topic_name"]);
                        progress.Report((double)i / phonecalls.Count);
                    }

                }
            }
        }
        static void CreatePhonecall(Dictionary<string, string> subdict, Dictionary<string, string> staicfields, Guid campaign_id, string topic)
        {
            using (Service)
            {
                try
                {
                    Entity phonecall = new Entity("phonecall");
                    phonecall["regardingobjectid"] = new EntityReference("bulkoperation", campaign_id);
                    phonecall["subject"] = topic;
                    foreach (KeyValuePair<string, string> pair in subdict)
                    {
                        if (entyty_array.Contains(pair.Key) && !pair.Value.Equals(string.Empty))
                        {
                            phonecall[pair.Key] = pair.Value;
                        }

                    }
                    foreach (KeyValuePair<string, string> pair in staicfields)
                    {
                        if (entyty_ref.ContainsKey(pair.Key) && !pair.Value.Equals(string.Empty))
                        {
                            if (pair.Key != "ownerid")
                            {
                                phonecall[pair.Key] = new EntityReference(entyty_ref[pair.Key], new Guid("{" + pair.Value + "}"));
                            }
                            else
                            {
                                if (pair.Value != "D4BD185E-B2C3-E711-90F2-0050568769C6")
                                {
                                    phonecall[pair.Key] = new EntityReference("systemuser", new Guid("{" + pair.Value + "}"));
                                }
                                else
                                {
                                    phonecall[pair.Key] = new EntityReference("team", new Guid("{" + pair.Value + "}"));
                                }
                            }
                        }
                    }

                    Service.Create(phonecall);
                }
                catch {

                    //Console.WriteLine("Faild on:" + subdict["getcrm_inn"]);
                    Logger.Main("Faild on: " + subdict["getcrm_inn"], GlobalVars.LogsPath);
                }
            }
        }


        public static Guid CreateCampaign(Dictionary<string, string> parameters)
        {
            try
            {
                // Проверка готовности сервиса
                if (!Service.IsReady)
                {
                    throw new Exception(Service.LastCrmError);
                }
                else
                {
                    CreateCampaignsPlusPhonecalls cls = new CreateCampaignsPlusPhonecalls();
                    Console.WriteLine("Connected to {0}", CRM.Prod);

                    Guid listGuid = Guid.Empty;

                    // Проверка и создание маркетингового списка, если list_name не пустой
                    if (!string.IsNullOrEmpty(parameters["list_name"]))
                    {
                        Entity marketingList = new Entity("list");
                        marketingList["listname"] = parameters["list_name"];
                        marketingList["type"] = false;
                        marketingList["createdfromcode"] = new OptionSetValue(2);

                        // Создание списка и получение его GUID
                        listGuid = Service.Create(marketingList);
                        Console.WriteLine("Created {0} with GUID {1}", marketingList.LogicalName, listGuid);
                    }

                    // Создание звонка
                    Entity phoneCall = new Entity("phonecall");
                    phoneCall["subject"] = parameters["topic_name"];

                    // Отправка запроса на создание звонка
                    Service.Create(phoneCall);

                    // Получение ID текущего пользователя
                    WhoAmIResponse currentUser = (WhoAmIResponse)Service.Execute(new WhoAmIRequest());

                    // Формирование быстрой кампании
                    CreateActivitiesListRequest quickCampaignRequest = new CreateActivitiesListRequest()
                    {
                        Activity = phoneCall,
                        ListId = listGuid,
                        OwnershipOptions = PropagationOwnershipOptions.ListMemberOwner,
                        Propagate = true,
                        TemplateId = Guid.Empty,
                        FriendlyName = parameters["campaign_name"],
                        Owner = new EntityReference("systemuser", currentUser.UserId),
                        PostWorkflowEvent = true
                    };

                    // Выполнение запроса на создание быстрой кампании
                    CreateActivitiesListResponse quickCampaignResponse = (CreateActivitiesListResponse)Service.Execute(quickCampaignRequest);
                    Console.WriteLine("Created Quick Campaign with GUID {0}", quickCampaignResponse.BulkOperationId);

                    return quickCampaignResponse.BulkOperationId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось установить соединение с CRM.\r\n\r\n{ex.Message}");
                return Guid.Empty;
            }
        }
    }
}