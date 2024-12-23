using Microsoft.Xrm.Sdk;

namespace Microsoft.Crm.Sdk.Messages
{
    internal class ExecuteMultipleRequest
    {
        public ExecuteMultipleRequest()
        {
        }

        public ExecuteMultipleSettings Settings { get; set; }
        public OrganizationRequestCollection Requests { get; set; }
    }
}