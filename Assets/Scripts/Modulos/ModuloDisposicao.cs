using System;
using TMPro;
using UnityEngine;

public class ModuloDisposicao : MonoBehaviour
{
    //a "stamina" do jogador
    //trabalho 40%
    //lazer 10%
    //estudos de 10% a 30%
    public int disposicao = 100;
    //text que recebe a disposi��o
    public TextMeshProUGUI disposicaov;
    //evento se o personagem ta esgotado(basicamente uma fila de fun��es que vao ser executadas)
    public static event Action isEsgotado;

    //quando o evento for ativado
    private void OnEnable()
    {
        //adiciona RecuperarDisposi��o a isSemanaAvancada
        ModuloTempo.isSemanaAvancada += RecuperarDisposicao;
    }

    //quando o evento acaba vulgo desativado
    private void OnDisable()
    {
        //remove RecuperarDisposicao da fila is isSemanaAvancada

        ModuloTempo.isSemanaAvancada -= RecuperarDisposicao;

    }

    void Start()
    {
        //disposi��o come�a em 100
        // disposicao = 100;
        // CalcularDisposicao(-130);
        AlterarDisposicao();
    }

    public void IniciarPartidaNova()
    {
        disposicao = 100;
        AlterarDisposicao();
    }

    // Update is called once per frame
    void Update()
    {
        AlterarDisposicao();
    }

    //fun��o que calcula a disposi��o do personagem 
    //para adicionar use numero positivo e para remover negativos
    public void CalcularDisposicao(int newD)
    {
        //calcula a disposi��o
        disposicao = disposicao + newD;
        //caso a disposi��o seja menor ou igual que -30 chama o evento isEsgotado
        if (disposicao <= -30)
        {
            Debug.Log("Bobbie goods");
            isEsgotado?.Invoke();
        }
    }

    //fun��o de recupera��o da disposi��o
    public void RecuperarDisposicao()
    {
        //se a disposi��o terminar a semana positiva
        if (disposicao >= 0)
        {
            disposicao = 100;
        }
        //se nao se a disposi��o terminar entre -1 e 29
        else if (disposicao < 0 && disposicao>-30) {
            disposicao = 100 + disposicao;
        }
        //caso chegue a -30 ou menos, vulgo estourou o limite
        else
        {
            disposicao = 100 - 40;
        }
    }
    public void AlterarDisposicao()
    {
        ModuloInterface.AtualizarTxt(disposicaov, disposicao.ToString(), "%");
       
    }
}
