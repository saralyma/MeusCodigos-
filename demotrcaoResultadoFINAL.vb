LoadWWPContext(&Context)
&CTBUsuId	= &Context.CTBUsuId
&CTBUsuNome	= &Context.CTBUsuNome


For Each CTBEmp
	Where CTBEmpId = &Context.CTBEmpId
	
	&CTBEmpId			= CTBEmpId
	&CTBEmpRazaoSocial	= CTBEmpRazaoSocial
	&CTBEmpCNPJ			= CTBEmpCNPJ
	
EndFor



For Each CTBGrupoPlano
	Where CTBGrupoPlanoId = &Context.CTBGrupoPlanoId
	
	&CTBGrupoPlanoAtivo			= CTBGrupoPlanoAtivo
	&CTBGrupoPlanoPassivo		= CTBGrupoPlanoPassivo
	&CTBGrupoPlanoReceita		= CTBGrupoPlanoReceita
	&CTBGrupoPlanoCusto			= CTBGrupoPlanoCusto
	&CTBGrupoPlanoDespesa		= CTBGrupoPlanoDespesa
	
EndFor

&Termo_Abertura_Data_Extenso = 'Paraná, ' + Trim(Str(Day(&Today))) + ' de ' + Trim(CMonth(&Today)) + ' de ' + Trim(Str(Year(&Today)))


Header

	If &ImprControlPag = true OR &ImprimeDemostracao = true
		&ImprNrPagina  = &ImprNrPagina + 1
		&folha = &ImprNrPagina
		Print PBCabec
		Print PBHeader
	endif

end


XFor Each 'vCTBLanc' Order CTBPlanoContaAgrup
	Where CTBPlanoGrupoPlanoId	= &Context.CTBGrupoPlanoId
	Where CTBGrupoNegId			= &Context.CTBGrupoNegId
	Where CTBLancEmpId			= &Context.CTBEmpId
	Where CTBLancData			>= &CTBLancDataInicio
	Where CTBLancData			<= &CTBLancDatafinal
	
	If
		CTBPlanoContaAgrup LIKE &CTBGrupoPlanoReceita	OR
		CTBPlanoContaAgrup LIKE &CTBGrupoPlanoCusto		OR
		CTBPlanoContaAgrup LIKE &CTBGrupoPlanoDespesa	OR
		CTBPlanoContaAgrup LIKE '6%'
		
		Do Case
			
			Case &sdtCTBBalancete.Count.IsEmpty() OR &sdtCTBBalancete.Item(&sdtCTBBalancete.Count).CTBPlanoContaAgrup <> CTBPlanoContaAgrup
				
				&sdtCTBBalanceteItem.CTBPlanoContaAgrup		= CTBPlanoContaAgrup
				&sdtCTBBalanceteItem.CTBPlanoTotalDebito	= CTBLancValorDeb
				&sdtCTBBalanceteItem.CTBPlanoTotalCredito	= CTBLancValorCred
				&sdtCTBBalanceteItem.CTBPlanoTipoAS			= CTBPlanoTipoAS
				
				&sdtCTBBalancete.Add(&sdtCTBBalanceteItem)
				&sdtCTBBalanceteItem						= New()
				
			Case &sdtCTBBalancete.Item(&sdtCTBBalancete.Count).CTBPlanoContaAgrup = CTBPlanoContaAgrup
				
				&sdtCTBBalancete.Item(&sdtCTBBalancete.Count).CTBPlanoTotalDebito	+= CTBLancValorDeb
				&sdtCTBBalancete.Item(&sdtCTBBalancete.Count).CTBPlanoTotalCredito	+= CTBLancValorCred
				
		EndCase
		
	EndIf
	
XEndFor

For &sdtCTBBalanceteItem In &sdtCTBBalancete
	
	&CTBPlanoContaAgrup		= &sdtCTBBalanceteItem.CTBPlanoContaAgrup
	
	For Each CTBPlano
		Where CTBPlanoGrupoPlanoId	= &Context.CTBGrupoPlanoId
		Where CTBPlanoConta			= &sdtCTBBalanceteItem.CTBPlanoContaAgrup
		
		&CTBPlanoDesc		= PadL('', &sdtCTBBalanceteItem.CTBPlanoContaAgrup.Length()) + Trim(CTBPlanoDesc)
		&CTBPlanoDesc		= SubStr(&CTBPlanoDesc, 1, 50)
		Exit
		
	When None
		&CTBPlanoDesc		= NullValue(&CTBPlanoDesc)
	EndFor	
	 
	Do Case		
		Case  &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '2.01'
		
		
		Case  &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '3.01'
			Do 'TotaisSTR'
		    &teste = &sdtCTBBalanceteItem.CTBPlanoTotalCredito  
			
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '3.02'
			Do 'TotaisSTR'
			&teste1 = &sdtCTBBalanceteItem.CTBPlanoTotalDebito
			&teste3 = &CTBGrupoPlanoReceita_Money
			&teste4 = &teste3 - &teste1
			If &CTBGrupoPlanoReceita_Money < 0
				&sit = 'D'
			else
				&sit = 'C'
			endif
		    
			Print PBVendaBruta
						
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '4' 
			Do 'TotaisSTR'
			If &teste4 < 0
				&sit = 'D'
			else
				&sit = 'C'
			endif	
			Print PBReceitaLiquida
			Print PBTotReceita
			&testecusto =  &CTBGrupoPlanoCusto_Money
			
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '4' 	
			Do 'TotaisSTR'
			&TTreceita = &TTReceitadeducao -&CTBGrupoPlanoReceita_Money 
		    &teste5 = &sdtCTBBalanceteItem.CTBPlanoTotalCredito
			&teste6 = &sdtCTBBalanceteItem.CTBPlanoTotalDebito
						
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '4.01.002' 	
			Do 'TotaisSTR'
			&TTreceita = &TTReceitadeducao -&CTBGrupoPlanoReceita_Money 
		    &teste5 = &sdtCTBBalanceteItem.CTBPlanoTotalCredito
			&teste6 = &sdtCTBBalanceteItem.CTBPlanoTotalDebito
			print PBtotalCusto
			
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '5'
			Do 'TotaisSTR'
			&teste11 =  &CTBGrupoPlanoCusto_Money- &teste5 
			Print PBDeducaoCusto
			Print PBTotCustos
						
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '5.04.001'
			Do 'TotaisSTR'
			If &testeCSLLD < 0
				&sit = 'D'
			else
				&sit = 'C'
			endif	
			
			&testeCSLLD = &sdtCTBBalanceteItem.CTBPlanoTotalDebito
	
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '5.04.002'
			Do 'TotaisSTR'
			If &testeIRPJD < 0
				&sit = 'D'
			else
				&sit = 'C'
			endif	
			
			&testeIRPJD = &sdtCTBBalanceteItem.CTBPlanoTotalDebito
								
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '6'
			Do 'TotaisSTR'
			&teste7 = &sdtCTBBalanceteItem.CTBPlanoTotalCredito
			&teste8 = &sdtCTBBalanceteItem.CTBPlanoTotalDebito 
			&teste9 = &sdtCTBBalanceteItem.CTBPlanoTotalSaldo
			&teste10 = &teste7 - &testecusto -&CTBGrupoPlanoDespesa_Money
			&teste20 = &teste10 - &testeIRPJD - &testeCSLLD  
			If &teste10 < 0
				&sit = 'D'
			else
				&sit = 'C'
			endif
			If &teste20 < 0
				&sit = 'D'
			else
				&sit = 'C'
			endif	
			Print PBTotDespesas
			   
	EndCase
	// imprime lancamentos 
	
	If NOT &sdtCTBBalanceteItem.CTBPlanoContaAgrup LIKE '1%'
		AND NOT &sdtCTBBalanceteItem.CTBPlanoContaAgrup LIKE '2%'
		//AND NOT &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '3' 
	   AND NOT &sdtCTBBalanceteItem.CTBPlanoContaAgrup LIkE '6%' 
			
		If &CTBPlanoTotalSaldo <> 0
			If &sdtCTBBalanceteItem.CTBPlanoTipoAS = PlanoTipoAS.Analitica 
				Print PBContaAnalitica
			Else
				Print PBConta2Bold
			EndIf								
	   	Endif
	EndIf
	
	
	iF &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '6'
		&valorLucro      	= &sdtCTBBalanceteItem.CTBPlanoTotalSaldo - &CTBGrupoPlanoCusto_Money - &CTBGrupoPlanoDespesa_Money
	ENDIF
		If &sdtCTBBalanceteItem.CTBPlanoContaAgrup LIKE '3.01.%'
		       &VlTotalreceitabruta  += &sdtCTBBalanceteItem.CTBPlanoTotalSaldo
	endIf
	EndFor
 
 
	 Print PBLucroOperacional
     Print  PBLucroBruto
   // Print PBLucroAntesImpostos
     Print PBLucro
		 

// Lugar da Sub 'TotaisSTR'

If NOT &sdtCTBBalancete.Count.IsEmpty() AND &ImpDiferenca
	&DiferencaAtivoPassivo_Money = &CTBGrupoPlanoAtivoTotal_Money + &CTBGrupoPlanoPassivoTotal_Money
	
	Do Case
		Case &DiferencaAtivoPassivo_Money < 0
			&DiferencaAtivoPassivo_MoneyStr = Trim(&DiferencaAtivoPassivo_Money.ToFormattedString()) + ' C'
			
		Case &DiferencaAtivoPassivo_Money = 0
			&DiferencaAtivoPassivo_MoneyStr = Trim(&DiferencaAtivoPassivo_Money.ToFormattedString())
			
		Case &DiferencaAtivoPassivo_Money > 0
			&DiferencaAtivoPassivo_MoneyStr = Trim(&DiferencaAtivoPassivo_Money.ToFormattedString()) + ' D'
			
	EndCase
	
	&DiferencaAtivoPassivo_MoneyStr	= &DiferencaAtivoPassivo_MoneyStr.ReplaceRegEx('^\-', '')
		
Endif

Print PBAssinatura

Footer
	Print PBFooter
	&folha +=1
End





	
