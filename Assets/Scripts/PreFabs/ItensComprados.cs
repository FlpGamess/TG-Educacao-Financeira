using System;

[Serializable]
public class ItensComprados
{
    public Itens dados;

    public int DuracaoAtual;
    
    public string itemId;

    public ItensComprados(Itens itemOriginal)
    {
        dados = itemOriginal;
        itemId = itemOriginal != null ? itemOriginal.Id : "";
        DuracaoAtual = itemOriginal.Duracao;
    }
}