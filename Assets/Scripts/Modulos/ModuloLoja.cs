using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

//using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;
//classe da loja do jogo
public class ModuloLoja : MonoBehaviour
{   [Header("Paginas")]
    //painel principal da loja
    public GameObject menuLoja;
    //painel de compra do item
    public GameObject menuCompraItem;

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

    public void CarregarInterfaceCompra()
    {
        MenuCompraItem container = menuCompraItem.GetComponent<MenuCompraItem>();
        InfosGeraisLoja infog = Instantiate(container.infosGerais, container.infosContainer);
        InfosPagamento infop = Instantiate(container.infosPagamento, container.infosContainer);
        DropdownContainer infoparcel = Instantiate(container.infosParcela, container.infosContainer);
        GameObject botao;

        container.saldoConta.text = "$" + player.patrimonio;
        container.saldoDisposicao.text =  disposicao.disposicao +"%" ;

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

        infop.SetInfosPagamento("Tipo de Pagamento", TipoPagamento.AVista, TipoPagamento.Parcelado);
        infop.SetTooglesGroup(infop.GetComponent<ToggleGroup>());

        infoparcel.Titulo.text = "parcela";
        infoparcel.dropdownParcelas.ClearOptions();
        infoparcel.dropdownParcelas.AddOptions(new List<string> { "1x", "2x", "3x", "4x", "5x", "x6" });

        foreach (string bt in container.btnsTitulos)
        //foreach (KeyValuePair<AtributosFinanceiros, int> atb in Player.AtbFinanceiros)
        {
            //cria um botão atraves do preFabBotao no containerBotoes
            botao = Instantiate(preFabBotao, container.btnsContainer);
            //se a lista de botões da loja não tiver esse botão e ele existir
            if (!container.Botoes.Contains(botao) && botao)
            {
                //executa a configuração do botão mandando ele,o texto e a função
                botao = HelperConfig.ConfigurarBtn(botao, bt, () => Comprar(itemSelecionado, infop.OpcaoMarcada(), infoparcel.dropdownParcelas.value +1));
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



        /*
        int count = 0;
        GameObject linha = new GameObject("Linha");
        linha = HelperConfig.ConfigurarLinhaCompra(linha, container);
        foreach (var item in infos)
        {
            if (count >= 2)
            {
                linha = new GameObject("Linha");
                linha = HelperConfig.ConfigurarLinhaCompra(linha, container);
                count = 0;
            }
            CelulaCompra cedulaItem = Instantiate(prefabCelulaCompra,linha.transform);
            cedulaItem=  HelperConfig.ConfigurarCelulaCompra(cedulaItem, item);
            container.botoes.Add(cedulaItem);
            count++;

        }
        GameObject pagamentocontainer = new GameObject("PagamentoContainer");
        pagamentocontainer.transform.SetParent(container.InfosContainer, false);
        foreach (TipoPagamento tipo in Enum.GetValues(typeof(TipoPagamento)))
        {
            Toggle tg = Instantiate(toggle, container.InfosContainer);
          
        }*/





    }

    public void LimparCedulas()
    {
        foreach(CelulaItemLoja item in itens)
        {
            Destroy(item.gameObject);
        }
        itens.Clear();
    }
    //vou deletar isso no futuro, relaxa, é só pra aparecer uma mensagem pra entender que ta funcionando 
    //o click no botao
    public void MudarCategoria(AtributosFinanceiros Categoria)
    {
        categoriaAtual = Categoria;
        CarregarLoja();

    }

    public void Comprar(Itens compra, TipoPagamento tipopg, int parcela) {
        Despesas despesa = new Despesas(compra, tipopg, compra.Preco);
        for (int i = 0; i < parcela; i++) {
        
        }

        if (parcela == 0) {
            Parcela p = new Parcela(compra.Preco,ModuloTempo.semana);
            Debug.Log("parcela " + p.semana);
            despesa.parcelas.Add(p);
        }
        else if (parcela > 0){
            for (int i = 0; i < parcela; i++) {
                Parcela p = new Parcela(compra.Preco, (ModuloTempo.semana + (4 * (i + 1))));
                despesa.parcelas.Add(p);
                Debug.Log("parcela " + p.semana);
            }
        }

            player.ProcessarCompra(compra, despesa);

        player.ProcessaCompra(compra.Preco);

     }
    public void SimularCompra() {
        Debug.Log("naõ tem oq simular");
    }

    }
