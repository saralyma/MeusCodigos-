//---------------------------------------------------------------------------------------------------------------
// CONTEXT
//--------------------------------------------------------------------------------------------------------------
LoadWWPContext(&Context)
&CTBUsuId	= &Context.CTBUsuId
&CTBUsuNome	= &Context.CTBUsuNome

For Each CTBEmp
	Where CTBEmpId = &Context.CTBEmpId
	&CTBEmpId			= CTBEmpId
	&CTBEmpRazaoSocial	= CTBEmpRazaoSocial
	&CTBEmpCNPJ			= CTBEmpCNPJ
//--------------------------------------------------------------------------------------	
EndFor

&Pagina = 2

Header
	Print PBCabec
	Print PBHeader
End


////-------------filtros------------------------------------------------
//// Atribuindo valores as variaveis
////--------------------------------------------------------------------
	&ImprimeTodasContas        = False
	&ImprimeCentroCusto        = False
	&ImprimeCodRed             = False
	&ImprimeContasSemMov       = False
	&ImprimePaginaEntreContas  = False
	&ImprimeTotalDia           = False 
	&ImprimeTotalMes           = False
	&ImprimirTotalDebCred      = False
////--------------------------------------------------------------------
////                   Seleção da conta 
////-------------------------------------------------------------------
For Each CTBPlano Order CTBPlanoConta
 	Where CTBPlanoTipoAS = PlanoTipoAS.Analitica
	Where CTBPlanoConta  >= &CTBPlanoContaInicial WHEN &ImprimeTodasContas = False
	Where CTBPlanoConta  <= &CTBPlanoContaFinal	  WHEN &ImprimeTodasContas = False
	
	&CTBPlanoId	= CTBPlanoId
	&CTBPlanoDesc = CTBPlanoDesc
	&DataAuxiliar = &CTBLancDataEntradaInicial
	
////-----------------------------Imprime codigo reduzido ---------------	
	If &ImprimeCodRed = True
		&CTBPlanoConta	= CTBPlanoCodRed.ToString()
	Else
		&CTBPlanoConta	= CTBPlanoConta
	EndIf
//--------------------------------------------------------------------------------------------
//// saldo das contas - Aanalisar  saldo final e saldo inicial  
//// se Houver saldo inicial - devera ser colocado , caso contrario , o saldo inicial sera zero.
//---------------------------------------------------------------------------------------------
	
	&SaldoCredito.SetEmpty()
	&SaldoDebito.SetEmpty()	
	&Saldo.SetEmpty()
	&SaldoFinal.SetEmpty()
	&Contador = 0
//-------------------------------------------------------------------------
//---------- Imprime os lancamentos
//-------------------------------------------------------------------------
	
	For Each CTBLanc Order CTBLancData CTBLancId
		Where CTBLancEmpId		 = &CTBEmpId
		Where CTBLancPlanoDebId	 = &CTBPlanoId OR CTBLancPlanoCredId = &CTBPlanoId
		Where CTBLancData		>= &CTBLancDataEntradaInicial
		Where CTBLancData		<= &CTBLancDataEntradaFinal
		Where CTBLancCustoDebId  <> 0 OR CTBLancCustoCredId <> 0
		&Contador += 1
			
//----------------------------------------------------------------------------
// Saldo das colunas debito e credito 
//----------------------------------------------------------------------------
			If &Contador = 1
				If CTBLancPlanoDebId.IsEmpty()
					&ContaAuxiliar = CTBLancPlanoCredId
				EndIf
				If CTBLancPlanoCredId.IsEmpty()
					&ContaAuxiliar = CTBLancPlanoDebId 
				EndIf
			EndIf

//--------------total dia ----------------------------------------------------
	
			If &ImprimeTotalDia = True 
				If (&DataAuxiliar <> CTBLancData AND &ContaAuxiliar <> CTBLancPlanoCredId) OR (&DataAuxiliar <> CTBLancData AND &ContaAuxiliar <> CTBLancPlanoDebId) 
					&SaldoFinal = &Saldo
					&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
					&SaldoFinalString = &SaldoFinalString.Replace('-', '')

					
//--------colunas com saldo debito e credito ------------------------------

					If &ImprimirTotalDebCred = True
						Print PBSaldoDiaDebCred
						Else
						PBsaldoDia
					EndIf

					&DataAuxiliar = CTBLancData
					&SaldoCredito.SetEmpty()
					&SaldoDebito.SetEmpty()
					&Saldo.SetEmpty()
				EndIf
			EndIf	
			
//-------------------total mes -----------------------------------------------

			If &ImprimeTotalMes = True
				If (Month(&DataAuxiliar) <> Month(CTBLancData) AND &ContaAuxiliar <> CTBLancPlanoCredId) OR (Month(&DataAuxiliar) <> Month(CTBLancData) AND &ContaAuxiliar <> CTBLancPlanoDebId)
					&SaldoFinal = &Saldo
					&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
					&SaldoFinalString = &SaldoFinalString.Replace('-', '')
					
//-------------------saldo debito e credito -------------------------------------			
					If &ImprimirTotalDebCred = True
						Print PBSaldoMesDebCred
					Else
						return
					EndIf
					
					&DataAuxiliar = CTBLancData
					&Saldo.SetEmpty()
					&SaldoCredito.SetEmpty()
					&SaldoDebito.SetEmpty()
				EndIf
			EndIf
		
//----------------------------------------------------------------------------------------------------------------
//  imprime cabeçalho com Centro de custo 
//-----------------------------------------------------------------------------------------------------------------
		If &ImprimeCentroCusto = True
				If &Contador = 1
					Print PBConta  
					Print PBTitCC  
					Do 'SaldoInicial'
					Print  PBSaldoInicial
				endif
			
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
		
//---------------imprime linha - calcula  debito - credito = Saldo da linha ------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
			If &Contador = 1
				&Saldo = &SaldoInicial + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric()
			Else
				&Saldo = &Saldo + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric() 
			EndIf
			
			&SaldoDebito  = &SaldoDebito + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric()
			&SaldoCredito = &SaldoCredito + &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric() 
			
			&SaldoString = Trim(&Saldo.ToFormattedString())
			&SaldoString = &SaldoString.Replace('-', '')
			
//--------------------------------------------------------------------------------------------------------------------------
//--------------Tipo de saldo - D ou C 
//-------------- quando for positivo  - colocar D 
//-------------- quando for negativo - colocar C 
//---------------------------------------------------------------------------------------------------------------------------
			
			If &Saldo > 0 
				&DebCred = 'D'
			EndIf 
			
			If &Saldo < 0
				&DebCred = 'C'
			endif
endIf // Endif do centro de custo 
//------------------------------------------------------------------------------------------------------------------
//------------imprime lancamentos com centro de custo --------------------------------------------------------------
			
		 If &ImprimeCentroCusto = True
			Print PBLancCusto
		 Endif
	 
		 If &ImprimeCentroCusto = False
			 Print PBlanc
		 endif
	 
//------------ Imprimir Complemento do Historico caso houver--------------------------------------------------------------
			If NOT &CTBLancHistObs.IsEmpty()
				Print PBCompl
			Endif
		
//------------------------------------------------------------------------------------------------------------------------
//------------ imprime contas sem movimento 
//--------------------------------------------------------------------------------------------------------------------------
	When None
		If &ImprimeContasSemMov = True	
				&Contador = 0
				Print PBSemMov
		EndIf
		
	EndFor
//------------------------------------------------------------------------------------------------------------------------------		
//------------- imprime saldo final , que é  a ultima linha impressa com o SALDO
//------------------------------------------------------------------------------------------------------------------------------		
		If &Contador > 0
			&SaldoFinal = &Saldo
			&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
			&SaldoFinalString = &SaldoFinalString.Replace('-', '')
			
// ----------------- imprime saldo das colunas de debito e credito -----------------------------------------------------------

			If &ImprimirTotalDebCred = True
				Print PBSaldoDebCred 
			Else
				return// verif
			EndIf
//-----------------salta pagina entre contas ---------------------------	
			If &ImprimePaginaEntreContas = True
				Eject
			EndIf
		EndIf
Endif  // endif para imprime com centro de custo * antes estava Else 
		
		if &ImprimeCentroCusto = False
					
		For Each CTBLanc Order CTBLancData CTBLancId
			Where CTBLancEmpId		 = &CTBEmpId
			Where CTBLancPlanoDebId	 = &CTBPlanoId OR CTBLancPlanoCredId = &CTBPlanoId
			Where CTBLancData		>= &CTBLancDataEntradaInicial
			Where CTBLancData		<= &CTBLancDataEntradaFinal
			
			&Contador += 1
			
			If &Contador = 1
				Print PBConta
				Print PBTitLanc
				Do 'SaldoInicial'
				Print  PBSaldoInicial
		EndIf
//---------------- filtro  de saldo de contas debito e credito -------------------------

				  If &Contador = 1
				     If CTBLancPlanoDebId.IsEmpty()
					        &ContaAuxiliar = CTBLancPlanoCredId
			      	EndIf

					 If CTBLancPlanoCredId.IsEmpty()
					        &ContaAuxiliar = CTBLancPlanoDebId 
				    EndIf
			    EndIf
//			
//	//-------------------------------------------------------------------------------------
//	// imprime total por dia - sem centro de custo
//	//-------------------------------------------------------------------------------------
//	If &ImprimeTotalDia = True 
//				If (&DataAuxiliar <> CTBLancData AND &ContaAuxiliar <> CTBLancPlanoCredId) OR (&DataAuxiliar <> CTBLancData AND &ContaAuxiliar <> CTBLancPlanoDebId) 
//					&SaldoFinal = &Saldo
//					&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
//					&SaldoFinalString = &SaldoFinalString.Replace('-', '')
//					
//	//------------------imprime saldo de debito e credito -------------------				
//					If &ImprimirTotalDebCred = True
//						Print PBSaldoDia 
//					Else
//						Return
//					EndIf
//					&DataAuxiliar = CTBLancData
//					&SaldoCredito.SetEmpty()
//					&SaldoDebito.SetEmpty()
//					&Saldo.SetEmpty()
//				EndIf
//			EndIf	
////	//------------------------------------------------------------------------------------------
////	//----------------------------------imprime total mes --------------------------------------
////	//------------------------------------------------------------------------------------------
//			If &ImprimeTotalMes = True
//				If (Month(&DataAuxiliar) <> Month(CTBLancData) AND &ContaAuxiliar <> CTBLancPlanoCredId) OR (Month(&DataAuxiliar) <> Month(CTBLancData) AND &ContaAuxiliar <> CTBLancPlanoDebId)
//					&SaldoFinal = &Saldo
//					&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
//					&SaldoFinalString = &SaldoFinalString.Replace('-', '')
//					
////------------------imprime saldo de debito e credito -------------------
//					If &ImprimirTotalDebCred = True
//						Print PBSaldoMes 
//					Else
//						return
//					EndIf
//				
//					&DataAuxiliar = CTBLancData
//					&Saldo.SetEmpty()
//					&SaldoCredito.SetEmpty()
//					&SaldoDebito.SetEmpty()
//				EndIf
//			EndIf
////	//--------------------------------------------------------------------------
////	//    Imprime lancamentos -sem centro de custo 
////	//---------------------------------------------------------------------------			
//			&CTBLancData		= CTBLancData
//			&CTBLancId			= CTBLancId
//			&CTBLancHistDesc	= CTBLancHistDesc
//			&CTBLancHistObs		= CTBLancHistObs
//			
//			Do Case
//				Case CTBLancPlanoDebId	        = &CTBPlanoId
//					&CTBLancPlanoDebCredConta	= CTBLancPlanoCredConta
//					&CTBLancValorDebito			= Trim(CTBLancValor.ToFormattedString())
//					&CTBLancValorCredito		= ''
//					
//				Case CTBLancPlanoCredId	= &CTBPlanoId
//					&CTBLancPlanoDebCredConta	= CTBLancPlanoDebConta
//					&CTBLancValorDebito			= ''
//					&CTBLancValorCredito		= Trim(CTBLancValor.ToFormattedString())				
//			EndCase					
//////-----------------------------------------------------------------------------------------------------------------------
////// Imprimir o saldo do lancamento 
//////-----------------------------------------------------------------------------------------------------------------------
//			If &Contador = 1
//				&Saldo = &SaldoInicial + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric()
//			Else
//				&Saldo = &Saldo + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric() - &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric() 
//			EndIf
//			
//			&SaldoString = Trim(&Saldo.ToFormattedString())
//			&SaldoString = &SaldoString.Replace('-', '')
//			
//			&SaldoDebito  = &SaldoDebito + &CTBLancValorDebito.Replace('.', '').Replace(',', '.').ToNumeric()
//			&SaldoCredito = &SaldoCredito + &CTBLancValorCredito.Replace('.', '').Replace(',', '.').ToNumeric() 
//	//-----------------------------------------------------------------------------------------------------------------------
//	//-----------------------imprime saldo de debito e credito----------------------------------------------------------------
//	//------------------------------------------------------------------------------------------------------------------------
//	
//			If &ImprimirTotalDebCred = True
//						Print PBSaldoMes 
//					Else
//						return
//					EndIf
//			
////---------------------------------------------------------------------------------------------------------------------------
//			// Tipo de saldo - D ou C 
//			// quando for positivo  - colocar D 
//			// quando for negativo - colocar C 
////----------------------------------------------------------------------------------------------------------------------------
//			If &Saldo > 0 
//				&DebCred = 'D'
//			EndIf 
//			
//			If &Saldo < 0
//				&DebCred = 'C'
//			endif
//			
//			Print PBLanc
////------------------------------------------------------------------------------------------------			
//// Imprimir Complemento do Historico caso houver
////-----------------------------------------------------------------------------------------------
//			If NOT &CTBLancHistObs.IsEmpty()
//				Print PBCompl
//			Endif
////----------------------------------------------------------------------------------------------
//// imprime contas sem movimento 
////-------------------------------------------------------------------------------------------------
//	When None
//			
//		If &ImprimeContasSemMov = True	
//			&Contador = 0
//			Print PBSemMov
//		EndIf	
EndFor
//-------------------------------------------------------------------------------------------------		
//		// imprime saldo final , que é  a ultima linha impressa com o SALDO
//-------------------------------------------------------------------------------------------------
		If &Contador > 0
			&SaldoFinal = &Saldo
			&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
			&SaldoFinalString = &SaldoFinalString.Replace('-', '')
			Print PBSaldo
		EndIf
//	EndIf	
EndFor

//---------------------------------------------------------------------------------------------------
// Somente para a última conta do relatório quando se está usando
//---------------------------------------------------------------------------------------------------
	If (&Contador > 0 AND &ImprimeTotalDia = True) OR (&Contador > 0 AND &ImprimeTotalMes = True)
		&SaldoFinal = &Saldo
		&SaldoFinalString = Trim(&SaldoFinal.ToFormattedString())
		&SaldoFinalString = &SaldoFinalString.Replace('-', '')
		
		If &ImprimirTotalDebCred = True
			Print PBSaldoDebCred 
		Else
			return 
		EndIf	
		
		////--------------salta pagina entre contas ---------------------------------------------------
		
		If &ImprimePaginaEntreContas = True
			Eject
		EndIf
	EndIf
Endif


//EndIf
//
//-------------------------------------------------------------------------
// ----------------imprime Rodape----------------------------------- 
//--------------------------------------------------------------------------
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
	
	If &SaldoInicial > 0 
		&DebCred = 'D'
	EndIf 
		
	If &SaldoInicial < 0
		&DebCred = 'C'
	EndIf 

EndSub




