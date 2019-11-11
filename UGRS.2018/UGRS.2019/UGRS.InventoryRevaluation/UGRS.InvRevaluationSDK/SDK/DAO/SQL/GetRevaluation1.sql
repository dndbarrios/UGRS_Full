declare  @DocNumSMAjuste int
select @DocNumSMAjuste= '{DocNum}'

------------------------------------------------ACTUALIZA PRIMER REVALORIZACION-------------------------------------------------------------------
select 
    ROW_NUMBER() OVER(ORDER BY T0.ItemCode) [Row], 
	T0.Warehouse [Whs],
	T0.ItemCode [ItemCode],
	(((SUM(T0.TransValue) /SUM (ISNULL(T0.InQty,0) - ISNULL(T0.OutQty,0)) )-t5.CostAjuste)*t5.QtyAjuste) [Rev1],
	T3.DocDateSMAjuste  [DocDate],
	T3.Dscription  [Description]
from OINM T0
INNER join (select T2.ItemCode, T2.WhsCode, T1.DocDate DocDateSMAjuste , T2.Dscription
                 from OIGE T1
                 inner join IGE1 T2
                 ON T1.DocEntry = T2.DocEntry
                 AND T1.DocNum=@DocNumSMAjuste) T3
ON T0.ItemCode = T3.ItemCode
AND T0.Warehouse = T3.WhsCode
inner join (select T4.Warehouse,T4.ItemCode,T4.TransValue, (T4.InQty - T4.OutQty) QtyAjuste,T4.CalcPrice CostAjuste
                 from OIGE T1
                 inner join IGE1 T2
                 ON T1.DocEntry = T2.DocEntry
                 AND T1.DocNum=@DocNumSMAjuste
                 inner join OINM T4
                 ON CAST(T1.DocNum as varchar(22)) = T4.Ref1
                 and T1.ObjType = T4.TransType
                 AND T2.ItemCode = T4.ItemCode
                 AND T2.WhsCode = T4.Warehouse) T5
ON T0.ItemCode = T5.ItemCode
and t0.Warehouse = t5.Warehouse
where T0.DocDate <=t3.DocDateSMAjuste
AND not (T0.BASE_REF = @DocNumSMAjuste and t0.TransType = 60)
GROUP BY T0.Warehouse,T0.ItemCode,t5.QtyAjuste, t5.CostAjuste, T3.DocDateSMAjuste, T3.Dscription