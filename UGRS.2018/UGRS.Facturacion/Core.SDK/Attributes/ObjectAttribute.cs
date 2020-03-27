// file:	Attributes\ObjectAttribute.cs
// summary:	Implements the object attribute class

using SAPbobsCOM;
using System;

namespace Core.SDK.Attributes
{
    /// <summary>   Attribute for object. </summary>
    /// <remarks>   Ranaya, 02/05/2017. </remarks>

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ObjectAttribute : Attribute
    {
        #region Attributes

        /// <summary>   The int priority. </summary>
        private int mIntPriority;

        /// <summary>   Name of the string. </summary>
        private string mStrName;

        /// <summary>   Information describing the string. </summary>
        private string mStrDescription;

        /// <summary>   Type of the enm. </summary>
        private BoUDOObjType mEnmType;

        /// <summary> True if bol can cancel. </summary>
        private bool mBolCanCancel;

        /// <summary> True if bol can close. </summary>
        private bool mBolCanClose;

        /// <summary> True if bol can create default form. </summary>
        private bool mBolCanCreateDefaultForm;

        /// <summary> True if bol can delete. </summary>
        private bool mBolCanDelete;

        /// <summary> True if bol can find. </summary>
        private bool mBolCanFind;

        /// <summary> True if bol can year transfer. </summary>
        private bool mBolCanYearTransfer;

        /// <summary> True to bol manage series. </summary>
        private bool mBolManageSeries;

        /// <summary> True if bol can archive. </summary>
        private bool mBolCanArchive;

        /// <summary> The string object code. </summary>
        private string mStrObjectCode;

        /// <summary> Information describing the string object. </summary>
        private string mStrObjectDescription;

        #endregion

        #region Properties

        /// <summary> Gets or sets the priority. </summary>
        /// <value> The priority. </value>

        public int Priority
        {
            get { return mIntPriority; }
            set { mIntPriority = value; }
        }

        /// <summary> Gets or sets the name. </summary>
        /// <value> The name. </value>

        public string Name
        {
            get { return mStrName; }
            set { mStrName = value; }
        }

        /// <summary> Gets or sets the description. </summary>
        /// <value> The description. </value>

        public string Description
        {
            get { return mStrDescription; }
            set { mStrDescription = value; }
        }

        /// <summary> Gets or sets the type. </summary>
        /// <value> The type. </value>

        public BoUDOObjType Type
        {
            get { return mEnmType; }
            set { mEnmType = value; }
        }

        /// <summary> Gets or sets a value indicating whether we can cancel. </summary>
        /// <value> True if we can cancel, false if not. </value>

        public bool CanCancel
        {
            get { return mBolCanCancel; }
            set { mBolCanCancel = value; }
        }

        /// <summary> Gets or sets a value indicating whether we can close. </summary>
        /// <value> True if we can close, false if not. </value>

        public bool CanClose
        {
            get { return mBolCanClose; }
            set { mBolCanClose = value; }
        }

        /// <summary> Gets or sets a value indicating whether we can create default form. </summary>
        /// <value> True if we can create default form, false if not. </value>

        public bool CanCreateDefaultForm
        {
            get { return mBolCanCreateDefaultForm; }
            set { mBolCanCreateDefaultForm = value; }
        }

        /// <summary> Gets or sets a value indicating whether we can delete. </summary>
        /// <value> True if we can delete, false if not. </value>

        public bool CanDelete
        {
            get { return mBolCanDelete; }
            set { mBolCanDelete = value; }
        }

        /// <summary> Gets or sets a value indicating whether we can find. </summary>
        /// <value> True if we can find, false if not. </value>

        public bool CanFind
        {
            get { return mBolCanFind; }
            set { mBolCanFind = value; }
        }

        /// <summary> Gets or sets a value indicating whether we can year transfer. </summary>
        /// <value> True if we can year transfer, false if not. </value>

        public bool CanYearTransfer
        {
            get { return mBolCanYearTransfer; }
            set { mBolCanYearTransfer = value; }
        }

        /// <summary> Gets or sets a value indicating whether the manage series. </summary>
        /// <value> True if manage series, false if not. </value>

        public bool ManageSeries
        {
            get { return mBolManageSeries; }
            set { mBolManageSeries = value; }
        }

        /// <summary> Gets or sets a value indicating whether we can archive. </summary>
        /// <value> True if we can archive, false if not. </value>

        public bool CanArchive
        {
            get { return mBolCanArchive; }
            set { mBolCanArchive = value; }
        }

        /// <summary> Gets or sets the object code. </summary>
        /// <value> The object code. </value>

        public string ObjectCode
        {
            get { return mStrObjectCode; }
            set { mStrObjectCode = value; }
        }

        /// <summary> Gets or sets information describing the object. </summary>
        /// <value> Information describing the object. </value>

        public string ObjectDescription
        {
            get { return mStrObjectDescription; }
            set { mStrObjectDescription = value; }
        }

        #endregion

        #region Constructor

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>

        public ObjectAttribute()
        {
            mIntPriority = 0;
            mStrName = string.Empty;
            mStrDescription = string.Empty;
            mEnmType = BoUDOObjType.boud_Document;
            mBolCanArchive = false;
            mStrObjectCode = string.Empty;
            mStrObjectDescription = string.Empty;
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <param name="pStrName">        Name of the string. </param>
        /// <param name="pStrDescription"> Information describing the string. </param>

        public ObjectAttribute(string pStrName, string pStrDescription)
        {
            mIntPriority = 0;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = BoUDOObjType.boud_Document;
            mBolCanArchive = false;
            mStrObjectCode = string.Empty;
            mStrObjectDescription = string.Empty;
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <param name="pStrName">        Name of the string. </param>
        /// <param name="pStrDescription"> Information describing the string. </param>
        /// <param name="pEnmType">        Type of the enm. </param>

        public ObjectAttribute(string pStrName, string pStrDescription, BoUDOObjType pEnmType)
        {
            mIntPriority = 0;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = pEnmType;
            mBolCanArchive = false;
            mStrObjectCode = string.Empty;
            mStrObjectDescription = string.Empty;
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <param name="pIntPriority">          The int priority. </param>
        /// <param name="pStrName">              Name of the string. </param>
        /// <param name="pStrDescription">       Information describing the string. </param>
        /// <param name="pEnmType">              Type of the enm. </param>
        /// <param name="pBolCanArchive">        True if bol can archive. </param>
        /// <param name="pStrObjectCode">        The string object code. </param>
        /// <param name="pStrObjectDescription"> Information describing the string object. </param>

        public ObjectAttribute(int pIntPriority, string pStrName, string pStrDescription, BoUDOObjType pEnmType, bool pBolCanArchive, string pStrObjectCode, string pStrObjectDescription)
        {
            mIntPriority = pIntPriority;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = pEnmType;
            mBolCanArchive = pBolCanArchive;
            mStrObjectCode = pStrObjectCode;
            mStrObjectDescription = pStrObjectDescription;
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <param name="pIntPriority">             The int priority. </param>
        /// <param name="pStrName">                 Name of the string. </param>
        /// <param name="pStrDescription">          Information describing the string. </param>
        /// <param name="pEnmType">                 Type of the enm. </param>
        /// <param name="pBolCanCancel">            True if bol can cancel. </param>
        /// <param name="pBolCanClose">             True if bol can close. </param>
        /// <param name="pBolCanCreateDefaultForm"> True if bol can create default form. </param>
        /// <param name="pBolCanDelete">            True if bol can delete. </param>
        /// <param name="pBolCanFind">              True if bol can find. </param>
        /// <param name="pBolCanYearTransfer">      True if bol can year transfer. </param>
        /// <param name="pBolManageSeries">         True to bol manage series. </param>
        /// <param name="pBolCanArchive">           True if bol can archive. </param>
        /// <param name="pStrObjectCode">           The string object code. </param>
        /// <param name="pStrObjectDescription">    Information describing the string object. </param>

        public ObjectAttribute(int pIntPriority, string pStrName, string pStrDescription, BoUDOObjType pEnmType, bool pBolCanCancel, bool pBolCanClose, bool pBolCanCreateDefaultForm, bool pBolCanDelete, bool pBolCanFind, bool pBolCanYearTransfer, bool pBolManageSeries, bool pBolCanArchive, string pStrObjectCode, string pStrObjectDescription)
        {
            mIntPriority = pIntPriority;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = pEnmType;
            mBolCanCancel = pBolCanCancel;
            mBolCanClose = pBolCanClose;
            mBolCanCreateDefaultForm = pBolCanCreateDefaultForm;
            mBolCanDelete = pBolCanDelete;
            mBolCanFind = pBolCanFind;
            mBolCanYearTransfer = pBolCanYearTransfer;
            mBolManageSeries = pBolManageSeries;
            mBolCanArchive = pBolCanArchive;
            mStrObjectCode = pStrObjectCode;
            mStrObjectDescription = pStrObjectDescription;
        }

        #endregion
    }
}
