using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

//Script do player
//Responsavel pelos atributos do player financeiros
//Stamina, entre outros referentes a ele
public class Player : MonoBehaviour
{
    //guarda os atributos financeiros do player
    /*despesas do lar = desplar
      educacao = educ
      moradia = morad
      saude_bem_estar = saube
      lazer = laz
     */
    public static IDictionary<AtributosFinanceiros, int> AtbFinanceiros = new Dictionary<AtributosFinanceiros, int>()
    {
        {AtributosFinanceiros.DespesasDoLar,0},
        {AtributosFinanceiros.Moradia,0},
        {AtributosFinanceiros.Lazer,0},
        {AtributosFinanceiros.SaudeBemEstar,0},
        {AtributosFinanceiros.Educacao,0}
    };

    //total na conta do jogador
    public float patrimonio = 0;
    public TextMeshProUGUI saldocontav;

    public ModuloTempo moduloTempo;

    void Start()
    {
        patrimonio = 1621.00f;
        AlterarSaldoConta();
        ModuloTempo.isSemanaAvancada += AtualizarPatrimonio;
    }

    void AtualizarPatrimonio()
    {
        if (ModuloTempo.semana > 1 && (ModuloTempo.semana - 1) % 4 == 0)
        {
            patrimonio += 1621.00f;
            AlterarSaldoConta();
        }
    }

    public void AlterarSaldoConta()
    {
        saldocontav.text = "$" + patrimonio.ToString();
    }


}
