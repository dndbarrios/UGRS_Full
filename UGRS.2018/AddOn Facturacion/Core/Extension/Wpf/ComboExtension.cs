using System;
using System.Collections.Generic;
using Core.DTO.Utility;
using Core.Extension.Enum;
using Core.Utility;

namespace Core.Extension.Wpf
{
    public static class ComboExtension
    {
        public static void LoadSource<T>(this System.Windows.Controls.ComboBox pObjSender) where T : IConvertible
        {
            if(typeof(T).IsEnum)
            {
                pObjSender.ItemsSource = ComboUtility.ParseEnumToCombo<T>();
                pObjSender.DisplayMemberPath = "Text";
                pObjSender.SelectedValuePath = "Value";
            }
        }

        public static int GetIndex(this System.Windows.Controls.ComboBox pObjSender, int pIntId)
        {
            for (int x = 0; x <= ((System.Windows.Controls.ComboBox)pObjSender).Items.Count - 1; x++)
            {
                if (((ComboDTO)((System.Windows.Controls.ComboBox)pObjSender).Items[x]).Value == pIntId)
                    return x;
            }
            return 0;
        }

        public static T GetSelectedEnumValue<T>(this System.Windows.Controls.ComboBox pObjSender) where T : IConvertible
        {
            return (T)pObjSender.SelectedValue;
        }
    }
}
