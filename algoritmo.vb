algoritmo "Promo_de_livros"
// Função : aplicar desconto em livros
// Autor :  Sara
// Data : 09/11/2021
// Seção de Declarações 
var

DA : real
DB : Real
DC : Real
N: Real
MD : Real
SN : Caractere

Inicio
escreval("--------------------------------------------------------------")
escreval("")
escreval(" -------APROVEITE NOSSA GRANDE PROMOÇÃO ----------------------")
escreval("")
escreval("--------------------------------------------------------------")
 
Escreva("Qual a quantidade de livros " )
Leia(N)
DA <- (0.25* N) + 7.50
DB <- (0.50* N)  + 2.50
DC <- (0.65* N) + 1.50

 Escreval( "O desconto A é igual a R$...........", DA )
 Escreval(" O desconto B é igual a R$..........." , DB)
 EscrevaL ("O desconto C é igual a R$........... " , DC)

  escreval("Quer saber qual o melhor desconto  (S/N) ?")
  Leia(SN)
  
Se (SN)= "S" entao
  Se ( DA > DB ) OU ( DA > DC ) entao
      MD <-DA
      FimSe
        Se ( DB > DA ) OU ( DB > DC ) entao
            MD <-  DB
        Fimse
            Se ( DB > DC ) OU ( DB > DA ) entao
                MD <- DC
            FimSe
            escreval("--------------------------------------------------------------")
            escreval ("")
            escreval(" ----------------- O melhor desconto é de R$ -----------------")
            escreval("                          ", MD , "                           ")
            escreval("--------------------------------------------------------------")
 senao
    Se (SN) "N" entao
      escreval("obrigado por usar nosso sistema")
   FimSe
FimSe

fimalgoritmo