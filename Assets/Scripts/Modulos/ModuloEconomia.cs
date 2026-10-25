using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public enum TipoInvestimento // criação dos tipos de investimentos que serão usados
{
    Poupanca, //Rendimento de acordo com o CDI, tem uma espera de 4 semanas para não perder o rendimento
    CDB, // Rendimento com uma certa aleatoriedade em relação ao mercado e retem imposto de renda regressiva sobre os lucros
    Fundos, // Rendimento variavel com dedução de taxa de administração mensal
}

[System.Serializable]
public class DadosInvestimento //Variaveis para a criação do banco
{
    public TipoInvestimento tipo; // qual tipo de investimento
    [TextArea]
    public string descricao;
    public float percentualCDI; //Quantos % do CDI vão ser 
    public float valorInvestido; // valor total
    public float valorOriginal;  //Usado para poupança e no calculo de ir do CDB
    public int semanaInvestimento = -1; // quando foi colocado o valor
    public float mudanca = 0f; //usado para verificar a variação aleatoria
    public float taxaAd = 0f; // Taxa de administração cobrada no rendiemento usado pelos fundos
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

    public ModuloEventosAleatorios moduloEventosA; 

    private int semanaLiberaInvest = -1;

    private bool cartaoLib = false;

    private List<(DadosInvestimento banco, GameObject item)> itensInstanciados = new List<(DadosInvestimento, GameObject)>();

    public int ObterSemanaLiberaInvest()
    {
        return semanaLiberaInvest;
    }

    public bool ObterCartaoLib()
    {
        return cartaoLib;
    }

    public void SetSemanaLiberaInvest(int semanaSalva, bool cartaoSalvo)
    {
        semanaLiberaInvest = semanaSalva;
        cartaoLib = cartaoSalvo;
    }
    
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
        for (int i = player.Dividas.Count - 1; i>=0; i--)
        {
            Despesas despesa = player.Dividas[i];
            Parcela parcela = despesa.parcelas.FirstOrDefault();
            if (parcela != null && parcela.semana == ModuloTempo.semana)
            {
                player.DebitarPagamento(parcela.valor);
                player.RegistrarDespesaDebitada(despesa.categoria, parcela.valor);
                player.HistoricoPagamentos.Add(new PagamentoRealizado(despesa.item.Nome, parcela.valor,parcela.semana,parcela.numero,despesa.categoria,despesa.tipocompra));
                despesa.parcelas.RemoveAt(0);
                if (despesa.parcelas.Count == 0){
                    despesa.isPaga = true;
                    Debug.Log($"[Pagamento]: Despesa {despesa.item.Nome} paga");
                }
            }
        }
    }


    void TempoGravado() //verificação realizada para a liberação da aba de investimentos, onde se o valor do patrimonio no final do mês for maior ou igual a 100 ele pode investir se não só no proximo mês.
{
    if (semanaLiberaInvest == -1 && ModuloTempo.fimMes && player.patrimonio > 100){
        semanaLiberaInvest = ModuloTempo.semana;
    }

    if  (player.patrimonio >= 100)
        valorTot = true;
    else
        valorTot = false;

    if ( semanaLiberaInvest != -1 && !cartaoLib){ //lógica para liberar o cartão de crédito, onde se a partir da semana que o investimento for liberado se passarem 4 semanas e ele investiu no total mais que 100 reais o cartão é liberado.
        int semanaCheck = semanaLiberaInvest +4;
        if (ModuloTempo.semana >= semanaCheck && ModuloTempo.fimMes)
        {
            float TotalInvs = 0f;
            foreach (var banco in bancos)
                TotalInvs += banco.valorInvestido;

            if (valorTot && TotalInvs >= 100)
                cartaoLib = true;
        }
    }
}
    public bool CartaoDeCredito()
    {
        return cartaoLib;
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
            float cdi = banco.percentualCDI;
            if (ModuloEventosAleatorios.tiposAfetados.Contains(TipoAfetado.Investimentos))
            {
                cdi = moduloEventosA.ExecutarEventos(TipoAfetado.Investimentos, cdi, moduloEventosA.eventosAtivos, banco.tipo.ToString());
            }
            ModuloInterface.AtualizarTxt(item.transform.Find("PainelTexto/TextoNome").GetComponent<TextMeshProUGUI>(), "", $"{banco.tipo}: {cdi}%");
            ModuloInterface.AtualizarTxt(item.transform.Find("PainelTexto/TextoDescricao").GetComponent<TextMeshProUGUI>(), "", banco.descricao);

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
        moduloEventosA.VerificarGatilho(GatilhoEvento.Investir);
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
            ModuloInterface.AtualizarTxt(par.item.transform.Find("PainelTexto/TextoValor").GetComponent<TextMeshProUGUI>(), "Investido: R$", par.banco.valorInvestido.ToString("F2"));
            var textoDescricao = par.item.transform.Find("PainelTexto/TextoDescricao").GetComponent<TextMeshProUGUI>();

            if (par.banco.tipo == TipoInvestimento.Poupanca && par.banco.valorInvestido > 0 && !PodeResgatar(par.banco))
            {
                int semanaFaltando = 4 - (ModuloTempo.semana - par.banco.semanaInvestimento);
                ModuloInterface.AtualizarTxt(textoDescricao, "", $"Resgatar agora perde o rendimento! Faltam {semanaFaltando} semana(s) para resgate sem perdas.");
                textoDescricao.color = Color.yellow;
            }

            else if (par.banco.tipo == TipoInvestimento.CDB && par.banco.valorInvestido > 0)
            {
                float aliquota = CalcularAliquotaIR(par.banco) * 100f;
                ModuloInterface.AtualizarTxt(textoDescricao, "", $"Imposto de Renda atual sobre o lucro: {aliquota}%. Quanto mais tempo investido, menor o imposto.");
                textoDescricao.color = Color.yellow;
            }

            else if (par.banco.tipo == TipoInvestimento.Fundos && par.banco.taxaAd > 0 && par.banco.valorInvestido > 0)
            {
                ModuloInterface.AtualizarTxt(textoDescricao, "", $"Taxa de administracao: {(par.banco.taxaAd * 100f):F2}% ao mes, descontada do rendimento.");
                textoDescricao.color = Color.yellow;
            }            else
            {
                ModuloInterface.AtualizarTxt(textoDescricao, "", par.banco.descricao);
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
    //Executado quando se avança a semana, calcula o rendimento de cada banco ativo e adiciona o lucro ao GanhoMensal do Player
    void RenderInvestimentos()
    {
        float valorini = 0f;
        foreach (var banco in bancos)
        {
            if (banco.valorInvestido <= 0) continue;
            valorini = banco.valorInvestido;
            Debug.Log("Percentual cdi "+banco.percentualCDI+ "valorInvestido " + banco.valorInvestido+ " mudanca" + banco.mudanca+ "taxaAd " + banco.taxaAd);
            banco.valorInvestido = CalcularInvestimento(banco.tipo.ToString(), banco.percentualCDI,banco.valorInvestido,banco.mudanca, banco.taxaAd);
            float valorf = banco.valorInvestido;
            Debug.Log(valorini + " - " + valorf);
            player.GanhoMensal += valorf -valorini;
        }

        AtualizarUI();
    }
    // Aplica os eventos Aleatorios que irão mexer com os investimentos, e calcula também a taxa mensal(CDI base+ oscilação do mercado) e desconta a taxa de administração.
    public float CalcularInvestimento(string nomeinvestimento, float percentualCDI, float valorInvestido, float mudanca, float taxaAd)
    {
        if (ModuloEventosAleatorios.tiposAfetados.Contains(TipoAfetado.Investimentos)) {
            percentualCDI = moduloEventosA.ExecutarEventos(TipoAfetado.Investimentos, percentualCDI,moduloEventosA.eventosAtivos,nomeinvestimento);
            mudanca = moduloEventosA.ExecutarEventos(TipoAfetado.Investimentos, mudanca, moduloEventosA.eventosAtivos, nomeinvestimento);
            taxaAd = moduloEventosA.ExecutarEventos(TipoAfetado.Investimentos, taxaAd, moduloEventosA.eventosAtivos, nomeinvestimento);

        }
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
        if(!float.TryParse(inputResgate.text, out float valorResgatar) || valorResgatar <= 0)//impede resgate de valores menores ou iguais a zero
        {
            Debug.Log("Digite um valor válido para resgatar.");
            return;
        }
        if(valorResgatar > banco.valorInvestido) //impede resgate de valores maiores
        {
            Debug.Log("Valor de resgate maior que o investido.");
            return;
        }

        float proporcao = valorResgatar / banco.valorInvestido; //calculo do valor resgatado em relação ao total
        float principalRetirado = banco.valorOriginal * proporcao;
        float valorLiquido = valorResgatar;


        if (banco.tipo == TipoInvestimento.Poupanca && !PodeResgatar(banco))//Verifica a caracteristica do tipo se for resgatado antes de 4 semanas ele pega só o valor original investido
        {
            valorLiquido = principalRetirado; // Retira apenas o valor original, sem rendimento
        }
        else if (banco.tipo == TipoInvestimento.CDB)// calcula o lucro correspondete e aplica uma tabela regressiva de imposto sobre o lucro; -- até 4 semanas é 22.5%, de 5 a 8 é 20.0%, de 9 a 16 semanas é de 17.5% e acima de 16 semanas eé de 15%
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

    public void RestaurarControles(int semanaSalva, bool cartaoSalvo)
    {
        semanaLiberaInvest = semanaSalva;
        cartaoLib = cartaoSalvo;
    }

    public void AtualizarDepoisDeRestaurar()
    {
        valorTot = player.patrimonio >= 100;
        AtualizarVisibilidadeBotao();

        MontarLista();
    }

}
