using System.ComponentModel;

namespace Core.Enums.Query
{
    /// <summary>
    /// The enums for query condition relations 
    /// </summary>
    public enum RelationEnum : int
    {
        [DescriptionAttribute("None")]
        NONE = 0,
        [DescriptionAttribute("And")]
        AND = 1,
        [DescriptionAttribute("Or")]
        OR = 2,
    }
}
