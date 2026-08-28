using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
//using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEditor.PlayerSettings;
//classe da loja do jogo
public class ModuloLoja : MonoBehaviour
{   [Header("Paginas")]
    //painel principal da loja
    public GameObject menuLoja;
    //painel de compra do item
    public GameObject menuCompraItem;
    public GameObject menuCompraSim;

    [Header("Prefabs")]
    //componente das celulas dos itens da loja que ficarão em lista
    public CelulaItemLoja prefabCelulaItemLoja;

    //componente de botão
    public GameObject preFabBotao;

    [Header("Containers")]
    //containers vulgo lugares onde vão ficar cada coisa
    public Transform containerBotoes;
    public Transform containerItens;

    [Header("Variaveis Seleção")]
    //Categoria atual selecionada na loja
    public AtributosFinanceiros categoriaAtual;
    //Item selecionado pra compra
    public Itens itemSelecionado;
    //Catalogo de Itens a ser mostrado na loja
    public CatalogoLoja catalogo;

    [Header("Listas")]
    //lista dos botões da loja
    List<GameObject> botoes = new List<GameObject>();
    //lista dos itens da loja
    List<CelulaItemLoja> itens = new List<CelulaItemLoja>();

    [Header("Modulos")]
    public Player player;
    public ModuloInterface moduloInterface;
    public ModuloDisposicao disposicao;
    public ModuloTempo tempo;
    public ModuloRendimentos modulorendimentos;
    public ModuloEconomia moduloEconomia;

    public InfosPagamento InfoPag;
    public DropdownContainer InfoParcel;


    private UnityAction[] FuncoesCompraItem; //= {() => Comprar(itemSelecionado), ()=> SimularCompra(itemSelecionado) };





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void CarregarLoja()
    {
        LimparCedulas();
        //variavel que guarda o botão
        GameObject botao;
        //percorre os atributos financeiros chave a chave
        foreach (AtributosFinanceiros atb in Player.AtbFinanceiros.Keys)
        //foreach (KeyValuePair<AtributosFinanceiros, int> atb in Player.AtbFinanceiros)
        {
            //cria um botão atraves do preFabBotao no containerBotoes
            botao = Instantiate(preFabBotao, containerBotoes);
            //se a lista de botões da loja não tiver esse botão e ele existir
            if (!botoes.Contains(botao) && botao)
            {
                //executa a configuração do botão mandando ele,o texto e a função
                botao = HelperConfig.ConfigurarBtn(botao, atb.ToString(),() => MudarCategoria(atb) );
                //botao = HelperConfig.ConfigurarBtn(botao, atb.Key.ToString(), funcao67temporaria);               
                //adiciona o botão na lista de botões
                botoes.Add(botao);
            }
        }
        catalogo.CarregarCatalogo();
        foreach (Itens item in catalogo.catalogo[categoriaAtual])
        {
            // Debug.Log(item);
            //cria a celula do item a venda na loja, futuramente com os itens sera mudado pra um for each
            CelulaItemLoja cedulaItem = Instantiate(prefabCelulaItemLoja, containerItens);
            cedulaItem = HelperConfig.ConfigurarCedulaItem(cedulaItem, item,  /*UnityAction*/() =>ConfigItemCompra(item));
            //adiciona a celula a lista de celulas
            itens.Add(cedulaItem);
        }


    }

    public void ConfigItemCompra(Itens item)
    {
        itemSelecionado = item;
        moduloInterface.Ativarjanela(menuCompraItem);
    }

    public void ConfigJanelas(GameObject pagina)
    {
        moduloInterface.Ativarjanela(pagina);

    }



    public void CarregarInterfaceCompra()
    {
        LimparInterfaceCompra();
        MenuCompraItem container = menuCompraItem.GetComponent<MenuCompraItem>();
        InfosGeraisLoja infog = Instantiate(container.infosGerais, container.infosContainer);
        InfoPag = Instantiate(container.infosPagamento, container.infosContainer);
        InfoParcel = Instantiate(container.infosParcela, container.infosContainer);
        GameObject botao;
        container.saldoConta = AtualizarPlayerInfos(container.saldoConta,0);
        container.saldoDisposicao = AtualizarPlayerInfos(container.saldoDisposicao,1);


        infog.Container1.GetChild(0).GetComponent<CelulaCompra>().Titulo.text = "Nome";
        infog.Container1.GetChild(0).GetComponent<CelulaCompra>().Informacao.text = itemSelecionado.Nome;
        infog.Container1.GetChild(1).GetComponent<CelulaCompra>().Titulo.text = "Preço";
        infog.Container1.GetChild(1).GetComponent<CelulaCompra>().Informacao.text = $"R$ {itemSelecionado.Preco}";

        infog.Container2.GetChild(0).GetComponent<CelulaCompra>().Titulo.text = "Tipo";
        infog.Container2.GetChild(0).GetComponent<CelulaCompra>().Informacao.text = itemSelecionado.Tipo.ToString();
        infog.Container2.GetChild(1).GetComponent<CelulaCompra>().Titulo.text = "Categoria";
        infog.Container2.GetChild(1).GetComponent<CelulaCompra>().Informacao.text = itemSelecionado.Categoria.ToString();

        infog.Container3.GetChild(0).GetComponent<CelulaCompra>().Titulo.text = "Descrição";
        infog.Container3.GetChild(0).GetComponent<CelulaCompra>().Informacao.text = itemSelecionado.Descricao;

        InfoPag.SetInfosPagamento("Tipo de Pagamento", TipoPagamento.AVista, TipoPagamento.Parcelado);
        InfoPag.SetTooglesGroup(InfoPag.GetComponent<ToggleGroup>());
        TipoPagamento opcaoPagamento = InfoPag.OpcaoMarcada();

        InfoPag.Opcao1.onValueChanged.AddListener ((valor) => AtualizarPagamento());
        InfoPag.Opcao2.onValueChanged.AddListener ((valor) => AtualizarPagamento());

        InfoParcel.ConfigurarDDPagamento(opcaoPagamento);
        FuncoesCompraItem = new UnityAction[] { () => Comprar(itemSelecionado), ()=> SimularCompra(itemSelecionado) };
        for(int i = 0; i<container.btnsTitulos.Length; i++)
        //foreach (string bt in container.btnsTitulos)
        //foreach (KeyValuePair<AtributosFinanceiros, int> atb in Player.AtbFinanceiros)
        {

            //cria um botão atraves do preFabBotao no containerBotoes
            botao = Instantiate(preFabBotao, container.btnsContainer);
            //se a lista de botões da loja não tiver esse botão e ele existir
            if (!container.Botoes.Contains(botao) && botao)
            {
                //executa a configuração do botão mandando ele,o texto e a função
                botao = HelperConfig.ConfigurarBtn(botao, container.btnsTitulos[i], FuncoesCompraItem[i]);
                //botao = HelperConfig.ConfigurarBtn(botao, atb.Key.ToString(), funcao67temporaria);               
                //adiciona o botão na lista de botões
                container.Botoes.Add(botao);
            }
        }

        //percorre a variavel por variavel da classe
        /*foreach( FieldInfo cont in typeof(InfosGeraisLoja).GetFields())
        {
        }*/
        /*
        List < (string, string) > infos = new List<(string, string)>
            {
                ("Nome", itemSelecionado.Nome),
                ("Descrição", itemSelecionado.Descricao),
                ("Preço", $"R$ {itemSelecionado.Preco}"),
                ("Categoria", itemSelecionado.Categoria.ToString()),
                ("Tipo", itemSelecionado.Tipo.ToString())
            };*/
    }

    public void AtualizarPagamento()
    {
        TipoPagamento TPagamento = InfoPag.OpcaoMarcada();
        InfoParcel.ConfigurarDDPagamento(TPagamento);
    }

    public void LimparCedulas()
    {
        foreach(CelulaItemLoja item in itens)
        {
            Destroy(item.gameObject);
        }
        itens.Clear();
    }

    public void LimparInterfaceCompra()
    {
        MenuCompraItem container = menuCompraItem.GetComponent<MenuCompraItem>();
        foreach(Transform comp in container.infosContainer)
        {
            Destroy(comp.gameObject);
        }
        foreach(Transform comp in container.btnsContainer)
        {
            Destroy(comp.gameObject);
        }
    }
    //vou deletar isso no futuro, relaxa, é só pra aparecer uma mensagem pra entender que ta funcionando 
    //o click no botao
    public void MudarCategoria(AtributosFinanceiros Categoria)
    {
        categoriaAtual = Categoria;
        CarregarLoja();

    }

    public Despesas GerarDespesas(Itens compra)
    {
        TipoPagamento tipopg = InfoPag.OpcaoMarcada();
        int parcela = InfoParcel.ConverterValor();
        int semana = ModuloTempo.semana;

        Despesas despesa = new Despesas(compra, tipopg, compra.Preco);

        despesa.GerarParcelas(compra.Preco, semana, parcela);

        return despesa;

    }

    public void Comprar(Itens compra) {
        MenuCompraItem container = menuCompraItem.GetComponent<MenuCompraItem>();
        Despesas despesa = GerarDespesas(compra);

        player.ProcessarCompra(compra, despesa);

        //player.DebitarPagamento(compra.Preco);
        container.saldoConta = AtualizarPlayerInfos(container.saldoConta, 0);
        container.saldoDisposicao = AtualizarPlayerInfos(container.saldoDisposicao, 1);

    }

    public void SimularCompra(Itens compra) {
        MenuCompraSim container = menuCompraSim.GetComponent<MenuCompraSim>();
        int semana = ModuloTempo.semana;
        int parcela = InfoParcel.ConverterValor();
        ConfigJanelas(menuCompraSim);
        Despesas despesa = GerarDespesas(compra);

        //aqui depois vou dividir mais
        int mes = ((semana - 1) / 4) + 1;
        int mesfinal = mes + parcela;
        float investidosimulado = 0;
        Dictionary<int, float> gastos = new();
        Dictionary<int, float> gIfCompra = new();
        Dictionary<int, float> rendimentos = new();
        Dictionary<int, float> saldo = new();

        for (int m = mes; m <= (mesfinal); m++) {
            gastos.Add(m, 0);
            gIfCompra.Add(m, 0);
            rendimentos.Add(m,modulorendimentos.salario);
            foreach (var banco in moduloEconomia.bancos)
            {
                if (banco.valorInvestido <= 0) continue;
                if (m == mes)
                {
                    investidosimulado = moduloEconomia.CalcularInvestimento(banco.percentualCDI, banco.valorInvestido);
                    rendimentos[m] += (investidosimulado - banco.valorInvestido);
                    Debug.Log("Primeirinha "+investidosimulado);
                }
                else
                {
                    
                    float aux = moduloEconomia.CalcularInvestimento(banco.percentualCDI, investidosimulado);
                    rendimentos[m] += (aux - investidosimulado);
                    investidosimulado = aux;
                }
                }
        }

        foreach (Despesas despesas in player.Dividas)
        {
            foreach (Parcela parcel in despesas.parcelas)
            {
                int mesparcela = ((parcel.semana - 1) / 4) + 1;
                if (gastos.ContainsKey(mesparcela)) {
                    gastos[mesparcela] += parcel.valor;
                    gIfCompra[mesparcela] += parcel.valor;

                }
            }
        }
        
        foreach(Parcela parcel in despesa.parcelas)
        {
            int mesparcela = ((parcel.semana - 1) / 4) + 1;
            if (gIfCompra.ContainsKey(mesparcela))
            {
                gIfCompra[mesparcela] += parcel.valor;
            }

        }

        //tem que arrumar, arredondar os valores pra duas casas depois da virgula
        foreach (int m in gastos.Keys.ToList())
        {
            gastos[m]= Mathf.Round(gastos[m] * 100f)/100f;
            gIfCompra[m] = Mathf.Round(gIfCompra[m] * 100f) / 100f;
            rendimentos[m] = Mathf.Round(rendimentos[m] * 100f) / 100f;
        }

            foreach (int m in gastos.Keys)
        {
            Debug.Log(
                $"Mês {m} | " +
                $"Gastos: R$ {gastos[m]} | " +
                $"Gastos + Compra: R$ {gIfCompra[m]} | " +
                $"Rendimentos: R$ {rendimentos[m]}"
            );
        }
        container.saldoConta = AtualizarPlayerInfos(container.saldoConta, 0);
        moduloInterface.CriarGraficoLinhaSimples(gastos,gIfCompra,rendimentos, "Simulação de suas Finanças Caso Compre o Item Desejado ao Longo das Parcelas");
        HelperConfig.ConfigurarBtn(container.btnComprar,"Comprar", () => BtnComprarSim());


    }

    public void BtnComprarSim()
    {
        Comprar(itemSelecionado);
        moduloInterface.OcultarJanela(menuCompraSim);
        moduloInterface.OcultarJanela(menuCompraItem);


    }


    public TextMeshProUGUI AtualizarPlayerInfos(TextMeshProUGUI info, int modo)
    {
        switch (modo)
        {
            case 0:
                info.text = "$" + player.patrimonio;
                break;
                case 1:
                info.text = disposicao.disposicao + "%";
                break;

        }
        return info;
    }

    public MenuCompraItem AtualizarPlayerInfosIC(MenuCompraItem container)
    {
        container.saldoConta.text = "$" + player.patrimonio;
        container.saldoDisposicao.text = disposicao.disposicao + "%";

        return container;
    }



}

