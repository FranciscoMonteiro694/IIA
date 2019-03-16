using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Mage : Unit
{

    private int[,] MageAttackRange = {
            {0, 2}, {2, 0}, {0, -2}, {-2, 0},
            {0, 1}, {1, 0}, {0, -1}, {-1, 0} ,
        };
    private int[,] MageNeighbourhood = {
            {0, 1}, {1, 0},  {-1, 0}, {0, -1}, 
        };
    public Mage()
    {
        base.neighbourhood = MageNeighbourhood;
        base.attackrange = MageAttackRange;
        base.attack = 50;
        base.defense = 0;
        base.hp = 200;
        this.maxHp = 200;
        this.pontos = 1;
    }
}
