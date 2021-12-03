
LoadWWPContext(&Context)
&CTBUsuId	= &Context.CTBUsuId
&CTBUsuNome	= &Context.CTBUsuNome



For Each CTBEmp
	Where CTBEmpId = &Context.CTBEmpId
	&CTBEmpId			= CTBEmpId
	&CTBEmpRazaoSocial	= CTBEmpRazaoSocial
	&CTBEmpCNPJ			= CTBEmpCNPJ
	
EndFor

&Pagina = 2

Header
	Print PBCabec
	Print PBHeader
End
 
For Each CTBPlano Order CTBPlanoConta
	Where CTBPlanoTipoAS = PlanoTipoAS.Analitica
	Where CTBPlanoConta  = &CTBPlanoConta

	&CTBPlanoId		= CTBPlanoId
	&CTBPlanoDesc	= CTBPlanoDesc
	
	// saldo das contas - Aanalisar  saldo final e saldo inicial  
	// se Houver saldo inicial - devera ser colocado , caso contrario , o saldo inicial sera zero.
	
	&Saldo.SetEmpty()
	&SaldoFinal.SetEmpty()
	&Contador = 0
	// Imprime os lancamentos
	
	For Each CTBLanc Order CTBLancData CTBLancId
		Where CTBLancEmpId		 = &CTBEmpId
		Where CTBLancPlanoDebId	 = &CTBPlanoId OR CTBLancPlanoCredId = &CTBPlanoId
		Where CTBLancData		>= &CTBLancDataEntradaInicio
		Where CTBLancData		<= &CTBLancDataEntradaFinal
		
		&Contador += 1
		
		If &Contador = 1
			Print PBConta
			Print PBTit
			
			Do 'SaldoInicial'
			Print  PBSaldo
		EndIf
		
		&CTBLancData		= CTBLancData
		&CTBLancId			= CTBLancId
		&CTBLancHistDesc	= CTBLancHistDesc
		&CTBLancHistObs		= CTBLancHistObs
					
		Do Case
			Case CTBLancPlanoDebId	        = &CTBPlanoId
				&CTBLancPlanoDebCredConta	= CTBLancPlanoCredConta
				&CTBLancValorDebito			= Trim(CTBLancValor.ToFormattedString())
				&CTBLancValorCredito		= ''
				
			Case CTBLancPlanoCredId	= &CTBPlanoId
				&CTBLancPlanoDebCredConta	= CTBLancPlanoDebConta
				&CTBLancValorDebito			= ''
				&CTBLancValorCredito		= Trim(CTBLancValor.ToFormattedString())				
		EndCase
				
				
		// Imprimir o saldo do lancamento 
		
		If &Contador = 1
			&Saldo = &SaldoInicial + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric()
		Else
			&Saldo = &Saldo + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric() 
			EndIf
		
	//	&SaldoString = Trim(&Saldo.ToFormattedString())
	//  &SaldoString = &SaldoString.Replace('-', '')
		
		
		// Tipo de saldo - D ou C 
		// quando for positivo  - colocar D 
		// quando for negativo - colocar C 
		
		//If &Saldo > 0 
	//		&DebCred = 'D'
	//	EndIf 
		
		If &Saldo < 0
			&DebCred = 'C'
		endif
					
		Print PBLanc
		
				 
	EndFor
	
	// imprime saldo final , que é  a ultima linha impressa com o SALDO
//	If &Contador > 0
//		&SaldoFinal = &Saldo
//		&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
//		&SaldoFinalString = &SaldoFinalString.Replace('-', '')
//    	Print PBSaldo
//	EndIf
//	
EndFor
//-------------------------------------------------------------------------
// imprime Rodape 
//-----------------------------------------------------------------------------
Footer
	Print PBFooter
	&Pagina += 1
End
//-------------------------------------------------------------------------------------------
// Sub rotina do saldo inicial -   
//------------------------------------------------------------------------------------------
Sub 'SaldoInicial'
	
	&SaldoInicial.SetEmpty()
	For Each CTBLanc Order CTBLancData CTBLancId
		Where CTBLancEmpId		= &CTBEmpId
		Where CTBLancPlanoDebId	= &CTBPlanoId OR CTBLancPlanoCredId = &CTBPlanoId
		Where CTBLancData  		< &CTBLancDataEntradaInicio
		
		Do Case
			Case CTBLancPlanoDebId	        = &CTBPlanoId			
				&CTBLancValorDebito			= Trim(CTBLancValor.ToFormattedString())
				&CTBLancValorCredito		= ''
				
			Case CTBLancPlanoCredId	= &CTBPlanoId				
				&CTBLancValorDebito			= ''
				&CTBLancValorCredito		= Trim(CTBLancValor.ToFormattedString())				
		EndCase
		
		&SaldoInicial += &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric()
		
	EndFor
	
	&SaldoInicialString = Trim(&SaldoInicial.ToFormattedString())
	//&SaldoInicialString	= &SaldoInicialString.Replace('-', '')
	
	If &SaldoInicial > 0 
		&DebCred = 'D'
	EndIf 
		
	If &SaldoInicial < 0
		&DebCred = 'C'
	EndIf 

EndSub
