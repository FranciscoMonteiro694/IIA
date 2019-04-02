using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Assassin : Unit
{

    private int[,] AssassinAttackRange = {
            {0, 1}, {1, 0}, {0, -1}, {-1, 0} ,
            {1,1},  {-1,1}, {1,-1}, {-1,-1}
        };
    private int[,] AssassinNeighbourhood = {
            {0, 1}, {1, 0},  {0, -1}, {-1, 0},
            {1,1},  {-1,1}, {1,-1}, {-1,-1},
        };
    public Assassin()
    {
        base.neighbourhood = AssassinNeighbourhood;
        base.attackrange = AssassinAttackRange;
        base.attack = 100;
        base.defense = 0;
        base.hp = 150;
        this.maxHp = 150;
        this.pontos = 1;
    }
}
