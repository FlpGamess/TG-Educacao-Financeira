using UnityEngine;

public class ParcelaCalendario
{
    public string nome;
    public float valor;
    public AtributosFinanceiros categoria;

    public ParcelaCalendario(string nome, float valor, AtributosFinanceiros categoria)
    {
        this.nome = nome;
        this.valor = valor;
        this.categoria = categoria;
    }
}
