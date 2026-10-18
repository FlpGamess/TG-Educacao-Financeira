using System.Collections;
using UnityEngine;



public class InicializarPartida : MonoBehaviour
{

    public void Salvar()
    {
        salvamento.SalvarJogo(player, moduloDisposicao, moduloEconomia);
    }

    public void Carregar()
    {
        salvamento.CarregarJogo(player, moduloDisposicao, moduloEconomia);
    }
    public ModuloGameplay salvamento;

    public ModuloDisposicao moduloDisposicao;
    public ModuloEconomia moduloEconomia;
    public ModuloTempo moduloTempo;
    public Player player;

    private IEnumerator Start()
    {
        yield return null;

        ModuloTempo.semana = 1;
        ModuloTempo.mes = 1;
        ModuloTempo.inicioMes = true;
        ModuloTempo.fimMes= false;

        player.IniciarPartidaNova();
        moduloDisposicao.IniciarPartidaNova();

        moduloEconomia.RestaurarControles(-1, false);
        moduloEconomia.AtualizarDepoisDeRestaurar();

        ModuloInterface.AtualizarTxt(player.moduloTempo.semanav,"Semana: ",ModuloTempo.semana.ToString());

    }
}