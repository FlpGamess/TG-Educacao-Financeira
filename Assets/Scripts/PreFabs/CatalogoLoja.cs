using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class CatalogoLoja : MonoBehaviour
{
    public List<Itens> itens = new List<Itens>();
    public Dictionary<AtributosFinanceiros, List<Itens>> catalogo = new() {
    {AtributosFinanceiros.DespesasDoLar,new List<Itens>()},
        {AtributosFinanceiros.Moradia,new List<Itens>()},
        {AtributosFinanceiros.Lazer,new List<Itens>()},
        {AtributosFinanceiros.SaudeBemEstar,new List<Itens>()},
        {AtributosFinanceiros.Educacao,new List<Itens>()}
    };


    void Start()
    {
        CarregarCatalogo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CarregarCatalogo()
    {
        LimparListas();

        //inicialização com o dicionario vazio
        /*foreach(AtributosFinanceiros atbf in Enum.GetValues(typeof(AtributosFinanceiros))){
            catalogo.Add(atbf, new List<Itens>());
        }*/

        itens.AddRange(Resources.LoadAll<Itens>("ScriptaableObjects"));

        foreach(Itens it in itens)
        {
            catalogo[it.Categoria].Add(it);
        }
    }

    public void LimparListas() {
        itens.Clear();

        foreach (List<Itens> lista in catalogo.Values)
        {
            lista.Clear();
        }
    }
}
