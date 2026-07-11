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
    //text que recebe a disposição
    public TextMeshProUGUI disposicaov;
    //evento se o personagem ta esgotado(basicamente uma fila de funções que vao ser executadas)
    public static event Action isEsgotado;

    //quando o evento for ativado
    private void OnEnable()
    {
        //adiciona RecuperarDisposição a is Esgotado
        isEsgotado += RecuperarDisposicao;
    }

    //quando o evento acaba vulgo desativado
    private void OnDisable()
    {
        //remove RecuperarDisposicao da fila is Esgotado
        isEsgotado -= RecuperarDisposicao;
    }

    void Start()
    {
        //disposição começa em 100
        disposicao = 100;
        CalcularDisposicao(-130);
    }

    // Update is called once per frame
    void Update()
    {
        AlterarDisposicao();
    }

    //função que calcula a disposição do personagem 
    //caso ela seja menor ou igual que -30 chama o evento isEsgotado
    public void CalcularDisposicao(int newD)
    {
        disposicao = disposicao + newD;

        if(disposicao <= -30)
        {
            Debug.Log("Bobbie goods");
            isEsgotado?.Invoke();
        }
    }

    //função de recuperação da disposição
    public void RecuperarDisposicao()
    {
        //se a disposição terminar a semana positiva
        if (disposicao >= 0)
        {
            disposicao = 100;
        }
        //se nao se a disposição terminar entre -1 e 29
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
        disposicaov.text = disposicao.ToString() + "%";
    }
}
