using System.Collections.Generic;
using System.Linq;
using Core.DTO.Utility;

namespace Core.Extension.Enum
{
    public static class ListExtension
    {
        public static IList<EnumDTO> ToEnumList<T>(this IList<T> pLstUnkList)
        {
            return pLstUnkList.Select(x => new EnumDTO(x)).ToList();
        }
    }
}
