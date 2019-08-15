select DISTINCT 0 AS '#', --ROW_NUMBER() OVER(ORDER BY OcrCode2 ASC) AS '#',
a2.* from (
select T1.OcrCode2 from OPCH T0 with (nolock) inner join PCH1 T1 with (nolock) on t0.DocEntry= t1.DocEntry where T1.OcrCode2 is not null and T0.U_MQ_Rise = '{RiseId}' and t1.Quantity>0 and t0.CANCELED='N' union all
select T1.OcrCode2 from OWTR T0 with (nolock) inner join WTR1 T1 with (nolock) on t0.DocEntry= t1.DocEntry where T1.OcrCode2 is not null and T0.U_MQ_Rise = '{RiseId}' and t1.Quantity>0 and t0.CANCELED='N' union all
select T1.OcrCode2 from OWTQ T0 with (nolock) inner join WTQ1 T1 with (nolock) on t0.DocEntry= t1.DocEntry where T1.OcrCode2 is not null and T0.U_MQ_Rise = '{RiseId}' and t1.Quantity>0 union all
---- Anexa los AF relacionados
select T2.U_PrcCode from [@UG_TBL_MQ_RISE] T0 with (nolock) inner join [@UG_TBL_MQ_RISE] T1 with (nolock) on t0.U_DocRef = t1.U_IdRise inner join [@UG_MQ_RIFR] T2 with (nolock) on t1.U_IdRise= t2.U_IdRise where T0.U_IdRise = '{RiseId}'
) a
LEFT JOIN
(
	SELECT T0.[OcrCode] AS AFCode, t0.ocrname AS AFDesc, t1.itemname AS DescItem, ISNULL(t2.OcrCode,'') AreaCode
	FROM OOCR T0 with (nolock)
	INNER JOIN OITM T1 with (nolock) ON T0.OcrCode=T1.ITEMCODE
	left join (select A0.ItemCode, A0.OcrCode from ITM6 A0 with (nolock)
	inner join (select ItemCode,max(LineNum) LineNum from ITM6  with (nolock) group by ItemCode) A1 on a1.ItemCode=a0.ItemCode and A1.LineNum=a0.LineNum) T2 on t2.ItemCode= t1.ItemCode
) a2 on a.OcrCode2 = a2.AFCode
