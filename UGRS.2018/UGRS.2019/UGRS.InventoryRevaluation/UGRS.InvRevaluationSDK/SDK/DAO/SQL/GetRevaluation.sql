
		declare  @DocNumSMAjuste int;
		declare  @DocDateSMAjuste as date;
		declare  @CamtDocRev1 int;
		declare  @CamtDocRev2 int;
		declare  @DocTypeES as varchar(1);
		declare  @RevFolFind as varchar(30);

		select @DocTypeES = '{DocTypeES}'
		select @DocNumSMAjuste= '{DocNum}'

                   
              declare @DocDate as date, @Cantidad as decimal(18,6), @Costo as decimal(18,6), @Valor as decimal(18,6), @DocType as int, @DocEntry as int;
              declare @ItemCode as nvarchar(10), @whCode as nvarchar(10), @DocEntrySal as int;
              Declare @CantidadAcum as decimal(30,6), @ValorAcum as decimal(30,10), @CostoV as decimal(30,10), @CostoVR1 as decimal(30,10), @Reg as int; 
			  Declare @TotRev1 as decimal(30,6), @TotRev2 as decimal(30,6);
              Declare @ValorAcum2 as decimal(30,10);
              Declare @ValorRev1 as decimal(30,10), @ValorRev2 as decimal(30,6); 
			   set @TotRev1=0; set @TotRev2=0;
              select  @DocDateSMAjuste = CASE WHEN @DocTypeES = 'S'
                                                                   THEN --Salidas
                                                                          (select  top 1 T0.DocDate from OIGE T0 where T0.DocNum= @DocNumSMAjuste)
                                                                   ELSE --Entradas
                                                                          (select  top 1 T0.DocDate from OIGN T0 where T0.DocNum= @DocNumSMAjuste)
                                                                   END

              set @RevFolFind = @DocTypeES + cast(@DocNumSMAjuste as nvarchar(30));
              select @CamtDocRev1 = case when @DocTypeES = 'E' THEN 1 ELSE (select count(*) from OMRV t0 where  t0.U_DocNumSalida =@RevFolFind and t0.DocDate=@DocDateSMAjuste) END;
              select @CamtDocRev2 = case when @DocTypeES = 'S' AND @CamtDocRev1 = '0' THEN 1 ELSE (select count(*) from OMRV t0 where  t0.U_DocNumSalida = @RevFolFind and t0.DocDate>@DocDateSMAjuste) END;

              IF OBJECT_ID('tempdb..#UGRStblTempCUR') is not null
                  drop table #UGRStblTempCUR
                 CREATE TABLE #UGRStblTempCUR(ItemCode nvarchar(15), whCode nvarchar(15), DocDateRev1 date, Rev1 decimal(14,6), CostoRev1 decimal(14,6), DocDateRev2 date, Rev2 decimal(14,6), CostoRev2 decimal(14,6)) 

                           
              if (@CamtDocRev1=0 or @CamtDocRev2=0) begin

              DECLARE MovL CURSOR LOCAL FAST_FORWARD FOR 
                          SELECT * FROM (
                           select t0.DocEntry, t1.ItemCode, T1.WhsCode
                           from OIGE T0 inner join IGE1 T1 on t0.DocEntry= t1.DocEntry where T0.DocNum= @DocNumSMAjuste AND @DocTypeES='S') tb1
                                           UNION ALL 
                    SELECT * FROM (
                           select t0.DocEntry, t1.ItemCode, T1.WhsCode
                           from OIGN T0 inner join IGN1 T1 on t0.DocEntry= t1.DocEntry where T0.DocNum= @DocNumSMAjuste AND @DocTypeES='E') tb1
              OPEN MovL
              FETCH NEXT FROM MovL INTO @DocEntrySal,@ItemCode,@whCode
              WHILE @@FETCH_STATUS = 0 
                     BEGIN

                        set @CantidadAcum=0; set @ValorAcum=0;set @ValorAcum2=0;set @Reg=0;

                             DECLARE MovLotes CURSOR LOCAL FAST_FORWARD FOR 
                                            select t0.DocDate, isnull(case when t0.InQty =0 then t0.OutQty * -1 else t0.InQty end,0) Cantidad
                                                                                 ,t0.CalcPrice Costo,  isnull(t0.TransValue,0.0) Valor, T0.TransType, T0.CreatedBy
                                            from OINM T0 where T0.itemcode=@ItemCode and t0.Warehouse=@whCode and (t0.TransValue<> 0 or t0.OutQty <>0 or t0.InQty <>0) 
                                            and t0.TransNum not in (select t0.TransNum from OINM T0 where t0.TransType=162 and t0.CreatedBy in (select DocEntry from OMRV where U_DocNumSalida in (@RevFolFind)))
                                           -- and T0.CreatedBy=28945
                                            order by t0.DocDate, t0.TransNum asc
                             OPEN MovLotes
                             FETCH NEXT FROM MovLotes INTO @DocDate,@Cantidad,@Costo,@Valor,@DocType,@DocEntry
                             WHILE @@FETCH_STATUS = 0 
                                         BEGIN
                                                                   
                                                      
                                                IF @DocType=60 and @DocEntry=@DocEntrySal and @DocTypeES='S'
                                                       -- ENCONTRO AL DOCUMENTO DE SALIDA
                                                       BEGIN
                                                         set @ValorRev1 = (@Valor/case when @Cantidad <> 0 then @Cantidad else 1 end - (@ValorAcum/case when @CantidadAcum <> 0 then @CantidadAcum else 1 end )) * (@Cantidad*-1);
                                                         set @ValorAcum +=@ValorRev1;
                                                         set @Reg=1
                                                         set @CantidadAcum += @Cantidad;
                                                         set @ValorAcum += @Valor;
                                                         set @ValorAcum2= @ValorAcum;
                                                         set @CostoV= @ValorAcum/case when @CantidadAcum <> 0 then @CantidadAcum else 1 end;
                                                         set @CostoVR1= @CostoV
                                                       END
                                                                                 ELSE IF @DocType=59 and @DocTypeES='E' and @DocEntry=@DocEntrySal
                                                                                        BEGIN
                                                              set @CantidadAcum += @Cantidad;
                                                              set @ValorAcum += @Valor;
                                                                                                       set @ValorRev1=0;
                                                              set @ValorAcum2= @ValorAcum;
                                                                                                       set @CostoVR1= @ValorAcum/@CantidadAcum;
                                                                                                       set @CostoV= @CostoVR1;--@ValorAcum/case when @CantidadAcum <> 0 then @CantidadAcum else 1 end;
                                                                                                       set @Reg=1;
                                                                                          END
                                                ELSE IF @Reg=1
                                                       BEGIN
                                                                                               
                                                              set @CantidadAcum += @Cantidad;
                                                              set @ValorAcum += @Valor;
                                                              set @ValorAcum2+= case when @Cantidad =0 then @Valor when @Cantidad>0 then @Cantidad*@Costo else @CostoV*@Cantidad end;
                                                                                                       --- posiblemente sea este calculo
                                                              set @CostoV = @ValorAcum2/case when @CantidadAcum <> 0 then @CantidadAcum else 1 end;
                                            
                                                       END
                                                ElSE
                                                       BEGIN
                                                              -- insert into #UGRStblTempCUR (Cantidad , Valor) values (@Cantidad,@Valor)
                                                              set @CantidadAcum += @Cantidad;
                                                              set @ValorAcum += CONVERT(DECIMAL(18,10),@Valor);
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

							 set @TotRev1 += case when @CamtDocRev1>0 then 0 else @ValorRev1 end;
							  set @TotRev2 += case when @CamtDocRev2>0 then 0 else (@CostoV - @ValorAcum/case when @CantidadAcum <> 0 then @CantidadAcum else 1 end) *@CantidadAcum end;
  FETCH NEXT FROM MovL INTO @DocEntrySal,@ItemCode,@whCode
       END
  CLOSE MovL
  DEALLOCATE MovL
  end

select case when @TotRev1 = 0 then 1 else @CamtDocRev1 end  AS HasRev1, case when @TotRev2 = 0 then 1 when @TotRev1 = 0 AND @CamtDocRev2 = 0 then 0 else @CamtDocRev2 end AS HasRev2, * from #UGRStblTempCUR





