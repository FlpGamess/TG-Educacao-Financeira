using System;
using TMPro;
using UnityEngine;

public class ModuloTempo : MonoBehaviour
{
    public static int semana = 0;
    public static int mes = 1;

    public static bool inicioMes;
    public static bool fimMes;
    
    public TextMeshProUGUI semanav;
    public static event Action isSemanaAvancada;
    public static event Action isMesAvancado;


    //caso o evento seja ativado
    private void OnEnable()
    {
        //adiciona na fila do evento isEsgotado a fun  o atualizar semana
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

    public static int[] SemanasDMes(int mes)
    {
        int primeirasemana = ((mes - 1) * 4) + 1;

        return new int[]
        {
            primeirasemana,
            primeirasemana + 1,
            primeirasemana + 2,
            primeirasemana + 3,
        };
    }


    
    //atualiza a semana, resumidamente semana+=1
    public void AtualizarSemana()
    {
        semana++;
        ModuloInterface.AtualizarTxt(semanav, "Semana", semana.ToString());
        isSemanaAvancada?.Invoke();
        AtualizarMes();
    }

    public void AtualizarMes()
    {
        int aux = ((semana - 1) / 4) + 1;
        inicioMes = (semana -1 )%4 ==0;
        fimMes= semana % 4 ==0 && semana != 0;
        if (mes < aux)
        {
            mes = aux;
           isMesAvancado?.Invoke();
        }
    }




}
