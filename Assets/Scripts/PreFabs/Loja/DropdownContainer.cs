using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownContainer : MonoBehaviour
{
    public TextMeshProUGUI Titulo;
    public TMP_Dropdown dropdownParcelas;

    public void ConfigurarDDPagamento(TipoPagamento tpagamento)
    {
        Titulo.text = "Parcela";
        dropdownParcelas.ClearOptions();

        if (tpagamento == TipoPagamento.Pix)
        {
            dropdownParcelas.AddOptions(new List<string> { "0x" });
            dropdownParcelas.interactable = false;

        }
        // else if (tpagamento == TipoPagamento.Pix_Parcelado)
        // {
        //     dropdownParcelas.AddOptions(new List<string> { "1x", "2x", "3x", "4x", "5x", "6x" });
        //     dropdownParcelas.interactable = true;
        // }
        else if (tpagamento == TipoPagamento.Cartão_Credito)
        {
            dropdownParcelas.AddOptions(new List<string> { "1x", "2x", "3x", "4x - juros de 5%" , "5x - juros de 5%", "6x - juros de 5%"});
            dropdownParcelas.interactable = true;
        }
        Debug.Log(tpagamento + "a lista ai" + dropdownParcelas.value);
        dropdownParcelas.value = 0;
        dropdownParcelas.RefreshShownValue();

    }

    public int ConverterValor()
    {
        string valor = dropdownParcelas.options[dropdownParcelas.value].text;
        return int.Parse(valor.Split('x')[0]);

    }
    

}
