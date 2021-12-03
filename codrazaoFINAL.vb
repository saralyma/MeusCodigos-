
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
	Where CTBPlanoConta  >= &CTBPlanoContaInicial When &ImprimeTodasContas = False
	Where CTBPlanoConta  <= &CTBPlanoContaFinal	  When &ImprimeTodasContas = False
	
	&CTBPlanoId		= CTBPlanoId
	&CTBPlanoConta	= iif(&ImprimeCodRed, Trim(CTBPlanoCodRed.ToString()), CTBPlanoConta)
	&CTBPlanoDesc	= CTBPlanoDesc
	
	&SaldoDebito.SetEmpty()
	&SaldoCredito.SetEmpty()	
	&Saldo.SetEmpty()
	&SaldoFinal.SetEmpty()
	&Contador = 0

	&DataAuxiliar = &CTBLancDataEntradaInicial
	//Imprime os lancamentos
	For Each CTBLanc Order CTBLancData CTBLancId
		Where CTBLancEmpId		 = &CTBEmpId
		Where CTBLancPlanoDebId	 = &CTBPlanoId OR CTBLancPlanoCredId = &CTBPlanoId
		Where CTBLancData		>= &CTBLancDataEntradaInicial
		Where CTBLancData		<= &CTBLancDataEntradaFinal
		
		&Contador += 1
		
		If &ImprimeTotalDia = True AND &Contador <> 1
			If Day(&DataAuxiliar) <> Day(CTBLancData) AND Month(&DataAuxiliar) <> Month(CTBLancData) OR
			   Day(&DataAuxiliar) = Day(CTBLancData)  AND Month(&DataAuxiliar) <> Month(CTBLancData) OR
			   Day(&DataAuxiliar) <> Day(CTBLancData) AND Month(&DataAuxiliar) = Month(CTBLancData)
				&SaldoFinal = &Saldo
				&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
				&SaldoFinalString = &SaldoFinalString.Replace('-', '')
				If &ImprimirTotalDebCred = True					
					Print PBSaldoDiaDebCred 					
				Else					
					Print PBSaldoDia					
				EndIf
				&DataAuxiliar = CTBLancData
				&SaldoCredito.SetEmpty()
				&SaldoDebito.SetEmpty()
				&Saldo.SetEmpty()
			EndIf
		EndIf
		
		If &ImprimeTotalMes = True AND &Contador <> 1
			If Month(&DataAuxiliar) <> Month(CTBLancData)
				&SaldoFinal = &Saldo
				&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
				&SaldoFinalString = &SaldoFinalString.Replace('-', '')
				If &ImprimirTotalDebCred = True					
					Print PBSaldoMesDebCred 					
				Else					
					Print PBSaldoMes				
				EndIf
				&DataAuxiliar = CTBLancData
				&Saldo.SetEmpty()
				&SaldoCredito.SetEmpty()
				&SaldoDebito.SetEmpty()
			EndIf
		EndIf
		
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
		
		// Imprimir o saldo do lancamento - Linha  
		
		If &Contador = 1
			&Saldo = &SaldoInicial + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric()
		Else
			&Saldo = &Saldo + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric() 
		EndIf
		
		If &ImprimirTotalDebCred
			&SaldoCredito += &CTBLancValorCredito.Replace('.','').Replace(',','.').ToNumeric()
			&SaldoDebito += &CTBLancValorDebito.Replace('.','').Replace(',','.').ToNumeric()
		EndIf
	 	
		&SaldoString = Trim(&Saldo.ToFormattedString())
		&SaldoString = &SaldoString.Replace('-', '')
		
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
		
		If NOT &CTBLancHistObs.IsEmpty()
			Print PBCompl
		Endif	
		   
	WHEN NONE
		
		If &ImprimeContasSemMov = True
			Print PBConta
			Print PBTitLanc
			Do 'Saldoinicial' 
			Print PBSaldoInicial
			Print PBSemMov
			&SaldoFinal = &SaldoInicial
			&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
			&SaldoFinalString = &SaldoFinalString.Replace('-', '')
			Print PBSaldoSemMov
		Endif
		
	EndFor
	
	If &Contador > 0
		Do Case
			Case &ImprimeTotalDia = True
				&SaldoFinal = &Saldo
				&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
				&SaldoFinalString = &SaldoFinalString.Replace('-', '')
				If &ImprimirTotalDebCred = True
					Print PBSaldoDiaDebCred 
				Else
					Print PBSaldoDia
				EndIf
				
			Case &ImprimeTotalMes = True
				&SaldoFinal = &Saldo
				&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
				&SaldoFinalString = &SaldoFinalString.Replace('-', '')
				If &ImprimirTotalDebCred = True
					Print PBSaldoMesDebCred 
				Else
					Print PBSaldoMes
				EndIf
				
			Case &ImprimirTotalDebCred = True AND &ImprimeTotalMes = False AND &ImprimeTotalDia = False
				IF &Contador > 0
					&SaldoFinal = &Saldo
					&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
					&SaldoFinalString = &SaldoFinalString.Replace('-', '')
					Print PBSaldoDebCred	 	
				Endif		
				
			Otherwise
				If &Contador > 0 
					&SaldoFinal = &Saldo
					&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
					&SaldoFinalString = &SaldoFinalString.Replace('-', '')
					Print PBSaldo
				Endif 		
		EndCase
	EndIf
		
	If &ImprimePaginaEntreContas = True 
		Eject
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
