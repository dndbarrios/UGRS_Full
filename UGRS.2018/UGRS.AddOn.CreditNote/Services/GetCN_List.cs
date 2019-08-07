using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.CreditNote;
using UGRS.Core.SDK.DI.CreditNote.DTO;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.Core.Services;

namespace UGRS.AddOn.CreditNote.Services
{
    public class GetCN_List
    {
        CreditNoteFactory mObjCreditNoteFactory = new CreditNoteFactory();
         ProgressBarManager mObjProgressBar = null;
         SAPbouiCOM.DataTable mDtMatrix;
         string mStrDate = string.Empty;
       
        public GetCN_List(DataTable pDtMatrix, string pStrDate)
        {
            mDtMatrix = pDtMatrix;
            mStrDate = pStrDate;
        }

        /// <summary>
        /// Obtener los datos de la matriz
        /// </summary>
        public string GetId()
        {
            string lStrId = "";

            lStrId = string.Format("NC_{0}", mObjCreditNoteFactory.GetCreditNoteService().GetLastCode() + 1);
            return lStrId;
        }

        /// <summary>
        /// Obtener los datos de la matriz
        /// </summary>
        public List<CreditNoteDet> GetMatrixData(string pStrId)
        {
            List<CreditNoteDet> lLstCN = new List<CreditNoteDet>();
            try
            {
                if (mDtMatrix != null)
                {
                    mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Obtencion de registros", mDtMatrix.Rows.Count);
                    for (int i = 0; i < mDtMatrix.Rows.Count; i++)
                    {
                        lLstCN.Add(GetDTMatrixRow(i, pStrId));
                        mObjProgressBar.NextPosition();
                    }
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
            return lLstCN;
        }

        /// <summary>
        /// Obtener el renglon de la matriz
        /// </summary>
        public CreditNoteDet GetDTMatrixRow(int pIntRow, string pStrId)
        {
            CreditNoteDet lObjCN_DTO = new CreditNoteDet
            {
                Amount = float.Parse(mDtMatrix.GetValue("C_Amount", pIntRow).ToString()),
                IsCanceled = "N",
                CardCode = mDtMatrix.GetValue("C_CardCode", pIntRow).ToString(),
                CardName = mDtMatrix.GetValue("C_CardName", pIntRow).ToString(),
                Cert = mDtMatrix.GetValue("C_Cert", pIntRow).ToString(),
                CreationDate = DateTime.Now,
                CreationTime = DateTime.Now.ToString("hhmm"),
                DocEntryINV = Convert.ToInt32(mDtMatrix.GetValue("C_DocEntry", pIntRow)),
                DocNumINV = mDtMatrix.GetValue("C_DocNum", pIntRow).ToString(),
                Line = pIntRow,
                NcId = pStrId,
                IsProcessed = "N",
                QtyExp = Convert.ToInt32(mDtMatrix.GetValue("C_HeadExp", pIntRow).ToString()),
                QtyNoCruz = Convert.ToInt32(mDtMatrix.GetValue("C_HeadNoC", pIntRow).ToString()),
                QtyInv = Convert.ToInt32(mDtMatrix.GetValue("C_InvHead", pIntRow).ToString()),
                FolioFiscal = mDtMatrix.GetValue("UUID", pIntRow).ToString()
            };


            return lObjCN_DTO;
        }


        /// <summary>
        /// Obtener el docmento ordenado por cardcode
        /// </summary>
        public List<CreditNoteDoc> GetNC_Doc(string pStrId, List<CreditNoteDet> pLstCreditNoteDet)
        {
            List<CreditNoteDoc> lLstCreditNoteDocuments = new List<CreditNoteDoc>();
            List<CreditNoteDet> lLstByCardCode = pLstCreditNoteDet.GroupBy(x => x.CardCode).Select(y => new CreditNoteDet { CardCode = y.First().CardCode }).ToList();
            int i = 1;
            string lStrTaxCode = mObjCreditNoteFactory.GetCreditNoteService().GetTaxCode("0");
            foreach (var lObjCardCode in lLstByCardCode)
            {
                string lStrFolioDoc = string.Format("{0}_{1}", pStrId, i);
                List<CreditNoteDet> lLstCreditNoteByCardcode = pLstCreditNoteDet.Where(y => y.CardCode == lObjCardCode.CardCode && y.IsCanceled == "N").ToList();
                foreach (var item in lLstCreditNoteByCardcode)
                {
                    item.FolioDoc = lStrFolioDoc;
                }

                CreditNoteDoc lObjCreditNoteDoc = new CreditNoteDoc
                {
                    Amount = lLstCreditNoteByCardcode.Sum(x => x.Amount),
                    NcId = pStrId,
                    CardCode = lObjCardCode.CardCode,
                    CardName = lLstCreditNoteByCardcode.First().CardName,
                    CreationDate = DateTime.Now,
                    CreationTime = DateTime.Now.ToString("hhmm"),
                    DocEntry = "",
                    IVA = 0,
                    TaxCode = lStrTaxCode,
                    Line = i,
                    LstCreditNoteDet = lLstCreditNoteByCardcode,
                    IsCanceled = "N",
                    IsDraft = "N",
                    IsDocRel = "N",
                    IsDocument = "N",
                    IsProcessed = "N",
                    IsDelDraft = "N",
                    QtyInv = lLstCreditNoteByCardcode.Count(),
                    User = DIApplication.Company.UserName,
                    FolioDoc =  string.Format("{0}_{1}", pStrId, i)
                };
                lLstCreditNoteDocuments.Add(lObjCreditNoteDoc);
                i++;
            }

            return lLstCreditNoteDocuments;
        }

        /// <summary>
        /// Obtener el encabezado
        /// </summary>
        public CreditNoteT GetNC_Header(string pStrId, List<CreditNoteDoc> pLstCreditNoteDoc)
        {
            CreditNoteT lObjCreditNoteT = new CreditNoteT
            {
                NcId = pStrId,
                Date = string.IsNullOrEmpty(mStrDate) ? DateTime.Now :
                                DateTime.ParseExact(mStrDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                CreationDate = DateTime.Now,
                IsAutorized = "N",
                IsCanceled = "N",
                IsProcessed = "N",
                CreationTime = DateTime.Now.ToString("hhmm"),
                QtyDoc = pLstCreditNoteDoc.Count(),
                QtyInv = pLstCreditNoteDoc.Sum(x => x.LstCreditNoteDet.Count),
                Total = pLstCreditNoteDoc.Sum(x => x.Amount),
                User = DIApplication.Company.UserName,
                Attach = "",
                LstCreditNoteDoc = pLstCreditNoteDoc,
            };
            return lObjCreditNoteT;
        }

       
        public CreditNoteT GetCreditNoteTSaved(string pStrId)
        {
            CreditNoteT lObjCreditNoteT = mObjCreditNoteFactory.GetCreditNoteService().GetCreditNoteTSaved(pStrId);
            if (lObjCreditNoteT != null)
            {
                lObjCreditNoteT.LstCreditNoteDoc = new List<CreditNoteDoc>();
                lObjCreditNoteT.LstCreditNoteDoc = GetCreditNoteDocSaved(pStrId, GetCreditNoteDetSaved(pStrId));
            }
            return lObjCreditNoteT;
        }

        public List<CreditNoteDoc> GetCreditNoteDocSaved(string pStrId, List<CreditNoteDet> pLstDet)
        {
            List<CreditNoteDoc> lLstDoc = new List<CreditNoteDoc>();
            lLstDoc = mObjCreditNoteFactory.GetCreditNoteService().GetCreditNoteDocSaved(pStrId);

            foreach (CreditNoteDoc lObjDoc in lLstDoc)
            {
                lObjDoc.LstCreditNoteDet = new List<CreditNoteDet>();
                lObjDoc.LstCreditNoteDet.AddRange(pLstDet.Where(x => x.FolioDoc == lObjDoc.FolioDoc));
            }
            return lLstDoc;
        }

        public List<CreditNoteDet> GetCreditNoteDetSaved(string pStrId)
        {
            return mObjCreditNoteFactory.GetCreditNoteService().GetCreditNoteDetSaved(pStrId);
        }

        public List<CreditNoteReferenceDTO> GetDraftReference(string pStrNcId)
        {
            return mObjCreditNoteFactory.GetCreditNoteService().GetDraftReference(pStrNcId);
        }
    }
}
