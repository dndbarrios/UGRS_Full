using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.CreditNote;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;

namespace UGRS.AddOn.CreditNote.Services
{
    public class SaveNC_UDT
    {
        CreditNoteFactory mObjCreditNoteFactory = new CreditNoteFactory();
        /// <summary>
        /// Guardado en tablas
        /// </summary>
        /// 
        public void SaveInUDT(CreditNoteT pObjCreditNoteT)
        {
            DIApplication.Company.StartTransaction();
            int lIntResult = 0;
            try
            {
                //Guardado de encabezado UDT
                lIntResult = SaveTInUDT(pObjCreditNoteT);
                if (lIntResult == 0)
                {
                    //Guardado de documento UDT
                    lIntResult = SaveDocInUDT(pObjCreditNoteT.LstCreditNoteDoc);
                    if (lIntResult == 0)
                    {
                        foreach (var lObjDet in pObjCreditNoteT.LstCreditNoteDoc)
                        {
                            //Guardado de detalle UDT
                            lIntResult = SaveDetInUDT(lObjDet.LstCreditNoteDet);
                            if (lIntResult != 0)
                            {
                                break;
                            }
                        }
                    }
                }
                if (lIntResult != 0)
                {
                    LogService.WriteError(DIApplication.Company.GetLastErrorDescription());
                }
            }
            catch (Exception ex)
            {
                lIntResult = -1;
                LogService.WriteError(ex);
            }
            finally
            {
                CloseTransaction(lIntResult == 0 ? true : false);
            }
        }


        /// <summary>
        /// Guarda encabezado en UDT
        /// </summary>
        public int SaveTInUDT(CreditNoteT pObjCreditNoteT)
        {
            int lIntResult = mObjCreditNoteFactory.GetCreditNoteTService().Add(pObjCreditNoteT);

            return lIntResult;
        }

        /// <summary>
        /// Guarda documento en UDT
        /// </summary>
        public int SaveDocInUDT(List<CreditNoteDoc> pLstCreditNoteDoc)
        {
            //Test
            int lIntResult = mObjCreditNoteFactory.GetCreditNoteDocService().Add(pLstCreditNoteDoc.First());

            //foreach (var lObjCreditNoteDoc in pLstCreditNoteDoc)
            //{
            //    mObjCreditNoteFactory.GetCreditNoteDocService().Add(pObjCreditNoteT.LstCreditNoteDoc);
            //}
            return lIntResult;

        }

        /// <summary>
        /// Guarda detalle en UDT
        /// </summary>
        public int SaveDetInUDT(List<CreditNoteDet> pLstCreditNoteDet)
        {
            int lIntResult = mObjCreditNoteFactory.GetCreditNoteDetService().Add(pLstCreditNoteDet.First());
            return lIntResult;
        }


        /// <summary>
        /// Cierra transaccion
        /// </summary>
        public bool CloseTransaction(bool pBolSuccess)
        {
            try
            {
                if (pBolSuccess)
                {
                    DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    pBolSuccess = true;
                    LogService.WriteSuccess("Generado correctamente");
                    UIApplication.ShowSuccess("Generado correctamente");
                    // MenuNewForm();
                }
                else
                {
                    if (DIApplication.Company.InTransaction)
                    {
                        pBolSuccess = false;
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    }
                }
            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message);
                LogService.WriteError("(CloseTransaction): " + ex.Message);
                LogService.WriteError(ex);
            }
            return pBolSuccess;
        }

        public void SaveCreditNoteDoc(List<CreditNoteDoc> pLstCreditNoteDoc)
        {
            foreach (CreditNoteDoc lObjCreditNoteDoc in pLstCreditNoteDoc)
            {
                mObjCreditNoteFactory.GetCN_DocService().CreateCreditNote(lObjCreditNoteDoc);
            }
        }
    }
}
