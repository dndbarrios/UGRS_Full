select T0.Code, T0.U_Folio, T0.U_Status, U_Area, U_Employee, U_Date, U_Total, T1.DocTotal from [@UG_GLO_VOUC] T0 with (Nolock)
left join OVPM T1 with (Nolock) on T1.U_GLO_CodeMov = T0.U_Folio and T0.U_Area = T1.U_GLO_CostCenter and DocType = 'A' and U_GLO_PaymentType = 'GLREM'
where  T0.U_TypeVoucher = '0' --and  (T0.U_Total >  ISNULL(T1.DocTotal, 0 ) or (T0.U_Total = 0  and ISNULL(T1.DocTotal, 0 ) = 0))

/*	--select * from OPCH where DocNum ='8471'

	select T0.Code, T0.U_Folio, T0.U_Status, T0.U_Area, T0.U_Employee, T0.U_Date,  OPCH.DocEntry  ,
	 sum( (case when OPCH.DocTotal  is null then 0 else OPCH.DocTotal  end) 
	- (case when S1.DocTotal is null then 0 else S1.DocTotal end) 
	- (case when S2.ReconSum is null then 0 else S2.ReconSum end)
	- (case when S3.DocTotal is null then 0 else S3.DocTotal end)
	--+ (case when S4.Total is null then 0 else S4.Total end)
	)
	--(case when OPCH.DocTotal  is null then 0 else OPCH.DocTotal  end) 
	--, (case when S1.DocTotal is null then 0 else S1.DocTotal end) as NC
	--, (case when S2.ReconSum is null then 0 else S2.ReconSum end) as Reconciliacines
	--, (case when S3.DocTotal is null then 0 else S3.DocTotal end) as Cancelado
	----, (case when S4.Total is null then 0 else S4.Total end) as Asiento

	-- as DocTotal--, VODE.U_DocEntry, OPCH.DocNum
	from  OPCH with(nolock)
	
	left join [@UG_GLO_VOUC] T0  with (Nolock) on OPCH.U_MQ_OrigenFol = T0.U_Folio
	
	 left join [@UG_GLO_VODE] VODE with (nolock) on VODE.U_CodeVoucher = T0.Code  and VODE.U_DocEntry = OPCH.DocEntry
 left join OVPM T1 with (Nolock) on T1.U_GLO_CodeMov = T0.U_Folio and T0.U_Area = T1.U_GLO_CostCenter and OPCH.DocType = 'A' and U_GLO_PaymentType = 'GLREM' 
	left join (

	-- Notas de credito
	SELECT  T0.DocEntry,  T2.DocTotal --T2.[ObjType], T2.[DocDate], T2.[CardCode], T2.[CardName],
	FROM PCH1  T1 with (Nolock)
	INNER JOIN OPCH T0 with (Nolock) ON T0.DocEntry = T1.DocEntry 
	INNER JOIN RPC1 T3 with (Nolock) ON T3.BaseRef = T0.DocNum 
	INNER JOIN ORPC T2 with (Nolock) ON T2.DocEntry = T3.DocEntry
	where T2.CANCELED = 'N'
	group by T0.DocEntry, T2.DocTotal
	) S1 on S1.DocEntry = OPCH.DocEntry

	left join 
	(
	-- Reconciliaciones
	select t1.SrcObjAbs, t1.ReconSum--, t1.ReconSumSC 
			from OITR T0 with (Nolock)
			inner join ITR1 T1 with (Nolock) on t1.ReconNum= t0.ReconNum
			where T0.IsSystem = 'N' and t0.Canceled='N' and t1.SrcObjTyp=18
			group by  t1.SrcObjAbs, t1.ReconSum
	) S2 on S2.SrcObjAbs = OPCH.DocEntry

	-- Canceladas
	left join 
	(
	select OPCH.DocEntry, OPCH.DocTotal
	from OPCH
	where OPCH.CANCELED = 'Y'
	) S3 on S3.DocEntry = OPCH.DocEntry
	--select * from  [@UG_GLO_VODE] where U_Type = 'Nota'

	where  T0.U_TypeVoucher = '0' and T0.U_Folio = 'CG_TR_TRANS_7'  --and  (T0.U_Total >  ISNULL(T1.DocTotal, 0 ) or (T0.U_Total = 0  and ISNULL(T1.DocTotal, 0 ) = 0)) 
	group by  T0.Code, T0.U_Folio, T0.U_Status, U_Area, U_Employee, T0.U_Date, OPCH.DocEntry, OPCH.DocTotal, S1.DocTotal, S2.ReconSum, S3.DocTotal--, S4.Total
	order by Code desc
	union all




	--Asiento
	
		select VOUC.Code, VOUC.U_Folio, VOUC.U_Status, VOUC.U_Area, VOUC.U_Employee, VOUC.U_Date
		
		from OJDT A0 with (Nolock)
		inner join JDT1 A1 with (Nolock) on A0.TransId = A1.TransId  
		inner join [@UG_GLO_VODE] VODE with (Nolock) on VODE.U_DocEntry = A1.TransId and Vode.U_Type = 'Nota' 
		inner join [@UG_GLO_VOUC] VOUC with (Nolock) on VODE.U_CodeVoucher = VOUC.Code and VOUC.U_TypeVoucher=0
		--inner join [@UG_Config] A4 with (Nolock) on A4.Name = 'MQ_DEUDORESVIAT' and A1.Account = A4.U_Value
	
		full outer join OJDT A5 with (Nolock) on A5.StornoToTr = A0.TransId 
		where A0.AutoStorno='N' and A5.TransId is null and A1.Debit > 0 and VOUC.U_Folio = 'CG_TR_TRANS_7'
		--group by A0.Ref1
	)
	--S4 on T0.U_Folio = S4.Ref1 and VODE.U_Type = 'Nota' and VODE.U_DocEntry = S4.TransId

		
		--select * from OPCH where docNum = '8464'
		
		--select * from  [@UG_GLO_VOUC] where U_Folio = 'CG_TR_TRANS_7'


	*/