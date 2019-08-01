using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.CreditNote;
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
                Canceled = "N",
                CardCode = mDtMatrix.GetValue("C_CardCode", pIntRow).ToString(),
                CardName = mDtMatrix.GetValue("C_CardName", pIntRow).ToString(),
                Cert = mDtMatrix.GetValue("C_Cert", pIntRow).ToString(),
                CreationDate = DateTime.Now,
                CreationTime = DateTime.Now.ToString("hhmm"),
                DocEntryINV = Convert.ToInt32(mDtMatrix.GetValue("C_DocEntry", pIntRow)),
                DocNumINV = mDtMatrix.GetValue("C_DocNum", pIntRow).ToString(),
                Line = pIntRow,
                NcId = pStrId,
                Processed = "N",
                QtyExp = Convert.ToInt32(mDtMatrix.GetValue("C_HeadExp", pIntRow).ToString()),
                QtyNoCruz = Convert.ToInt32(mDtMatrix.GetValue("C_HeadNoC", pIntRow).ToString()),
                QtyInv = Convert.ToInt32(mDtMatrix.GetValue("C_InvHead", pIntRow).ToString()),
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
                List<CreditNoteDet> lLstCreditNoteByCardcode = pLstCreditNoteDet.Where(y => y.CardCode == lObjCardCode.CardCode && y.Canceled == "N").ToList();
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
                    Processed = "N",
                    Canceled = "N",
                    QtyInv = lLstCreditNoteByCardcode.Count(),
                    User = DIApplication.Company.UserName
                };
                lLstCreditNoteDocuments.Add(lObjCreditNoteDoc);
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
                Autorized = "N",
                Canceled = "N",
                Processed = "N",
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
        
    }
}
