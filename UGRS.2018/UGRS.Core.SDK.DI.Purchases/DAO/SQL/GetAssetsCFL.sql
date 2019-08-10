SELECT ROW_NUMBER() OVER(ORDER BY T0.[OcrCode] ASC) AS '#', T0.[OcrCode] AS AFCode, t0.ocrname AS AFDesc, t1.itemname AS DescItem, ISNULL(t2.OcrCode,'') AreaCode
FROM OOCR T0 with (nolock)
INNER JOIN OITM T1 with (nolock) ON T0.OcrCode=T1.ITEMCODE
left join (select A0.ItemCode, A0.OcrCode from ITM6 A0 with (nolock)
inner join (select ItemCode,max(LineNum) LineNum from ITM6  with (nolock) group by ItemCode) A1 on a1.ItemCode=a0.ItemCode and A1.LineNum=a0.LineNum) T2 on t2.ItemCode= t1.ItemCode