
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
	Where CTBPlanoConta  >= &CTBPlanoContaInicial	When &ImprimeTodasContas = False
	Where CTBPlanoConta  <= &CTBPlanoContaFinal		When &ImprimeTodasContas = False
	
	&CTBPlanoId		= CTBPlanoId
	&CTBPlanoConta	= iif(&ImprimeCodRed, Trim(CTBPlanoCodRed.ToString()), CTBPlanoConta)
	&CTBPlanoDesc	= CTBPlanoDesc
	
	/*
	Saldo das contas - Aanalisar  saldo final e saldo inicial 
	Se Houver saldo inicial - devera ser colocado , caso contrario , o saldo inicial sera zero
	*/
	&Saldo.SetEmpty()
	&SaldoFinal.SetEmpty()
	&Contador = 0
	
	//Imprime os lancamentos
	For Each CTBLanc Order CTBLancData CTBLancId
		Where CTBLancEmpId		 = &CTBEmpId
		Where CTBLancPlanoDebId	 = &CTBPlanoId OR CTBLancPlanoCredId = &CTBPlanoId
		Where CTBLancData		>= &CTBLancDataEntradaInicial
		Where CTBLancData		<= &CTBLancDataEntradaFinal
		
		&Contador += 1
		
		If &Contador = 1
			Print PBConta
			
			If &ImprimeCentroCusto
				Print PBTitCC
			Else
				Print PBTitLanc
			EndIf
			
			Do 'SaldoInicial'
			Print  PBSaldoInicial
		EndIf
		
		&CTBLancData		= CTBLancData
		&CTBLancId			= CTBLancId
		&CTBLancHistDesc	= CTBLancHistDesc
		&CTBLancHistObs		= CTBLancHistObs
		
		Do Case
			Case CTBLancPlanoDebId	        = &CTBPlanoId
				&CTBLancPlanoDebCredConta	= iif(&ImprimeCodRed, Trim(CTBLancPlanoCredCodRed.ToString()), CTBLancPlanoCredConta)
				&CTBLancValorDebito			= Trim(CTBLancValor.ToFormattedString())
				&CTBLancValorCredito		= ''
				&CTBCustoId					= CTBLancCustoCredId
				
			Case CTBLancPlanoCredId	= &CTBPlanoId
				&CTBLancPlanoDebCredConta	= iif(&ImprimeCodRed, Trim(CTBLancPlanoDebCodRed.ToString()), CTBLancPlanoDebConta)
				&CTBLancValorDebito			= ''
				&CTBLancValorCredito		= Trim(CTBLancValor.ToFormattedString())
				&CTBCustoId					= CTBLancCustoDebId
				
		EndCase
				
				
		// Imprimir o saldo do lancamento 
		If &Contador = 1
			&Saldo = &SaldoInicial + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric()
		Else
			&Saldo = &Saldo + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric() 
		EndIf
		
		&SaldoString = Trim(&Saldo.ToFormattedString())
		&SaldoString = &SaldoString.Replace('-', '')
		
		/*
		Tipo de saldo - D ou C 
		Quando for positivo  - colocar D 
		Quando for negativo - colocar C 
		*/
		Do Case
			Case &Saldo > 0
				&DebCred = 'D'
			Case &Saldo < 0
				&DebCred = 'C'
			Otherwise
				&DebCred = ''
		EndCase
		
		If &ImprimeCentroCusto
			Print PBLancCusto
		Else
			Print PBLanc
		EndIf
		
		// Imprimir Complemento do Historico caso houver
		If NOT &CTBLancHistObs.IsEmpty()
			Print PBCompl
		Endif
	
	    
		when none
			if &Contador = 0
				Print PBConta
				Print PBTitLanc
				Do 'Saldoinicial' 
				Print PBSaldoInicial
				Print PBSemMov
				Print PBSaldo
			endif	
	
			 
	EndFor
	
	//imprime saldo final , que é  a ultima linha impressa com o SALDO
	If &Contador > 0
		&SaldoFinal = &Saldo
		&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
		&SaldoFinalString = &SaldoFinalString.Replace('-', '')
    	Print PBSaldo
	EndIf
	
	
EndFor


Footer
	Print PBFooter
	&Pagina += 1
End


Sub 'SaldoInicial'
	
	&SaldoInicial.SetEmpty()
	For Each CTBLanc Order CTBLancData CTBLancId
		Where CTBLancEmpId		= &CTBEmpId
		Where CTBLancPlanoDebId	= &CTBPlanoId OR CTBLancPlanoCredId = &CTBPlanoId
		Where CTBLancData  		< &CTBLancDataEntradaInicial
		
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
	&SaldoInicialString	= &SaldoInicialString.Replace('-', '')
	
	Do Case
		Case &SaldoInicial > 0
			&DebCred = 'D'
		Case &SaldoInicial < 0
			&DebCred = 'C'
		Otherwise
			&DebCred = ''
	EndCase
	
EndSub