// file:	Models\Section.cs
// summary:	Implements the section class

using SAPbouiCOM;

namespace Core.SDK.Models
{
    /// <summary> A section. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public class Section
    {
        public string UniqueID { get; set; }
        public string String { get; set; }
        public BoMenuType Type { get; set; }
        public int Position { get; set; }
    }
}
