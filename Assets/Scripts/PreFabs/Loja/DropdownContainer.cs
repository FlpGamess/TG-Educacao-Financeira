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

        if (tpagamento == TipoPagamento.AVista)
        {
            dropdownParcelas.AddOptions(new List<string> { "0x" });
            dropdownParcelas.interactable = false;

        }
        else
        {
            dropdownParcelas.AddOptions(new List<string> { "1x", "2x", "3x", "4x", "5x", "6x" });
            dropdownParcelas.interactable = true;
        }
        Debug.Log(tpagamento + "a lista ai" + dropdownParcelas.value);
        dropdownParcelas.value = 0;
        dropdownParcelas.RefreshShownValue();

    }

    public int ConverterValor()
    {
        string valor = dropdownParcelas.options[dropdownParcelas.value].text;
        return int.Parse(valor.Replace("x",""));

    }
    

}
