using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

//using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;
//classe da loja do jogo
public class ModuloLoja : MonoBehaviour
{
    //painel principal da loja
    public GameObject painelLoja;
    //componente de botão
    public GameObject prefabBotao;
    //componente das celulas dos itens da loja que ficarão em lista
    public CelulaItemLoja prefabItens;

    public GameObject painelInterfaceCompra;

    public CedulaCompra prefabCelulaCompra;
    //containers vulgo lugares onde vão ficar cada coisa
    public Transform ContainerBotoes;
    public Transform ContainerItens;

    public AtributosFinanceiros CategoriaAtual;
    public Itens ItemSelecionado;  
    public CatalogoLoja catalogo;

    public Toggle toggle;
    List<Toggle> togglesPagamento = new List<Toggle>();

    //lista dos botões da loja
    List<GameObject> botoes = new List<GameObject>();
    //lista dos itens da loja
    List<CelulaItemLoja> itens = new List<CelulaItemLoja>();
    public Player player;
    public ModuloInterface ModuloInterface;


    
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
            //cria um botão atraves do prefabBotao no ContainerBotoes
            botao = Instantiate(prefabBotao, ContainerBotoes);
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
        foreach (Itens item in catalogo.catalogo[CategoriaAtual])
        {
            // Debug.Log(item);
            //cria a celula do item a venda na loja, futuramente com os itens sera mudado pra um for each
            CelulaItemLoja cedulaItem = Instantiate(prefabItens, ContainerItens);
            cedulaItem = HelperConfig.ConfigurarCedulaItem(cedulaItem, item,  /*UnityAction*/() =>ConfigItemCompra(item));
            //adiciona a celula a lista de celulas
            itens.Add(cedulaItem);
        }


    }

    public void ConfigItemCompra(Itens item)
    {
        ItemSelecionado = item;
        ModuloInterface.Ativarjanela(painelInterfaceCompra);
    }

    public void CarregarInterfaceCompra()
    {
        List < (string, string) > infos = new List<(string, string)>
            {
                ("Nome", ItemSelecionado.Nome),
                ("Descrição", ItemSelecionado.Descricao),
                ("Preço", $"R$ {ItemSelecionado.Preco}"),
                ("Categoria", ItemSelecionado.Categoria.ToString()),
                ("Tipo", ItemSelecionado.Tipo.ToString())
            };

        MenuCompraItem container = painelInterfaceCompra.GetComponent<MenuCompraItem>();


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
            CedulaCompra cedulaItem = Instantiate(prefabCelulaCompra,linha.transform);
            cedulaItem=  HelperConfig.ConfigurarCedulaCompra(cedulaItem, item);
            container.botoes.Add(cedulaItem);
            count++;

        }
        GameObject pagamentocontainer = new GameObject("PagamentoContainer");
        pagamentocontainer.transform.SetParent(container.InfosContainer, false);
        foreach (TipoPagamento tipo in Enum.GetValues(typeof(TipoPagamento)))
        {
            Toggle tg = Instantiate(toggle, container.InfosContainer);
          
        }
      




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
        CategoriaAtual = Categoria;
        CarregarLoja();

    }

    public void Comprar(Itens compra) {

        Despesas despesa = new Despesas(compra,"A vista",compra.Preco);

        player.ProcessarCompra(compra.Preco);

        Debug.Log("tilapia"+ despesa+" "+despesa.valor);
    }
    public void SimularCompra() {
        Debug.Log("naõ tem oq simular");
    }

    }
