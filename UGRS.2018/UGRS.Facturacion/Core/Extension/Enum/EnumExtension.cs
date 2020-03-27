using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Core.DTO.Utility;
using System.Linq;

namespace Core.Extension.Enum
{
    public static class EnumExtension
    {
        public static string GetDescription(this System.Enum pObjElement)
        {
            Type lObjType = pObjElement.GetType();
            MemberInfo[] lArrObjMemberInfo = lObjType.GetMember(pObjElement.ToString());
            if (lArrObjMemberInfo != null && lArrObjMemberInfo.Length > 0)
            {
                object[] lArrObjAtributes = lArrObjMemberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (lArrObjAtributes != null && lArrObjAtributes.Length > 0)
                {
                    return ((DescriptionAttribute)lArrObjAtributes[0]).Description;
                }
            }

            return pObjElement.ToString();
        }

        public static T GetValueFromDescription<T>(string pStrDescription)
        {
            var lObjType = typeof(T);
            if (!lObjType.IsEnum) throw new InvalidOperationException();

            foreach (var lObjField in lObjType.GetFields())
            {
                var lObjAttribute = Attribute.GetCustomAttribute(lObjField, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (lObjAttribute != null)
                {
                    if (lObjAttribute.Description == pStrDescription)
                        return (T)lObjField.GetValue(null);
                }
                else
                {
                    if (lObjField.Name == pStrDescription)
                        return (T)lObjField.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "Description");
            // or return default(T);
        }
    }
}
