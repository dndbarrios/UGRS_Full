 select T0.Price,t0.ItemCode from ITM1 T0 with (Nolock)
 inner join OPLN T1 with (Nolock) on T1.ListNum = T0.PriceList
 where t1.U_GLO_Location = '{WHS}' AND t0.ItemCode = '{ItemCode}'