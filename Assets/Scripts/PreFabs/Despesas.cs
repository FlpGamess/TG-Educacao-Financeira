using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Despesas
{
    public Itens item;
    public AtributosFinanceiros categoria;
    public TipoPagamento tipocompra;
    public List<Parcela> parcelas = new List<Parcela>();
    public float juros;
    public bool isPaga;
    public float valor;

    public Despesas(Itens item, TipoPagamento tipoCompra, float valor)
    {
        this.item = item;
        this.categoria = item.Categoria;
        this.tipocompra = tipoCompra;
        this.valor = valor;
        this.isPaga = false;
    }

}

