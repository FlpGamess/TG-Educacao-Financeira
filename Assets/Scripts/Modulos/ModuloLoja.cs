using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    //containers vulgo lugares onde vão ficar cada coisa
    public Transform ContainerBotoes;
    public Transform ContainerItens;
    public Transform ContainerBotoesLista;

    //lista dos botões da loja
    List<GameObject> botoes = new List<GameObject>();
    //lista dos itens da loja
    List<CelulaItemLoja> itens = new List<CelulaItemLoja>();
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
        //variavel que guarda o botão
        GameObject botao;
        //percorre os atributos financeiros chave a chave
        foreach (KeyValuePair<string, int> atb in Player.AtbFinanceiros)
        {
            //cria um botão atraves do prefabBotao no ContainerBotoes
            botao = Instantiate(prefabBotao, ContainerBotoes);
            //se a lista de botões da loja não tiver esse botão e ele existir
            if (!botoes.Contains(botao) && botao)
            {
                //executa a configuração do botão mandando ele,o texto e a função
                botao = HelperConfig.ConfigurarBtn(botao, atb.Key, funcao67temporaria);               
                //adiciona o botão na lista de botões
                botoes.Add(botao);
            }
        }
        //cria a celula do item a venda na loja, futuramente com os itens sera mudado pra um for each
        CelulaItemLoja ItensLoja = Instantiate(prefabItens, ContainerItens);
        //loop pra gerar os 2 botões de uma celula
        foreach (String atb in ItensLoja.btnNomes)
        {
            //cria um botão pelo prefabBotao e o container do  botão
            botao = Instantiate(prefabBotao, ItensLoja.containerBotoes);
            //configura ele pela função de configurarção do botão recebendo botao, texto e função
            botao = HelperConfig.ConfigurarBtn(botao, atb, funcao67temporaria);
            //adiciona o botao a celula
            ItensLoja.AdicionarBotao(botao);
        }
        //adiciona a celula a lista de celulas
        itens.Add(ItensLoja);


    }
    //vou deletar isso no futuro, relaxa, é só pra aparecer uma mensagem pra entender que ta funcionando 
    //o click no botao
    public void funcao67temporaria()
    {
        Debug.Log("Disponivel no futuro!!");
    }
}
