using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HelperConfig : MonoBehaviour
{
public static GameObject ConfigurarBtn(GameObject botao, string texto, UnityAction action)
    {
        ModuloInterface.AtualizarTxt(botao.GetComponentInChildren<TextMeshProUGUI>(), "", texto);
        Button btn = botao.GetComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
        //botao.GetComponent<Button>().onClick.AddListener(action);

        return botao;
    }

    public static CelulaItemLoja ConfigurarCedulaItem(CelulaItemLoja cedula, Itens item,UnityAction funcaobtn)
    {
        ModuloInterface.AtualizarTxt(cedula.nome,"", item.Nome);
        ModuloInterface.AtualizarTxt(cedula.preco, "R$", item.Preco.ToString());
        ModuloInterface.AtualizarTxt(cedula.descricao,"", item.Descricao);
        cedula.btnComprar = HelperConfig.ConfigurarBtn(cedula.btnComprar, "Comprar", funcaobtn);
        return cedula;
    }

    public static CelulaCompra ConfigurarCedulaCompra(CelulaCompra cedula, (string titulo, string valor) item)
    {
        ModuloInterface.AtualizarTxt(cedula.Titulo, "", item.titulo);
        ModuloInterface.AtualizarTxt(cedula.Informacao, "", item.valor);
        return cedula;
    }

    public static GameObject ConfigurarLinhaCompra(GameObject linha,MenuCompraItem container)
    {
        linha.AddComponent<RectTransform>();
       // linha.transform.SetParent(container.InfosContainer, false);

        HorizontalLayoutGroup layout = linha.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.spacing = 10;

        return linha;
    }


}
