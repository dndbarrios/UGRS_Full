SELECT DISTINCT T0.FieldID, T0.FldValue, T0.Descr FROM [UFD1] T0
INNER JOIN [CUFD] T1
	ON T0.FieldID = T1.FieldID 
WHERE T1.AliasID = 'MQ_TipoCont'