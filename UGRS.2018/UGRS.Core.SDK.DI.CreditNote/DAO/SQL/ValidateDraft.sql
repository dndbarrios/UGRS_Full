
select ODRF.DocEntry, DRF21.RefDocEntr, DRF21.RefDocNum, ODRF.U_MQ_OrigenFol_Det from ODRF 
inner join DRF21 on ODRF.DocEntry = DRF21.DOcEntry
where U_MQ_OrigenFol =  '{NcId}' 