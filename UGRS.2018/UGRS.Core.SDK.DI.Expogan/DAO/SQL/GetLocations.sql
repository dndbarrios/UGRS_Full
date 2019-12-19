
select TL.Code, TL.Name, max(TL.U_Total) U_Total
from [@UG_EX_LOCAL] TL
--left join [@UG_EX_LOC_CONTRACT] TC on TL.Code = TC.U_LocalID and TC.U_Status = '0'
--left join ORDR on ORDR.DocEntry = TC.U_DocEntryO and (ORDR.CANCELED = 'N' and ORDR.U_GLO_Status <> 'CN')

left join (
	select ORDR.DocEntry, TC.U_LocalID from OPRJ 
	join ORDR on ORDR.DocDate > OPRJ.ValidFrom  and ORDR.CANCELED = 'N' and ORDR.U_GLO_Status <> 'CN'
	join [@UG_EX_LOC_CONTRACT] TC on TC.U_Status = '0' and ORDR.DocEntry = TC.U_DocEntryO
	where U_GR_Type = 'EX' and ValidFrom <= GETDATE() and U_EX_DateI >= GETDATE()
) PJ on PJ.U_LocalID = TL.Code

where 
U_Group = '{Group}' and PJ.DocEntry is null --and TC.Code is null 
group by TL.Code, TL.Name
