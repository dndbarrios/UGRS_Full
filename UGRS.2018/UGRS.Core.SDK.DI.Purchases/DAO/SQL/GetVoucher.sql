--select T0.Code, T0.U_Folio, T0.U_Status, U_Area, U_Employee, U_Date, U_Total, T1.DocTotal from [@UG_GLO_VOUC] T0 with (Nolock)
--left join OVPM T1 with (Nolock) on T1.U_GLO_CodeMov = T0.U_Folio and T0.U_Area = T1.U_GLO_CostCenter and DocType = 'A' and U_GLO_PaymentType = 'GLREM'
--where  T0.U_TypeVoucher = '0' --and  (T0.U_Total >  ISNULL(T1.DocTotal, 0 ) or (T0.U_Total = 0  and ISNULL(T1.DocTotal, 0 ) = 0))


select b0.Code, B0.U_Folio, B0.U_Status, B0.U_Area, B0.U_Employee, B0.U_Date, sum(isnull(dum.U_Total,0)) U_Total , max(isnull(B1.DocTotal,0)) DocTotal
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
       WHERE T0.U_Type = 'Nota' AND T1.U_TypeVoucher = '0'
       group by T1.Code, T1.U_Folio
   UNION ALL
    select T3.Code, T3.U_Folio,
       sum(T0.DocTotal-isnull(t6.PaidSum,0) - isnull(t7.PaidRecon,0)) as U_Total
       from OPCH T0 
       inner join OUSR U1 with (Nolock) on T0.UserSign = U1.USERID
       inner join [@UG_GLO_VOUC] T3 with (Nolock) on T3.U_Folio = t0.U_MQ_OrigenFol AND T3.U_TypeVoucher = '0'
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
where B0.U_TypeVoucher = '0'
--group by b0.Code, B0.U_Folio, B0.U_Status, B0.U_Area, B0.U_Employee, B0.U_Date
--order by Code
