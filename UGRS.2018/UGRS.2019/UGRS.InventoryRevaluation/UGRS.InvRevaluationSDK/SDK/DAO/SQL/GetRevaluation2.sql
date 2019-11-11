declare  @DocNumSMAjuste int
select @DocNumSMAjuste= '{DocNum}'

-------------------------------------------------------SEGUNDA REVALORIZACION----------------------------------------------------------------------------------

--MOVIMIENTOS REALIZADOS DESPUES DE LA FECHA DEL AJUSTE DE INVENTARIO POR MEDIO DE UNA SALIDA DE MERCANCIA
select t0.TransNum,T0.DocDate,T0.Warehouse,T0.ItemCode,T0.Dscription,T0.Ref1,T0.JrnlMemo, (ISNULL(T0.InQty,0) - ISNULL(T0.OutQty,0)) Qty,T0.CalcPrice, 
              T0.TransValue,ISNULL(T0.InQty,0) InQty,ISNULL(T0.OutQty,0) OutQty,t3.DocDateSMAjuste, ROW_NUMBER ( ) OVER ( ORDER BY t0.DocDate,t0.TransNum asc )  as Contador
into #HitorialMov
from OINM T0
INNER join (select T2.ItemCode, T2.WhsCode,t2.DocDate DocDateSMAjuste
                 from OIGE T1
                 inner join IGE1 T2
                 ON T1.DocEntry = T2.DocEntry
                 AND T1.DocNum=@DocNumSMAjuste) T3
ON T0.ItemCode = T3.ItemCode
AND T0.Warehouse = T3.WhsCode
where T0.DocDate>DocDateSMAjuste and T0.DocDate  <= GETDATE()
ORDER BY t0.DocDate,t0.TransNum ASC


--TABLA QUE GUARDA LA CANTIDAD, CANTIDAD ACUMULADA Y VALOR DE TRANSACCION A LA FECHA DE LA SALIDA DE MERCANCIA POR AJUSTE
select T0.Warehouse,T0.ItemCode,T3.DocDateSMAjuste, T3.Dscription,
              convert(decimal(12,4),SUM(T0.TransValue)) TotalValorTran, SUM (ISNULL(T0.InQty,0) - ISNULL(T0.OutQty,0)) CantTotal,
              ISNULL((SELECT (SUM(ISNULL(Y.InQty,0))-SUM(ISNULL(Y.OutQty,0)))FROM OINM Y WHERE Y.ItemCode = T0.ItemCode AND Y.Warehouse = T0.Warehouse  AND Y.DocDate<=T3.DocDateSMAjuste),0) CantAcum,
                       CASE WHEN SUM (ISNULL(T0.InQty,0) - ISNULL(T0.OutQty,0)) =0 THEN 0 ELSE convert(decimal(12,4),SUM(T0.TransValue)) /SUM (ISNULL(T0.InQty,0) - ISNULL(T0.OutQty,0)) END CostoCalculado,
              T1.AvgPrice,convert(decimal(12,4),0) ImpRev1, convert(decimal(12,4),0) CostoAjuste, convert(decimal(12,4),0) ImpRev22
into #SumQtyValoTrans
from OINM T0
INNER join (select T2.ItemCode, T2.WhsCode,t2.DocDate DocDateSMAjuste, T2.Dscription
                 from OIGE T1
                 inner join IGE1 T2
                 ON T1.DocEntry = T2.DocEntry
                 AND T1.DocNum=@DocNumSMAjuste) T3
ON T0.ItemCode = T3.ItemCode
AND T0.Warehouse = T3.WhsCode
INNER JOIN OITW T1 ON T0.ItemCode = T1.ItemCode AND T0.Warehouse = T1.WhsCode 
where T0.DocDate <=t3.DocDateSMAjuste

GROUP BY T0.Warehouse,T0.ItemCode,T3.DocDateSMAjuste,T1.AvgPrice, T3.Dscription


DECLARE @count INT = (Select TOP 1 Contador From #HitorialMov order by Contador asc )

--RECORRE LOS MOVIMIENTOS QUE SE HICIERON DESPUES DE LA SALIDA DE MERCANCIA AL A FECHA DE EJECUCION DEL LA SEGUNFA REVALORIZACION
WHILE (@count is not null)
       BEGIN
       
                        UPDATE H
                SET H.CalcPrice = case when H.OutQty>0 AND  S.CantTotal>0 THEN (S.TotalValorTran/S.CantTotal) ELSE H.CalcPrice END,--,
                        H.TransValue=  case when H.OutQty>0 AND S.CantTotal>0 THEN h.Qty *(S.TotalValorTran/S.CantTotal) else H.TransValue END
                FROM #HitorialMov H
                INNER JOIN #SumQtyValoTrans S
                ON S.ItemCode = H.ItemCode
                AND S.Warehouse = H.Warehouse
                WHERE H.Contador = @count

                        --Actualizar la sumatoria de la cantidad y el valor de la transaccion a la tabbla de #SumQtyValoTrans
              UPDATE S
              SET S.CantTotal =convert(decimal(12,6),S.CantTotal + H.Qty),
                     S.TotalValorTran =convert(decimal(12,6),S.TotalValorTran + H.TransValue),
                     S.CantAcum = CantAcum +H.Qty,
                                  S.CostoCalculado= case when (convert(decimal(12,6),S.CantTotal + H.Qty))=0 then 0 else  convert(decimal(12,6),S.TotalValorTran + H.TransValue)/convert(decimal(12,6),S.CantTotal + H.Qty) end
              FROM #SumQtyValoTrans S
              INNER JOIN #HitorialMov H
              ON S.ItemCode = H.ItemCode
              AND S.Warehouse = H.Warehouse
              WHERE H.Contador = @count

              SET @count  = (Select TOP 1 Contador From #HitorialMov where Contador> @count order by Contador asc  )
       END
                 SELECT ROW_NUMBER() OVER(ORDER BY R1.ItemCode) [Row], 
                  GETDATE()   [DocDate], 
                  R1.ItemCode, 
                   R1.Warehouse [Whs], 
                  ((R1.CostoCalculado - R1.AvgPrice) * R1.CantAcum) [Rev1], 
                  R1.Dscription [Description]
                 FROM #SumQtyValoTrans  R1
				               
drop table #HitorialMov,#SumQtyValoTrans
