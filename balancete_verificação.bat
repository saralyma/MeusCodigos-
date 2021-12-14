LoadWWPContext(&Context)
&CTBUsuId	= &Context.CTBUsuId
&CTBUsuNome	= &Context.CTBUsuNome

&Termo_Abertura_Data_Extenso = 'Paraná, ' + Trim(Str(Day(&Today))) + ' de ' + Trim(CMonth(&Today)) + ' de ' + Trim(Str(Year(&Today)))
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
if &ImprControlPAg = True  
	&ImprPagina = &ImprPagina+ 1 
	Print PBCabec
	Print PBHeader
	Print PBContaTit2
else
	Print PBCabec
	Print PBHeader
	Print PBContaTit2
	 
endif	
End


PCTBBalancete(&CTBLancDataInicial, &CTBLancDatafinal, &sdtCTBBalancete, &sdtQVBalancete)


For &sdtCTBBalanceteItem In &sdtCTBBalancete
	
	&CTBPlanoContaAgrup		= &sdtCTBBalanceteItem.CTBPlanoContaAgrup
	&CTBPlanoDesc			= &sdtCTBBalanceteItem.CTBPlanoDesc
	&SaldoInicialString		= &sdtCTBBalanceteItem.CTBPlanoSaldoAnteriorStr
	&CTBPlanoTotalDebito	= &sdtCTBBalanceteItem.CTBPlanoTotalDebito
	&CTBPlanoTotalCredito	= &sdtCTBBalanceteItem.CTBPlanoTotalCredito
	&CTBPlanoTotalSaldo		= &sdtCTBBalanceteItem.CTBPlanoTotalSaldo
	&CTBPlanoTotalSaldoStr	= &sdtCTBBalanceteItem.CTBPlanoTotalSaldoStr
	
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
	
	
	If &ImpContasSemSaldo OR &CTBPlanoTotalSaldo <> 0
		If &sdtCTBBalanceteItem.CTBPlanoTipoAS = PlanoTipoAS.Analitica
			Print PBConta2
		Else
			Print PBConta2Bold
		EndIf
	EndIf
	
EndFor


If &sdtCTBBalancete.Count.IsEmpty()
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
	
	Print PBTotalGrupos
	Print PBTotal
	
EndIf


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
	
	If &DiferencaAtivoPassivo_Money.IsEmpty()
		Print PBSemDif
	Else
		Print PBComDif
	EndIf
	
EndIf


If NOT &sdtCTBBalancete.Count.IsEmpty() AND &ImpAssinatura
	Print PBAssinatura
	
EndIf


Footer
	Print PBFooter
	&ImprPagina = &ImprPagina+1
End
