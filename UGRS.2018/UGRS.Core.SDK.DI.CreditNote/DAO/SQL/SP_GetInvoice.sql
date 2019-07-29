USE [UGRS_20190115]
GO
/****** Object:  StoredProcedure [dbo].[SP_Addon_CreditNote]    Script Date: 29-Jul-19 09:51:06 AM ******/
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
	SELECT * FROM (--HEMBRAS
SELECT 
		ROW_NUMBER() 
		OVER(ORDER BY T1.DocEntry )	as '#'
		,T1.CardCode					as 'C_CardCode'
		, T0.CardName					as 'C_CardName'
		, T0.U_PE_Certificate			as 'C_Cert'
		, T1.DocEntry					as 'C_DocEntry'
		, T1.DocNum						as 'C_DocNum'
		, T8.U_Quantity					as 'C_InvHead'
		, CASE WHEN SUM(isnull(t2.U_QtyUsed,0))=0 then  sum(isnull(t3.Cruzadas,0)) else sum(isnull(t2.U_QtyUsed,0)) end
										as 'C_HeadExp'
		, T8.U_Quantity- CASE WHEN SUM(isnull(t2.U_QtyUsed,0))=0 then  sum(isnull(t3.Cruzadas,0)) else sum(isnull(t2.U_QtyUsed,0)) END 
										as 'C_HeadNoC'
		, '0'							as 'C_Amount'
		--Amount
		, T0.DocDate
		, case when (CASE WHEN T9.U_IdSAGARPA =105 THEN T9.Name ELSE  T7.U_Value END) = 'Hembra' then   T8.U_Quantity else 0 END AutHembra
		, ISNULL(case when (CASE WHEN T9.U_IdSAGARPA =105 THEN T9.Name ELSE  T7.U_Value END) = 'Hembra' then case when SUM(isnull(t2.U_QtyUsed,0))=0 then  sum(isnull(t3.Cruzadas,0)) else sum(isnull(t2.U_QtyUsed,0)) END END,0) CruHembra
		, T3.Remark
		,'Hembra' 'U_Value'
FROM ORDR T0
INNER JOIN OINV T1 ON T0.DocEntry = T1.DocEntry
INNER JOIN [@UG_CU_CERT] T2 ON T2.Name = T0.U_PE_Certificate
LEFT JOIN (	SELECT S1.U_IDInsp, sum(isnull(s1.U_Quantity,0)) - sum(isnull(s1.U_QuantityNP,0)) - sum(isnull(s1.U_QuantityReject,0))  Cruzadas,  S2.Remark,S1.U_DateInsp 
			FROM [@UG_CU_OINS] S1 
			INNER JOIN  NNM1 S2 ON S1.U_Series = S2.Series 
			WHERE S1.U_IDInsp>0 
			GROUP BY S1.U_IDInsp, S2.Remark,S1.U_DateInsp  ) AS T3 ON T3.U_IDInsp = T2.U_IdInsp
LEFT JOIN [@UG_PE_PETY] T4 ON T0.U_PE_IdPermitType= T4.Code
left JOIN OCRD T5 ON T0.CardCode=T5.CardCode
left JOIN [@UG_PE_WS_PERE] T6 ON T0.U_PE_FolioUGRS = T6.U_UgrsFolio and t4.Code =t6.U_MobilizationTypeId
left join [@UG_PE_WS_PRRE] T8 on t6.U_RequestId = t8.U_RequestId
left JOIN [@UG_PE_WS_PARE] T7 ON T6.U_RequestId = T7.U_RequestId and t7.[U_ParameterId]=14 
LEFT JOIN [@UG_PE_PRTS] T9 ON T8.U_ParentProductId = T9.U_IdSAGARPA
WHERE T0.SERIES = 179 
and T4.Code=2 --Exportación
and isnull(t2.U_Quantity,0)>0
and t0.CANCELED='N'
and (t3.U_DateInsp between @DateFrom and @DateTo)
group by T1.CardCode, T0.CardName,T0.U_PE_Certificate,T9.U_IdSAGARPA, T9.Name,T7.U_Value,T8.U_Quantity,T3.Remark,T0.DocDate, T1.DocEntry, T0.DocEntry, T1.DocNum

--MACHOS
UNION ALL
SELECT  
		ROW_NUMBER() 
		OVER(ORDER BY T1.DocEntry )	as '#'
		,T1.CardCode					as 'C_CardCode'
		, T0.CardName					as 'C_CardName'
		, T0.U_PE_Certificate			as 'C_Cert'
		, T1.DocEntry					as 'C_DocEntry'
		, T1.DocNum						as 'C_DocNum'
		, T8.U_Quantity					as 'C_InvHead'
		, CASE WHEN SUM(isnull(t2.U_QtyUsed,0))=0 then  sum(isnull(t3.Cruzadas,0)) else sum(isnull(t2.U_QtyUsed,0)) end
										as 'C_HeadExp'
		, T8.U_Quantity- CASE WHEN SUM(isnull(t2.U_QtyUsed,0))=0 then  sum(isnull(t3.Cruzadas,0)) else sum(isnull(t2.U_QtyUsed,0)) END 
										as 'C_HeadNoC'
		, '0'							as 'C_Amount'
		, T0.DocDate 
		, case when (CASE WHEN T9.U_IdSAGARPA =105 THEN T9.Name ELSE  T7.U_Value END) = 'Macho' then  T8.U_Quantity else 0 END AutMacho
		, ISNULL(case when (CASE WHEN T9.U_IdSAGARPA =105 THEN T9.Name ELSE  T7.U_Value END) = 'Macho' then case when SUM(isnull(t2.U_QtyUsed,0))=0 then  sum(isnull(t3.Cruzadas,0)) else sum(isnull(t2.U_QtyUsed,0)) END END,0)  CruMacho
		, T3.Remark
		,'Macho' 'U_Value'
FROM ORDR T0
INNER JOIN OINV T1 ON T0.DocEntry = T1.DocEntry
INNER JOIN [@UG_CU_CERT] T2 ON T2.Name = T0.U_PE_Certificate
LEFT JOIN (	SELECT S1.U_IDInsp, sum(isnull(s1.U_Quantity,0)) - sum(isnull(s1.U_QuantityNP,0)) - sum(isnull(s1.U_QuantityReject,0))  Cruzadas,  S2.Remark,S1.U_DateInsp 
			FROM [@UG_CU_OINS] S1 
			INNER JOIN  NNM1 S2 ON S1.U_Series = S2.Series 
			WHERE S1.U_IDInsp>0 
			GROUP BY S1.U_IDInsp, S2.Remark,S1.U_DateInsp  ) AS T3 ON T3.U_IDInsp = T2.U_IdInsp
LEFT JOIN [@UG_PE_PETY] T4 ON T0.U_PE_IdPermitType= T4.Code
left JOIN OCRD T5 ON T0.CardCode=T5.CardCode
left JOIN [@UG_PE_WS_PERE] T6 ON T0.U_PE_FolioUGRS = T6.U_UgrsFolio and t4.Code =t6.U_MobilizationTypeId
left join [@UG_PE_WS_PRRE] T8 on t6.U_RequestId = t8.U_RequestId
left JOIN [@UG_PE_WS_PARE] T7 ON T6.U_RequestId = T7.U_RequestId and t7.[U_ParameterId]=14 
LEFT JOIN [@UG_PE_PRTS] T9 ON T8.U_ParentProductId = T9.U_IdSAGARPA
WHERE T0.SERIES = 179 
and T4.Code=2 --Exportación
and isnull(t2.U_Quantity,0)>0
and t0.CANCELED='N'
and (t3.U_DateInsp between @DateFrom and @DateTo)
group by T1.CardCode, T0.CardName,T0.U_PE_Certificate,T9.U_IdSAGARPA, T9.Name,T7.U_Value,T8.U_Quantity,T3.Remark,T0.DocDate, T1.DocEntry, T0.DocEntry, T1.DocNum

--EQUINOS

UNION ALL

SELECT ROW_NUMBER() 
		OVER(ORDER BY T1.DocEntry )	as '#'
		, T1.CardCode					as 'C_CardCode'
		, T0.CardName					as 'C_CardName'
		, T0.U_PE_Certificate			as 'C_Cert'
		, T1.DocEntry					as 'C_DocEntry'
		, T1.DocNum						as 'C_DocNum'
		, T8.U_Quantity					as 'C_InvHead'
		, CASE WHEN SUM(isnull(t2.U_QtyUsed,0))=0 then  sum(isnull(t3.Cruzadas,0)) else sum(isnull(t2.U_QtyUsed,0)) end CruTotal, T8.U_Quantity- CASE WHEN SUM(isnull(t2.U_QtyUsed,0))=0 then  sum(isnull(t3.Cruzadas,0)) else sum(isnull(t2.U_QtyUsed,0)) END 
										as 'C_HeadNoC'
		, '0'							as 'C_Amount'
		, T0.DocDate
		, case when (CASE WHEN T9.U_IdSAGARPA =105 THEN T9.Name ELSE  T7.U_Value END) = 'Equino' then  T8.U_Quantity else 0 END AutEquino
		, ISNULL(case when (CASE WHEN T9.U_IdSAGARPA =105 THEN T9.Name ELSE  T7.U_Value END) = 'Equino' then case when SUM(isnull(t2.U_QtyUsed,0))=0 then  sum(isnull(t3.Cruzadas,0)) else sum(isnull(t2.U_QtyUsed,0)) END END,0)  CruEquino
		, T3.Remark
		,'Equino' 'U_Value'
FROM ORDR T0
INNER JOIN OINV T1 ON T0.DocEntry = T1.DocEntry
INNER JOIN [@UG_CU_CERT] T2 ON T2.Name = T0.U_PE_Certificate
LEFT JOIN (	SELECT S1.U_IDInsp, sum(isnull(s1.U_Quantity,0)) - sum(isnull(s1.U_QuantityNP,0)) - sum(isnull(s1.U_QuantityReject,0))  Cruzadas,  S2.Remark,S1.U_DateInsp 
			FROM [@UG_CU_OINS] S1 
			INNER JOIN  NNM1 S2 ON S1.U_Series = S2.Series 
			WHERE S1.U_IDInsp>0 
			GROUP BY S1.U_IDInsp, S2.Remark,S1.U_DateInsp  ) AS T3 ON T3.U_IDInsp = T2.U_IdInsp
LEFT JOIN [@UG_PE_PETY] T4 ON T0.U_PE_IdPermitType= T4.Code
left JOIN OCRD T5 ON T0.CardCode=T5.CardCode
left JOIN [@UG_PE_WS_PERE] T6 ON T0.U_PE_FolioUGRS = T6.U_UgrsFolio and t4.Code =t6.U_MobilizationTypeId
left join [@UG_PE_WS_PRRE] T8 on t6.U_RequestId = t8.U_RequestId
left JOIN [@UG_PE_WS_PARE] T7 ON T6.U_RequestId = T7.U_RequestId and t7.[U_ParameterId]=14 
LEFT JOIN [@UG_PE_PRTS] T9 ON T8.U_ParentProductId = T9.U_IdSAGARPA
WHERE T0.SERIES = 179 
and T4.Code=2 --Exportación
and isnull(t2.U_Quantity,0)>0
and t0.CANCELED='N'
and (t3.U_DateInsp between @DateFrom and @DateTo)
group by T1.CardCode, T0.CardName,T0.U_PE_Certificate,T9.U_IdSAGARPA, T9.Name,T7.U_Value,T8.U_Quantity,T3.Remark,T0.DocDate, T1.DocEntry, T0.DocEntry, T1.DocNum
) T23

WHERE T23.AutHembra > 0 --AND

--T23.Cliente like CASE WHEN ('{?CodCte1@ select 'TODOS' union all SELECT  CardName FROM OCRD WHERE CardType = 'C'}' IS NULL OR '{?CodCte1@ select 'TODOS' union all SELECT  CardName FROM OCRD WHERE CardType = 'C'}'= '' OR '{?CodCte1@ select 'TODOS' union all SELECT  CardName FROM OCRD WHERE CardType = 'C'}'='TODOS') THEN T23.Cliente  ELSE  '{?CodCte1@ select 'TODOS' union all SELECT  CardName FROM OCRD WHERE CardType = 'C'}' END



ORDER BY T23.C_Cert ASC


END
