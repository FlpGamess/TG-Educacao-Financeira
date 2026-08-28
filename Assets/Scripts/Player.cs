using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//
//Script do player
//Responsavel pelos atributos do player financeiros
//Stamina, entre outros referentes a ele
public class Player : MonoBehaviour
{
    public static IDictionary<AtributosFinanceiros, int> AtbFinanceiros = new Dictionary<AtributosFinanceiros, int>()
    {
        {AtributosFinanceiros.DespesasDoLar,0},
        {AtributosFinanceiros.Moradia,0},
        {AtributosFinanceiros.Lazer,0},
        {AtributosFinanceiros.SaudeBemEstar,0},
        {AtributosFinanceiros.Educacao,0}
    };
    [Header("Atributos")]
    //total na conta do jogador
    public float patrimonio = 0;
    public int desplar;
    public int educ;
    public int morad;
    public int saube;
    public int laz;

    [Header("Listas")]
    public List<Itens> Bens = new List<Itens>();

    [Header("Listas2")]
    public List<Despesas> Dividas = new List<Despesas>();

    [Header("Modulos")]
    public ModuloTempo moduloTempo;
    public ModuloRendimentos moduloRendimentos;

    [Header("Interfaces")]
    public TextMeshProUGUI saldocontav;
    public Image slot1;
    public Image slot2;
    public Image slot3;
    public Image slot4;
    public Image slot5;
    public Sprite spriteDesplar;
    public Sprite spriteEduc;
    public Sprite spriteMorad;
    public Sprite spriteSaube;
    public Sprite spriteLaz;

    public static event Action BensAtualizados;


    void Start()
    {
        patrimonio = moduloRendimentos.salario;
        AlterarSaldoConta();
        ModuloTempo.isSemanaAvancada += AtualizarPatrimonio;

        slot1.sprite = spriteDesplar;
        slot2.sprite = spriteEduc;
        slot3.sprite = spriteMorad;
        slot4.sprite = spriteSaube;
        slot5.sprite = spriteLaz;
    }

    void AtualizarPatrimonio()
    {
        if (ModuloTempo.semana > 1 &&(ModuloTempo.semana - 1) % 4 == 0)
        {
            patrimonio += moduloRendimentos.salario;
            AlterarSaldoConta();
        }
    }

    public void ProcessarCompra(Itens bem,Despesas despesa)
    {
        Bens.Add(bem);
        Dividas.Add(despesa);
        BensAtualizados.Invoke();

    }

    //deletar dps
    public void DebitarPagamento(float preco)
    {

        if (patrimonio-preco >= 0)
        {
            patrimonio -= preco;
            AlterarSaldoConta();
        }
        return;

    }

    public void AlterarSaldoConta()
    {
        saldocontav.text = "$" + patrimonio.ToString("F2");
    }


}
