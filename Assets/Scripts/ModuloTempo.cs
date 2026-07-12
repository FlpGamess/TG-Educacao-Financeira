using System;
using TMPro;
using UnityEngine;

public class ModuloTempo : MonoBehaviour
{
    public int semana = 0;
    public TextMeshProUGUI semanav;
    public static event Action isSemanaAvancada;

    //caso o evento seja ativado
    private void OnEnable()
    {
        //adiciona na fila do evento isEsgotado a função atualizar semana
        ModuloDisposicao.isEsgotado += AtualizarSemana;
    }
    //quando o evento acaba vulgo desativado
    private void OnDisable()
    {        
        //remove RecuperarDisposicao da fila is Esgotado
        ModuloDisposicao.isEsgotado -= AtualizarSemana;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {        
    }


    
    //atualiza a semana, resumidamente semana+=1
    public void AtualizarSemana()
    {
        isSemanaAvancada?.Invoke();
        semana++;
        semanav.text = "Semana"+semana.ToString() ;
    }


}
