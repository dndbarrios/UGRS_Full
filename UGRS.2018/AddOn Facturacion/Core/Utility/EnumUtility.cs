using System;
using System.Collections.Generic;
using Core.DTO.Utility;
using Core.Extension.Enum;

namespace Core.Utility
{
    public class EnumUtility
    {
        public static IList<EnumDTO> GetEnumList<T>() where T : struct, IConvertible
        {
            if (typeof(T).IsEnum)
            {
                IList<EnumDTO> lLstObjResult = new List<EnumDTO>();
                int lIntValueItem = 0;
                string lStrLabelItem = null;

                foreach (System.Enum lObjItem in typeof(T).GetEnumValues())
                {
                    lIntValueItem = Convert.ToInt32(lObjItem);
                    lStrLabelItem = lObjItem.GetDescription();

                    lLstObjResult.Add(new EnumDTO() { Value = lIntValueItem, Text = lStrLabelItem });
                }

                return lLstObjResult;
            }
            else
            {
                throw new Exception("La clase seleccionada no es del tipo enumerador.");
            }
        }
    }
}
