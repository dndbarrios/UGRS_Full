using System;
using System.IO.Ports;
using UGRS.Core.Utility;
using UGRS.Object.WeighingMachine.Events;

namespace UGRS.Object.WeighingMachine.Services
{
    public class SerialPortService : ISerialPortService 
    {
        private SerialPort mObjSerialPort;
        string mStrData;
        public SerialPortService()
        {
            mObjSerialPort = GetSerialPort();
        }

        public event SerialPortEventHandler DataReceived;

        protected virtual void OnDataReceived(SerialPortEventArgs pObjEventArgs)
        {
            if (DataReceived != null)
            {
                DataReceived(pObjEventArgs);
            }
        }

        //public void Write(string pStrValue)
        //{
        //    if (mObjSerialPort != null && mObjSerialPort.IsOpen)
        //    {
        //        mObjSerialPort.WriteLine(pStrValue);
        //    }
        //}

        public string GetName()
        {
            return mObjSerialPort != null ? mObjSerialPort.PortName : "Unknown";
        }

        public void Open()
        {
            if(mObjSerialPort != null && !mObjSerialPort.IsOpen)
            {
                mObjSerialPort.Open();
            }
        }

        public void Close()
        {
            if (mObjSerialPort != null && mObjSerialPort.IsOpen)
            {
                mObjSerialPort.Close();
            }
        }

        public bool IsOpen()
        {
            return mObjSerialPort.IsOpen;
        }

        private SerialPort GetSerialPort()
        {
            SerialPort lObjResult = new SerialPort();

            //Get and set properties
            lObjResult.PortName = GetPortName();
            lObjResult.BaudRate = GetBaudRate();
            lObjResult.DataBits = GetDataBits();
            lObjResult.Parity = GetParity();
            lObjResult.StopBits = GetStopBits();
            lObjResult.ReadTimeout = GetReadTimeout();

            //AddEvent
            lObjResult.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            return lObjResult;
        }

        protected void DataReceivedHandler(object pObjSender, SerialDataReceivedEventArgs pObjEventArgs)
        {
            SerialPort lObjSerialPort = (SerialPort)pObjSender;
            string lStrResult = lObjSerialPort.ReadExisting();
            //lStrResult = GetDataReceived(lStrResult);
            OnDataReceived(new SerialPortEventArgs(lStrResult));
           
        }

        #region Configuration

        private string GetPortName()
        {
            return ConfigurationUtility.GetValue<string>("PortName");
        }

        private int GetBaudRate()
        {
            return ConfigurationUtility.GetValue<int>("BaudRate");
        }

        private int GetDataBits()
        {
            return ConfigurationUtility.GetValue<int>("DataBits");
        }

        private Parity GetParity()
        {
            return ConfigurationUtility.GetValue<Parity>("Parity");
        }

        private StopBits GetStopBits()
        {
            return ConfigurationUtility.GetValue<StopBits>("StopBits");
        }

        private int GetReadTimeout()
        {
            return ConfigurationUtility.GetValue<int>("ReadTimeout");
        }

        #endregion
    }
}
