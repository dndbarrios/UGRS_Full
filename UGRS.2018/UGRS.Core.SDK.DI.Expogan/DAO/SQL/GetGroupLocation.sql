


select TG.Code, TG.Name from [@UG_EX_GRLOC] TG
Left join [@UG_EX_LOCAL] TL on TG.Code = TL.U_Group
--left join [@UG_EX_LOC_CONTRACT] TC on TL.Code = TC.U_LocalID
--left join ORDR  on ORDR.DocEntry = TC.U_DocEntryO  and ORDR.CANCELED = 'N' and ORDR.U_GLO_Status <> 'CN'
left join (
	select ORDR.DocEntry, TC.U_LocalID from OPRJ 
	join ORDR on ORDR.DocDate > OPRJ.ValidFrom  and ORDR.CANCELED = 'N' and ORDR.U_GLO_Status <> 'CN'
	join [@UG_EX_LOC_CONTRACT] TC on TC.U_Status = '0' and ORDR.DocEntry = TC.U_DocEntryO
	where U_GR_Type = 'EX' and ValidFrom <= GETDATE() and U_EX_DateI >= GETDATE()
) PJ on PJ.U_LocalID = TL.Code

where  Pj.DocEntry is null and  TL.Code is not null
group by TG.Code, TG.Name



--select TG.Code, TG.Name from [@UG_EX_GRLOC] TG
--Left join [@UG_EX_LOCAL] TL on TG.Code = TL.U_Group
--left join [@UG_EX_LOC_CONTRACT] TC on TL.Code = TC.U_LocalID
--left join ORDR  on ORDR.DocEntry = TC.U_DocEntryO  and ORDR.CANCELED = 'N' and ORDR.U_GLO_Status <> 'CN'
--where TC.Code is null and ORDR.DocEntry is null and TL.Code is not null
--group by TG.Code, TG.Name