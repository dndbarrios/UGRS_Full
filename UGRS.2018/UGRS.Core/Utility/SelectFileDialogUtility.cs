using System.Threading;
using System.Windows.Forms;

namespace UGRS.Core.Utility
{
    public enum DialogType
    {
        SAVE,
        OPEN,
        FOLDER
    };

    public class SelectFileDialogUtility
    {
        private ManualResetEvent lObjshutdownEvent = new ManualResetEvent(false);
        public string SelectedFile { get; private set; }
        public string SelectedFolder { get; private set; }

        private string mStrFolder, mStrFile, mStrFilter;
        private string mStrTitle;
        private DialogType pDltType;

        public SelectFileDialogUtility(DialogType pDltType, string pStrTitle, string pStrFilter = null)
        {
            this.pDltType = pDltType;
            this.mStrTitle = pStrTitle;
            this.mStrFilter = pStrFilter;
        }

        private void InternalSelectFileDialog()
        {
            var lObjForm = new System.Windows.Forms.Form();
            lObjForm.TopMost = true;
            lObjForm.Height = 0;
            lObjForm.Width = 0;
            lObjForm.WindowState = FormWindowState.Minimized;
            lObjForm.Visible = true;

            switch (pDltType)
            {
                case DialogType.FOLDER:
                    FolderDialog(lObjForm);
                    break;
                case DialogType.OPEN:
                    OpenDialog(lObjForm);
                    break;
            }
            lObjshutdownEvent.Set();
            lObjForm.Close();
        }

        private void FolderDialog(System.Windows.Forms.Form pObjForm)
        {
            FolderBrowserDialog lObjDialog = new FolderBrowserDialog();

            lObjDialog.Description = "Open Folder";
            lObjDialog.ShowNewFolderButton = false;
            //dialog.SelectedPath = @"C:\Users\ssandoval\Desktop\ELGA.2018\ELGA.AddOn.PayrollPolicyXML\XML Samples"; 
            //----------------------------------------------------------------//
            if (lObjDialog.ShowDialog() == DialogResult.OK)
            {
                pObjForm.Close();
                SelectedFolder = lObjDialog.SelectedPath;
            }
            else
            {
                pObjForm.Close();
                SelectedFolder = "";
            }
        }

        private void OpenDialog(System.Windows.Forms.Form pObjForm)
        {
            OpenFileDialog lObjDialog = new OpenFileDialog();

            lObjDialog.Title = mStrTitle;
            lObjDialog.Multiselect = false;
            if (mStrFilter != null)
            {
                lObjDialog.Filter = mStrFilter;
            }
            //dialog.SelectedPath = @"C:\Users\ssandoval\Desktop\ELGA.2018\ELGA.AddOn.PayrollPolicyXML\XML Samples"; 
            //----------------------------------------------------------------//
            if (lObjDialog.ShowDialog() == DialogResult.OK)
            {
                lObjDialog.Dispose();
                pObjForm.Close();
                SelectedFile = lObjDialog.FileName;
            }
            else
            {
                lObjDialog.Dispose();
                pObjForm.Close();
                SelectedFile = "";
            }
        }

        public void Open()
        {
            Thread lObjThread = new Thread(new ThreadStart(this.InternalSelectFileDialog));
            lObjThread.SetApartmentState(ApartmentState.STA);
            lObjThread.Start();
            lObjshutdownEvent.WaitOne();
        }

        public void Close()
        {
            lObjshutdownEvent.Set();
        }
    }
}
