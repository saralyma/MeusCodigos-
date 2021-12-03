LoadWWPContext(&Context)
&CTBUsuId	= &Context.CTBUsuId
&CTBUsuNome	= &Context.CTBUsuNome



For Each CTBEmp
	Where CTBEmpId = &Context.CTBEmpId
	&CTBEmpId			= CTBEmpId
	&CTBEmpRazaoSocial	= CTBEmpRazaoSocial
	&CTBEmpCNPJ			= CTBEmpCNPJ
	
EndFor
/*
PBCabec x
PBHeader x
PBConta x
PBTit
PBlanc
PBTitCC
PBLancCC
PBCompl
PBSemMov
PBSaldoInicial
PBSaldo
PBSaldoDia   x
PBSaldoMes   x
PBSaldoDebCred x
PBSaldoDebCredDia X
PBSaldoDebCredMes x
PBFooter x

*/

&Pagina = 2

Header
	Print PBCabec
	Print PBHeader
End

For Each CTBPlano Order CTBPlanoConta 
		Where CTBPlanoDesc = 'CAIXA'
		Where CTBPlanoConta = '1.01.001.0001'
		Defined BY CTBPlanoId
		&CTBPlanoConta = CTBPlanoConta
		&CTBPlanoDesc = CTBPlanoDesc 
		
		&TotalDebito.SetEmpty()
		&TotalCredito.SetEmpty()
		&saldo.SetEmpty()
		&saldoFinal.SetEmpty()
		&Contador = 0 
		&DataAuxiliar = &CTBLancDataEntradaInicio


			//lancamentos		
			
		For Each CTBLanc Order CTBLancData CTBLancId
			Where CTBLancEmpId = &CTBEmpId
			Where CTBLancPlanoDebId = &CTBPlanoId OR CTBLancPlanoCredId = &CTBPlanoId
			Where CTBLancData  >= &CTBLancDataEntradaInicio
			Where CTBLancData  <= &CTBLancDataEntradaFinal
			
			&Contador += 1
			
			If &ImprimeSaldoDia = True  And &Contador <> 1
				If	 Day(&DataAuxiliar) <> Day(CTBlancData) AND Month(&DataAuxiliar) <> Month(CTBLancData) OR				
			  		 Day(&DataAuxiliar) = Day(CTBLancData) AND Month(&DataAuxiliar) <> Month(CTBLancData) OR
			  		 Day(&DataAuxiliar) <> Day(CTBlancData) AND Month(&DataAuxiliar) = Month(CTBLancData) 
			  		 &saldoFinal = &saldo
			  	 	&saldoFinalString = Trim(&saldoFinal.ToFormattedString())
			  		 &saldoFinalString = &saldoFinalString.replace('-','')
			 		  If &ImprimeTotalDebCred = True
				 		  Print PBSaldoDebCredDia
				   		Else
				 		  Print PBSaldoDia
				  	  Endif
			  	 &DataAuxiliar = CTBLancData
			  	 &TotalCredito.SetEmpty()
			  	 &TotalDebito.SetEmpty()
			   	&saldo.SetEmpty()
				Endif
			Endif
		
		If &ImprimeSaldoMes = True  And &Contador <> 1
			If month(&DataAuxiliar) <> Month(CTBlancData) AND Year(&DataAuxiliar) <> Year(CTBLancData) OR				
			   month(&DataAuxiliar) = month(CTBLancData) AND Year(&DataAuxiliar) <> Year(CTBLancData) OR
			   month(&DataAuxiliar) <> Month(CTBlancData) AND Year(&DataAuxiliar) = Year(CTBLancData) 
			   &saldoFinal = &saldo
			   &saldoFinalString = Trim(&saldoFinal.ToFormattedString())
			   &saldoFinalString = &saldoFinalString.replace('-','')
			   If &ImprimeTotalDebCred = True
				   Print PBSaldoDebCredMes
			   Else
				   Print PBSaldoMes
				Endif
				&DataAuxiliar = CTBLancData
			    &TotalCredito.SetEmpty()
			    &TotalDebito.SetEmpty()
			     &saldo.SetEmpty()
			 Endif
		Endif

			If &Contador = 1
				Print PBConta
				If &ImprimeCCusto
					Print PBTitCC
				Else
					Print PBTit
				Endif
			Endif	
	  EndFor
EndFor
 
Footer
	Print PBFooter
	&Pagina += 1
End

Sub 'SaldoInicial'
	 &SaldoInicial.SetEmpty()
	 For Each CTBLanc Order CTBLancData CTBLancId
		 Where CTBlancEmpId       = &CTBEmpId
		 Where CTBLancPlanoDebId = &CtBPalnoId  OR CTBLancPlanoCredId  = &CTBPlanoId
		 Where CTBLancData  < &CTBLancDataEntradaInicio
		 
		 Do Case
			 Case CTBLancPlanoDebId     = &CTBPlanoId
				 &CTBLancValorDebito        = Trim(CTBLancValor.ToFormattedString())
				 &CTBLancValorCredito       =  ''
				 
			 Case CTBLancPlanoCredId    = &CTBPlanoId
			 	&CTBLancValorDebito         = ''
				&CTBLancValorCredito    = Trim(CTBLancValor.ToFormattedString())
		EndCase
		
		&SaldoInicial += &CTBLancValorDebito.Replace( '.', '').Replace(',','.').ToNumeric() - &CTBLancValorCredito.Replace('.','').Replace(',','.').ToNumeric()		 
		 
	EndFor
	
	&SaldoInicialString = Trim(&SaldoInicial.ToFormattedString())
	&SaldoInicialString = &SaldoInicialString.Replace('-','')
	
	Do Case
		Case &SaldoInicial > 0
			&DebCred = 'D'
		Case &SaldoInicial < 0
			&DebCred = 'C'
		OtherWise
			&DebCred = ''
	EndCase
EndSub