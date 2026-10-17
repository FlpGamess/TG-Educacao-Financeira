using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ModuloEventosAleatorios : MonoBehaviour
{
    [Header("Configuracao")]
    public  List<Eventos> listaEventos = new List<Eventos>();
    public  List<Eventos> eventosAtivos = new List<Eventos>();
    public static HashSet<TipoAfetado> tiposAfetados = new HashSet<TipoAfetado>();
    public ModuloInterface modulointerface;
    public GameObject btn;
    public Player player;
    public CatalogoLoja catalogoloja;

    [Header("Chance")]
    [Range(0f, 1f)]
    public float chance = 0.4f;

    public void Awake()
    {
        listaEventos = Resources.LoadAll<Eventos>("ScriptableObjects/Eventos").ToList();

    }
    private void OnEnable()
    {
        //adiciona RecuperarDisposição a isSemanaAvancada
        ModuloTempo.isSemanaAvancada += VencimentoEventos;
        
    }

    //quando o evento acaba vulgo desativado
    private void OnDisable()
    {
        //remove RecuperarDisposicao da fila is isSemanaAvancada

        ModuloTempo.isSemanaAvancada -= VencimentoEventos;
      

    }


    public  void VerificarGatilho(GatilhoEvento gatilho)
    {
        if (eventosAtivos.Count < 5)
        {
            List<Eventos> elegiveis = listaEventos.Where(e => e != null && !eventosAtivos.Any(a => a.id == e.id) && e.gatilho.Contains(gatilho)).ToList();
            if (elegiveis.Count == 0)
                return;
            if (Random.value > chance)
                return;

            CarregarEvento(elegiveis[Random.Range(0, elegiveis.Count)]);
        }
    }

    private void CarregarEvento(Eventos evento)
    {
        if (evento.decisao == true)
        {
            modulointerface.GerarEventosPopUp(evento, new System.Action[] { () => IniciarEvento(evento), () => { } /*() =>  Debug.Log("iniciou"), () =>  Debug.Log("nada")*/ }, new string[] { evento.opcao1, evento.opcao2 });

        }
        else
        {
            modulointerface.GerarEventosPopUp(evento, new System.Action[] { () => IniciarEvento(evento) /*() => Debug.Log("iniciou") */}, new string[] {evento.opcao1});
        }



    }

    private void IniciarEvento(Eventos evento)
    {
        Eventos novoEvento = Instantiate(evento);
        novoEvento.ativacao = true;
            eventosAtivos.Add(novoEvento);

        AttTiposAfetados();

        if(novoEvento.tipo == TipoEvento.Instantaneo)
        {
            player.PatrimonioEventos(novoEvento);
            catalogoloja.ItensEventos(novoEvento);

            eventosAtivos.Remove(novoEvento);
            AttTiposAfetados();
        }
    }

    public float ExecutarEventos(TipoAfetado tipo,float valor, IEnumerable<Eventos> eventos, string nomeAfetado = null)
    {
        foreach (Eventos evento in eventos) {
            foreach (Afetados afetado in evento.afetados)
            {
                if (afetado.tipo != tipo)
                    continue;

                if(nomeAfetado != null&& afetado.nome != "Todos" && afetado.nome != nomeAfetado)
                    continue;
                switch (afetado.tipobonus)
                {
                    case TipoBonus.Sum:
                        valor += afetado.bonus;
                        break;
                    case TipoBonus.Mult:
                        valor *= afetado.bonus;
                        break;
                    case TipoBonus.Div:
                        valor /= afetado.bonus;
                        break;
                    case TipoBonus.Porcent:
                        valor += valor * afetado.bonus;
                        break;
                }
            }

            }
        return valor;
    }

    public void VencimentoEventos()
    {
        foreach(Eventos evento in eventosAtivos.ToList())
        {
            if(evento.tipo == TipoEvento.Persistente)
            {
                continue;
            }
            evento.duracao--;
            if(evento.duracao < 0)
            {
                eventosAtivos.Remove(evento);
            }
        }
        AttTiposAfetados();
    }

    private void AttTiposAfetados()
    {
        tiposAfetados = new HashSet<TipoAfetado>(eventosAtivos.SelectMany(e => e.afetados).Select(a => a.tipo));
    }
}
