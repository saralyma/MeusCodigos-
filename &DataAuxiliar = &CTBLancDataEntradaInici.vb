&DataAuxiliar = &CTBLancDataEntradaInicial
	If &ImprimeCentroCusto = True
		For Each CTBLanc Order CTBLancData CTBLancId
			Where CTBLancEmpId		 = &CTBEmpId
			Where CTBLancPlanoDebId	 = &CTBPlanoId OR CTBLancPlanoCredId = &CTBPlanoId
			Where CTBLancData		>= &CTBLancDataEntradaInicial
			Where CTBLancData		<= &CTBLancDataEntradaFinal
			Where CTBLancCustoDebId  <> 0 OR CTBLancCustoCredId <> 0
			
			&Contador += 1
			
			If &Contador = 1
				If CTBLancPlanoDebId.IsEmpty()
					&ContaAuxiliar = CTBLancPlanoCredId
				EndIf
				If CTBLancPlanoCredId.IsEmpty()
					&ContaAuxiliar = CTBLancPlanoDebId 
				EndIf
			EndIf
			
			If &ImprimeTotalDia = True 
				If (&DataAuxiliar <> CTBLancData AND &ContaAuxiliar <> CTBLancPlanoCredId) OR (&DataAuxiliar <> CTBLancData AND &ContaAuxiliar <> CTBLancPlanoDebId) 
					&SaldoFinal = &Saldo
					&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
					&SaldoFinalString = &SaldoFinalString.Replace('-', '')
					If &ImprimirTotalDebCred = True
						Print PBSaldoDia 
					Else
						Print PBSaldo
					EndIf
					&DataAuxiliar = CTBLancData
					&SaldoCredito.SetEmpty()
					&SaldoDebito.SetEmpty()
					&Saldo.SetEmpty()
				EndIf
			EndIf	
			
			If &ImprimeTotalMes = True
				If (Month(&DataAuxiliar) <> Month(CTBLancData) AND &ContaAuxiliar <> CTBLancPlanoCredId) OR (Month(&DataAuxiliar) <> Month(CTBLancData) AND &ContaAuxiliar <> CTBLancPlanoDebId)
					&SaldoFinal = &Saldo
					&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
					&SaldoFinalString = &SaldoFinalString.Replace('-', '')
					If &ImprimirTotalDebCred = True
						Print PBSaldoMes 
					Else
						Print PBSaldo
					EndIf
					&DataAuxiliar = CTBLancData
					&Saldo.SetEmpty()
					&SaldoCredito.SetEmpty()
					&SaldoDebito.SetEmpty()
				EndIf
			EndIf
			
			If &Contador = 1
				Print PBConta
				Print PBTitCC
				Do 'SaldoInicial'
				Print  PBSaldoInicial
			EndIf
			
			&CTBLancData		= CTBLancData
			&CTBLancId			= CTBLancId
			&CTBLancHistDesc	= CTBLancHistDesc
			&CTBLancHistObs		= CTBLancHistObs
			&CTBlancCustoCredId = CTBLancCustoDebId // CTBLancCustoCredId
			
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
			
			If &Contador = 1
				&Saldo = &SaldoInicial + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric()
			Else
				&Saldo = &Saldo + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric() 
			EndIf
		
			&SaldoDebito  = &SaldoDebito + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric()
			&SaldoCredito = &SaldoCredito + &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric() 
			
			&SaldoString = Trim(&Saldo.ToFormattedString())
			&SaldoString = &SaldoString.Replace('-', '')
			
			// Tipo de saldo - D ou C 
			// quando for positivo  - colocar D 
			// quando for negativo - colocar C 
			
			If &Saldo > 0 
				&DebCred = 'D'
			EndIf 
			
			If &Saldo < 0
				&DebCred = 'C'
			endif
			
			Print PBLancCusto
			
			// Imprimir Complemento do Historico caso houver
			If NOT &CTBLancHistObs.IsEmpty()
				Print PBCompl
			Endif
			
		When None
			
			If &ImprimeContasSemMov = True	
				&Contador = 0
				Print PBConta
				// Print escrito sem movimento
			EndIf
			
		EndFor
		
		// imprime saldo final , que é  a ultima linha impressa com o SALDO
		If &Contador > 0
			&SaldoFinal = &Saldo
			&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
			&SaldoFinalString = &SaldoFinalString.Replace('-', '')
			
			If &ImprimirTotalDebCred = True
				Print PBSaldo 
			Else
				// SOmente o saldo total
			EndIf
		
			If &ImprimePaginaEntreContas = True
				Eject
			EndIf
		EndIf
		
	Else	

    If (&Contador > 0 AND &ImprimeTotalDia = True) OR (&Contador > 0 AND &ImprimeTotalMes = True)
		&SaldoFinal = &Saldo
		&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
		&SaldoFinalString = &SaldoFinalString.Replace('-', '')
		
		