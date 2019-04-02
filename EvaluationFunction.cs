using UnityEngine;
using System.Collections;
using System;

public class EvaluationFunction
{
    // estado terminal é quando morrem todos de uma equipa
    // função de avaliação calcula o valor para cada um dos nós
    // Do the logic to evaluate the state of the game !
    public float evaluate(State s)
    {
        float acum1 = 0;
        float acum2 = 0;
        foreach (Unit t in s.PlayersUnits)
        {
            acum1 += t.hp * t.pontos;
        }
        foreach (Unit t in s.AdversaryUnits)
        {
            acum2 += t.hp * t.pontos;
        }
        return acum1-acum2;
    }
}
