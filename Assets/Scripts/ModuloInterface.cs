using UnityEngine;
using System.Collections.Generic;

public class ModuloInterface : MonoBehaviour
{
    //recebe o objeto pai da interface(primeiro painel)
    public GameObject Interface;
    GameObject menuCelular;
    //Lista que guarda todas as janelas que tão abertas
    List<GameObject> janelasAbertas = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    //define menuCelular como o menu do MenuCelular sendo o menu padrão
     menuCelular = Interface.transform.Find("MenuCelular").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Função para abrir janelas do Menu, recebe o obj da janela que quer abrir
    public void Ativarjanela(GameObject janela)
    {
        //se a janela não esta na lista de janelas abertas
        if(!janelasAbertas.Contains(janela)) {
            //adiciona janela em janelasAbertas no indice 0
            janelasAbertas.Insert(0,janela);
            //ative a janela no indice 0 de janelasAbertas
            janelasAbertas[0].SetActive(true);
        }
    }
    //descobrir o indice de uma janela
    //janelasAbertas.IndexOf(janela);

    //remover por indice
    //janelasAbertas.removeAt(numero);

    //Função para ocultar janelas, recebe a janela a ser ocultada(fechada)
    public void OcultarJanela(GameObject janela)
    {
        //se a janela estiver presente na lista janelasAbertas
        if (janelasAbertas.Contains(janela))
        {
            //ache a posição dessa janela na lista
            int indice = janelasAbertas.IndexOf(janela);
            //desative a janela na posição encontrada
            janelasAbertas[indice].SetActive(false);
            //remova a janela na posição encontrada da lista 
            janelasAbertas.RemoveAt(indice);
        }
    }

    public void FuncaoProvisoriaVouApagarDpsHomenagemACaioPrime()
    {
        Debug.Log("Disponivel no futuro!!");
    }
}

