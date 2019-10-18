
declare @Area nvarchar(30), @Folio nvarchar(30)
set @Area= '{Area}'
set @Folio='{Folio}'


select * from (SELECT 
       T0.U_ISR,
       T0.U_IVA,
       T0.U_RetIVA,
       T3.Debit as U_Subtotal,
       T3.Debit as U_Total,
       T0.U_IEPS,
       CASE WHEN T0.U_Type = 'XML' THEN '' ELSE T0.U_DocNum END   [DocNum],
       T3.LineMemo as U_Provider,
       T0.U_Status,
       T0.U_Coment,
       T3.U_GLO_Coments as U_Coments,
       T0.U_UserCode,
       T0.U_Date,
       T0.U_Type,
       T0.U_NA,
       T0.U_CodeVoucher,
       T0.Code,
       CASE WHEN T0.U_Type = 'XML' THEN '' ELSE T0.U_DocEntry END   [DocEntry],
       T0.U_Line,
       T3.Ref2 as DocFolio
FROM  
       [dbo].[@UG_GLO_VODE]  T0
       INNER JOIN [@UG_GLO_VOUC]  T1 with (Nolock) ON T0.U_CodeVoucher = T1.Code and T1.U_Folio =@Folio
       INNER JOIN OJDT T2 with (Nolock) on (T1.U_Folio = T2.Ref1 or T1.U_CodeMov = T2.Ref1) and T0.U_DocEntry = T2.TransId
       INNER JOIN JDT1 T3 with (Nolock) on T2.TransId = T3.TransId and T3.Debit > 0
       WHERE T1.U_Area = @Area and T0.U_Type = 'Nota' -- AND T2.DocStatus = 'O'
union all
select 
       ISNULL((select sum(WTAmnt) as WTAmnt from PCH5 where T0.DocEntry = AbsEntry) ,0) as U_ISR,
       T0.VatSum as U_IVA,
       T4.U_RetIVA,
       --T0.BaseAmnt as U_Subtotal,
       sum(T5.LineTotal) as U_Subtotal,
       max(T0.DocTotal-isnull(t6.PaidSum,0) - isnull(t7.PaidRecon,0)) as U_Total, 
       T4.U_IEPS as IEPS,
       T0.DocNum,
       T0.CardCode as U_Provider,
       T4.U_Status as U_Status,
       T4.U_Coment as U_Coment,
       T0.Comments as U_Coments,
       U1.USER_CODE as U_UserCode,
       T0.DocDate as U_UDate,
       'XML' as U_Type,
       T4.U_NA as U_NA,
       T4.U_CodeVoucher as U_CodeVoucher,
       T4.Code  as Code,
       T0.DocEntry as DocEntry,
       T4.U_Line,
       T0.NumAtCard as DocFolio
       from OPCH T0 
       inner join PCH1 T5 with (Nolock) on T5.DocEntry = T0.DocEntry
       inner join OUSR U1 with (Nolock) on T0.UserSign = U1.USERID
       left join [@UG_GLO_VOUC] T3 with (Nolock) on T3.U_Folio = @Folio
       left join [@UG_GLO_VODE] T4 with (Nolock) on T4.U_CodeVoucher = T3.Code and T0.U_MQ_OrigenFol_Det = T4.U_Line and U_Type = 'XML'
     left join (
       --- NOTAS DE CREDITO
                        select max(t0.doctotal) PaidSum, t1.baseentry DocEntry from ORPC T0
                        left join RPC1 T1 on t0.docentry=t1.DocEntry and t1.BaseType =18 
                        where t0.CANCELED='N' and t1.baseentry is not null group by t1.baseentry) T6 on T6.DocEntry = t0.DocEntry
       --- RECONCILIACIONES
       left join (SELECT t1.SrcObjAbs DocEntry, Sum(t1.ReconSum) PaidRecon FROM OITR T0
                           inner join ITR1 T1 on T0.ReconNum= t1.ReconNum and t1.SrcObjTyp=18
                           where t0.IsSystem='N' group by t1.SrcObjAbs) T7 on t7.DocEntry = t0.DocEntry
       where  T3.U_Folio = T0.U_MQ_OrigenFol and T0.CANCELED != 'C' and T3.U_Area = @Area
       group by T0.VatSum,T0.NumAtCard, T4.U_RetIVA, T0.BaseAmnt, T0.DocTotal,  T0.PaidToDate, T4.U_IEPS , T0.DocNum, T0.CardCode, T4.U_Status,
       T4.U_Coment, T0.Comments, U1.USER_CODE, T0.DocDate, T4.U_Type, T4.U_NA, T4.U_CodeVoucher, T4.Code, T0.DocEntry, T4.U_Line
       ) dum
       order by Code
       




/*select * from (SELECT 
	T0.U_ISR,
	T0.U_IVA,
	T0.U_RetIVA,
	T3.Debit as U_Subtotal,
	T3.Debit as U_Total,
	T0.U_IEPS,
	CASE WHEN T0.U_Type = 'XML' THEN '' ELSE T0.U_DocNum END   [DocNum],
	T3.LineMemo as U_Provider,
	T0.U_Status,
	T0.U_Coment,
	T3.U_GLO_Coments as U_Coments,
	T0.U_UserCode,
	T0.U_Date,
	T0.U_Type,
	T0.U_NA,
	T0.U_CodeVoucher,
	T0.Code,
	CASE WHEN T0.U_Type = 'XML' THEN '' ELSE T0.U_DocEntry END   [DocEntry],
	T0.U_Line,
	T3.Ref2 as DocFolio
FROM  
	[dbo].[@UG_GLO_VODE]  T0
	INNER JOIN [@UG_GLO_VOUC]  T1 with (Nolock) ON T0.U_CodeVoucher = T1.Code and T1.U_Folio = '{Folio}'
	INNER JOIN OJDT T2 with (Nolock) on (T1.U_Folio = T2.Ref1 or T1.U_CodeMov = T2.Ref1) and T0.U_DocEntry = T2.TransId
	INNER JOIN JDT1 T3 with (Nolock) on T2.TransId = T3.TransId and T3.Debit > 0
	WHERE T1.U_Area = '{Area}' and T0.U_Type = 'Nota' -- AND T2.DocStatus = 'O'
union all
select 
	ISNULL((select sum(WTAmnt) as WTAmnt from PCH5 where T0.DocEntry = AbsEntry) ,0) as U_ISR,
	T0.VatSum as U_IVA,
	T4.U_RetIVA,
	--T0.BaseAmnt as U_Subtotal,
	sum(T5.LineTotal) as U_Subtotal,
	(T0.DocTotal) as U_Total, 
	T4.U_IEPS as IEPS,
	T0.DocNum,
	T0.CardCode as U_Provider,
	T4.U_Status as U_Status,
	T4.U_Coment as U_Coment,
	T0.Comments as U_Coments,
	U1.USER_CODE as U_UserCode,
	T0.DocDate as U_UDate,
	'XML' as U_Type,
	T4.U_NA as U_NA,
	T4.U_CodeVoucher as U_CodeVoucher,
	T4.Code  as Code,
	T0.DocEntry as DocEntry,
	T4.U_Line,
	T0.NumAtCard as DocFolio
	from OPCH T0 
	inner join PCH1 T5 with (Nolock) on T5.DocEntry = T0.DocEntry
	inner join OUSR U1 with (Nolock) on T0.UserSign = U1.USERID
	 left join [@UG_GLO_VOUC] T3 with (Nolock) on T3.U_Folio = '{Folio}'
	 left join [@UG_GLO_VODE] T4 with (Nolock) on T4.U_CodeVoucher = T3.Code and T0.U_MQ_OrigenFol_Det = T4.U_Line and U_Type = 'XML'
	where  T3.U_Folio = T0.U_MQ_OrigenFol and T0.CANCELED != 'C' and T3.U_Area = '{Area}'
	group by T0.VatSum,T0.NumAtCard, T4.U_RetIVA, T0.BaseAmnt, T0.DocTotal,  T0.PaidToDate, T4.U_IEPS , T0.DocNum, T0.CardCode, T4.U_Status, T4.U_Coment, T0.Comments, U1.USER_CODE, T0.DocDate, T4.U_Type, T4.U_NA, T4.U_CodeVoucher, T4.Code, T0.DocEntry, T4.U_Line
	) dum
	order by Code
	*/