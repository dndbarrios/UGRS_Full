// file:	Attributes\FieldAttribute.cs
// summary:	Implements the field attribute class

using SAPbobsCOM;
using System;

namespace Core.SDK.Attributes
{
    /// <summary> Attribute for field. </summary>
    /// <remarks> Ranaya, 02/05/2017. </remarks>

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldAttribute : Attribute
    {
        #region Attributes

        /// <summary> Priority. </summary>
        private int mIntPriority;

        /// <summary> Name. </summary>
        private string mStrName;

        /// <summary> Information describing the string. </summary>
        private string mStrDescription;

        /// <summary> Type of the enm. </summary>
        private BoFieldTypes mEnmType;

        /// <summary> Type of the enm sub. </summary>
        private BoFldSubTypes mEnmSubType;

        /// <summary> Size of the int. </summary>
        private int mIntSize;

        /// <summary> Size of the int sub. </summary>
        private int mIntSubSize;

        private string mStrLinkedTable;

        private string mStrLinkedUDO;


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

        public BoFieldTypes Type
        {
            get { return mEnmType; }
            set { mEnmType = value; }
        }

        /// <summary> Gets or sets the type of the sub. </summary>
        /// <value> The type of the sub. </value>

        public BoFldSubTypes SubType
        {
            get { return mEnmSubType; }
            set { mEnmSubType = value; }
        }

        /// <summary> Gets or sets the size. </summary>
        /// <value> The size. </value>

        public int Size
        {
            get { return mIntSize; }
            set { mIntSize = value; }
        }

        /// <summary> Gets or sets the size of the sub. </summary>
        /// <value> The size of the sub. </value>

        public int SubSize
        {
            get { return mIntSubSize; }
            set { mIntSubSize = value; }
        }

        public string LinkedTable
        {
            get { return mStrLinkedTable; }
            set { mStrLinkedTable = value; }
        }

        public string LinkedUDO
        {
            get { return mStrLinkedUDO; }
            set { mStrLinkedUDO = value; }
        }

        #endregion

        #region Constructor

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>

        public FieldAttribute()
        {
            mIntPriority = 0;
            mStrName = string.Empty;
            mStrDescription = string.Empty;
            mEnmType = BoFieldTypes.db_Alpha;
            mEnmSubType = BoFldSubTypes.st_None;
            mIntSize = 11;
            mIntSubSize = 10;
            mStrLinkedTable = "";
            mStrLinkedUDO = "";
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <param name="pStrName">        Name of the string. </param>
        /// <param name="pStrDescription"> Information describing the string. </param>

        public FieldAttribute(string pStrName, string pStrDescription)
        {
            mIntPriority = 0;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = BoFieldTypes.db_Alpha;
            mEnmSubType = BoFldSubTypes.st_None;
            mIntSize = 11;
            mIntSubSize = 10;
            mStrLinkedTable = "";
            mStrLinkedUDO = "";
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <param name="pIntPriority">    The int priority. </param>
        /// <param name="pStrName">        Name of the string. </param>
        /// <param name="pStrDescription"> Information describing the string. </param>
        /// <param name="pEnmType">        Type of the enm. </param>
        /// <param name="pEnmSubType">     Type of the enm sub. </param>
        /// <param name="pIntSize">        Size of the int. </param>
        /// <param name="pIntSubSize">     Size of the int sub. </param>

        public FieldAttribute(int pIntPriority, string pStrName, string pStrDescription, BoFieldTypes pEnmType, BoFldSubTypes pEnmSubType, int pIntSize, int pIntSubSize)
        {
            mIntPriority = pIntPriority;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = pEnmType;
            mEnmSubType = pEnmSubType;
            mIntSize = pIntSize;
            mIntSubSize = pIntSubSize;
            mStrLinkedTable = "";
            mStrLinkedUDO = "";
        }

        #endregion
    }
}
