// file:	Models\Credentials.cs
// summary:	Implements the credentials class

using SAPbobsCOM;

namespace Core.SDK.Models
{
    /// <summary> A credentials. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public class Credentials
    {
        public BoDataServerTypes DbServerType { get; set; }
        public string LicenseServer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DataBaseName { get; set; }
        public string SQLServer { get; set; }
        public string SQLUserName { get; set; }
        public string SQLPassword { get; set; }
        public BoSuppLangs Language { get; set; }
    }
}
