using System;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.CreditNote.DOC
{
    public class CN_AttachDI
    {
        public int AttachFile(string pStrFile)
        {
            int lIntAttEntry = -1;
            SAPbobsCOM.Attachments2 lObjAttachment = (SAPbobsCOM.Attachments2)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2);
            try
            {
                string lStrFileName = pStrFile;
                lObjAttachment.Lines.Add();
                lObjAttachment.Lines.FileName = System.IO.Path.GetFileNameWithoutExtension(lStrFileName);
                lObjAttachment.Lines.FileExtension = System.IO.Path.GetExtension(lStrFileName).Substring(1);
                lObjAttachment.Lines.SourcePath = System.IO.Path.GetDirectoryName(lStrFileName);
                lObjAttachment.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;

                if (lObjAttachment.Add() == 0)
                {
                    lIntAttEntry = int.Parse(DIApplication.Company.GetNewObjectKey());
                }
                else
                {
                    UIApplication.ShowMessageBox(DIApplication.Company.GetLastErrorDescription());
                    LogService.WriteError("AttachmentDI (AttachFile) " + DIApplication.Company.GetLastErrorDescription());
                }

            }
            catch (Exception ex)
            {
               
                LogService.WriteError("AttachmentDI (AttachFile) " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessage(ex.Message);

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjAttachment);
            }
            return lIntAttEntry;
        }
    }
}
