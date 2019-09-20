select DocEntry, U_MQ_OrigenFol, U_MQ_OrigenFol_Det, CANCELED from OPCH where U_MQ_OrigenFol = '{VoucherCode}' and U_MQ_OrigenFol_Det = '{VoucherLine}' and CANCELED = 'N'

