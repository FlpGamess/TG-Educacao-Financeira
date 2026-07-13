using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

//Script do player
//Responsavel pelos atributos do player financeiros
//Stamina, entre outros referentes a ele
public class Player : MonoBehaviour
{
    //guarda os atributos financeiros do player
    /*despesas do lar = desplar
      educação = educ
      moradia = morad
      saude_bem_estar = saube
      lazer = laz
     */
    IDictionary<string,int> AtbFinanceiros = new Dictionary<string,int>()
    {
        {"desplar",0},
        {"educ",0},
        {"morad",0},
        {"saube",0},
        {"laz",0}
    };

    //total na conta do jogador
    public float patrimonio = 0;
    public TextMeshProUGUI saldocontav;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AlterarSaldoConta();
    }

    // Update is called once per frame
    void Update()
    {
        AlterarSaldoConta();
     
    }

    //teste

    public void AlterarSaldoConta()
    {
        saldocontav.text = "$"+patrimonio.ToString();
    }

 
}
