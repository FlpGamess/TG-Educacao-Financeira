using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
public class InfosPagamento : MonoBehaviour
{
    public TextMeshProUGUI Titulo;
    public Toggle Opcao1;
    public Toggle Opcao2;

    public void SetTooglesGroup(ToggleGroup grupo)
    {
        Debug.Log(grupo);

        Opcao1.group = grupo;
        Opcao2.group = grupo;
    }
    public void SetInfosPagamento(string titulo,TipoPagamento op1, TipoPagamento op2)
    {
        Opcao1.GetComponentInChildren<Text>().text = op1.ToString();
        Opcao2.GetComponentInChildren<Text>().text = op2.ToString();
        Titulo.text = titulo;
        
    }
    public TipoPagamento OpcaoMarcada()
    {
        if (Opcao1.isOn)
        {
           return TipoPagamento.AVista;
        }
        else
        {
           return TipoPagamento.Parcelado;
        }

    }

}
