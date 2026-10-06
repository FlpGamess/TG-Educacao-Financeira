using UnityEngine;

[System.Serializable]
public class PagamentoRealizado
{
    public string nome;
    public float valor;
    public int semana;
    public int parcela;
    public AtributosFinanceiros categoria;
    public TipoPagamento tpagamento;

    public PagamentoRealizado(string nome, float valor,int semana, int parcela, AtributosFinanceiros categoria, TipoPagamento tpagamento) {
        this.nome = nome;
        this.valor = valor;
        this.semana = semana;
        this.parcela = parcela;
        this.categoria = categoria;
        this.tpagamento = tpagamento;
}
}
