LoadWWPContext(&Context)
&CTBUsuId	= &Context.CTBUsuId
&CTBUsuNome	= &Context.CTBUsuNome


For Each CTBEmp
	Where CTBEmpId = &CTBEmpId
	&CTBEmpRazaoSocial	= CTBEmpRazaoSocial
	&CTBEmpCNPJ			= CTBEmpCNPJ
Endfor

&Pagina = 2

Header
     Print PBCabec
     Print PBHeader
	 if &ImprimeCCusto = True
	Print PBTitCC
else
	Print PBTitLanc
endif	
End






&SaldoDia.SetEmpty()
&SaldoMes.SetEmpty()
&TotalCredito.SetEmpty()
&TotalDebito.SetEmpty()
&soma.SetEmpty()
&contador = 0

&DataAuxiliar = &CTBLancDataEntradaInicial

For Each CTBLanc Order CTBLancData CTBLancId
	Where CTBLancEmpId = &CTBEmpId
	Where CTBLancData >= &CTBlancDatainicio
	Where CTBLancData <= &CTBLancDataFinal
	
	&contador		   	   += 1
	
	If &ImprimeTotalDia = True And &contador <> 1
		If DAy(&DataAuxiliar) <> Day(CTBLancData) AND Month(&DataAuxiliar) <> Month(CTBLancData) OR
		   Day(&DataAuxiliar) = Day(CTBLancData)  AND Month(&DataAuxiliar) <> Month(CTBLancData) OR
		   Day(&DataAuxiliar) <> Day(CTBLancData) AND Month(&DataAuxiliar) = Month(CTBLancData)	
			&SaldoDia += CTBLancValor 
			Print PBSaldoDia	
// impr deb cred		
			If &ImprimeDebCred = True
	 	    	If NOT CTBLancPlanoDebId.IsEmpty()
                   	&TotalDebito  += CTBLancValor
           	     EndIf
           	     If NOT CTBLancPlanoCredId.IsEmpty()
                     &TotalCredito += CTBLancValor
           		EndIf
				&SaldoDia += CTBLancValor
				Print PBSaldoDebCredDia
		 	ENDIF
//sem movimento dia 
			if &ImprimeSemMov = true and Day(&DataAuxiliar) <> Day(CTBLancData)  AND &contador = 0  
				Print PBTitLanc
				Print PBSemMovDia
			endif
			&DataAuxiliar= CTBLancData
			&TotalCredito.SetEmpty()
			&TotalDebito.SetEmpty()
			&soma.SetEmpty()
		Endif
		// salta pagina
		
		
//		If &imprimeSaltaMes = true
//			month(&DataAuxiliar) <> Month(CTBLancData)
//			Eject	
//		Endif
Endif

	
	If &ImprimeTotalMes = True AND &Contador <> 1
		If Month(&DataAuxiliar) <> Month(CTBLancData)
			&SaldoMes += CTBLancValor
			Print PBSaldoMes
//deb cred			
			If &ImprimeDebCred = True					
				If NOT CTBLancPlanoDebId.IsEmpty()
                   	&TotalDebito  += CTBLancValor
           	     EndIf
           	     If NOT CTBLancPlanoCredId.IsEmpty()
                     &TotalCredito += CTBLancValor
           		EndIf	
				Print PBSaldoDebCredMes 					
			EndIf
//sem mov		
		if &ImprimeSemMov = true and Month(&DataAuxiliar) <> Month(CTBLancData)  And &contador = 0 
				Print PBTitLanc
				Print PBSemMovMes
		endif
			&DataAuxiliar= CTBLancData
			&TotalCredito.SetEmpty()
			&TotalDebito.SetEmpty()
			&soma.SetEmpty()
   Endif
//salta mes		
//		If &imprimeSaltaMes= true
//				month(&DataAuxiliar) <> Month(CTBLancData)  
//				Eject
Endif	
			
		&CTBLancData        	= CTBLancData 
		&CTBLancId          	= CTBLancId
	
    	&CTBLancPlanoDebConta	= CTBLancPlanoDebConta
	    &CTBLancPlanoCredConta	= CTBLancPlanoCredConta
	
     	&CTBLancHistId      	= CTBLancHistId
	    &CTBLancHistDesc    	= CTBLancHistDesc
	    &CTBLancHistObs	    	= CTBLancHistObs
	    &CTBLancValor       	= CTBLancValor
	    &CTBLancCustoDebId  	= CTBLancCustoDebId
	    &CTBLancCustoCredId 	= CTBLancCustoCredId
	
	&soma         	   	   += CTBLancValor
	
	If NOT CTBLancPlanoDebId.IsEmpty()
		&TotalDebito  += CTBLancValor
	EndIf
	
	If NOT CTBLancPlanoCredId.IsEmpty()
		&TotalCredito += CTBLancValor
	EndIf
	
	If &ImprimeCCusto = true
		Print PBLancCC
	else		
		Print PBLanc
	Endif
	
When none
	if &ImprimeSemMov = True and &Contador = 0
		Print PBTitLanc
		Print PBSemMov
	Endif
	
Endfor


If &ImprimeDebCred = true
	Print PBSaldoDebCred
else
	Print PBSaldo
Endif


Footer
	Print PBFooter
	&Pagina += 1
End

