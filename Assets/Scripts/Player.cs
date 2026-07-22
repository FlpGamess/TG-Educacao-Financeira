using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Script do player
//Responsavel pelos atributos do player financeiros
//Stamina, entre outros referentes a ele
public class Player : MonoBehaviour
{
    //guarda os atributos financeiros do player
    /*despesas do lar = desplar
      educacao = educ
      moradia = morad
      saude_bem_estar = saube
      lazer = laz
     */
    public static IDictionary<AtributosFinanceiros, int> AtbFinanceiros = new Dictionary<AtributosFinanceiros, int>()
    {
        {AtributosFinanceiros.DespesasDoLar,0},
        {AtributosFinanceiros.Moradia,0},
        {AtributosFinanceiros.Lazer,0},
        {AtributosFinanceiros.SaudeBemEstar,0},
        {AtributosFinanceiros.Educacao,0}
    };

    public int desplar;
    public int educ;
    public int morad;
    public int saube;
    public int laz;

    public Image slot1;
    public Image slot2;
    public Image slot3;
    public Image slot4;
    public Image slot5;

    public Sprite spriteDesplar;
    public Sprite spriteEduc;
    public Sprite spriteMorad;
    public Sprite spriteSaube;
    public Sprite spriteLaz;



    //total na conta do jogador
    public float patrimonio = 0;
    public TextMeshProUGUI saldocontav;

    public ModuloTempo moduloTempo;

    void Start()
    {
        patrimonio = 1621.00f;
        AlterarSaldoConta();
        ModuloTempo.isSemanaAvancada += AtualizarPatrimonio;

        slot1.sprite = spriteDesplar;
        slot2.sprite = spriteEduc;
        slot3.sprite = spriteMorad;
        slot4.sprite = spriteSaube;
        slot5.sprite = spriteLaz;
    }

    void AtualizarPatrimonio()
    {
        if (ModuloTempo.semana > 1 && (ModuloTempo.semana - 1) % 4 == 0)
        {
            patrimonio += 1621.00f;
            AlterarSaldoConta();
        }
    }

    public void AlterarSaldoConta()
    {
        saldocontav.text = "$" + patrimonio.ToString();
    }


}
