EXEC [SP_GetFacturaTimbrar_ByID] 1, 12272

USE [UGRS_20190208A]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetFacturasTimbrar]    Script Date: 21/03/2019 08:54:50 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
alter PROCEDURE [dbo].[SP_GetFacturaTimbrar_ByID]
	-- Add the parameters for the stored procedure here
	--declare
	@IdSuc int = 4,
	@DocEntry int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	/*
		Modificado por: R. Jiménez el día: 01/marzo/2019
		Se agrega validación para que no retorne registros si la sucursal no existe en la tabla de parametrizaciones
		esto porque si la sucursal no existe en parametrizaciones trae muchos registros y esto agrega mucha carga al servicio
		y BD.
	*/
	IF( NOT EXISTS( SELECT 1 FROM ServicioFacturacion.dbo.Parametrizaciones WHERE IDSuc = @IdSuc ))
	BEGIN
		PRINT(CONCAT('La sucursal ',CAST(@IdSuc AS VARCHAR(10)), ' no se encuentra en la tabla de parametrizaciones.'));
		RETURN;
	END

    -- Insert statements for procedure here
SELECT Factura.Series,
       Factura.DocNum,
       Factura.DocEntry,
       Factura.DocType,
       Factura.Series,
       Factura.ObjType,
	  factura.DocDate,
       Serie.SeriesName,
	  Factura.CardName as NombreSN,
	  Emisor.TaxIdNum as RFCEmisor,
       isnull(Bitacora.MetodoPago,'') as MetodoPago,
       isnull(Bitacora.NombreMetodoPago,'') as NombreMetodoPago,
       isnull(Bitacora.CuentaCliente,'') as CuentaCliente,
       isnull(Bitacora.RFC,Factura.LicTradNum) as RFC,
       isnull(Bitacora.FolioFiscal,'') as FolioFiscal,
       isnull(Bitacora.CertEmisor,'') as CertEmisor,
       isnull(Bitacora.CertSAT,'') as CertSAT,
       isnull(Bitacora.SelloSAT,'') as SelloSAT,
       isnull(Bitacora.SelloCFD,'') as SelloCFD,
       isnull(Bitacora.FechaTimbrado,'') as FechaTimbrado,
       isnull(Bitacora.TimbreFiscalDigital,'') as TimbreFiscalDigital,
       isnull(Bitacora.CadenaOriginal,'') as CadenaOriginal,
       isnull(Bitacora.ArchivoXML,'') as ArchivoXML,
       isnull(Bitacora.ArchivoPDF,'') as ArchivoPDF,
       isnull(Bitacora.ImporteFactura,0) as ImporteFactura,
       isnull(Bitacora.XMLModificado,0) as XMlModificado,
       isnull(Bitacora.Timbrado,0) as Timbrado,
       isnull(Bitacora.XMLGenerado,0) as XMLGenerado,
       isnull(Bitacora.PDFGenerado,0) as PDFGenerado,
       isnull(Bitacora.EmailEnviado,0) as EmailEnviado,
       isnull(Bitacora.ArchivosAdjuntados,0) as ArchivosAdjuntados,
       isnull(Bitacora.XMLOriginalBorrado,0) as XMLOriginalBorrado,
       isnull(Bitacora.XmlActualizado,'') as XMLActualizado,
	  isnull(Bitacora.IdSucursal, @IdSuc) as IdSucursal,
	  ''                                as NumRegIdTrib
FROM OINV Factura
INNER JOIN oadm Emisor ON 1 = 1
     INNER JOIN NNM1 Serie ON Serie.Series = Factura.Series
     LEFT JOIN [ServicioFacturacion].[dbo].[FacturasCFDi] Bitacora ON Bitacora.DocEntry = Factura.DocEntry
                                                          AND CAST(Bitacora.CodigoObjeto AS INT) = CAST(Factura.ObjType AS INT) and Bitacora.IdSucursal = @IdSuc
WHERE (Factura.EDocGenTyp = 'G'
	  OR Factura.EDocGenTyp = 'L')
      AND Factura.CANCELED = 'N'
      AND Factura.DocDate BETWEEN CONVERT(DATETIME, (CONVERT(VARCHAR(10), GETDATE() - 3, 120)+' 00:00:00.000'), 120) AND CONVERT(DATETIME, (CONVERT(VARCHAR(10), GETDATE(), 120)+' 23:59:59.000'), 120)
	 --and Factura.DocDate >= CONVERT(DATETIME, '2018-01-14 00:00:00.000', 120)
      AND isnull(Bitacora.XMLOriginalBorrado, 0) = 0 
	  and NOT (ISNULL(Bitacora.Timbrado, 0) = 1 AND ISNULL(Bitacora.XMLGenerado, 0) = 1 AND ISNULL(Bitacora.PDFGenerado, 0) = 1 AND ISNULL(Bitacora.EmailEnviado, 0) = 0)
	  and isnull(Bitacora.IdSucursal, @IdSuc) = @Idsuc
	  --and Factura.U_CE_TipoOperacion = 'NA'
	  and factura.DocEntry = @DocEntry
END
