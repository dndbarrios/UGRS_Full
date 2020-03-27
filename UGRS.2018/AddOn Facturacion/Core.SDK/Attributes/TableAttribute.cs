// file:	Attributes\TableAttribute.cs
// summary:	Implements the table attribute class

using SAPbobsCOM;
using System;

namespace Core.SDK.Attributes
{
    /// <summary> Attribute for table. </summary>
    /// <remarks> Ranaya, 02/05/2017. </remarks>

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        #region Attributes

        /// <summary> Priority of the table. </summary>
        private int mIntPriority;

        /// <summary> Name of the table. </summary>
        private string mStrName;

        /// <summary> Information describing the table. </summary>
        private string mStrDescription;

        /// <summary> Type of the table. </summary>
        private BoUTBTableType mEnmType;

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

        public BoUTBTableType Type
        {
            get { return mEnmType; }
            set { mEnmType = value; }
        }

        #endregion

        #region Constructor

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>

        public TableAttribute()
        {
            mIntPriority = 0;
            mStrName = string.Empty;
            mStrDescription = string.Empty;
            mEnmType = BoUTBTableType.bott_NoObject;
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <param name="pStrName">        Name of the table. </param>
        /// <param name="pStrDescription"> Information describing the table. </param>

        public TableAttribute(string pStrName, string pStrDescription)
        {
            mIntPriority = 0;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = BoUTBTableType.bott_NoObject;
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <param name="pStrName">        Name of the table. </param>
        /// <param name="pStrDescription"> Information describing the table. </param>
        /// <param name="pEnmType">        Type of the table. </param>

        public TableAttribute(string pStrName, string pStrDescription, BoUTBTableType pEnmType)
        {
            mIntPriority = 0;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = pEnmType;
        }

        #endregion
    }
}
