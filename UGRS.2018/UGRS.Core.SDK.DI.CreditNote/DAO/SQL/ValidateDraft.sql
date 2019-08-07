
select ORIN.DocEntry, RIN21.RefDocEntr, RIN21.RefDocNum, ORIN.U_MQ_OrigenFol_Det from ORIN 
inner join RIN21 on ORIN.DocEntry = RIN21.DOcEntry
where U_MQ_OrigenFol =  '{NcId}' 