using System.ComponentModel;

namespace Core.Enums.Configuration
{
    public enum LocationEnum : int
    {
        [DescriptionAttribute("Hermosillo")]
        HERMOSILLO = 0,
        [DescriptionAttribute("Sonora sur")]
        SONORA_SUR = 1
    }
}
