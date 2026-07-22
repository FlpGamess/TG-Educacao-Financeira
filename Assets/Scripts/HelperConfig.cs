using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HelperConfig : MonoBehaviour
{
public static GameObject ConfigurarBtn(GameObject botao, string texto, UnityAction action)
    {
        Debug.Log(botao);
        botao.GetComponentInChildren<TextMeshProUGUI>().text = texto;
        botao.GetComponent<Button>().onClick.AddListener(action);

        return botao;
    }
}
