// file:	Connection\IDIConnection.cs
// summary:	Declares the IDIConnection interface

using SAPbobsCOM;
using Core.SDK.Models;

namespace Core.SDK.Connection
{
    /// <summary> Interface for DI connection. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public interface IDIConnection : IConnection
    {
        /// <summary> Gets the token. </summary>
        /// <value> The token. </value>

        string Token
        {
            get;
        }

        /// <summary> Connects to DI. </summary>
        /// <param name="credentials"> The credentials. </param>

        void ConnectToDI(Credentials credentials);

        /// <summary> Gets the company. </summary>
        /// <returns> The company. </returns>

        Company GetCompany();

        /// <summary> Reconnect DI. </summary>

        void ReconnectDI();

        /// <summary> Sets a company. </summary>
        /// <param name="company"> The company. </param>

        void SetCompany(Company company);
    }
}
