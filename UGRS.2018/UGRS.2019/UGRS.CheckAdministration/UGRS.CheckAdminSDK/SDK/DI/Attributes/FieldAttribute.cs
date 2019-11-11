using SAPbobsCOM;
using System;

namespace UGRS.CheckAdminSDK.SDK.DI.Attributes {

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldAttribute : Attribute {
        #region Attributes

        private int mIntPriority;
        private string mStrName;
        private string mStrDescription;
        private BoFieldTypes mEnmType;
        private BoFldSubTypes mEnmSubType;
        private int mIntSize;
        private int mIntSubSize;
        private string mStrLinkedTable;
        private string mStrLinkedUDO;
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

        public BoFieldTypes Type {
            get { return mEnmType; }
            set { mEnmType = value; }
        }

        public BoFldSubTypes SubType {
            get { return mEnmSubType; }
            set { mEnmSubType = value; }
        }

        public int Size {
            get { return mIntSize; }
            set { mIntSize = value; }
        }

        public int SubSize {
            get { return mIntSubSize; }
            set { mIntSubSize = value; }
        }

        public string LinkedTable {
            get { return mStrLinkedTable; }
            set { mStrLinkedTable = value; }
        }

        public string LinkedUDO {
            get { return mStrLinkedUDO; }
            set { mStrLinkedUDO = value; }
        }

        #endregion

        #region Constructor
        public FieldAttribute() {
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

        public FieldAttribute(string pStrName, string pStrDescription) {
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

        public FieldAttribute(int pIntPriority, string pStrName, string pStrDescription, BoFieldTypes pEnmType, BoFldSubTypes pEnmSubType, int pIntSize, int pIntSubSize) {
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
