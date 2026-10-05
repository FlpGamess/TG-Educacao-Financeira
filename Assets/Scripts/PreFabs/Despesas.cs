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
        juros = tipocompra == TipoPagamento.Cartão_Credito && parcela >= 4 ? 0.05f : 0f;

        float total = Mathf.Round((compra + (compra * juros)) * 100f) / 100f;
        this.valor = total;
        if (tipocompra == TipoPagamento.Cartão_Credito)
        {
            //codigo para todas as parcelas do cartão vencerem na primeir a semana de cada mes
            //comentado pra utilização do calendario ja que a abordagem dos pagamentos foi simplificada
            /*
            int semanaPGTO = semana - (semana % 4) +1;
            if (semanaPGTO <= semana)
            {
                semanaPGTO += 4;
            }
            semana += semanaPGTO;*/
            semana += 4;
        }    
        if (parcela == 0)
        {
            Parcela p = new Parcela(compra, semana);
            parcelas.Add(p);
            Debug.Log($"[Cobrança]: Valor de {compra} agendado para a semana {semana}");
        }
        else if (parcela > 0)
        {
            float tparcelado = 0;
            float vparcelado = Mathf.Round((total / parcela) * 100f) / 100f;
            for (int i = 0; i < parcela; i++)
            {
                tparcelado += vparcelado;
                if(parcela-i ==1 && compra > tparcelado)
                {
                    vparcelado = vparcelado + (compra - tparcelado);
                }
                Parcela p = new Parcela(vparcelado, semana);
                parcelas.Add(p);
                Debug.Log($"[Cobrança]: Parcela de {vparcelado} agendada para a semana {semana}");
                semana += 4;
               
            }
        }
       
        }

}

