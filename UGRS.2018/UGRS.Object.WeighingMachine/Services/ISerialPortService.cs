using UGRS.Object.WeighingMachine.Events;

namespace UGRS.Object.WeighingMachine.Services
{
    public interface ISerialPortService
    {
        event SerialPortEventHandler DataReceived;

        string GetName();

        void Open();

        void Close();

        bool IsOpen();

        //void Write(string pStrValue);
    }
}
