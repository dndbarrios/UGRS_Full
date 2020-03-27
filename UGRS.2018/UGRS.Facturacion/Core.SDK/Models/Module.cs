// file:	Models\Module.cs
// summary:	Implements the module class

using SAPbouiCOM;
using System.Collections.Generic;

namespace Core.SDK.Models
{
    /// <summary> A module. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public class Module
    {
        public string UniqueID { get; set; }
        public string String { get; set; }
        public BoMenuType Type { get; set; }
        public bool Enable { get; set; }
        public string Image { get; set; }
        public int Position { get; set; }
        public IList<Section> Sections { get; set; }
    }
}
