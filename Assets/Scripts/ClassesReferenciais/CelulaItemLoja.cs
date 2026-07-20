using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//classe do componente da celula da Loja
public class CelulaItemLoja : MonoBehaviour
{
    //container dos botões de compra e simulação
    public Transform containerBotoes;
    //texts com informações do produto
    public TextMeshProUGUI nome;
    public TextMeshProUGUI preco;
    public TextMeshProUGUI descricao;
    //Lista dos botões  do componente
    List<GameObject> botoesItem = new List<GameObject>();
    //texto dos botões
    public string[] btnNomes = { "Comprar", "Simular Compra"};

    //função para adicionar os botões, tentando manter conceito de classe quando der
    public void AdicionarBotao(GameObject btn)
    {
        botoesItem.Add(btn);
    }

}
