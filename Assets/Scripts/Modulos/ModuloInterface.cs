using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using XCharts;
using XCharts.Runtime;

public class ModuloInterface : MonoBehaviour
{
    public Button btnHistorico;
    //recebe o objeto pai da interface(primeiro painel)
    public GameObject Interface;
    GameObject menuCelular;
    //Lista que guarda todas as janelas que t�o abertas
    List<GameObject> janelasAbertas = new List<GameObject>();
    public ModuloLoja moduloloja;
    public MCalendario menucalendario;
    public MHistorico menuhistorico;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    //define menuCelular como o menu do MenuCelular sendo o menu padr�o
     menuCelular = Interface.transform.Find("MenuCelular").gameObject;

         btnHistorico.onClick.AddListener(() => Debug.Log("OIA O BOTÃO"));

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)        
        {
            btnHistorico.onClick.Invoke();
        }
    }

    //Fun��o para abrir janelas do Menu, recebe o obj da janela que quer abrir
    public void Ativarjanela(GameObject janela)
    {
        //se a janela n�o esta na lista de janelas abertas
        if(!janelasAbertas.Contains(janela)) {
            //adiciona janela em janelasAbertas no indice 0
            janelasAbertas.Insert(0,janela);
            //ative a janela no indice 0 de janelasAbertas
            janelasAbertas[0].SetActive(true);
            CarregarCompoJanela(janela.name);
        }
    }
    //descobrir o indice de uma janela
    //janelasAbertas.IndexOf(janela);

    //remover por indice
    //janelasAbertas.removeAt(numero);

    //Fun��o para ocultar janelas, recebe a janela a ser ocultada(fechada)
    public void OcultarJanela(GameObject janela)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            foreach(GameObject jnl in janelasAbertas)
            {
                jnl.SetActive(false);
            }
            janelasAbertas.Clear();
            return;
        }
        //se a janela estiver presente na lista janelasAbertas
        if (janelasAbertas.Contains(janela))
        {
            //ache a posi��o dessa janela na lista
            int indice = janelasAbertas.IndexOf(janela);
            //desative a janela na posi��o encontrada
            janelasAbertas[indice].SetActive(false);
            //remova a janela na posi��o encontrada da lista 
            janelasAbertas.RemoveAt(indice);
            CarregarCompoJanela(janela.name);
        }
    }

    public void CarregarCompoJanela(string jname)
    {
        switch (jname)
        {
            case "MenuLoja":
                Debug.Log("teste"+jname);
                moduloloja.CarregarLoja();
                break;
            case "MenuCompraItem":
               
                moduloloja.CarregarInterfaceCompra();
                break;
            case "MenuCalendario":
                menucalendario.CarregarMCalendario();
                break;
            case "MenuHistorico":
                menuhistorico.CarregarHistorico();
                break;
        }

    }
    public void CriarGraficoLinhaSimples(LineChart grafico, Dictionary<string, Dictionary<int,float>> series,string titulo)
    {
      grafico.RemoveData();
       grafico.EnsureChartComponent<Title>().text = titulo;
       grafico.EnsureChartComponent<Legend>();
       grafico.EnsureChartComponent<YAxis>().axisLabel.numericFormatter = "F2";
       grafico.EnsureChartComponent<Tooltip>().numericFormatter = "F2";
        foreach(var serie in series)
        {
            grafico.AddSerie<Line>(serie.Key);
        }
        if(series.Count ==0)
            return;

        List<int> meses = series.First().Value.Keys.OrderBy(m=>m).ToList();
       foreach( var mes in meses)
        {
            grafico.AddXAxisData("Mês" + mes);
            int indice = 0;
            foreach (var serie in series)
            {
                float valor = serie.Value.TryGetValue(mes, out float v) ? v : 0f;
                grafico.AddData(indice, valor);
                indice++;
            }
        }
        


    }
  
    public void CriarGraficoBarraStLinha(BaseChart grafico, List<int> meses, Dictionary<AtributosFinanceiros, Dictionary<int, float>> gastos, Dictionary<int, float> ganhos, string titulo)
    {
        grafico.RemoveData();
        grafico.EnsureChartComponent<Title>().text = titulo;
        grafico.EnsureChartComponent<Legend>();
        grafico.EnsureChartComponent<YAxis>().axisLabel.numericFormatter = "F2";
        grafico.EnsureChartComponent<Tooltip>().numericFormatter = "F2";

        foreach (var categoria in gastos)
        {
            var barra = grafico.AddSerie<Bar>(categoria.Key.ToString());
            barra.stack = "Gastos";
            foreach (int mes in meses)
            {
                float valor = categoria.Value.TryGetValue(mes, out float v)?v: 0f;
                grafico.AddData(grafico.series.Count - 1, valor);
            }
        }
        grafico.AddSerie<Line>("Ganhos");

        foreach(int mes in meses)
        {
            float valor = ganhos.TryGetValue(mes,out float v)?v: 0f;
            grafico.AddData(grafico.series.Count-1, valor);
        }
        foreach(int mes in meses)
        {
            grafico.AddXAxisData("Mês" + mes);
        }
    }

    /*MenuCalendario Re-padronização*/
    public static void AtualizarTxt(TextMeshProUGUI componente, string texto, string valor)
    {
        componente.text = texto + valor;
    }

    public static void DefinirTamanhoMinimo(LayoutElement layout,float minw,float minh)
    {
        layout.minWidth = minw;
        layout.minHeight = minh;
    }

    public static Color CoresAtributosFinanceiros(AtributosFinanceiros atributo)
    {
        switch (atributo)
        {
            case AtributosFinanceiros.DespesasDoLar:
                return new Color32(255, 230, 150, 255);

            case AtributosFinanceiros.Educacao:
                return new Color32(170, 210, 245, 255);

            case AtributosFinanceiros.Moradia:
                return new Color32(220, 185, 160, 255);

            case AtributosFinanceiros.SaudeBemEstar:
                return new Color32(175, 225, 185, 255);

            case AtributosFinanceiros.Lazer:
                return new Color32(220, 185, 235, 255);

            default:
                return Color.white;
        }
    }



    public void FuncaoProvisoriaVouApagarDpsHomenagemACaioPrime()
    {
        Debug.Log("Disponivel no futuro!!");
    }
}

