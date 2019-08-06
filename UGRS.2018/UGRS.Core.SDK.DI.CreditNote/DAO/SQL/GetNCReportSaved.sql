select 
	U_CardCode as 'C_CardCode'
	, U_CardName as 'C_CardName'
	, U_Cert as 'C_Cert'
	, U_DocEntryINV as 'C_DocEntry'
	, U_DocNumINV as 'C_DocNum'
	, U_QtyInv as 'C_InvHead'
	, U_QtyExp as 'C_HeadExp'
	, U_QtyNoCruz as 'C_HeadNoC'
	, U_Amount as 'C_Amount'
	, U_FolioFiscal as 'UUID'
	
  from [@UG_PE_NCDET]
  where U_NcId = '{NcId}'