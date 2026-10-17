using UnityEngine;
using System.Collections.Generic;


public enum TipoEvento
{
    Instantaneo,
    Temporario,
    Persistente

}

public enum GatilhoEvento
{
    Trabalhar,Comprar,Investir,AvancarSemana,
}

public enum TipoAfetado
{
    Item,
    AtributoFinanceiro,Salario,Investimentos,Disposicao, Patrimonio
}

public enum TipoBonus
{
  Sum,
  Mult,
  Div,
  Porcent
}

[System.Serializable]
public class Afetados {
    public TipoAfetado tipo;
    public string nome = null;
    public TipoBonus tipobonus;
    public float bonus;
}

[CreateAssetMenu(fileName = "Eventos", menuName = "Scriptable Objects/Eventos/Eventos")]
public class Eventos : ScriptableObject
{
    [Header("Identificacao")]
    public int id;
    public string nome =null;

    [TextArea]
    public string descricao;

    [Header("Ativacao")]
    public List<GatilhoEvento>  gatilho = new List<GatilhoEvento>();
    public TipoEvento tipo;
    public bool decisao;
    public string opcao1;
    public string opcao2;
    public bool ativacao = false;

   public List<Afetados> afetados = new List<Afetados>();

    public int duracao;

}
