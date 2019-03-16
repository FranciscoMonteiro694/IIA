using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Warrior : Unit
{

    private int[,] WarriorAttackRange = {
            {0, 1}, {1, 0}, {0, -1}, {-1, 0} ,
        };
    private int[,] WarriorNeighbourhood = {
            {0, 1}, {1, 0},  {-1, 0}, {0, -1},
            };

    public Warrior() {
        base.neighbourhood = WarriorNeighbourhood;
        base.attackrange = WarriorAttackRange;
        base.hp = 300;
        base.attack = 25;
        base.defense = 0;
        this.maxHp = 300;
        base.attackbonus = .20f;
        this.pontos = 2;
    }



}
