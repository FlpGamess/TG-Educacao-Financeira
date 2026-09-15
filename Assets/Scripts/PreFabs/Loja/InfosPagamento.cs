using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
public class InfosPagamento : MonoBehaviour
{
    public TextMeshProUGUI Titulo;
    public Toggle Opcao1;
    // public Toggle Opcao2;
    public Toggle Opcao3;

    public void SetTooglesGroup(ToggleGroup grupo)
    {
        Debug.Log(grupo);

        Opcao1.group = grupo;
        // Opcao2.group = grupo;
        Opcao3.group = grupo;
    }
    public void SetInfosPagamento(string titulo,TipoPagamento op1, TipoPagamento op3)
    {
        Opcao1.GetComponentInChildren<Text>().text = op1.ToString() + " - Paagamento é feito na hora";
        // Opcao2.GetComponentInChildren<Text>().text = op2.ToString();
        Opcao3.GetComponentInChildren<Text>().text = op3.ToString() + " - Valor será pago na primeira semana do mês e após 5x tem juros";
        Titulo.text = titulo;
        
    }
    public TipoPagamento OpcaoMarcada()
    {
        if (Opcao1.isOn)
        {
           return TipoPagamento.Pix;
        }
        // else if (Opcao2.isOn)
        // {
        //    return TipoPagamento.Pix_Parcelado;
        // }
        else
        {
           return TipoPagamento.Cartão_Credito;
        }

    }
    public void CartaoAparece(bool disponivel)
    {
        Opcao3.gameObject.SetActive(disponivel);

        if(!disponivel && Opcao3.isOn)
        {
            Opcao1.isOn = true;
        }
    }

}
