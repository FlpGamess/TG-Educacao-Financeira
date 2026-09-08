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
    public float valorOriginal;  //Só para poupança
    public int semanaInvestimento = -1;
    public float mudanca = 0f;
    public float taxaAd = 0f;
}

public class ModuloEconomia : MonoBehaviour
{
    public List<DadosInvestimento> bancos;
    public Player player;
    public GameObject prefabItemInvestimento;
    public Transform content;
    public ModuloRendimentos moduloRendimento;
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

            TMP_InputField inputInvestir = item.transform.Find("PainelValor/LinhaInvestir/inputInvestir").GetComponent<TMP_InputField>();
            TMP_InputField inputResgate = item.transform.Find("PainelValor/LinhaResgatar/inputResgate").GetComponent<TMP_InputField>();
            Button btn = item.transform.Find("PainelValor/LinhaInvestir/BtnInvestir").GetComponent<Button>();
            Button btnRetirar = item.transform.Find("PainelValor/LinhaResgatar/BtnRetirar").GetComponent<Button>();
            btn.onClick.AddListener(() => Investir(banco, inputInvestir));
            btnRetirar.onClick.AddListener(() => Resgatar(banco, inputResgate));

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
        banco.valorOriginal += valor;
        banco.semanaInvestimento = ModuloTempo.semana;

        player.AlterarSaldoConta();
        AtualizarUI();

        input.text = "";
    }

    float CalcularAliquotaIR(DadosInvestimento banco)
    {
        int semanasInvestido = ModuloTempo.semana - banco.semanaInvestimento;

        if (semanasInvestido <= 4) return 0.225f;   // até ~180 dias
        if (semanasInvestido <= 8) return 0.20f;    // até ~360 dias
        if (semanasInvestido <= 16) return 0.175f;  // até ~720 dias
        return 0.15f;                               // acima disso
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

            var textoDescricao = par.item.transform.Find("PainelTexto/TextoDescricao").GetComponent<TextMeshProUGUI>();

            if (par.banco.tipo == TipoInvestimento.Poupanca && par.banco.valorInvestido > 0 && !PodeResgatar(par.banco))
            {
                int semanaFaltando = 4 - (ModuloTempo.semana - par.banco.semanaInvestimento);
                textoDescricao.text = $"Resgatar agora perde o rendimento! Faltam {semanaFaltando} semana(s) para resgate sem perdas.";
                textoDescricao.color = Color.yellow;
            }

            else if (par.banco.tipo == TipoInvestimento.CDB && par.banco.valorInvestido > 0)
            {
                float aliquota = CalcularAliquotaIR(par.banco) * 100f;
                textoDescricao.text = $"Imposto de Renda atual sobre o lucro: {aliquota}%. Quanto mais tempo investido, menor o imposto.";
                textoDescricao.color = Color.yellow;
            }

            else if (par.banco.tipo == TipoInvestimento.Fundos && par.banco.taxaAd > 0 && par.banco.valorInvestido > 0)
            {
                textoDescricao.text = $"Taxa de administracao: {(par.banco.taxaAd * 100f):F2}% ao mes, descontada do rendimento.";
                textoDescricao.color = Color.yellow;
            }            else
            {
                textoDescricao.text = par.banco.descricao;
                textoDescricao.color = Color.white;
            }



            // bool podeResgatar = PodeResgatar(par.banco);
            // Button btnRetirar = par.item.transform.Find("PainelValor/PainelBotoes/BtnRetirar").GetComponent<Button>();
            // btnRetirar.interactable = podeResgatar;

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


            banco.valorInvestido = CalcularInvestimento(banco.percentualCDI,banco.valorInvestido,banco.mudanca, banco.taxaAd);

        }

        AtualizarUI();
    }
    
    public float CalcularInvestimento(float percentualCDI, float valorInvestido, float mudanca, float taxaAd)
    {
        //cdi normal
        float taxaBase = (percentualCDI / 100f) * moduloRendimento.cdiSemanal;
        //vaiação pro cdb
        float variacao = Random.Range(-mudanca,mudanca);
        //fundo de rendimento
        float taxaSemanal = Mathf.Max(taxaBase + variacao,0);
        float valorRendido = valorInvestido * (1 + taxaSemanal);
        if (taxaAd > 0)
        {
            valorRendido  *= (1 - taxaAd);
        }
        return valorRendido;
    }

    public void Resgatar(DadosInvestimento banco, TMP_InputField inputResgate)
    {
        if(!float.TryParse(inputResgate.text, out float valorResgatar) || valorResgatar <= 0)
        {
            Debug.Log("Digite um valor válido para resgatar.");
            return;
        }
        if(valorResgatar > banco.valorInvestido)
        {
            Debug.Log("Valor de resgate maior que o investido.");
            return;
        }

        float proporcao = valorResgatar / banco.valorInvestido;
        float principalRetirado = banco.valorOriginal * proporcao;
        float valorLiquido = valorResgatar;


        if (banco.tipo == TipoInvestimento.Poupanca && !PodeResgatar(banco))
        {
            valorLiquido = principalRetirado; // Retira apenas o valor original, sem rendimento
        }
        else if (banco.tipo == TipoInvestimento.CDB)
        {
            float lucro = valorResgatar - principalRetirado;
            if (lucro > 0)
            {
                float aliquota = CalcularAliquotaIR(banco);
                float imposto = lucro * aliquota;
                valorLiquido = valorResgatar - imposto;
            }
        }

        banco.valorInvestido -= valorResgatar;
        banco.valorOriginal -= principalRetirado;

        player.patrimonio += valorLiquido;

        AtualizarUI();
        player.AlterarSaldoConta();
    }

}
