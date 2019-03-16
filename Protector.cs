using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Protector : Unit
{
    private int[,] ProtectorAttackRange = {
            {0, 1}, {1, 0}, {0, -1}, {-1, 0} ,
        };

    private int[,] ProtectorNeighbourhood = {
            {0, 1}, {1, 0},  {-1, 0}, {0, -1},
        };

    public Protector()
    {
        base.neighbourhood = ProtectorNeighbourhood;
        base.attackrange = ProtectorAttackRange;
        base.attack = 25;
        base.hp = 300;
        base.defense = 0;
        this.maxHp = 300;
        base.hpbonus = .20f;
        this.pontos = 2;
        
    }

}
