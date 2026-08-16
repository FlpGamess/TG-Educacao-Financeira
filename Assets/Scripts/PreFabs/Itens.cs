using UnityEngine;

[CreateAssetMenu(fileName = "Itens", menuName = "Scriptable Objects/Itens")]
public class Itens : ScriptableObject
{
    //Nome do Item
    public string Nome;
    //Descricao do Item
    public string Descricao;
    //Valor do item
    public float Preco;
    //Categoria financeira
    public AtributosFinanceiros Categoria;
    //O que o item é: movel,serviço,comida,etc
    public string Tipo;
    //caso o item aumente algo
    public int efeito;
    //quantas semanas o item dura, para itens eternos colocar -1
    public int Duracao;
    //quantidade maxima que um item pode ser obtido
    public int QuantidadeMax;

}
