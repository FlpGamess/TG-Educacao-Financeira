using TMPro;
using UnityEngine;
using XCharts.Runtime;

public class MenuCompraSim : MonoBehaviour
{
    [Header("Cabeçalho")]
    public TextMeshProUGUI saldoConta;
    [Header("Containers")]
    public Transform contacontainer;
    public Transform graficocontainer;
    public Transform btncontainer;

    [Header("Grafico")]
    public LineChart gHistoricoIfCompra;

    [Header("Botões")]
    public GameObject btnCancelar;
    public GameObject btnComprar;

}
