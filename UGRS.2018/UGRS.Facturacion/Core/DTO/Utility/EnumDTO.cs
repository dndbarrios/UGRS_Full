using System;

namespace Core.DTO.Utility
{
    [Serializable]
    public class EnumDTO
    {
        public int Value { get; set; }
        public string Text { get; set; }

        public EnumDTO()
        {
        }

        public EnumDTO(dynamic pUnkObject)
        {
            Value = (int)pUnkObject.Id;
            Text = pUnkObject.Name;
        }
    }
}
