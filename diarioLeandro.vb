
For Each CTBEmp
	Where CTBEmpId = &CTBEmpId
	&CTBEmpRazaoSocial	= CTBEmpRazaoSocial
	&CTBEmpCNPJ			= CTBEmpCNPJ
Endfor

&Pagina = 2

Header
     Print PBCabec
     Print PBHeader
     Print PBTitLanc
End


&soma.SetEmpty()
&PrimeiraData = &CTBLancDataInicio


For Each CTBLanc order CTBLancData , CTBLancId	
	Where CTBLancEmpId = &CTBEmpId
	Where CTBLancData  >= &CTBLancDataInicio
	Where CTBLancData  <= &CTBLancDataFinal
	
	
	If &PrimeiraData <> CTBLancData and NOT &soma.IsEmpty()		
		Print PBTotais			
		&PrimeiraData = CTBLancData
		&soma.SetEmpty()
		Eject
	EndIf
	
	&CTBLancData        = CTBLancData 
	&CTBLancId          = CTBLancId
	&CTBLancPlanoDebId  = CTBLancPlanoDebId  // tem que ser a conta analitica 
	&CTBLancPlanoCredId = CTBLancPlanoCredId // tem que ser a conta analitica
	&CTBLancHistId      = CTBLancHistId
	&CTBLancHistDesc    = CTBLancHistDesc
	&CTBLancValor       = CTBLancValor
	
	
    
    &soma += CTBLancValor
	
	Print PBLanc
	
When None
	
	Print PBSemMov
	
Endfor


Print PBTotais

Footer
	Print PBFooter
	&Pagina += 1
End
