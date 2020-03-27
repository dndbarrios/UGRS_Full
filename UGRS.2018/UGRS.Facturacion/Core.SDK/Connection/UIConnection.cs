// file:	Connection\UIConnection.cs
// summary:	Implements the UI connection class

using SAPbouiCOM;
using System;

namespace Core.SDK.Connection
{
    /// <summary> An UI connection. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public class UIConnection : Connection, IUIConnection, IConnection
    {
        #region Attributes

        /// <summary> The object sbo application. </summary>
        private SboGuiApi mObjSBOApplication;

        /// <summary> The object application. </summary>
        private Application mObjApplication;

        /// <summary> Identifier for the int application. </summary>
        private static int mIntApplicationId;

        #endregion

        #region Properties

        /// <summary> Gets or sets the sbo application. </summary>
        /// <value> The sbo application. </value>

        private SboGuiApi SBOApplication
        {
            get { return mObjSBOApplication; }
            set { mObjSBOApplication = value; }
        }

        /// <summary> Gets or sets the application. </summary>
        /// <value> The application. </value>

        public Application Application
        {
            get { return mObjApplication; }
            private set { mObjApplication = value; }
        }

        /// <summary> Gets or sets the identifier of the application. </summary>
        /// <value> The identifier of the application. </value>

        private static int ApplicationId
        {
            get { return mIntApplicationId; }
            set { mIntApplicationId = value; }
        }

        #endregion

        #region Contruct

        /// <summary> Static constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        static UIConnection()
        {
            ApplicationId = -1;
        }

        #endregion

        #region Methods

        /// <summary> Gets the application. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <returns> The application. </returns>

        public Application GetApplication()
        {
            return Application;
        }

        /// <summary> Connects UI. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        protected void ConnectToUI()
        {
            //Create new SBO Application instance
            SBOApplication = new SboGuiApi();

            //Get connection string from command line
            ConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1));

            //Connect using connection string
            SBOApplication.Connect(ConnectionString);

            //Get UI Application
            Application = this.SBOApplication.GetApplication(ApplicationId);
        }

        #endregion
    }
}
