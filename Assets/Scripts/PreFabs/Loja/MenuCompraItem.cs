using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuCompraItem : MonoBehaviour
{
    [Header("Cabeçalho")]
    public TextMeshProUGUI saldoConta;
    public TextMeshProUGUI saldoDisposicao;
    [Header("Containers")]
    public Transform infosContainer;
    public Transform btnsContainer;
    [Header("Caracteristicas da Compra")]
    public InfosGeraisLoja infosGerais;
    public InfosPagamento infosPagamento;
    public DropdownContainer infosParcela;
    [Header("Variaveis de Controle")]
    public List<GameObject> Botoes = new List<GameObject>();
    public string[] btnsTitulos = {"Comprar","Simular"};


}
