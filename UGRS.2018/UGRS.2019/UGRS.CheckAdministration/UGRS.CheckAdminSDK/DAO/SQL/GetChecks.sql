SELECT 
	OCHO.CheckNum,    
	T4.AcctName,                         
	OCHO.CheckAcct                          [CheckAc],    
	CHECKADMIN.U_Area                       [Area],
	CHECKADMIN.U_Status                     [Status]
FROM 
	[dbo].OCHO
	LEFT JOIN "@UG_CHECKADMIN" CHECKADMIN ON OCHO.CheckNum = CHECKADMIN.U_CheckNum
	INNER JOIN OACT T4 ON T4.AcctCode = OCHO.CheckAcct





