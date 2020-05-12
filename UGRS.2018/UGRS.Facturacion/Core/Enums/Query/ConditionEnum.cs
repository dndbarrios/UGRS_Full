using System.ComponentModel;

namespace Core.Enums.Query
{
    /// <summary>
    /// The enums for query conditions
    /// </summary>
    public enum ConditionEnum : int
    {
        [DescriptionAttribute("Equal")]
        EQUAL = 0,
        [DescriptionAttribute("Not equal")]
        NOT_EQUAL = 1,
        [DescriptionAttribute("Like")]
        LIKE = 2,
    }
}
