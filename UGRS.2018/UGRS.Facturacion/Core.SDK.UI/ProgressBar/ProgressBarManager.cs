using Core.Utility;

namespace Core.SDK.UI.ProgressBar
{
    public class ProgressBarManager
    {
        private static int DEFAULT_CURRENT_POSITION = 0;
        private static bool DEFAULT_STOPABLE_VALUE = false;

        private SAPbouiCOM.Application pObjApplication;
        private SAPbouiCOM.ProgressBar mObjProgressBar;

        private int MaximumPosition { get; set; }
        private int CurrentPosition { get; set; }
        private string Message { get; set; }
        private bool IsStopable { get; set; }

        public int Posision
        {
            get { return this.mObjProgressBar.Value; }
        }

        public int Max
        {
            get { return this.mObjProgressBar.Maximum; }
            set { this.mObjProgressBar.Maximum = value; }
        }

        public ProgressBarManager(SAPbouiCOM.Application pObjApplication, string pStrProgressBarMessage, int pIntMaximumPosition)
        {
            this.pObjApplication = pObjApplication;
            this.Message = pStrProgressBarMessage;
            this.MaximumPosition = pIntMaximumPosition;

            this.IsStopable = ProgressBarManager.DEFAULT_STOPABLE_VALUE;
            this.CurrentPosition = ProgressBarManager.DEFAULT_CURRENT_POSITION;

            this.mObjProgressBar = pObjApplication.StatusBar.CreateProgressBar(this.Message, MaximumPosition, this.IsStopable);
            this.mObjProgressBar.Value = 0;
            this.mObjProgressBar.Maximum = MaximumPosition;
        }

        public void NextPosition()
        {
            if (this.CurrentPosition < this.MaximumPosition)
            {
                try
                {
                    this.CurrentPosition = this.CurrentPosition + 1;
                    this.mObjProgressBar.Value = this.CurrentPosition;
                }
                catch
                {
                    MemoryUtility.ReleaseComObject(this.mObjProgressBar);
                    this.mObjProgressBar = this.pObjApplication.StatusBar.CreateProgressBar(this.Message, this.MaximumPosition, this.IsStopable);
                    this.mObjProgressBar.Value = this.CurrentPosition;
                }
            }
            else
            {
                try
                {
                    this.mObjProgressBar.Value = this.MaximumPosition;
                }
                catch
                {
                    MemoryUtility.ReleaseComObject(this.mObjProgressBar);
                    this.mObjProgressBar = this.pObjApplication.StatusBar.CreateProgressBar(this.Message, this.MaximumPosition, this.IsStopable);
                    this.mObjProgressBar.Value = this.CurrentPosition;
                }
            }
        }

        public void PreviousPosition()
        {
            if (this.CurrentPosition > 0)
            {
                try
                {
                    this.CurrentPosition = this.CurrentPosition - 1;
                    this.mObjProgressBar.Value = this.CurrentPosition;
                }
                catch
                {
                    MemoryUtility.ReleaseComObject(this.mObjProgressBar);
                    this.mObjProgressBar = this.pObjApplication.StatusBar.CreateProgressBar(this.Message, this.MaximumPosition, this.IsStopable);
                    this.mObjProgressBar.Value = this.CurrentPosition;
                }
            }
            else
            {
                try
                {
                    this.mObjProgressBar.Value = 0;
                }
                catch
                {
                    MemoryUtility.ReleaseComObject(this.mObjProgressBar);
                    this.mObjProgressBar = this.pObjApplication.StatusBar.CreateProgressBar(this.Message, this.MaximumPosition, this.IsStopable);
                    this.mObjProgressBar.Value = 0;
                }
            }
        }

        public void SetValue(int pIntPosition)
        {
            try
            {
                this.mObjProgressBar.Value = pIntPosition;
                this.CurrentPosition = pIntPosition;
            }
            catch
            {
                MemoryUtility.ReleaseComObject(this.mObjProgressBar);
                this.mObjProgressBar = this.pObjApplication.StatusBar.CreateProgressBar(this.Message, this.MaximumPosition, this.IsStopable);
                this.mObjProgressBar.Value = pIntPosition;
            }
        }

        public void Stop()
        {
            try
            {
                this.mObjProgressBar.Stop();
            }
            catch
            {
                MemoryUtility.ReleaseComObject(this.mObjProgressBar);
                this.mObjProgressBar = this.pObjApplication.StatusBar.CreateProgressBar(this.Message, this.MaximumPosition, this.IsStopable);
                this.mObjProgressBar.Value = this.CurrentPosition;
                this.mObjProgressBar.Stop();
            }
        }

        public void Dispose()
        {
            try
            {
                MemoryUtility.ReleaseComObject(this.mObjProgressBar);
                this.mObjProgressBar = (SAPbouiCOM.ProgressBar)null;
            }
            catch
            {
                //Ignore
            }
        }
    }
}
