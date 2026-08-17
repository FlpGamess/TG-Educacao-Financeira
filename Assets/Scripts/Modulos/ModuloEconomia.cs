using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public enum TipoInvestimento
{
    Poupanca,
    CDB,
    Fundos,
}

[System.Serializable]
public class DadosInvestimento
{
    public TipoInvestimento tipo;
    [TextArea]
    public string descricao;
    public float percentualCDI;
    public float valorInvestido;
    public int semanaInvestimento = -1;
}

public class ModuloEconomia : MonoBehaviour
{
    public List<DadosInvestimento> bancos;
    public Player player;
    public GameObject prefabItemInvestimento;
    public Transform content;
    public float cdiSemanal = 0.002f;
    public Button btnInvestimentos;
    private bool valorTot = false;

    private List<(DadosInvestimento banco, GameObject item)> itensInstanciados = new List<(DadosInvestimento, GameObject)>();

    public void OnEnable()
    {
        Player.BensAtualizados += DebitarDespesas;
        ModuloTempo.isSemanaAvancada += DebitarDespesas;

    }

    public void OnDisable()
    {
        Player.BensAtualizados -= DebitarDespesas;
        ModuloTempo.isSemanaAvancada -= DebitarDespesas;
    }

    void Start()
    {
        ModuloTempo.isSemanaAvancada += RenderInvestimentos;
        ModuloTempo.isSemanaAvancada += AtualizarVisibilidadeBotao;
        ModuloTempo.isSemanaAvancada += TempoGravado;
        
        AtualizarVisibilidadeBotao();
        // MontarLista();
    }

    public void DebitarDespesas()
    {
        foreach (Despesas despesa in player.Dividas)
        {
                Parcela parcela = despesa.parcelas.FirstOrDefault();
                if (parcela != null && parcela.semana == ModuloTempo.semana)
                {
                    player.DebitarPagamento(parcela.valor);
                    despesa.parcelas.RemoveAt(0);
                }
            
        }
    }

    void TempoGravado()
{
    // Se estiver na semana 4, ele avalia e grava o resultado
    if (ModuloTempo.semana % 4 == 0)
    {
        if (player.patrimonio >= 100)
        {
            valorTot = true;  
        }
        else
        {
            valorTot = false; 
        }
    }
}

    void AtualizarVisibilidadeBotao()
    {
        btnInvestimentos.gameObject.SetActive(ModuloTempo.semana >= 5 && valorTot);
    }

    public void MontarLista()
    {
        foreach (var par in itensInstanciados)
            Destroy(par.item);
        itensInstanciados.Clear();

        foreach (var banco in bancos)
        {
            GameObject item = Instantiate(prefabItemInvestimento, content);
            itensInstanciados.Add((banco, item));

            item.transform.Find("PainelTexto/TextoNome").GetComponent<TextMeshProUGUI>().text = $"{banco.tipo}: {banco.percentualCDI}%";
            item.transform.Find("PainelTexto/TextoDescricao").GetComponent<TextMeshProUGUI>().text = banco.descricao;

            TMP_InputField input = item.GetComponentInChildren<TMP_InputField>();
            Button btn = item.transform.Find("PainelValor/PainelBotoes/BtnInvestir").GetComponent<Button>();
            Button btnRetirar = item.transform.Find("PainelValor/PainelBotoes/BtnRetirar").GetComponent<Button>();
            btn.onClick.AddListener(() => Investir(banco, input));
            btnRetirar.onClick.AddListener(() => Resgatar(banco));

            Canvas.ForceUpdateCanvases(); 
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        }
        AtualizarUI();
    }

    public void Investir(DadosInvestimento banco, TMP_InputField input)
    {
        if (!float.TryParse(input.text, out float valor) || valor <= 0)
        {
            Debug.Log("Digite um valor válido para ser investido.");
            return;
        }
        if (player.patrimonio < valor)
        {
            Debug.Log("Saldo insuficiente!");
            return;
        }
        player.patrimonio -= valor;
        banco.valorInvestido += valor;
        banco.semanaInvestimento = ModuloTempo.semana;

        player.AlterarSaldoConta();
        AtualizarUI();

        input.text = "";
    }

    bool PodeResgatar(DadosInvestimento banco)
    {
        if (banco.semanaInvestimento < 0) return false;
        return (ModuloTempo.semana - banco.semanaInvestimento) >= 4;
    }

    void AtualizarUI()
    {
        foreach (var par in itensInstanciados)
        {
            var textoValor = par.item.transform.Find("PainelTexto/TextoValor").GetComponent<TextMeshProUGUI>();
            textoValor.text = "Investido: R$" + par.banco.valorInvestido.ToString("F2");

            bool podeResgatar = PodeResgatar(par.banco);
            Button btnRetirar = par.item.transform.Find("PainelValor/PainelBotoes/BtnRetirar").GetComponent<Button>();
            btnRetirar.interactable = podeResgatar;

            //CanvasGroup cg = par.item.GetComponent<CanvasGroup>();
            //if (cg == null) cg = par.item.AddComponent<CanvasGroup>();
            //cg.alpha = podeResgatar ? 1f : 0.5f;
        }
    }

    void RenderInvestimentos()
    {
        foreach (var banco in bancos)
        {
            if (banco.valorInvestido <= 0) continue;

            float taxaSemanal = (banco.percentualCDI / 100f) * cdiSemanal;
            banco.valorInvestido *= (1 + taxaSemanal);
        }

        AtualizarUI();
    }

    public void Resgatar(DadosInvestimento banco)
    {
        player.patrimonio += banco.valorInvestido;
        banco.valorInvestido = 0;
        AtualizarUI();
        player.AlterarSaldoConta();
    }

    void ResgatarTudo()
    {
        float totalResgatado = 0;
        foreach (var banco in bancos)
        {
            totalResgatado += banco.valorInvestido;
            banco.valorInvestido = 0;
        }
        player.patrimonio += totalResgatado;
        AtualizarUI();
        player.AlterarSaldoConta();
    }
}