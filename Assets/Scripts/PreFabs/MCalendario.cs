using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static XCharts.Runtime.RadarCoord;

public class MCalendario : MonoBehaviour
{
    public TextMeshProUGUI mes;
    public GameObject btnRetornar;
    public GameObject btnAvancar;
    public TextMeshProUGUI[] semanascalendario;
    public Transform[] blocossemanas;
    public List<BlocoCalendario> blocosCriados = new List<BlocoCalendario>();
    public BlocoCalendario blocoprefab;
    public List<BlocoCalendario> indicadoresCriados = new List<BlocoCalendario>();
    public Transform indicadores;
    public Player player;
    public int mesCalendario;

    public void CarregarMCalendario()
    {
        mesCalendario = ModuloTempo.mes;
        ModuloInterface.AtualizarTxt(mes, "Mês", mesCalendario.ToString());
        CarregarSemanasCalendario();
        CarregarIndicadores();

    }

    public void BtnsCalendario(int avanco)
    {
        mesCalendario += avanco;
        if(mesCalendario < ModuloTempo.mes)
        {
            mesCalendario = ModuloTempo.mes;
        }
        ModuloInterface.AtualizarTxt(mes, "Mês", mesCalendario.ToString());
        CarregarSemanasCalendario();
       
    }


    public void CarregarSemanasCalendario()
    {
        int[] semanasm = ModuloTempo.SemanasDMes(mesCalendario);
        LimparBlocos(blocosCriados); 
        for (int i = 0;semanascalendario.Length> i; i++)
        {

            ModuloInterface.AtualizarTxt(semanascalendario[i], "Semana", semanasm[i].ToString());
        }
        GerarBlocos(semanasm);
    }
    public void LimparBlocos( List<BlocoCalendario> blocos)
    {
        foreach (BlocoCalendario bloco in blocos)
        {
            Destroy(bloco.gameObject);
        }

        blocos.Clear();
    }

    public void GerarBlocos(int[] semanas)
    {

        for (int i = 0; i < semanas.Length; i++) {
            List<ParcelaCalendario> parcelas = player.FiltrarParcelasSemana(semanas[i]);

            foreach (ParcelaCalendario parcela in parcelas) {
                BlocoCalendario bloco = Instantiate(blocoprefab,blocossemanas[i]);
                RectTransform blocoRect = bloco.GetComponent<RectTransform>();
                RectTransform imagemRect = bloco.fundo.GetComponent<RectTransform>();
                ModuloInterface.AtualizarTxt(bloco.nome, "", parcela.nome);
                ModuloInterface.AtualizarTxt(bloco.valor, "", parcela.valor.ToString("F2"));
                bloco.fundo.color = ModuloInterface.CoresAtributosFinanceiros(parcela.categoria);
                blocosCriados.Add(bloco);

            }

        }
        
        
    }
    public void CarregarIndicadores()
    {
        LimparBlocos(indicadoresCriados);
        foreach (AtributosFinanceiros atb in Player.AtbFinanceiros.Keys)
        {
            BlocoCalendario celula = Instantiate(blocoprefab, indicadores);
            ModuloInterface.AtualizarTxt(celula.nome, "", atb.ToString());
            ModuloInterface.AtualizarTxt(celula.valor,"R$ ",Player.AtbFinanceiros[atb].ToString());
            celula.fundo.color = ModuloInterface.CoresAtributosFinanceiros(atb);
            ModuloInterface.DefinirTamanhoMinimo(celula.GetComponent<LayoutElement>(), 100f, 50f);

            indicadoresCriados.Add(celula);
        }
    }
}
