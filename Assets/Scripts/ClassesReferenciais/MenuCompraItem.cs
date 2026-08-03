using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuCompraItem : MonoBehaviour
{
    public TextMeshProUGUI saldoConta;
    public TextMeshProUGUI saldoDisposicao;
    public Transform InfosContainer;
    public Transform BtnsContainer;
    public List<CedulaCompra> botoes = new();

}
