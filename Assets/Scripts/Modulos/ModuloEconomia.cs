using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class DadosInvestimento
{
    public string nome;
    public float percentualCDI;
    public float valorInvestido;
}

public class ModuloEconomia : MonoBehaviour
{
     public List<DadosInvestimento> bancos;
    public Player player;
    public GameObject prefabItemInvestimento;
    public Transform content;
    public float cdiSemanal = 0.002f; 


    private List<GameObject> itensInstanciados = new List<GameObject>();

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
      MontarLista();   
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
        ModuloTempo.isSemanaAvancada += RenderInvestimentos; // ADICIONAR

        foreach (var banco in bancos)
        {
            GameObject item = Instantiate(prefabItemInvestimento, content);
            itensInstanciados.Add(item);

            item.transform.Find("TextoNome").GetComponent<TextMeshProUGUI>().text = $"{banco.nome}: {banco.percentualCDI}%";

            TMP_InputField input = item.transform.Find("InputValor").GetComponent<TMP_InputField>();
            Button btn = item.transform.Find("BtnInvestir").GetComponent<Button>();

            btn.onClick.AddListener(() => Investir(banco, input));

        }
        AtualizarUI();
    }
    public void Investir(DadosInvestimento banco, TMP_InputField input)
    {
        if(!float.TryParse(input.text, out float valor) || valor <= 0)
        {
            Debug.Log("Digite um valor válido para investir.");
            return;
        }
        if(player.patrimonio < valor)
        {
            Debug.Log("Saldo insuficiente!");
            return;
        }
        player.patrimonio -= valor;
        banco.valorInvestido += valor;

        player.AlterarSaldoConta();
        AtualizarUI();

        input.text = "";
    }
    void AtualizarUI()
    {
        for(int i = 0; i < bancos.Count; i++)
        {
            var textoValor = itensInstanciados[i].transform.Find("TextoValor").GetComponent<TextMeshProUGUI>();
            textoValor.text = "Investido: R$" + bancos[i].valorInvestido.ToString("F2");
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
}
