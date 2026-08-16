using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

    public void GerarParcelas(float compra, int semana, int parcela)
    {
        if (parcela == 0)
        {
            Parcela p = new Parcela(compra, semana);
            parcelas.Add(p);
        }
        else if (parcela > 0)
        {
            float tparcelado = 0;
            float vparcelado = Mathf.Round( (compra / parcela)*100f)/100f;
            for (int i = 0; i < parcela; i++)
            {
                tparcelado += vparcelado;
                if(parcela-i ==1 && compra > tparcelado)
                {
                    vparcelado = vparcelado + (compra - tparcelado);
                }
                Parcela p = new Parcela(vparcelado, semana + 4);
                parcelas.Add(p);
                semana += 4;
               
            }
        }
    }

}

