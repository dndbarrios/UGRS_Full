// file:	Connection\DIConnection.cs
// summary:	Implements the DI connection class

using SAPbobsCOM;
using System;
using Core.SDK.Exceptions;
using Core.SDK.Models;

namespace Core.SDK.Connection
{
    /// <summary> A DI connection. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public class DIConnection : Connection, IDIConnection, IConnection
    {
        #region Attributes

        /// <summary> The object credentials. </summary>
        private Credentials mObjCredentials;

        /// <summary> The object company. </summary>
        private Company mObjCompany;

        /// <summary> The token. </summary>
        private string mToken;

        #endregion

        #region Properties

        /// <summary> Gets or sets the credentials. </summary>
        /// <value> The credentials. </value>

        private Credentials Credentials
        {
            get { return mObjCredentials; }
            set { mObjCredentials = value; }
        }

        /// <summary> Gets or sets the company. </summary>
        /// <value> The company. </value>

        public Company Company
        {
            get { return mObjCompany; }
            private set { mObjCompany = value; }
        }

        /// <summary> Gets or sets the token. </summary>
        /// <value> The token. </value>

        public string Token
        {
            get { return mToken; }
            private set { mToken = value; }
        }

        #endregion

        #region Construct

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        public DIConnection()
        {
            Token = string.Empty;
        }

        #endregion

        #region Methods

        /// <summary> Gets the company. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <returns> The company. </returns>

        public Company GetCompany()
        {
            return Company;
        }

        /// <summary> Connects to DI. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <exception cref="DIConnectionException">
        /// Thrown when a DI Connection error condition occurs.
        /// </exception>
        /// <param name="pObjCredentials"> The object credentials. </param>

        public void ConnectToDI(Credentials pObjCredentials)
        {
            string lStrErrorMessage = string.Empty;
            int lIntErrorCode = 0;

            Token = Guid.NewGuid().ToString();

            //Company = (Company)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("632F4591-AA62-4219-8FB6-22BCF5F60090")));
            Company = new Company();
            Company = SetCompanyData(Company, pObjCredentials);

            Disconnect(Company);

            lIntErrorCode = Company.Connect();
            if (lIntErrorCode != 0)
            {
                Company.GetLastError(out lIntErrorCode, out lStrErrorMessage);
                throw new DIConnectionException(string.Concat("Exception: ", lStrErrorMessage));
            }
        }

        /// <summary> Reconnect DI. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <exception cref="DIConnectionException">
        /// Thrown when a DI Connection error condition occurs.
        /// </exception>

        public void ReconnectDI()
        {
            string lStrErrorMessage = string.Empty;
            int lIntErrorCode = 0;

            Company.Disconnect();
            Company = new Company();
            Company = SetCompanyData(Company, Credentials);

            Disconnect(Company);

            lIntErrorCode = Company.Connect();
            if (lIntErrorCode != 0)
            {
                Company.GetLastError(out lIntErrorCode, out lStrErrorMessage);
                throw new DIConnectionException(string.Concat("Exception: ", lStrErrorMessage));
            }
        }

        /// <summary> Sets a company. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjCompany"> The object company. </param>

        public void SetCompany(Company pObjCompany)
        {
            Company = pObjCompany;
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjCredentials"> The object credentials. </param>

        private DIConnection(Credentials pObjCredentials)
        {
            this.ConnectToDI(pObjCredentials);
        }

        /// <summary> Disconnects the given company. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="company"> The company to disconnect. </param>

        private void Disconnect(Company company)
        {
            if (company.Connected)
            {
                company.Disconnect();
            }
        }

        /// <summary> Sets company data. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjCompany">     The object company. </param>
        /// <param name="pObjCredentials"> The object credentials. </param>
        /// <returns> A Company. </returns>

        private Company SetCompanyData(Company pObjCompany, Credentials pObjCredentials)
        {
            pObjCompany.Server = pObjCredentials.SQLServer;
            pObjCompany.CompanyDB = pObjCredentials.DataBaseName;
            pObjCompany.DbUserName = pObjCredentials.SQLUserName;
            pObjCompany.DbPassword = pObjCredentials.SQLPassword;
            pObjCompany.UserName = pObjCredentials.UserName;
            pObjCompany.Password = pObjCredentials.Password;
            //pObjCompany.language = pObjCredentials.Language;
            pObjCompany.DbServerType = pObjCredentials.DbServerType;
            //pObjCompany.LicenseServer = pObjCredentials.LicenseServer;

            return pObjCompany;
        }

        /// <summary> Gets an instance. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <exception cref="DIConnectionException">
        /// Thrown when a DI Connection error condition occurs.
        /// </exception>
        /// <param name="pObjCredentials"> The object credentials. </param>
        /// <returns> The instance. </returns>

        internal static IDIConnection GetInstance(Credentials pObjCredentials)
        {
            IDIConnection lObjDIConnection;
            try
            {
                lObjDIConnection = new DIConnection(pObjCredentials);
            }
            catch (Exception e)
            {
                throw new DIConnectionException(e.Message, e);
            }

            return lObjDIConnection;
        }

        #endregion
    }
}
