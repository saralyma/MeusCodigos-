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


Header
	Print PBCabec
	Print PBHeader

end


XFor Each 'vCTBLanc' Order CTBPlanoContaAgrup
	Where CTBPlanoGrupoPlanoId	= &Context.CTBGrupoPlanoId
	Where CTBGrupoNegId			= &Context.CTBGrupoNegId
	Where CTBLancEmpId			= &Context.CTBEmpId
	Where CTBLancData			>= &CTBLancDataInicio
	Where CTBLancData			<= &CTBLancDatafinal
	
	
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
	
XEndFor


For &sdtCTBBalanceteItem In &sdtCTBBalancete
	
	&CTBPlanoContaAgrup		= &sdtCTBBalanceteItem.CTBPlanoContaAgrup
	
	For Each CTBPlano
		Where CTBPlanoGrupoPlanoId	= &Context.CTBGrupoPlanoId
		Where CTBPlanoConta			= &sdtCTBBalanceteItem.CTBPlanoContaAgrup
		
		
		&CTBPlanoDesc		= PadL('', &sdtCTBBalanceteItem.CTBPlanoContaAgrup.Length()) + Trim(CTBPlanoDesc)
		&CTBPlanoDesc		= SubStr(&CTBPlanoDesc, 1,50)
		Exit
		
	When None
		&CTBPlanoDesc		= NullValue(&CTBPlanoDesc)
	EndFor
	
	
	Do 'SaldoInicial'
	
	
	&CTBPlanoTotalDebito	= &sdtCTBBalanceteItem.CTBPlanoTotalDebito
	&CTBPlanoTotalCredito	= &sdtCTBBalanceteItem.CTBPlanoTotalCredito
	&CTBPlanoTotalSaldo		= &SaldoInicial + &CTBPlanoTotalDebito - &CTBPlanoTotalCredito
	
	
	Do Case
		Case &CTBPlanoTotalSaldo < 0
			&CTBPlanoTotalSaldoStr = Trim(&CTBPlanoTotalSaldo.ToFormattedString()) + ' C'
			
		Case &CTBPlanoTotalSaldo = 0
			&CTBPlanoTotalSaldoStr = Trim(&CTBPlanoTotalSaldo.ToFormattedString())
			
		Case &CTBPlanoTotalSaldo > 0
			&CTBPlanoTotalSaldoStr = Trim(&CTBPlanoTotalSaldo.ToFormattedString()) + ' D'
			
	EndCase
	
	&CTBPlanoTotalSaldoStr	= &CTBPlanoTotalSaldoStr.ReplaceRegEx('^\-', '')
	
	Do Case
		Case &CTBPlanoContaAgrup.Trim() = &CTBGrupoPlanoAtivo.Trim()
			&CTBGrupoPlanoAtivo_Money	= &CTBPlanoTotalSaldo
			
		Case &CTBPlanoContaAgrup.Trim() = &CTBGrupoPlanoPassivo.Trim()
			&CTBGrupoPlanoPassivo_Money	= &CTBPlanoTotalSaldo
			
		Case &CTBPlanoContaAgrup.Trim() = &CTBGrupoPlanoReceita.Trim()
			&CTBGrupoPlanoReceita_Money	= &CTBPlanoTotalSaldo
			
		Case &CTBPlanoContaAgrup.Trim() = &CTBGrupoPlanoCusto.Trim()
			&CTBGrupoPlanoCusto_Money	= &CTBPlanoTotalSaldo
			
		Case &CTBPlanoContaAgrup.Trim() = &CTBGrupoPlanoDespesa.Trim()
			&CTBGrupoPlanoDespesa_Money	= &CTBPlanoTotalSaldo
			
	EndCase
 separar Ativo e Passivo 		
	Do Case
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '2' 
			Do 'TotaisSTR'
			Print PBTotAtivo
		Case &sdtCTBBalanceteItem.CTBPlanoContaAgrup = '3'
			Do 'TotaisSTR'
			Print PBTotPassivo
	EndCase		
		
// Problema: o relatorio analitico precisa constar tudo - contas sinteticas e analiticas
		
	If &CTBPlanoTotalSaldo <> 0			
		If &sdtCTBBalanceteItem.CTBPlanoTipoAS = PlanoTipoAS.Analitica
			If &ImprimeContasAnalitica= true and &ImprimeColSaldo= TRUE
				Print PBContaAnaliticaSaldo     
			  else
	 			if &ImprimeContasAnalitica = true 
			   		Print PBContaAnalitica  
				Endif
			Endif
		Else
			If &ImprimeBalanco = true and &ImprimeColSaldo= true 
				Print PBGrupoSintSaldo
			else
			 If &ImprimeBalanco = true 
					Print PBGrupoSintetico
				EndIf
			Endif
		Endif
	Endif

EndFor

// Lugar da Sub 'TotaisSTR'

//If NOT &sdtCTBBalancete.Count.IsEmpty() AND &ImpDiferenca
//	&DiferencaAtivoPassivo_Money = &CTBGrupoPlanoAtivoTotal_Money + &CTBGrupoPlanoPassivoTotal_Money
//	
//	Do Case
//		Case &DiferencaAtivoPassivo_Money < 0
//			&DiferencaAtivoPassivo_MoneyStr = Trim(&DiferencaAtivoPassivo_Money.ToFormattedString()) + ' C'
//			
//		Case &DiferencaAtivoPassivo_Money = 0
//			&DiferencaAtivoPassivo_MoneyStr = Trim(&DiferencaAtivoPassivo_Money.ToFormattedString())
//			
//		Case &DiferencaAtivoPassivo_Money > 0
//			&DiferencaAtivoPassivo_MoneyStr = Trim(&DiferencaAtivoPassivo_Money.ToFormattedString()) + ' D'
//			
//	EndCase
//	
//	&DiferencaAtivoPassivo_MoneyStr	= &DiferencaAtivoPassivo_MoneyStr.ReplaceRegEx('^\-', '')
//	
//	If &DiferencaAtivoPassivo_Money.IsEmpty()
//		Print PBSemDif
//	Else
//		Print PBComDif
//	EndIf
//	
//EndIf


Print PBAssinatura

Footer
	Print PBFooter
End


Sub 'SaldoInicial'
	
	&SaldoInicial.SetEmpty()
	
	XFor Each 'vCTBLanc'
		Where CTBPlanoGrupoPlanoId	= &Context.CTBGrupoPlanoId
		Where CTBGrupoNegId			= &Context.CTBGrupoNegId
		Where CTBLancEmpId			= &Context.CTBEmpId
		Where CTBPlanoContaAgrup	= &CTBPlanoContaAgrup
		Where CTBLancData			< &CTBLancDataInicio
		
		&SaldoInicial += CTBLancValorDeb - CTBLancValorCred
		
	XEndFor
	
	Do Case
		Case &SaldoInicial < 0
			&SaldoInicialString = Trim(&SaldoInicial.ToFormattedString()) + ' C'
			
		Case &SaldoInicial = 0
			&SaldoInicialString = Trim(&SaldoInicial.ToFormattedString())
			
		Case &SaldoInicial > 0
			&SaldoInicialString = Trim(&SaldoInicial.ToFormattedString()) + ' D'
			
	EndCase
	
	&SaldoInicialString	= &SaldoInicialString.ReplaceRegEx('^\-', '')
	
EndSub

Sub 'TotaisSTR'
	If &sdtCTBBalancete.Count.IsEmpty()   // nao esta funcionando - so funciona se nao houver nenhum lancamento no CTBLANC
		Print PBSemMov
	Else
		Do Case
			Case &CTBGrupoPlanoAtivo_Money < 0
				&CTBGrupoPlanoAtivo_MoneyStr = Trim(&CTBGrupoPlanoAtivo_Money.ToFormattedString()) + ' C'
				
			Case &CTBGrupoPlanoAtivo_Money = 0
				&CTBGrupoPlanoAtivo_MoneyStr = Trim(&CTBGrupoPlanoAtivo_Money.ToFormattedString())
				
			Case &CTBGrupoPlanoAtivo_Money > 0
				&CTBGrupoPlanoAtivo_MoneyStr = Trim(&CTBGrupoPlanoAtivo_Money.ToFormattedString()) + ' D'
				
		EndCase
		
		Do Case
			Case &CTBGrupoPlanoPassivo_Money < 0
				&CTBGrupoPlanoPassivo_MoneyStr = Trim(&CTBGrupoPlanoPassivo_Money.ToFormattedString()) + ' C'
				
			Case &CTBGrupoPlanoPassivo_Money = 0
				&CTBGrupoPlanoPassivo_MoneyStr = Trim(&CTBGrupoPlanoPassivo_Money.ToFormattedString())
				
			Case &CTBGrupoPlanoPassivo_Money > 0
				&CTBGrupoPlanoPassivo_MoneyStr = Trim(&CTBGrupoPlanoPassivo_Money.ToFormattedString()) + ' D'
				
		EndCase
		
		Do Case
			Case &CTBGrupoPlanoDespesa_Money < 0
				&CTBGrupoPlanoDespesa_MoneyStr = Trim(&CTBGrupoPlanoDespesa_Money.ToFormattedString()) + ' C'
				
			Case &CTBGrupoPlanoDespesa_Money = 0
				&CTBGrupoPlanoDespesa_MoneyStr = Trim(&CTBGrupoPlanoDespesa_Money.ToFormattedString())
				
			Case &CTBGrupoPlanoDespesa_Money > 0
				&CTBGrupoPlanoDespesa_MoneyStr = Trim(&CTBGrupoPlanoDespesa_Money.ToFormattedString()) + ' D'
				
		EndCase
		
		Do Case
			Case &CTBGrupoPlanoReceita_Money < 0
				&CTBGrupoPlanoReceita_MoneyStr = Trim(&CTBGrupoPlanoReceita_Money.ToFormattedString()) + ' C'
				
			Case &CTBGrupoPlanoReceita_Money = 0
				&CTBGrupoPlanoReceita_MoneyStr = Trim(&CTBGrupoPlanoReceita_Money.ToFormattedString())
				
			Case &CTBGrupoPlanoReceita_Money > 0
				&CTBGrupoPlanoReceita_MoneyStr = Trim(&CTBGrupoPlanoReceita_Money.ToFormattedString()) + ' D'
				
		EndCase
		
		Do Case
			Case &CTBGrupoPlanoCusto_Money < 0
				&CTBGrupoPlanoCusto_MoneyStr = Trim(&CTBGrupoPlanoCusto_Money.ToFormattedString()) + ' C'
				
			Case &CTBGrupoPlanoCusto_Money = 0
				&CTBGrupoPlanoCusto_MoneyStr = Trim(&CTBGrupoPlanoCusto_Money.ToFormattedString())
				
			Case &CTBGrupoPlanoCusto_Money > 0
				&CTBGrupoPlanoCusto_MoneyStr = Trim(&CTBGrupoPlanoCusto_Money.ToFormattedString()) + ' D'
				
		EndCase
		
		&CTBGrupoPlanoAtivo_MoneyStr	= &CTBGrupoPlanoAtivo_MoneyStr.ReplaceRegEx('^\-', '')
		&CTBGrupoPlanoPassivo_MoneyStr	= &CTBGrupoPlanoPassivo_MoneyStr.ReplaceRegEx('^\-', '')
		&CTBGrupoPlanoDespesa_MoneyStr	= &CTBGrupoPlanoDespesa_MoneyStr.ReplaceRegEx('^\-', '')
		&CTBGrupoPlanoReceita_MoneyStr	= &CTBGrupoPlanoReceita_MoneyStr.ReplaceRegEx('^\-', '')
		&CTBGrupoPlanoCusto_MoneyStr	= &CTBGrupoPlanoCusto_MoneyStr.ReplaceRegEx('^\-', '')
		
		&CTBGrupoPlanoAtivoTotal_Money		= &CTBGrupoPlanoAtivo_Money		+ &CTBGrupoPlanoDespesa_Money + &CTBGrupoPlanoCusto_Money
		&CTBGrupoPlanoPassivoTotal_Money	= &CTBGrupoPlanoPassivo_Money	+ &CTBGrupoPlanoReceita_Money
		
		Do Case
			Case &CTBGrupoPlanoAtivoTotal_Money < 0
				&CTBGrupoPlanoAtivoTotal_MoneyStr = Trim(&CTBGrupoPlanoAtivoTotal_Money.ToFormattedString()) + ' C'
				
			Case &CTBGrupoPlanoAtivoTotal_Money = 0
				&CTBGrupoPlanoAtivoTotal_MoneyStr = Trim(&CTBGrupoPlanoAtivoTotal_Money.ToFormattedString())
				
			Case &CTBGrupoPlanoAtivoTotal_Money > 0
				&CTBGrupoPlanoAtivoTotal_MoneyStr = Trim(&CTBGrupoPlanoAtivoTotal_Money.ToFormattedString()) + ' D'
				
		EndCase
		
		Do Case
			Case &CTBGrupoPlanoPassivoTotal_Money < 0
				&CTBGrupoPlanoPassivoTotal_MoneyStr = Trim(&CTBGrupoPlanoPassivoTotal_Money.ToFormattedString()) + ' C'
				
			Case &CTBGrupoPlanoPassivoTotal_Money = 0
				&CTBGrupoPlanoPassivoTotal_MoneyStr = Trim(&CTBGrupoPlanoPassivoTotal_Money.ToFormattedString())
				
			Case &CTBGrupoPlanoPassivoTotal_Money > 0
				&CTBGrupoPlanoPassivoTotal_MoneyStr = Trim(&CTBGrupoPlanoPassivoTotal_Money.ToFormattedString()) + ' D'
				
		EndCase
		
		&CTBGrupoPlanoAtivoTotal_MoneyStr	= &CTBGrupoPlanoAtivoTotal_MoneyStr.ReplaceRegEx('^\-', '')
		&CTBGrupoPlanoPassivoTotal_MoneyStr	= &CTBGrupoPlanoPassivoTotal_MoneyStr.ReplaceRegEx('^\-', '')
		
		//	Totais 
		//	Print PBTotAtivo
		//	Print PBTotPassivo
		//	Print PBTotalGrupos
		//	Print PBTotal
		//	Print PBTotAtivo
		
	EndIf
EndSub
