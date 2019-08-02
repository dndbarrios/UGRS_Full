USE [UGRS_20190115]
GO
/****** Object:  StoredProcedure [dbo].[SP_Addon_CreditNote]    Script Date: 01-Aug-19 04:03:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_Addon_CreditNote]
	-- Add the parameters for the stored procedure here
		@DateFrom datetime,
		@DateTo datetime
		
AS
BEGIN


  select 
 B2.cardcode 'C_CardCode', B2.CardName 'C_CardName', b1.Certificado 'C_Cert', b1.DocEntry 'C_DocEntry', 
        b1.DocNum 'C_DocNum', b1.FolioFac 'C_FolioFac', b1.Quantity 'C_InvHead', b0.Total 'C_HeadExp', 
    b1.Quantity- b0.Total 'C_HeadNoC', (b1.Quantity- b0.Total) * b1.Price 'C_Amount'
   from 
  (select sum(isnull(t0.u_qtyused,0)) Total, T0.NAME Certificado, max(t0.U_UsedDate) UsedDate from [@UG_CU_CERT] T0 group by T0.NAME) B0
  left join (
		 select a0.U_PE_Certificate Certificado, SUM(a2.Quantity) Quantity, max(a2.Price) Price, 
		        a0.docentry DocEntry, max(A0.cardcode) cardcode, max(cast(A0.docentry as nvarchar(30))) FolioFac,
				A0.DocNum
		 from oinv A0 
		 inner join inv1 A2 on a2.DocEntry = a0.DocEntry
		 inner join (select distinct U_ItemPE from [@UG_CU_CUIT]) A1 on a1.U_ItemPE = a2.ItemCode
		 where isnull(A0.U_PE_Certificate,'')<>'' and a0.CANCELED='N'
		 group by a0.U_PE_Certificate,a0.docentry, A0.DocNum
		 union all
		 select U_Certificate Certif, U_Quantity Quantity, isnull(U_Price,0) Price, 
			   u_Docentry DocEntry, Name CardCode, 'Saldo Inicial' FolioFac,
			   '' DocNum
		 from [@UG_CU_SICERT] 
    ) B1 on B1.Certificado=B0.Certificado
	inner join OCRD B2 on b2.cardcode= b1.cardcode
	where b1.Quantity- b0.Total >0 and b0.UsedDate between @DateFrom and @DateTo


END
