       declare  @DocNumSMAjuste int;
       declare  @DocDateSMAjuste as date;
       declare  @CamtDocRev1 int;
       declare  @CamtDocRev2 int;
       select @DocNumSMAjuste= '{DocNum}'
              
              declare @DocDate as date, @Cantidad as decimal(14,6), @Costo as decimal(14,6), @Valor as decimal(14,6), @DocType as int, @DocEntry as int;
              declare @ItemCode as nvarchar(10), @whCode as nvarchar(10), @DocEntrySal as int;
              Declare @CantidadAcum as decimal(14,6), @ValorAcum as decimal(14,6), @CostoV as decimal(14,6), @CostoVR1 as decimal(14,6), @Reg as int; 
			  Declare @TotRev1 as decimal(30,6), @TotRev2 as decimal(30,6);
              Declare @ValorAcum2 as decimal(14,6);
              Declare @ValorRev1 as decimal(14,6), @ValorRev2 as decimal(14,6); 

              select  @DocDateSMAjuste = (select  top 1 T0.DocDate from OIGE T0 where T0.DocNum= @DocNumSMAjuste)

              
              select @CamtDocRev1 = (select count(*) from OMRV t0 where  t0.U_DocNumSalida in (@DocNumSMAjuste) and t0.DocDate=@DocDateSMAjuste)
              select @CamtDocRev2 = (select count(*) from OMRV t0 where  t0.U_DocNumSalida in (@DocNumSMAjuste) and t0.DocDate>@DocDateSMAjuste)


                      IF OBJECT_ID('tempdb..#UGRStblTempCUR') is not null
                  drop table #UGRStblTempCUR
                 CREATE TABLE #UGRStblTempCUR(ItemCode nvarchar(15), whCode nvarchar(15), DocDateRev1 date, Rev1 decimal(14,6), CostoRev1 decimal(14,6), DocDateRev2 date, Rev2 decimal(14,6), CostoRev2 decimal(14,6)) 


              if (@CamtDocRev1=0 or @CamtDocRev2=0) begin

              DECLARE MovL CURSOR LOCAL FAST_FORWARD FOR 
                           select t0.DocEntry, t1.ItemCode, T1.WhsCode
                           from OIGE T0 inner join IGE1 T1 on t0.DocEntry= t1.DocEntry where T0.DocNum= @DocNumSMAjuste
              OPEN MovL
              FETCH NEXT FROM MovL INTO @DocEntrySal,@ItemCode,@whCode
              WHILE @@FETCH_STATUS = 0 
                     BEGIN

                        set @CantidadAcum=0; set @ValorAcum=0;set @Reg=0;

                             DECLARE MovLotes CURSOR LOCAL FAST_FORWARD FOR 
                                            select t0.DocDate, isnull(case when t0.InQty =0 then t0.OutQty * -1 else t0.InQty end,0) Cantidad,
                                                              t0.CalcPrice Costo,isnull(t0.TransValue,0) Valor,
                                                              T0.TransType, T0.CreatedBy
                                            from OINM T0 where T0.itemcode=@ItemCode and t0.Warehouse=@whCode and (t0.TransValue<> 0 or t0.OutQty <>0 or t0.InQty <>0) 
                                            and t0.TransNum not in (select t0.TransNum from OINM T0 where t0.TransType=162 and t0.CreatedBy in (select DocEntry from OMRV where U_DocNumSalida in (@DocNumSMAjuste)))
                                           -- and T0.CreatedBy=28945
                                                                          order by t0.DocDate, t0.TransNum asc
                             OPEN MovLotes
                             FETCH NEXT FROM MovLotes INTO @DocDate,@Cantidad,@Costo,@Valor,@DocType,@DocEntry
                             WHILE @@FETCH_STATUS = 0 
                                         BEGIN
                                          
                                                 IF @DocType=60 and @DocEntry=@DocEntrySal
                                                       -- ENCONTRO AL DOCUMENTO DE SALIDA
                                                       BEGIN
                                                         --  select @CantidadAcum,@ValorAcum
                                                         set @ValorRev1 = (@Valor/case when @Cantidad <> 0 then @Cantidad else 1 end - (@ValorAcum/case when @CantidadAcum <> 0 then @CantidadAcum else 1 end )) * (@Cantidad*-1);
                                                         set @ValorAcum +=@ValorRev1;
                                                         set @Reg=1
                                                         set @CantidadAcum += @Cantidad;
                                                         set @ValorAcum += @Valor;
                                                         set @ValorAcum2= @ValorAcum;
                                                         set @CostoV= @ValorAcum/case when @CantidadAcum <> 0 then @CantidadAcum else 1 end;
                                                                                              set @CostoVR1= @CostoV
                                                       END
                                                ELSE IF @Reg=1
                                                       BEGIN
                                                              set @CantidadAcum += @Cantidad;
                                                              set @ValorAcum += @Valor;
                                                              ----
                                                              set @ValorAcum2+= round(case when @Cantidad >0 then @Costo else @CostoV end * @Cantidad,6);
                                                              set @CostoV = round(@ValorAcum2/case when @CantidadAcum <> 0 then @CantidadAcum else 1 end,6);
                                            
                                                       END
                                                ElSE
                                                       BEGIN
                                                              -- insert into #UGRStblTempCUR (Cantidad , Valor) values (@Cantidad,@Valor)
                                                              set @CantidadAcum += @Cantidad;
                                                              set @ValorAcum += @Valor;
                                                       END
                           
                                         FETCH NEXT FROM MovLotes INTO @DocDate,@Cantidad,@Costo,@Valor,@DocType,@DocEntry
                                         END
                             CLOSE MovLotes
                             DEALLOCATE MovLotes
                             --select * from #UGRStblTempCUR
                             insert into #UGRStblTempCUR (ItemCode, whCode, DocDateRev1, Rev1,CostoRev1, DocDateRev2, Rev2, CostoRev2) 

                             select @ItemCode,@whCode,
                                                       @DocDateSMAjuste, case when @CamtDocRev1>0 then 0 else @ValorRev1 end,  @CostoVR1,
                                                            GETDATE(), case when @CamtDocRev2>0 then 0 else (@CostoV - @ValorAcum/case when @CantidadAcum <> 0 then @CantidadAcum else 1 end) *@CantidadAcum end Rev2, @CostoV

  FETCH NEXT FROM MovL INTO @DocEntrySal,@ItemCode,@whCode
       END
  CLOSE MovL
  DEALLOCATE MovL
  end

select * from #UGRStblTempCUR

