﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.CreditNote;
using UGRS.Core.SDK.DI.CreditNote.DTO;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.Core.Services;

namespace UGRS.AddOn.CreditNote.Services
{
    public class SaveNC_UDT
    {
        CreditNoteFactory mObjCreditNoteFactory = new CreditNoteFactory();
        ProgressBarManager mObjProgressBar = null;
        /// <summary>
        /// Guardado en tablas
        /// </summary>
        /// 
        public int SaveInUDT(CreditNoteT pObjCreditNoteT)
        {
            DIApplication.Company.StartTransaction();
            int lIntResult = 0;
            try
            {
                //Guardado de encabezado UDT
                UIApplication.ShowMessage("Guardando Encabezado");
                lIntResult = SaveTInUDT(pObjCreditNoteT);
                if (lIntResult == 0)
                {
                    UIApplication.ShowMessage("Encabezado guardado correctamente");
                        //Guardado de documento UDT
                        lIntResult = SaveDocInUDT(pObjCreditNoteT.LstCreditNoteDoc);
                        if (lIntResult == 0)
                        {
                                //Guardado de detalle UDT
                            lIntResult = SaveDetInUDT(pObjCreditNoteT.LstCreditNoteDoc);
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
                if (mObjProgressBar != null)
                {
                    mObjProgressBar.Dispose();
                }
            }
            return lIntResult;
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
        /// Actualiza encabezado en UDT
        /// </summary>
        public int UpdateTinUDT(CreditNoteT pObjCreditNoteT)
        {
            int lIntResult = mObjCreditNoteFactory.GetCreditNoteTService().Update(pObjCreditNoteT);

            return lIntResult;
        }

        /// <summary>
        /// Guarda documento en UDT
        /// </summary>
        public int SaveDocInUDT(List<CreditNoteDoc> pLstCreditNoteDoc)
        {
            //Test
            //int lIntResult = mObjCreditNoteFactory.GetCreditNoteDocService().Add(pLstCreditNoteDoc.First());
           
            int lIntResult = -1;

            try
            {
                UIApplication.ShowMessage("Guardando Documentos en UDT");
                mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Guardando Documentos en UDT", pLstCreditNoteDoc.Count);
                foreach (var lObjCreditNoteDoc in pLstCreditNoteDoc)
                {
                    lIntResult = mObjCreditNoteFactory.GetCreditNoteDocService().Add(lObjCreditNoteDoc);
                    if (lIntResult != 0)
                    {
                        break;
                    }
                    mObjProgressBar.NextPosition();
                }
              
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex);
            }
            finally
            {
                if (mObjProgressBar != null)
                {
                    mObjProgressBar.Dispose();
                }
            }
           
            return lIntResult;

        }

        /// <summary>
        /// Guarda detalle en UDT
        /// </summary>
        public int SaveDetInUDT(List<CreditNoteDoc> pLstCreditNoteDoc)
        {
            int lIntResult = -1;
            try
            {
                mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Guadado detalle", pLstCreditNoteDoc.Count);
                UIApplication.ShowMessage("Guardando Detalles en UDT");
                foreach (var lObjDoc in pLstCreditNoteDoc)
                {
                    foreach (var lObjCreditNoteDet in lObjDoc.LstCreditNoteDet)
                    {
                        lIntResult = mObjCreditNoteFactory.GetCreditNoteDetService().Add(lObjCreditNoteDet);
                        if (lIntResult != 0)
                        {
                            break;
                        }
                    }
                    mObjProgressBar.NextPosition();
                }
            }
            catch (Exception ex)
            {
                
                LogService.WriteError(ex);
            }
            finally
            {
                if (mObjProgressBar != null)
                {
                    mObjProgressBar.Dispose();
                }
            }
            return lIntResult;
        }

        public List<string> SaveCreditNoteDoc(List<CreditNoteDoc> pLstCreditNoteDoc)
        {
            List<string> lLstError = new List<string>();
            try
            {
                mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Guardando Notas de credito", pLstCreditNoteDoc.Where(x => x.IsDocument == "N").Count());
                foreach (CreditNoteDoc lObjCreditNoteDoc in pLstCreditNoteDoc.Where(x => x.IsDocument == "N"))
                {
                    //UIApplication.ShowSuccess(string.Format("Guardando {0} de {1}", i, pLstCreditNoteDoc.Count));
                    lLstError.AddRange(mObjCreditNoteFactory.GetCreditNoteService().CreateCreditNoteDOC(lObjCreditNoteDoc));
                   
                    mObjProgressBar.NextPosition();
                }
            }
            catch (Exception ex)
            {
                lLstError.Add(ex.Message);
            }
            finally
            {
                if (mObjProgressBar != null)
                {
                    mObjProgressBar.Dispose();
                }
            }
            return lLstError;
        }


        public void UpdateDocRel(List<CreditNoteDoc> pLstCreditNoteDoc)
        {
            int i = 1;
            try
            {
                foreach (var lObjCN in pLstCreditNoteDoc.Where(x => x.IsDocRel == "N"))
                {
                    UIApplication.ShowSuccess(string.Format("Actualizado {0} de {1}", i, pLstCreditNoteDoc.Count));
                    LogService.WriteSuccess(string.Format("Actualizado {0} de {1}", i, pLstCreditNoteDoc.Count));
                    mObjCreditNoteFactory.GetCreditNoteService().UpdateDocRel(lObjCN);
                    i++;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            finally
            {

            }
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
                    LogService.WriteSuccess("Proceso terminado correctamente");
                    UIApplication.ShowSuccess("Proceso terminado correctamente");
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


        public List<string> ValidateCreditNoteRelation(List<CreditNoteReferenceDTO> pLstNCRefDTO, CreditNoteT pObjCreditNoteT)
        {
            List<string> lLstResult = new List<string>();

            foreach (var lObjDoc in pObjCreditNoteT.LstCreditNoteDoc)
            {
                int lIntError = 0;
                foreach (var lObjDet in lObjDoc.LstCreditNoteDet)
                {
                    int i = pLstNCRefDTO.Where(x => x.DocEntryNC == lObjDoc.DocEntry && x.RefDocEntr == lObjDet.DocEntryINV).Count();
                    if (i > 0)
                    {
                        lObjDet.IsProcessed = "Y";
                        mObjCreditNoteFactory.GetCreditNoteDetService().Update(lObjDet);
                    }
                    else
                    {
                        lLstResult.Add(string.Format("No se pudo relacionar el documento {0} con la nota de credito {1}, {2}", lObjDet.DocNumINV, lObjDoc.DocEntry, lObjDoc.FolioDoc));
                        lIntError++;
                    }
                }
                if (lIntError == 0)
                {
                    lObjDoc.IsDocRel = "Y";
                    mObjCreditNoteFactory.GetCreditNoteDocService().Update(lObjDoc);
                }
            }

            return lLstResult;
        }


        public List<string> UpdateDocument(CreditNoteT pObjCreditNoteT)
        {

            List<string> lLstErrors = new List<string>();
            try
            {
                int i = 1;
                foreach (CreditNoteDoc lObjDoc in pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDocument == "Y"))
                {
                    UIApplication.ShowWarning(string.Format("Actualizando nota de crédito {0} de {1}", i, pObjCreditNoteT.LstCreditNoteDoc.Count()));
                    int lIntResult = mObjCreditNoteFactory.GetCreditNoteService().UpdateDocument(Convert.ToInt32(lObjDoc.DocEntry));

                    if (lIntResult != 0)
                    {
                        string lStrError = string.Format("Fallo al actualizar nota de crédito {0} Error: {1}", lObjDoc.DocEntryDraft, DIApplication.Company.GetLastErrorDescription());
                        LogService.WriteError(lStrError);
                        lLstErrors.Add(lStrError);
                    }
                    else
                    {
                        lObjDoc.IsProcessed = "Y";
                        mObjCreditNoteFactory.GetCreditNoteDocService().Update(lObjDoc);
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                lLstErrors.Add(ex.Message);
                LogService.WriteError(ex);

            }
            return lLstErrors;
        }

        //public List<string> RemoveDraft(CreditNoteT pObjCreditNoteT)
        //{
        //    List<string> lLstErrors = new List<string>();
        //    try
        //    {
        //        int i = 0;
        //        foreach (var lObjDoc in pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDocument == "Y"))
        //        {
        //            UIApplication.ShowMessage(string.Format("Borrando preliminar {0} de {1}", i, pObjCreditNoteT.LstCreditNoteDoc.Count()));
        //            int lIntResult = mObjCreditNoteFactory.GetCreditNoteService().RemoveDraft(Convert.ToInt32(lObjDoc.DocEntryDraft));

        //            if (lIntResult != 0)
        //            {
        //                string lStrError = string.Format("Fallo a crear nota de crédito {0} Error: {1}", lObjDoc.DocEntryDraft, DIApplication.Company.GetLastErrorDescription());
        //                LogService.WriteError(lStrError);
        //                lLstErrors.Add(lStrError);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lLstErrors.Add(ex.Message);
        //        LogService.WriteError(ex);

        //    }
        //    return lLstErrors;
        //}


        public bool CancelReport(CreditNoteT pObjCreditNoteT)
        {
            bool lBolResult = true;
            try
            {
                DIApplication.Company.StartTransaction();

                foreach (CreditNoteDoc lObjDoc in pObjCreditNoteT.LstCreditNoteDoc)
                {
                    if (lBolResult && lObjDoc.IsDocument == "N")
                    {
                        foreach (CreditNoteDet lObjDet in lObjDoc.LstCreditNoteDet)
                        {
                            lObjDet.IsCanceled = "Y";
                            if (mObjCreditNoteFactory.GetCreditNoteDetService().Update(lObjDet) != 0)
                            {
                                lBolResult = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        lBolResult = false;
                        break;
                    }
                }

                if (lBolResult)
                {
                    pObjCreditNoteT.IsCanceled = "Y";
                    if (mObjCreditNoteFactory.GetCreditNoteTService().Update(pObjCreditNoteT) != 0)
                    {
                        lBolResult = false;
                    }
                }
            }
            catch (Exception ex)
            {
                lBolResult = false;
                LogService.WriteError(ex);
            }

           return CloseTransaction(lBolResult);
        }
       
    }
}
