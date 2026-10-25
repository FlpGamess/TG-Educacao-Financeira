using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]

public class GanhoSalvo //variaveis usadas para destrinchar os dicionarios vindo de player.HistoricoGanhos em uma lista serializavel
{
    public int mes;

    public float valor;
}
[System.Serializable]
public class AtributoFinanceiroSalvo //atribuição de variaveis que convertem os dicionarios de categoria financeiras(Player.AtbFinanceiras e Player.DebitosMensais) em lista
{
    public AtributosFinanceiros categoria;
    public float valor;
}
[System.Serializable]
public class Salvamento // Classe onde é agrupados as variaveis que serão salvas depois, sendo elas as de Tempo, valores, historico de pagamentos, investimentos e as listas de bens e dividas
{
    public float patrimonio;
    public float GanhoMensal;
    public float GanhoAnterior;
    public int semana;
    public int mes;
    public int disposicao;

    public List<ItensComprados> Bens = new List<ItensComprados>();

    public List<Despesas> Dividas = new List<Despesas>();

    public List<PagamentoRealizado> HistoricoPagamentos = new List<PagamentoRealizado>();

    public List<GanhoSalvo> Ganhos = new List<GanhoSalvo>();

    public List<AtributoFinanceiroSalvo> atributosFinanceiros = new List<AtributoFinanceiroSalvo>();
    public List<AtributoFinanceiroSalvo> debitosMensais= new List<AtributoFinanceiroSalvo>();

    public bool fimMes;

    public bool inicioMes;

    public List<DadosInvestimento> bancos = new List<DadosInvestimento>();

    public int semanaLiberaInvest = -1;

    public bool cartaoLib;

}
public class ModuloGameplay : MonoBehaviour
{
    public ModuloHistorico historicoBens;
    public MHistorico historicoFinanceiro;
    public MCalendario calendario;
    private bool ReconstruirBensDividas(

        Salvamento save,
        out List<ItensComprados> bens,
        out List<Despesas> dividas)
    {
        bens = new List<ItensComprados>();
        dividas = new List<Despesas>();
        var itensPorId = new Dictionary<string, Itens>();//JsonUtility não serializa instancias vindas de scriptableObjects, por isso bem e despesas tem que carregar a string do ID
        foreach (var item in Resources.LoadAll<Itens>("ScriptableObjects")) // carrega todos os scriptableObjects disponiveis e os indexa em um dicionario itensPorId.
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                Debug.Log("Produto sem ID: "+ item.name);
                return false;
            }

            if( itensPorId.ContainsKey(item.Id))
            {
                Debug.LogError("Id de produto duplicado: " + item.Id);
                return false;
            }
            itensPorId.Add(item.Id, item);
        }        
        if (save.Bens != null) 
        {
            foreach (var registro in save.Bens)// para cada item em Bens o item é buscado pelo itemId e recria a instância de ItemComprados mantendo assim a DuracaoAtual
            {
                if(registro == null || string.IsNullOrWhiteSpace(registro.itemId) || !itensPorId.TryGetValue(registro.itemId,out Itens item))
                {
                    Debug.LogError("Não foi possivel localizar um item");
                    return false;
                }

                bens.Add(new ItensComprados(item)
                {
                    DuracaoAtual = registro.DuracaoAtual
                });
            }
        }

        if(save.Dividas != null)
        {
            foreach (var registro in save.Dividas) // para cada despesa em Dividas o item é associado com o seu correspondente e restaura as parcelas do mesmo.
            {
                if(registro == null || string.IsNullOrWhiteSpace(registro.itemId) || !itensPorId.TryGetValue(registro.itemId,out Itens item) || registro.parcelas == null)
                {
                    Debug.LogError("Não foi possivel localizar um item ou parcela inválida");
                    return false;
                }

                Despesas despesa = new Despesas(item, registro.tipocompra, registro.valor)
                {
                    categoria = registro.categoria,
                    juros = registro.juros,
                    isPaga = registro.isPaga,
                };

                despesa.parcelas = new List<Parcela>(registro.parcelas);

                dividas.Add(despesa);
            }
        }
        return true;
    }

        
    
   public void SalvarJogo(Player player, ModuloDisposicao moduloDisposicao, ModuloEconomia moduloEconomia)
{ //copia os valores vindos direto de PLayer, ModuloTempo, ModuloDisposicao e ModuloEconomia e atribiu esses valores em bens, dividas, historicoPagamentos e bancos.
    Salvamento save = new Salvamento
    {
        patrimonio = player.patrimonio,
        GanhoMensal = player.GanhoMensal,
        GanhoAnterior = player.GanhoAnterior,
        semana = ModuloTempo.semana,
        mes = ModuloTempo.mes,
        disposicao = moduloDisposicao.disposicao,
        HistoricoPagamentos = player.HistoricoPagamentos,
        fimMes = ModuloTempo.fimMes,
        inicioMes = ModuloTempo.inicioMes,
        bancos = moduloEconomia.bancos,
        semanaLiberaInvest = moduloEconomia.ObterSemanaLiberaInvest(),
        cartaoLib = moduloEconomia.ObterCartaoLib(),
        Bens = player.Bens,
        Dividas = player.Dividas,
    };
    foreach (var ganho in player.HistoricoGanhos) 
    {
        save.Ganhos.Add(new GanhoSalvo 
        { mes = ganho.Key, 
        valor = ganho.Value 
        });
    }
    foreach (var atributo in Player.AtbFinanceiros) 
    {
        save.atributosFinanceiros.Add(new AtributoFinanceiroSalvo 
        { 
        categoria = atributo.Key, 
        valor = atributo.Value 
        });
    }

    foreach (var debito in Player.DebitosMensais) 
    {
        save.debitosMensais.Add(new AtributoFinanceiroSalvo 
        { 
        categoria = debito.Key, 
        valor = debito.Value 
        });
    }

    string json = JsonUtility.ToJson(save);// feita a conversão em Json
    PlayerPrefs.SetString("salvamento", json);// armazena o JSON criado
    PlayerPrefs.Save();

    Debug.Log("Jogo salvo! JSON: " + json);
}

public bool CarregarJogo(Player player, ModuloDisposicao moduloDisposicao, ModuloEconomia moduloEconomia)
    {
        if(!PlayerPrefs.HasKey("salvamento"))// verifica se o json existe
        {
            Debug.LogWarning("Nenhum salvamento encontrado.");
            return false;
        }
        string json = PlayerPrefs.GetString("salvamento"); 
        if(string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("O salvamento está vazio.");
            return false;
        }

        Salvamento save;
        try
        {
            save = JsonUtility.FromJson<Salvamento>(json); 
        }
        catch(ArgumentException e)
        {
            Debug.LogError("Erro ao carregar o salvamento: " + e.Message);
            return false;
        }

        if(save == null)
        {
            Debug.LogError("Erro ao desserializar o salvamento.");
            return false;
        }

        if(!ReconstruirBensDividas(save, out var bens, out var dividas))
        return false;
        // atribuição de variveis aos modulos correspondentes, fazendo recalculos, no caso de investimentos os bancos são restaurados no ModuloEconomia e aplicado o RestaurarControles.
        player.Bens = bens;
        player.Dividas = dividas;

        player.patrimonio = save.patrimonio;
        player.GanhoMensal = save.GanhoMensal;
        player.GanhoAnterior = save.GanhoAnterior;
        ModuloTempo.semana = save.semana;
        ModuloTempo.mes = save.mes;
        ModuloTempo.inicioMes = (save.semana - 1) % 4 == 0;
        ModuloTempo.fimMes = save.semana % 4 == 0 && save.semana != 0;
        moduloEconomia.bancos = save.bancos ?? new List<DadosInvestimento>();
        moduloDisposicao.disposicao = save.disposicao;
        player.HistoricoPagamentos = save.HistoricoPagamentos ?? new List<PagamentoRealizado>();
        moduloEconomia.RestaurarControles(save.semanaLiberaInvest, save.cartaoLib);
        //Reconstrução dos dicionarios
        player.HistoricoGanhos.Clear();
        if(save.Ganhos != null)
        {
            foreach(var ganho in save.Ganhos)
                player.HistoricoGanhos[ganho.mes] = ganho.valor;
        }

        foreach (AtributosFinanceiros categoria in Enum.GetValues(typeof(AtributosFinanceiros)))
        {
         Player.AtbFinanceiros[categoria] =0f;
         Player.DebitosMensais[categoria] =0f;
        }

        if (save.atributosFinanceiros != null)
        {
            foreach (var atributo in save.atributosFinanceiros)
                Player.AtbFinanceiros[atributo.categoria] = atributo.valor;
        }

        if (save.debitosMensais != null)
        {
            foreach (var debito in save.debitosMensais)
                Player.DebitosMensais[debito.categoria] = debito.valor;
        }
        //Renotificação da UI
        player.AlterarSaldoConta();
        moduloDisposicao.AlterarDisposicao();
        ModuloInterface.AtualizarTxt(player.moduloTempo.semanav, "Semana", save.semana.ToString());
        moduloEconomia.AtualizarDepoisDeRestaurar();

        if (historicoBens != null && historicoBens.gameObject.activeInHierarchy)
            historicoBens.AbrirHistorico();
        if (historicoFinanceiro != null && historicoFinanceiro.gameObject.activeInHierarchy)
            historicoFinanceiro.CarregarHistorico();
        if (calendario != null && calendario.gameObject.activeInHierarchy)
            calendario.CarregarMCalendario();

        return true;
}
}