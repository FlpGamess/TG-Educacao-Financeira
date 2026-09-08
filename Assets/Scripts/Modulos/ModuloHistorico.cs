using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ModuloHistorico : MonoBehaviour
{
    public GameObject prefabItemHistorico;
    public Transform containerItens;
    public Player player;

    void OnEnable()
    {
        Player.BensAtualizados += MontarHistorico;
    }

    void OnDisable()
    {
        Player.BensAtualizados -= MontarHistorico;
    }

    // Esse é o método pro botão, sem parâmetro
    public void AbrirHistorico()
    {
        gameObject.SetActive(true);
        MontarHistorico();
    }

    void MontarHistorico()
{
    foreach (Transform child in containerItens)
    {
        Destroy(child.gameObject);
    }

    foreach (ItensComprados bem in player.Bens)
    {
        GameObject item = Instantiate(prefabItemHistorico, containerItens);

        item.transform.Find("LinhaTopo/Nome").GetComponent<TMP_Text>().text = "Item: " + bem.dados.Nome;
        item.transform.Find("LinhaTopo/Tipo").GetComponent<TMP_Text>().text = "Tipo: " + bem.dados.Tipo;
        item.transform.Find("LinhaTopo/Duracao").GetComponent<TMP_Text>().text = "Duracao: " + bem.DuracaoAtual.ToString();
        item.transform.Find("Descricao").GetComponent<TMP_Text>().text = "Descricao: " + bem.dados.Descricao;
    }
}
    void passarDuracao ()
        {
            ModuloTempo.isSemanaAvancada += MontarHistorico;

            if (player.Bens.Count > 0)
            {
                foreach (ItensComprados bem in player.Bens)
                {
                    bem.DuracaoAtual -= 1;
                }
            }    
        }
}