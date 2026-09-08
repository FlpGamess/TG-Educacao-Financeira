using System;

[Serializable]
public class ItensComprados
{
    public Itens dados;

    public int DuracaoAtual;

    public ItensComprados(Itens itemOriginal)
    {
        dados = itemOriginal;
        DuracaoAtual = itemOriginal.Duracao;
    }
}