using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HelperConfig : MonoBehaviour
{
public static GameObject ConfigurarBtn(GameObject botao, string texto, UnityAction action)
    {
        botao.GetComponentInChildren<TextMeshProUGUI>().text = texto;
        botao.GetComponent<Button>().onClick.AddListener(action);

        return botao;
    }

    public static CelulaItemLoja ConfigurarCedulaItem(CelulaItemLoja cedula, Itens item, GameObject prefabBotao,UnityAction[] funcaobtn)
    {
        cedula.nome.text = item.Nome;
        cedula.preco.text = "R$" + item.Preco.ToString();
        cedula.descricao.text = item.Descricao;
        //loop pra gerar os 2 botões de uma celula
        for(int i=0;i< cedula.btnNomes.Length;i++) 
        {
            //cria um botão pelo prefabBotao e o container do  botão
            GameObject botao = Instantiate(prefabBotao, cedula.containerBotoes);
            //configura ele pela função de configurarção do botão recebendo botao, texto e função
            botao = HelperConfig.ConfigurarBtn(botao, cedula.btnNomes[i], funcaobtn[i]);
            //adiciona o botao a celula
            cedula.AdicionarBotao(botao);
        }

        return cedula;

    }


}
