using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public Player player;
    public ModuloEventosAleatorios moduloEventosA;
    public event Action PrecosAtualizados;

    void Start()
    {
        CarregarCatalogo(player.Bens);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CarregarCatalogo(List<ItensComprados> bens)
    {
        LimparListas();

        //inicialização com o dicionario vazio
        /*foreach(AtributosFinanceiros atbf in Enum.GetValues(typeof(AtributosFinanceiros))){
            catalogo.Add(atbf, new List<Itens>());
        }*/
        bool hasEventoItem = ModuloEventosAleatorios.tiposAfetados.Contains(TipoAfetado.Item);
        bool hasEventoAtb = ModuloEventosAleatorios.tiposAfetados.Contains(TipoAfetado.AtributoFinanceiro);
        Itens[] ItensOriginais = Resources.LoadAll<Itens>("ScriptableObjects");

        foreach(Itens it in ItensOriginais)
        {
            if (bens.Count(bens => bens.dados.Nome == it.Nome) == it.QuantidadeMax){
                continue;
            }

            Itens item = Instantiate(it);
            if (hasEventoItem)
            {
;                item.Preco = moduloEventosA.ExecutarEventos(TipoAfetado.Item, item.Preco, moduloEventosA.eventosAtivos, item.Nome);
            }
             if (hasEventoAtb) {
                item.Preco = moduloEventosA.ExecutarEventos(TipoAfetado.AtributoFinanceiro, item.Preco, moduloEventosA.eventosAtivos,item.Categoria.ToString());
            }
                itens.Add(item);
            catalogo[item.Categoria].Add(item);
        }
    }

    public void LimparListas() {
        itens.Clear();

        foreach (List<Itens> lista in catalogo.Values)
        {
            lista.Clear();
        }
    }

    public void ItensEventos(Eventos evento)
    {
        foreach (Itens item in itens)
        {
            float precoAnterior = item.Preco;

            item.Preco = moduloEventosA.ExecutarEventos(
                TipoAfetado.Item,
                item.Preco,
                new[] { evento },
                item.Nome
            );

            Debug.Log(
                $"Evento: {evento.nome} | Item: {item.Nome} | " +
                $"Antes: {precoAnterior} | Depois: {item.Preco}"
            );
        }
        PrecosAtualizados?.Invoke();
    }
}
