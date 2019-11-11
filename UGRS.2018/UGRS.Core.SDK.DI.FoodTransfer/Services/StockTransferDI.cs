/*
Autor: LCC Abraham Saúl Sandoval Meneses
Description: Transfers For Food Plant Data Object
Date: 31/08/2018
Company: Qualisys
*/


using SAPbobsCOM;
using System;
using System.Linq;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.FoodTransfer.DAO;
using UGRS.Core.SDK.DI.FoodTransfer.DTO;
using UGRS.Core.Services;

namespace UGRS.Core.SDK.DI.FoodTransfer.Services {

    public class StockTransferDI {

        public static object padlock = new object();

        #region CreateDraft
        public static Result CreateDraft(RequestTransfer[] transferRequest, User user) {

            var result = new Result();
            string transitWhs = String.Empty;
            FoodTransferDAO transfersDAO = new FoodTransferDAO();

            try {

                var oStockTransferDraft = (Documents)DIApplication.Company.GetBusinessObject(BoObjectTypes.oDrafts); //ODRF

                var task = Task.Factory.StartNew(() => {
                    oStockTransferDraft.DocObjectCode = BoObjectTypes.oStockTransfer;
                    oStockTransferDraft.UserFields.Fields.Item("U_GLO_Status").Value = "O";
                    oStockTransferDraft.UserFields.Fields.Item("U_MQ_OrigenFol").Value = transferRequest[0].DocNum;
                    oStockTransferDraft.UserFields.Fields.Item("U_PL_WhsReq").Value = transferRequest[0].ToWhs;
                    oStockTransferDraft.Series = user.Series;
                    oStockTransferDraft.Comments = transferRequest[0].Observations;
                });

                foreach(var line in transferRequest) {

                    transitWhs = transfersDAO.GetAvailableTransitWarehouse(line.Item);
                    if(String.IsNullOrEmpty(transitWhs)) {
                        return new Result() { Success = false, Message = String.Format("Error: No hay almacenes de transito disponibles para el artículo: {0}", line.Item) };
                    }

                    oStockTransferDraft.Lines.ItemCode = line.Item;
                    oStockTransferDraft.Lines.BaseEntry = line.Folio;
                    oStockTransferDraft.Lines.BaseLine = line.LineNum;
                    oStockTransferDraft.Lines.WarehouseCode = transitWhs;
                    oStockTransferDraft.Lines.Quantity = line.Quantity;
                    oStockTransferDraft.Lines.BaseType = (int)BoObjectTypes.oInventoryTransferRequest;
                    oStockTransferDraft.Lines.CostingCode = user.Area;
                    oStockTransferDraft.Lines.Add();
                }

                task.Wait();
                if(oStockTransferDraft.Add() != 0) {

                    LogService.WriteError("TransferDI (Draft) " + DIApplication.Company.GetLastErrorDescription());
                    result.Success = false;
                    result.Message = "Error: " + DIApplication.Company.GetLastErrorDescription();
                }
                else {
                    result.DocEntry = int.Parse(DIApplication.Company.GetNewObjectKey());
                    result.Success = true;
                    //if(!UpdateTransferStatus(transferRequest[0].Folio, BoObjectTypes.oInventoryTransferRequest)) {
                    //    return new Result() { Success = false, Message = String.Format("Error al actualizar el estado del documento de solicitud de traslado con folio: {0}, {1} ", transferRequest[0].DocNum, DIApplication.Company.GetLastErrorDescription()) };
                    //}
                }
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    HandleException(e, "TransferDI(Draft)", ref result);
                    return true;
                });
            }
            catch(Exception ex) {
                HandleException(ex, "TransferDI(Draft)", ref result);
            }

            return result;
        }
        #endregion

        #region CreateTransfer
        public static Result CreateTransfer(DocumentTransfer transfer, bool switchWhs) {

            var result = new Result();
            var objectCode = (!switchWhs) ? BoObjectTypes.oStockTransfer : BoObjectTypes.oInventoryTransferRequest;
            LogService.WriteInfo(String.Format("Crear Transferencia Pendiente: {0}, {1}", transfer.Document.DocEntry, transfer.Document.Folio));

            try {

                var oStockTransfer = (StockTransfer)DIApplication.Company.GetBusinessObject(objectCode); //OWTR

                var task = Task.Factory.StartNew(() => {
                    oStockTransfer.ToWarehouse = (!switchWhs) ? transfer.Document.ToWhs : transfer.Document.FromWhs;
                    oStockTransfer.FromWarehouse = (!switchWhs) ? transfer.Document.FromWhs : transfer.Document.ToWhs;
                    oStockTransfer.UserFields.Fields.Item("U_GLO_Status").Value = "O";
                    oStockTransfer.UserFields.Fields.Item("U_MQ_OrigenFol").Value = transfer.Document.Folio;
                });

                var task2 = Task.Factory.StartNew(() => {
                    foreach(var line in transfer.Lines) {

                        LogService.WriteInfo(String.Format("Transfiriendo Artículo: {0}, {1}", line.Item, line.Quantity));
                        LogService.WriteInfo(String.Format("De almancen {0} a almacen {1}", line.ToWhs, oStockTransfer.ToWarehouse));

                        oStockTransfer.Lines.ItemCode = line.Item;
                        oStockTransfer.Lines.ItemDescription = line.Desc;
                        oStockTransfer.Lines.Quantity = line.Quantity;
                        oStockTransfer.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = line.Bags;
                        oStockTransfer.Lines.FromWarehouseCode = line.ToWhs;
                        oStockTransfer.Lines.WarehouseCode = oStockTransfer.ToWarehouse;

                        if(!Object.ReferenceEquals(transfer.Series, null)) {

                            var series = transfer.Series.Where(s => s.ItemCode == line.Item).AsParallel().ToList();
                            Parallel.For(0, series.Count - 1, row => {
                                oStockTransfer.Lines.SerialNumbers.Add();
                            });

                            SerialNumbers serialNumber;
                            Parallel.For(0, series.Count, i => {
                                lock(padlock) {
                                    oStockTransfer.Lines.SerialNumbers.SetCurrentLine(i);
                                    serialNumber = oStockTransfer.Lines.SerialNumbers;
                                }
                                serialNumber.InternalSerialNumber = series[i].Number;
                                serialNumber.SystemSerialNumber = series[i].SysNumber;
                                serialNumber.Quantity = series[i].Quantity;
                            });
                        }
                        oStockTransfer.Lines.Add();
                    }
                });

                Task.WaitAll(task, task2);

                if(oStockTransfer.Add() != 0) {

                    LogService.WriteError("TransferDI (Create Document) " + DIApplication.Company.GetLastErrorDescription());
                    result.Success = false;
                    result.Message = "Error: " + DIApplication.Company.GetLastErrorDescription();
                }
                else {

                    if(switchWhs) {
                        oStockTransfer.GetByKey(int.Parse(DIApplication.Company.GetNewObjectKey()));
                        result.DocEntry = oStockTransfer.DocNum;
                    }

                    if(!UpdateTransferStatus(transfer.Document.DocEntry, BoObjectTypes.oStockTransfer)) {
                        return new Result() { Success = false, Message = String.Format("Error al actualizar el estado del documento #: {0}, {1} ", transfer.Document.DocEntry, DIApplication.Company.GetLastErrorDescription()) };
                    }

                    result.Success = true;
                    result.Message = "El translado del almacenes se realizó con éxito";
                    LogService.WriteInfo(result.Message);
                }
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    HandleException(e, "TransferDI(Document)", ref result);
                    return true;
                });
            }
            catch(Exception ex) {
                HandleException(ex, "TransferDI(Document)", ref result);
            }
            return result;
        }

        #endregion

        #region UpdateTransferStatus
        private static bool UpdateTransferStatus(int docEntry, BoObjectTypes transferType) {

            var oTransitTransfer = (StockTransfer)DIApplication.Company.GetBusinessObject(transferType);
            oTransitTransfer.GetByKey(docEntry);
            oTransitTransfer.UserFields.Fields.Item("U_GLO_Status").Value = "C";

            if(oTransitTransfer.Update() != 0) {
                LogService.WriteError("TransferDI (Update TransferRequest) " + DIApplication.Company.GetLastErrorDescription());
                return false;
            }

            LogService.WriteInfo("TransferDI (Update TransferRequest): Se actualizo a cerrado el U_GLO_Status de la transferecia: " + docEntry);
            return true;
        }
        #endregion

        #region HandleException
        public static void HandleException(Exception ex, string section, ref Result result) {
            LogService.WriteError(String.Format("{0}: {1}", section, ex.Message));
            result.Success = false;
            result.Message = "Error: " + ex.Message;
        }
        #endregion
    }
}



