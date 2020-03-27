using System.ComponentModel;

namespace Core.Enums.Query
{
    /// <summary>
    /// The enums for query order
    /// </summary>
    public enum OrderEnum : int
    {
        [DescriptionAttribute("Ascending")]
        ASC = 0,
        [DescriptionAttribute("Descending")]
        DESC = 1
    }
}
