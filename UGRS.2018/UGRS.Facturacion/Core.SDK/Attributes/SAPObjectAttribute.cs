using System;

namespace Core.SDK.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SAPObjectAttribute : Attribute
    {
        #region Attributes

        /// <summary> Table name. </summary>
        private string mStrTablename;

        #endregion

        #region Properties

        /// <summary> Gets or sets the table name. </summary>
        /// <value> The table name. </value>

        public string TableName
        {
            get { return mStrTablename; }
            set { mStrTablename = value; }
        }

        #endregion

        #region Constructor

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <param name="pStrTableName"> Name of table. </param>

        public SAPObjectAttribute(string pStrTableName)
        {
            mStrTablename = pStrTableName;
        }

        #endregion
    }
}
