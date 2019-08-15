using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using UGRS.Core.Services;
using UGRS.Core.Utility;
using UGRS.Object.WeighingMachine.Events;
using UGRS.Object.WeighingMachine.Services;

namespace UGRS.Object.WeighingMachine
{
    [Serializable]
    [ComVisible(true)]
    public delegate void WeighingMachineEventHandler(string pStrValue);

    public class WeighingMachineServerObject : MarshalByRefObject
    {
        #region Attributes

        private ISerialPortService mObjSerialPortService;
        private IList<Guid> mLstObjConnections;
        private string mStrDataReceived;

        #endregion

        #region Properties

        private ISerialPortService SerialPort
        {
            get
            {
                return mObjSerialPortService;
            }
        }

        private IList<Guid> Connections
        {
            get
            {
                return mLstObjConnections;
            }
        }

        //private ISerialPortService WritePort
        //{
        //    get
        //    {
        //        return mObjSerialPortService;
        //    }
        //}

        #endregion

        public event WeighingMachineEventHandler DataReceived;

        public WeighingMachineEventHandler mObjDataReceivedDelegate;



        #region Constructor

        public WeighingMachineServerObject()
        {
            LogService.WriteInfo("Initiando constructor...");
            mObjDataReceivedDelegate = new WeighingMachineEventHandler(OnDataReceived);

            if (mObjSerialPortService == null)
            {
                mObjSerialPortService = GetSerialPortService();
                mObjSerialPortService.DataReceived += new SerialPortEventHandler(OnInternalDataReceived);
            }

            if (mLstObjConnections == null)
            {
                mLstObjConnections = new List<Guid>();
            }
        }

        //public void WriteSerialPort(string pStrText)
        //{
        //    WriteDisplayOne(SerialPort, pStrText);
        //}

        #endregion

        #region Events

        private delegate void DataReceivedWrapperDelegate(string pStrValue, WeighingMachineEventHandler pObjDelegate);

        private void OnDataReceived(string pStrValue)
        {
            if (DataReceived != null)
            {
                WeighingMachineEventHandler lObjWeighingMachineDelegate = null;
                Delegate[] lArrObjDelegates = null;

                try
                {
                    lArrObjDelegates = DataReceived.GetInvocationList();
                }
                catch (MemberAccessException lObjException)
                {
                    throw lObjException;
                }

                if (lArrObjDelegates != null)
                {
                    lock (this)
                    {
                        foreach (Delegate lObjDelegate in lArrObjDelegates)
                        {
                            try
                            {
                                lObjWeighingMachineDelegate = (WeighingMachineEventHandler)lObjDelegate;
                                lObjWeighingMachineDelegate(pStrValue);
                            }
                            catch
                            {
                                DataReceived -= lObjWeighingMachineDelegate;
                            }
                        }
                    }
                }
            }
        }

        private void DataReceivedBeginSend(string pStrValue, WeighingMachineEventHandler lObjDelegate)
        {
            try
            {
                System.Threading.Thread.Sleep(100);
                lObjDelegate(pStrValue);
            }
            catch
            {
                DataReceived -= lObjDelegate;
            }
        }

        private void DataReceivedEndSend(IAsyncResult pObjResult)
        {
            DataReceivedWrapperDelegate lObjWrapperDelegate = (DataReceivedWrapperDelegate)pObjResult.AsyncState;
            lObjWrapperDelegate.EndInvoke(pObjResult);
        }

        protected void OnInternalDataReceived(SerialPortEventArgs pObjEventArgs)
        {
            //LogService.WriteSuccess(string.Format("Dato recibido: {0}", pObjEventArgs.Value));
            mStrDataReceived += pObjEventArgs.Value;
            ProcessDataReceived();
        }

        private void ProcessDataReceived()
        {
            string lStrData = string.Empty;
            string lStrDataAux = string.Empty;

            switch (GetLocation())
            {
                case "HERMOSILLO":

                    if (mStrDataReceived.Contains("\r"))
                    {
                        //lStrData = mStrDataReceived.Replace("G", "").Replace("N", "").Replace("kg", "").Replace("\r", "").Trim();
                        lStrData = Regex.Replace(mStrDataReceived, "[^0-9]", "");
                        //LogService.WriteInfo("Cadena modificada : " + lStrData);
                        lStrData = lStrData.TrimStart('0');
                        //LogService.WriteInfo("Cadena modificada trimstart 0 : " + lStrData);
                        if (string.IsNullOrEmpty(lStrData) || Convert.ToDecimal(lStrData) == 0)
                        {
                            lStrData = "0";
                        }

                    }
                    break;

                case "SONORA_SUR":

                    if (mStrDataReceived.Contains(","))
                    {


                        lStrDataAux = Regex.Replace(mStrDataReceived, "[^0-9]", "");
                        //LogService.WriteInfo("Cadena modificada : " + lStrDataAux);


                        lStrDataAux = lStrDataAux.TrimStart('0');

                        //LogService.WriteInfo("Cadena modificada trimstart 0 : " + lStrDataAux);

                        lStrData = lStrDataAux.Length > 7 ? lStrDataAux.Substring(0,7).Trim('0').ToString() : lStrDataAux;

                        if (string.IsNullOrEmpty(lStrData) || Convert.ToDecimal(lStrData) == 0)
                        {
                            lStrData = "0";
                        }
                    }

                    break;
            }

            if (!string.IsNullOrEmpty(lStrData))
            {
                mStrDataReceived = string.Empty;
                OnDataReceived(lStrData);
            }
        }

        //private void WriteDisplayOne(ISerialPortService pObjDisplay, string pStrPrint)
        //{
        //    pObjDisplay.Write(pStrPrint);
        //}

        #endregion

        #region Methods

        public Guid Connect()
        {
            LogService.WriteInfo("Iniciando conexion...");
            Guid lObjConnection;

            lock (SerialPort)
            {
                if (SerialPort != null)
                {
                    LogService.WriteInfo("Puerto serial valido.");
                    if (!SerialPort.IsOpen())
                    {
                        LogService.WriteInfo("Iniciando puerto serial.");
                        SerialPort.Open();
                        LogService.WriteInfo("Iniciando correctamente.");
                    }
                }
            }

            lock (Connections)
            {
                LogService.WriteInfo("Connectando...");
                lObjConnection = Guid.NewGuid();
                Connections.Add(lObjConnection);
                LogService.WriteInfo("Conectado correctamente.");
            }

            return lObjConnection;
        }

        public void Disconnect(Guid pObjConnection)
        {
            LogService.WriteInfo("Deteniendo conexion...");
            lock (Connections)
            {
                if (Connections.Contains(pObjConnection))
                {
                    Connections.Remove(pObjConnection);
                    lock (SerialPort)
                    {
                        if (SerialPort != null)
                        {
                            LogService.WriteInfo("Puerto serial valido.");
                        }

                        if (Connections.Count == 0 && SerialPort.IsOpen())
                        {
                            LogService.WriteInfo("Deteniendo puerto serial.");
                            SerialPort.Close();
                            LogService.WriteInfo("Puerto serial detenido.");
                        }
                    }
                }
            }
            LogService.WriteInfo("Conexion detenida.");
        }

        public void DisconnectAll()
        {
            LogService.WriteInfo("Deteniendo todas las conexiones...");
            if (Connections != null)
            {
                lock (Connections)
                {
                    int lIntConnectionsCount = Connections.Count;
                    for (int i = 0; i < lIntConnectionsCount; i++)
                    {
                        Connections.Remove(Connections[i]);
                    }
                    if (Connections.Count == 0 && SerialPort != null && SerialPort.IsOpen())
                    {
                        lock (SerialPort)
                        {
                            LogService.WriteInfo("Deteniendo puerto serial.");
                            SerialPort.Close();
                            LogService.WriteInfo("Puerto serial detenido.");
                        }
                    }
                }
            }
            LogService.WriteInfo("Se han detenido todas las conexiones.");
        }

        private ISerialPortService GetSerialPortService()
        {
            if (IsVirtualMode())
            {
                LogService.WriteInfo("Obteniendo puerto serial virtual.");
                return new VirtualSerialPortService();
            }
            else
            {
                LogService.WriteInfo("Obteniendo puerto serial.");
                return new SerialPortService();
            }
        }

        private bool IsVirtualMode()
        {
            return ConfigurationUtility.GetValue<bool>("VirtualMode");
        }

        private string GetLocation()
        {
            return ConfigurationUtility.GetValue<string>("Location");
        }

        #endregion

        #region Other

        public override object InitializeLifetimeService()
        {
            return null;
        }

        ~WeighingMachineServerObject()
        {
            RemotingServices.Disconnect(this);
        }

        #endregion
    }
}
