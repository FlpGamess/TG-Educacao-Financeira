using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using XCharts.Runtime;

public class MHistorico : MonoBehaviour
{
    public TextMeshProUGUI patrimonio;
    public Transform historicoContainer;
    public PagamentoRealizadoBloco blocoHistoricoPreFab;
    public Player player;
    public ModuloInterface modulointerface;
    public List<PagamentoRealizadoBloco> blocosCriados = new List<PagamentoRealizadoBloco>();
    public BaseChart graficoGG;
    public LineChart graficoLP;

    public ModuloEconomia moduloEconomia;

    public void CarregarHistorico()
    {
        LimparBlocos();
        ModuloInterface.AtualizarTxt(patrimonio, "R$ ", player.patrimonio.ToString("F2"));

        if (moduloEconomia == null) moduloEconomia = FindFirstObjectByType<ModuloEconomia>();

        if(moduloEconomia != null && moduloEconomia.bancos != null)
        {
            foreach (var banco in moduloEconomia.bancos)
            {
                if (banco.valorInvestido > 0)
                {
                 PagamentoRealizadoBloco blocoInv = Instantiate(
                    blocoHistoricoPreFab,
                    historicoContainer); 

                    ModuloInterface.AtualizarTxt(blocoInv.nome, "", $"Investimento em {banco.tipo}");
                    ModuloInterface.AtualizarTxt(blocoInv.valor, "+ R$ ", banco.valorInvestido.ToString("F2"));
                    ModuloInterface.AtualizarTxt(blocoInv.tipo, "", "Investimento");
                    ModuloInterface.AtualizarTxt(blocoInv.pagamento, "", "Ativo");
                    ModuloInterface.AtualizarTxt(blocoInv.semana, "", banco.semanaInvestimento >= 0 ? $"Sem. {banco.semanaInvestimento}" : "");
                    ModuloInterface.AtualizarTxt(blocoInv.parcela, "", $"{banco.percentualCDI}% CDI");

                    if (blocoInv.valor != null) blocoInv.valor.color = new Color32(46, 204, 113, 255);
                    var fundo = blocoInv.GetComponent<UnityEngine.UI.Image>();
                    if(fundo != null) fundo.color = new Color32(235,247,240,255);

                    blocosCriados.Add(blocoInv);

                }
            }
        }
        
        player.HistoricoPagamentos.Sort((a, b) => b.semana.CompareTo(a.semana));
        foreach (PagamentoRealizado p in player.HistoricoPagamentos)
        {
            PagamentoRealizadoBloco bloco = Instantiate(
                blocoHistoricoPreFab,
                historicoContainer
            );
            ModuloInterface.AtualizarTxt(bloco.nome, "", p.nome);
            ModuloInterface.AtualizarTxt(bloco.valor, "R$ ", p.valor.ToString("F2"));
            ModuloInterface.AtualizarTxt(bloco.semana, "Semana ", p.semana.ToString());
            ModuloInterface.AtualizarTxt(bloco.parcela, "Parcela ", p.parcela.ToString());
            ModuloInterface.AtualizarTxt(bloco.tipo, "", p.categoria.ToString());
            ModuloInterface.AtualizarTxt(bloco.pagamento, "", p.tpagamento.ToString());
            
            blocosCriados.Add(bloco);
        }

        Canvas.ForceUpdateCanvases();
        var scrollRect = historicoContainer.GetComponentInParent<UnityEngine.UI.ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
        ManipularGraficoGG();
        ManipularLp();
    }
  
    public void ManipularGraficoGG()
    {
        int mesFinal = Mathf.Max(1, ModuloTempo.mes - 1);
        int mesInicial = Mathf.Max(1, mesFinal - 2);
        List<int> meses = new List<int>();

        Dictionary<AtributosFinanceiros, Dictionary<int, float>> gastostotal = new Dictionary<AtributosFinanceiros, Dictionary<int, float>>();

        foreach (AtributosFinanceiros categoria in Player.AtbFinanceiros.Keys)
        {
            gastostotal[categoria] = new Dictionary<int, float>();
            for(int m = mesInicial; m <= mesFinal; m++)
            {
                gastostotal[categoria][m] = 0f;
            }
        }
        foreach(PagamentoRealizado pagamento in player.HistoricoPagamentos)
        {
            int mesPg = ((pagamento.semana-1) / 4)+1;
            if (mesPg < mesInicial || mesPg > mesFinal)
                continue;
            gastostotal[pagamento.categoria][mesPg] += pagamento.valor;
        }
        Dictionary<int,float> ganhostotais = new Dictionary<int, float>();
        for (int m = mesInicial; m <= mesFinal; m++)
        {
            meses.Add(m);
            ganhostotais[m] = player.HistoricoGanhos.TryGetValue(m,out float valor) ? valor : (m == ModuloTempo.mes ? player.GanhoMensal : 0f);
        }

        modulointerface.CriarGraficoBarraStLinha(graficoGG,meses,gastostotal,ganhostotais,"Historico da Conta nos Ultimos 3 Meses");

    }

    public void ManipularLp()
    {
        int mesFinal = Mathf.Max(1, ModuloTempo.mes - 1);
        int mesInicial = Mathf.Max(1, mesFinal - 2);

        Dictionary<int,float> saldoMensal = new Dictionary<int,float>();

        for (int m = mesInicial;m <= mesFinal; m++)
        {
            float ganhos = player.HistoricoGanhos.TryGetValue(m, out float g) ? g: (m == ModuloTempo.mes ? player.GanhoMensal :0f);
            float gastos = 0f;
            foreach (PagamentoRealizado pagamento in player.HistoricoPagamentos)
            {
                int mesPagamento = ((pagamento.semana - 1) / 4) + 1;
                if (mesPagamento == m)
                    gastos += pagamento.valor;
            }
            saldoMensal[m] = ganhos - gastos;
        }
        var series = new Dictionary<string, Dictionary<int, float>> {
                {"Saldo Mensal",saldoMensal },
            };

        modulointerface.CriarGraficoLinhaSimples(graficoLP, series,"Ganho - Gastos Mensal");

    }

   

    public void LimparBlocos()
    {
        foreach (PagamentoRealizadoBloco bloco in blocosCriados)
        {
            Destroy(bloco.gameObject);
        }
        blocosCriados.Clear();
    }
}
