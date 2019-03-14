using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBarController : MonoBehaviour
{
    public Image content;
    public Unit theunit;
    public List<Unit> PlayersUnits;
    public Text unitinfo;
    public Text hptext;
    public Text movetext;



    // Update is called once per frame
    void Update()
    {
        if(theunit != null){
            UpdateBar(); 
        }
    }

    private void UpdateBar()
    {
        Tuple<float, float> currentUnitBonus = theunit.GetBonus(GameManager.instance.board, this.PlayersUnits);
        content.fillAmount = Map(theunit.hp + currentUnitBonus.Item1,0,theunit.maxHp ,0,1);
        unitinfo.text = theunit.id + " atk: " + (theunit.attack + currentUnitBonus.Item2);
        hptext.text = "HP: " + (theunit.hp + currentUnitBonus.Item1);
    }

    public float Map(float val, float min, float max,float outmin, float outmax){

        return (val - min) * (outmax-outmin) / (max- min) + outmin;
    }
}
