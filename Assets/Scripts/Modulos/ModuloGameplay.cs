using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]

public class GanhoSalvo
{
    public int mes;

    public float valor;
}
[System.Serializable]
public class AtributoFinanceiroSalvo
{
    public AtributosFinanceiros categoria;
    public float valor;
}
[System.Serializable]
public class BemSalvo
{
    public string itemId;
    public int duracaoAtual;
}
[System.Serializable]
public class ParcelaSalva
{
    public float valor;
    public int semana;
    public int numero;
}

[System.Serializable]
public class DividaSalva
{
    public string itemId;
    public AtributosFinanceiros categoria;
    public TipoPagamento tipocompra;
    public float juros;
    public bool isPaga;
    public float valor;
    public List<ParcelaSalva> parcelas = new List<ParcelaSalva>();
}
[System.Serializable]
public class Salvamento
{
    public float patrimonio;
    public float GanhoMensal;
    public float GanhoAnterior;
    public int semana;
    public int mes;
    public int disposicao;

    public List<BemSalvo> Bens = new List<BemSalvo>();

    public List<DividaSalva> Dividas = new List<DividaSalva>();

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
        var itensPorId = new Dictionary<string, Itens>();
        foreach (var item in Resources.LoadAll<Itens>("ScriptableObjects"))
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
            foreach (var registro in save.Bens)
            {
                if(registro == null || string.IsNullOrWhiteSpace(registro.itemId) || !itensPorId.TryGetValue(registro.itemId,out Itens item))
                {
                    Debug.LogError("Não foi possivel localizar um item");
                    return false;
                }

                bens.Add(new ItensComprados(item)
                {
                    DuracaoAtual = registro.duracaoAtual
                });
            }
        }

        if(save.Dividas != null)
        {
            foreach (var registro in save.Dividas)
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

                foreach (var parcela in registro.parcelas)
                {
                    if(parcela == null)
                    {
                        Debug.LogError("Parcela inválida no salvamento");
                        return false;
                    }
                    despesa.parcelas.Add(new Parcela(parcela.valor, parcela.semana, parcela.numero));
                }

                dividas.Add(despesa);
            }
        }
        return true;
    }

        
    
   public void SalvarJogo(Player player, ModuloDisposicao moduloDisposicao, ModuloEconomia moduloEconomia)
{
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

    foreach (var bem in player.Bens)
        {
            if (bem == null || bem.dados == null || string.IsNullOrWhiteSpace(bem.dados.Id))
            {
                Debug.LogError("Não foi possivel salvar: bem sem item ou ID.");
                return;
            }

            save.Bens.Add(new BemSalvo
            {
                itemId = bem.dados.Id,
                duracaoAtual = bem.DuracaoAtual
            });
        }
    
    foreach (var despesa in player.Dividas)
        {
            if (despesa == null || despesa.item == null || string.IsNullOrWhiteSpace(despesa.item.Id) || despesa.parcelas == null)
            {
                Debug.LogError("Não foi possivel salvar: bem sem item ou ID ou parcelas.");
                return;
            }

            DividaSalva divida = new DividaSalva
            {
                itemId = despesa.item.Id,
                categoria = despesa.categoria,
                tipocompra = despesa.tipocompra,
                juros = despesa.juros,
                isPaga = despesa.isPaga,
                valor = despesa.valor
            };

            foreach (var parcela in despesa.parcelas)
            {
                if (parcela == null)
                {
                    Debug.LogError("Não foi possível salvar: parcela inválida.");
                    return;

                }
                divida.parcelas.Add(new ParcelaSalva
                {
                    valor = parcela.valor,
                    semana = parcela.semana,
                    numero = parcela.numero
                });
            }      

            save.Dividas.Add(divida);
        }

    string json = JsonUtility.ToJson(save);
    PlayerPrefs.SetString("salvamento", json);
    PlayerPrefs.Save();

    Debug.Log("Jogo salvo! JSON: " + json);
}

public bool CarregarJogo(Player player, ModuloDisposicao moduloDisposicao, ModuloEconomia moduloEconomia)
    {
        if(!PlayerPrefs.HasKey("salvamento"))
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