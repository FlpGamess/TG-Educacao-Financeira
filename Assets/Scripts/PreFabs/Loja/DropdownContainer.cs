using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownContainer : MonoBehaviour
{
    public TextMeshProUGUI Titulo;
    public TMP_Dropdown dropdownParcelas;

    public void ConfigurarDDPagamento(TipoPagamento tpagamento)
    {
        Titulo.text = "parcela";
        dropdownParcelas.ClearOptions();
        if(tpagamento== TipoPagamento.AVista)
        {
            dropdownParcelas.AddOptions(new List<string> { "0x" });
            return;
        }
        dropdownParcelas.AddOptions(new List<string> { "1x", "2x", "3x", "4x", "5x", "6x" });

    }
    

}
