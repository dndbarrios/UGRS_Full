SELECT DISTINCT T1.Code, T1.U_Client, T1.U_CreatedDate, T1.U_DocRef, T1.U_DocStatus, T1.U_IdRise, T1.U_Supervisor, T1.U_OriginalFolio,
T1.U_UserId, CONCAT (T2.firstName, ' ', T2.lastName) AS ClientName, ISNULL(T1.U_HasStockTransfer, 'N') AS HasStockTransfer
FROM [@UG_TBL_MQ_RISE] T1
INNER JOIN OHEM T2
	ON T1.U_Supervisor = T2.empID
INNER JOIN (
	SELECT U_IdRise, SUM(ISNULL(U_F15W40, 0)) + SUM(ISNULL(U_Hidraulic, 0)) + SUM(ISNULL(U_SAE40, 0)) + SUM(ISNULL(U_Transmition, 0)) + SUM(ISNULL(U_Oils, 0)) AS Total
	FROM [@UG_MQ_RIFR] GROUP BY U_IdRise
)T3 ON T3.U_IdRise = T1.U_IdRise
WHERE T3.Total > 0 AND (T1.U_IdRise NOT IN(SELECT U_DocRef FROM [@UG_TBL_MQ_RISE] WHERE ISNULL(U_HasStockTransfer, 'N') = 'N') AND T1.U_DocStatus = 2 AND ISNULL(T1.U_HasStockTransfer, 'N') = 'N') 
	  AND T1.U_IdRise NOT IN (SELECT U_DocRef FROM [@UG_TBL_MQ_RISE] WHERE U_DocStatus = 1 AND U_DocRef <> 0)
/*SELECT DISTINCT T1.Code, T1.U_Client, T1.U_CreatedDate, T1.U_DocRef, T1.U_DocStatus, T1.U_IdRise, T1.U_Supervisor, T1.U_OriginalFolio,
T1.U_UserId, CONCAT (T2.firstName, ' ', T2.lastName) AS ClientName, ISNULL(T1.U_HasStockTransfer, 'N') AS HasStockTransfer
FROM [@UG_TBL_MQ_RISE] T1
INNER JOIN OHEM T2
	ON T1.U_Supervisor = T2.empID
WHERE (T1.U_IdRise NOT IN(SELECT U_DocRef FROM [@UG_TBL_MQ_RISE] WHERE ISNULL(U_HasStockTransfer, 'N') = 'N') AND T1.U_DocStatus = 2 AND ISNULL(T1.U_HasStockTransfer, 'N') = 'N') 
	   AND T1.U_IdRise NOT IN (SELECT U_DocRef FROM [@UG_TBL_MQ_RISE] WHERE U_DocStatus = 1 AND U_DocRef <> 0)*/