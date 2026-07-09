using TMPro;
using UnityEngine;

public class ModuloTempo : MonoBehaviour
{
    public int semana = 0;
    public TextMeshProUGUI semanav;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.disposicao == 0)
        {
            semana++;
            Player.disposicao = 100;
        }
        AtualizarSemana();
    }

    void AtualizarSemana()
    {
        semanav.text = "Semana"+semana.ToString() ;
    }
}
