
--select T0.U_GLO_CodeMov, DocEntry, DocNum, U_FZ_Auxiliar, CardName, U_GLO_CostCenter, DocDate, DocTotal, A1.GLFSV, A2.GLSSV, A3.U_Total, A3.U_Status
--from OVPM T0 
--left join (select sum(DocTotal) as GLFSV, U_GLO_CodeMov  from OVPM where U_GLO_PaymentType = 'GLFSV' and  DocType = 'A' and Canceled = 'N' group by U_GLO_CodeMov)A1
--on T0.U_GLO_CodeMov = A1.U_GLO_CodeMov
--left join(select sum(DocTotal) as GLSSV, U_GLO_CodeMov  from ORCT where U_GLO_PaymentType = 'GLSSV' and  DocType = 'A' and Canceled = 'N' group by U_GLO_CodeMov)A2
--on T0.U_GLO_CodeMov = A2.U_GLO_CodeMov
--left join(select U_CodeMov, sum(DE.U_Total)as U_Total, UC.U_Status from [@UG_GLO_VOUC] as UC left join [@UG_GLO_VODE] as DE on U_CodeVoucher = UC.Code and (DE.U_Status is null  or DE.U_Status != 'Cancelado')
--group by U_CodeMov, UC.U_Status)A3
--on T0.U_GLO_CodeMov = A3.U_CodeMov
--where DocType = 'A' and U_GLO_PaymentType = 'GLSOV' and Canceled = 'N' and T0.U_GLO_CodeMov is not null




/*
select T0.U_GLO_CodeMov,/* DocEntry, DocNum,*/ U_FZ_Auxiliar, CardName, U_GLO_CostCenter, /*DocDate*/ DocTotal, isnull(A1.GLFSV,0) + isnull(B1.GLFSV,0) as GLFSV, A2.GLSSV, A3.U_Total, A3.U_Status, MQ1.Debit, MQ1.Credit
from OVPM T0 with (Nolock)
left join (

	select Sum(A1.Debit)-Sum(A1.Credit) as GLFSV,A1.U_GLO_CodeMov from OJDT A0 with (Nolock)
	inner join JDT1 A1 with (Nolock) on A0.TransId = A1.TransId  
	inner join [@UG_GLO_VODE] A2 with (Nolock) on A2.U_DocEntry = A1.TransId and A2.U_Type = 'Nota' 
	inner join [@UG_GLO_VOUC] A3 with (Nolock) on A2.U_CodeVoucher = A3.Code and A3.U_TypeVoucher=0
	inner join [@UG_Config] A4 with (Nolock) on A4.Name = 'MQ_DEUDORESVIAT' and A1.Account = A4.U_Value
	full outer join OJDT A5 with (Nolock) on A5.StornoToTr = A0.TransId 
	where A0.AutoStorno='N' and A5.TransId is null
	group by A1.U_GLO_CodeMov
)A1
on T0.U_GLO_CodeMov = A1.U_GLO_CodeMov
left join (select sum(DocTotal) as GLFSV, U_GLO_CodeMov  from OVPM with (Nolock) where U_GLO_PaymentType = 'GLREG' and  DocType = 'A' and Canceled = 'N' group by U_GLO_CodeMov)B1
on T0.U_GLO_CodeMov = B1.U_GLO_CodeMov
left join(select sum(DocTotal) as GLSSV, U_GLO_CodeMov  from ORCT with (Nolock) where U_GLO_PaymentType = 'GLSSV' and  DocType = 'A' and Canceled = 'N' group by U_GLO_CodeMov)A2
on T0.U_GLO_CodeMov = A2.U_GLO_CodeMov


left join (select sum(Debit) as Debit, sum(Credit) as Credit, U_GLO_CodeMov from JDT1 (Nolock) where TransCode = 'MQCM' group by U_GLO_CodeMov)MQ1
on T0.U_GLO_CodeMov = MQ1.U_GLO_CodeMov

left join(
select U_CodeMov, UC.U_Total as U_Total, UC.U_Status from [@UG_GLO_VOUC]  as UC with (Nolock) left join [@UG_GLO_VODE] as DE on U_CodeVoucher = UC.Code and (DE.U_Status is null  or DE.U_Status = 'Cerrado'



)
group by U_CodeMov, UC.U_Total, UC.U_Status)A3
on T0.U_GLO_CodeMov = A3.U_CodeMov
where DocType = 'A' and U_GLO_PaymentType = 'GLSOV' and Canceled = 'N' and T0.U_GLO_CodeMov is not null
*/




select T0.U_GLO_CodeMov,/* DocEntry, DocNum,*/ U_FZ_Auxiliar, CardName, U_GLO_CostCenter, /*DocDate*/ DocTotal, isnull(A1.GLFSV,0) + isnull(B1.GLFSV,0) as GLFSV, A2.GLSSV, A3.U_Total, A3.U_Status, MQ1.Debit, MQ1.Credit
from OVPM T0 with (Nolock)
left join (

	select Sum(A1.Debit)-Sum(A1.Credit) as GLFSV,A1.U_GLO_CodeMov from OJDT A0 with (Nolock)
	inner join JDT1 A1 with (Nolock) on A0.TransId = A1.TransId  
	inner join [@UG_GLO_VODE] A2 with (Nolock) on A2.U_DocEntry = A1.TransId and A2.U_Type = 'Nota' 
	inner join [@UG_GLO_VOUC] A3 with (Nolock) on A2.U_CodeVoucher = A3.Code and A3.U_TypeVoucher=0
	inner join [@UG_Config] A4 with (Nolock) on A4.Name = 'MQ_DEUDORESVIAT' and A1.Account = A4.U_Value
	full outer join OJDT A5 with (Nolock) on A5.StornoToTr = A0.TransId 
	where A0.AutoStorno='N' and A5.TransId is null
	group by A1.U_GLO_CodeMov
)A1
on T0.U_GLO_CodeMov = A1.U_GLO_CodeMov
left join (select sum(DocTotal) as GLFSV, U_GLO_CodeMov  from OVPM with (Nolock) where U_GLO_PaymentType = 'GLREG' and  DocType = 'A' and Canceled = 'N' group by U_GLO_CodeMov)B1
on T0.U_GLO_CodeMov = B1.U_GLO_CodeMov
left join(select sum(DocTotal) as GLSSV, U_GLO_CodeMov  from ORCT with (Nolock) where U_GLO_PaymentType = 'GLSSV' and  DocType = 'A' and Canceled = 'N' group by U_GLO_CodeMov)A2
on T0.U_GLO_CodeMov = A2.U_GLO_CodeMov


left join (select sum(Debit) as Debit, sum(Credit) as Credit, U_GLO_CodeMov from JDT1 (Nolock) where TransCode = 'MQCM' group by U_GLO_CodeMov)MQ1
on T0.U_GLO_CodeMov = MQ1.U_GLO_CodeMov

left join(
--select U_CodeMov, UC.U_Total as U_Total, UC.U_Status from [@UG_GLO_VOUC]  as UC with (Nolock) left join [@UG_GLO_VODE] as DE on U_CodeVoucher = UC.Code and (DE.U_Status is null  or DE.U_Status = 'Cerrado'

			select U_CodeMov,  sum(isnull(dum.U_Total,0)) U_Total , B0.U_Status  --b0.Code, B0.U_Folio, B0.U_Status, B0.U_Area, B0.U_Employee, B0.U_Date, sum(isnull(dum.U_Total,0)) U_Total , max(isnull(B1.DocTotal,0)) DocTotal
			from 
			[@UG_GLO_VOUC] B0
			left join
			(
			SELECT 
				T1.Code, T1.U_Folio,
				   sum(T3.Debit) as U_Total
			FROM  
				   [dbo].[@UG_GLO_VODE]  T0
				   INNER JOIN [@UG_GLO_VOUC] T1 with (Nolock) ON T0.U_CodeVoucher = T1.Code
				   INNER JOIN OJDT T2 with (Nolock) on (T1.U_Folio = T2.Ref1 or T1.U_CodeMov = T2.Ref1) and T0.U_DocEntry = T2.TransId
				   INNER JOIN JDT1 T3 with (Nolock) on T2.TransId = T3.TransId and T3.Debit > 0
				   WHERE T0.U_Type = 'Nota' AND T1.U_TypeVoucher = '1'
				   group by T1.Code, T1.U_Folio
			   UNION ALL
				select T3.Code, T3.U_Folio,
				   sum(T0.DocTotal-isnull(t6.PaidSum,0) - isnull(t7.PaidRecon,0)) as U_Total
				   from OPCH T0 
				   inner join OUSR U1 with (Nolock) on T0.UserSign = U1.USERID
				   inner join [@UG_GLO_VOUC] T3 with (Nolock) on T3.U_Folio = t0.U_MQ_OrigenFol AND T3.U_TypeVoucher = '1'
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
				   where T0.CANCELED != 'C' and isnull(T4.U_Status,'') <>'Cancelado'
				   group by T3.Code, T3.U_Folio
			) dum on dum.Code = B0.Code
			left join OVPM B1 with (Nolock) on B1.U_GLO_CodeMov = dum.U_Folio and B0.U_Area = B1.U_GLO_CostCenter and DocType = 'A' and U_GLO_PaymentType = 'GLREM' and b1.Canceled='N'
			where B0.U_TypeVoucher = '1'
			--group by U_CodeMov, B0.U_Status --b0.Code, B0.U_Folio, B0.U_Status, B0.U_Area, B0.U_Employee, B0.U_Date

group by U_CodeMov, B0.U_Total, B0.U_Status

--)A3
)A3
on T0.U_GLO_CodeMov = A3.U_CodeMov
where DocType = 'A' and U_GLO_PaymentType = 'GLSOV' and Canceled = 'N' and T0.U_GLO_CodeMov is not null



