using UnityEngine;
using System.Collections;
using System;

public class UtilityFunction
{

    public float evaluate(State s)
    {
        float acum1 = 0;
        if(s.AdversaryUnits.Count == 0)
        {
            foreach (Unit t in s.PlayersUnits)
            {
                acum1 += t.hp * t.pontos;
            }
            return acum1;
        }
        else
        {
            foreach (Unit t in s.AdversaryUnits)
            {
                acum1 += t.hp * t.pontos;
            }
            return -acum1;
        }
    }
}
