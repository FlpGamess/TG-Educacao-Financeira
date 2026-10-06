using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//
//Script do player
//Responsavel pelos atributos do player financeiros
//Stamina, entre outros referentes a ele
public class Player : MonoBehaviour
{
    public static IDictionary<AtributosFinanceiros, float> AtbFinanceiros = new Dictionary<AtributosFinanceiros, float>()
    {
        {AtributosFinanceiros.DespesasDoLar,0f},
        {AtributosFinanceiros.Moradia,0f},
        {AtributosFinanceiros.Lazer,0f},
        {AtributosFinanceiros.SaudeBemEstar,0f},
        {AtributosFinanceiros.Educacao,0f}
    };

    public static IDictionary<AtributosFinanceiros, float> DebitosMensais = new Dictionary<AtributosFinanceiros, float>()
    {
        {AtributosFinanceiros.DespesasDoLar,0f},
        {AtributosFinanceiros.Moradia,0f},
        {AtributosFinanceiros.Lazer,0f},
        {AtributosFinanceiros.SaudeBemEstar,0f},
        {AtributosFinanceiros.Educacao,0f}
    };
    public float GanhoMensal = 0f;
    public float GanhoAnterior = 0f;

    [Header("Atributos")]
    //total na conta do jogador
    public float patrimonio = 0;
    public int desplar;
    public int educ;
    public int morad;
    public int saube;
    public int laz;

    [Header("Listas")]
    public List<ItensComprados> Bens = new List<ItensComprados>();

    [Header("Listas2")]
    public List<Despesas> Dividas = new List<Despesas>();


    public List<PagamentoRealizado> HistoricoPagamentos = new List<PagamentoRealizado>();

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
        AtualizarPatrimonio();
        //patrimonio = moduloRendimentos.salario;
        //AlterarSaldoConta();

        slot1.sprite = spriteDesplar;
        slot2.sprite = spriteEduc;
        slot3.sprite = spriteMorad;
        slot4.sprite = spriteSaube;
        slot5.sprite = spriteLaz;
    }

    void AtualizarPatrimonio()
    {
            patrimonio += moduloRendimentos.salario;
            GanhoMensal += moduloRendimentos.salario;
            AlterarSaldoConta();
        
    }

    public void ProcessarCompra(Itens bem,Despesas despesa)
    {
        Bens.Add(new ItensComprados(bem));
        Dividas.Add(despesa);
        AtbFinanceiros[despesa.categoria] += despesa.valor;

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

    void OnEnable()
    {
        ModuloTempo.isSemanaAvancada += RetirarItemHistorico;
        ModuloTempo.isMesAvancado += AtualizarGanhoMensal;
        ModuloTempo.isMesAvancado += AtualizarPatrimonio;
        ModuloTempo.isMesAvancado += AtualizarAtributosEconomicos;

    }

    void OnDisable()
    {
        ModuloTempo.isSemanaAvancada -= RetirarItemHistorico;
        ModuloTempo.isMesAvancado -= AtualizarGanhoMensal;
        ModuloTempo.isMesAvancado -= AtualizarPatrimonio;
        ModuloTempo.isMesAvancado -= AtualizarAtributosEconomicos;
    }

    void AtualizarGanhoMensal()
    {
        GanhoAnterior = GanhoMensal;
        GanhoMensal = 0f;
    }
    public void AlterarSaldoConta()
    {
        ModuloInterface.AtualizarTxt(saldocontav, "$", patrimonio.ToString("F2"));
    }

    public void AtualizarAtributosEconomicos()
    {
        foreach (AtributosFinanceiros atb in AtbFinanceiros.Keys.ToList())
        {
            AtbFinanceiros[atb] -= DebitosMensais[atb];
            DebitosMensais[atb] = 0f;
        };
    }

    public void RegistrarDespesaDebitada(AtributosFinanceiros atb, float valor)
    {
        DebitosMensais[atb] += valor;
    }

    void RetirarItemHistorico()
    {
        for ( int i = Bens.Count - 1; i >= 0; i--)
        {
            if (Bens[i].DuracaoAtual == -1)
            {
                continue;
            }
            Bens[i].DuracaoAtual--;

            if (Bens[i].DuracaoAtual <= 0)
            {
                Bens.RemoveAt(i);
            }
        }
                BensAtualizados.Invoke();

    }

    public List<ParcelaCalendario> FiltrarParcelasSemana(int semana)
    {
        List<ParcelaCalendario> parcelasf = new List<ParcelaCalendario>();

        foreach (Despesas despesa in Dividas)
        {
            foreach(Parcela parcela in despesa.parcelas)
            {
                if(parcela.semana == semana)
                {
                    parcelasf.Add(
                    new ParcelaCalendario(
                        despesa.item.Nome,
                        parcela.valor,
                        despesa.categoria
                    )
                );
                }
            }
        }
        return parcelasf;
    }


}
