using SAPbobsCOM;
using System;

namespace UGRS.CheckAdminSDK.SDK.DI.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute {
        #region Attributes

        private int mIntPriority;
        private string mStrName;
        private string mStrDescription;
        private BoUTBTableType mEnmType;

        #endregion

        #region Properties
        public int Priority {
            get { return mIntPriority; }
            set { mIntPriority = value; }
        }

        public string Name {
            get { return mStrName; }
            set { mStrName = value; }
        }

        public string Description {
            get { return mStrDescription; }
            set { mStrDescription = value; }
        }

        public BoUTBTableType Type {
            get { return mEnmType; }
            set { mEnmType = value; }
        }

        #endregion

        #region Constructor
        public TableAttribute() {
            mIntPriority = 0;
            mStrName = string.Empty;
            mStrDescription = string.Empty;
            mEnmType = BoUTBTableType.bott_NoObject;
        }

        public TableAttribute(string pStrName, string pStrDescription) {
            mIntPriority = 0;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = BoUTBTableType.bott_NoObject;
        }

        public TableAttribute(string pStrName, string pStrDescription, BoUTBTableType pEnmType) {
            mIntPriority = 0;
            mStrName = pStrName;
            mStrDescription = pStrDescription;
            mEnmType = pEnmType;
        }

        #endregion
    }
}
