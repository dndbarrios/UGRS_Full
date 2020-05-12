using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.DTO.Utility;
using Core.Extension.Enum;

namespace Core.Utility
{
    public class ComboUtility
    {
        public static IList<ComboDTO> ParseEnumToCombo<T>() where T : IConvertible
        {
            IList<ComboDTO> lLstObjComboResult = new List<ComboDTO>();

            int lIntValueItem = 0;
            string lStrLabelItem = null;

            if (typeof(T).IsEnum)
            {
                foreach (Enum lObjItem in typeof(T).GetEnumValues())
                {
                    lIntValueItem = Convert.ToInt32(lObjItem);
                    lStrLabelItem = lObjItem.GetDescription();
                    lLstObjComboResult.Add(new ComboDTO() { Value = lIntValueItem, Text = lStrLabelItem });
                }
                return lLstObjComboResult;
            }
            else
            {
                throw new Exception("La clase seleccionada no es del tipo enumerador.");
            }
        }

        public static IList<ComboDTO> ParseListObjectToCombo(dynamic pLstUnk)
        {
            IList<ComboDTO> lLstObjComboResult = new List<ComboDTO>();

            if (pLstUnk != null)
            {
                foreach (dynamic o in pLstUnk)
                {
                    lLstObjComboResult.Add(new ComboDTO() { Value = (int)o.Id, Text = o.Name});
                }
            }
            return lLstObjComboResult;
        }

        public static bool IsValidCbo(ComboBox pObjCombo)
        {
            if (((ComboDTO)pObjCombo.SelectedItem).Value == -1)
            {
                return false;
            }
            return (int)pObjCombo.SelectedIndex == 0 ? false : true;
        }

        public static ComboBox ReInitCombo(ComboBox pObjCombo, int pIntId, string pStrName)
        {
            pObjCombo.DataSource = LookForDisabledItem(pObjCombo, pIntId, pStrName);
            pObjCombo.DisplayMember = "Text";
            pObjCombo.ValueMember = "Value";
            pObjCombo.SelectedIndex = HaveDisabledItem(pObjCombo) ? GetIndexForCombo(pObjCombo, -1) : GetIndexForCombo(pObjCombo, pIntId);
            return pObjCombo;
        }

        public static int GetIndexForCombo(object pObjSender, int pIntId)
        {
            for (int x = 0; x <= ((ComboBox)pObjSender).Items.Count - 1; x++)
            {
                if (((ComboDTO)((ComboBox)pObjSender).Items[x]).Value == pIntId)
                    return x;
            }
            return 0;
        }

        private static IList<ComboDTO> LookForDisabledItem(ComboBox pObjCombo, int pIntId, string pStrName)
        {
            IList<ComboDTO> lLstObj = new List<ComboDTO>();
            bool lBolResult = false;

            foreach (ComboDTO lObjComboDTO in pObjCombo.Items)
            {
                if (lObjComboDTO.Value == pIntId)
                {
                    lBolResult = true;
                }
                lLstObj.Add(new ComboDTO() { Value = lObjComboDTO.Value, Text = lObjComboDTO.Text });
            }
            if (!lBolResult)
            {
                lLstObj.Add(new ComboDTO()
                {
                    Value = -1,
                    Text = pStrName != string.Empty ? pStrName : "Registro desactivado, favor de seleccionar otro"
                });
            }
            return lLstObj.OrderBy(a => a.Text).ToList();
        }

        private static bool HaveDisabledItem(ComboBox pObjCombo)
        {
            bool lBolResult = false;
            foreach (ComboDTO lObjComboDTO in pObjCombo.Items)
            {
                if (lObjComboDTO.Value == -1)
                    lBolResult = true;
            }
            return lBolResult;
        }
    }
}
