using System;

namespace Core.DTO.Log
{
    [Serializable]
    public class LogDTO
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
