// file:	Attributes\FormAttribute.cs
// summary:	Implements the form attribute class

using System;

namespace Core.SDK.Attributes
{
    /// <summary>   Attribute for form. </summary>
    /// <remarks>   Ranaya, 02/05/2017. </remarks>

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FormAttribute : Attribute
    {
        #region Attributes

        /// <summary>   Type of the form. </summary>
        private string mStrFormType;

        /// <summary>   Resource of the form. </summary>
        private string mStrResource;

        #endregion

        #region Properties

        /// <summary>   Gets or sets the type of the form. </summary>
        /// <value> The type of the form. </value>

        public string FormType 
        {
            get { return FormType; } 
            set { FormType = value; } 
        }

        /// <summary>   Gets or sets the resource. </summary>
        /// <value> The resource. </value>

        public string Resource
        {
            get { return mStrResource; }
            set { mStrResource = value; } 
        }

        #endregion

        #region Constructor

        /// <summary>   Constructor. </summary>
        /// <remarks>   Ranaya, 02/05/2017. </remarks>
        /// <param name="pStrFormType"> Type of the string form. </param>

        public FormAttribute(string pStrFormType)
        {
            FormType = pStrFormType;
        }

        /// <summary>   Constructor. </summary>
        /// <remarks>   Ranaya, 02/05/2017. </remarks>
        /// <param name="pStrFormType"> Type of the string form. </param>
        /// <param name="pStrResource"> The string resource. </param>

        public FormAttribute(string pStrFormType, string pStrResource)
        {
            FormType = pStrFormType;
            Resource = pStrResource;
        }

        #endregion
    }
}
