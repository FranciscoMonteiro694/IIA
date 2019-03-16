using UnityEngine;
using System.Collections;
using System;

public class UtilityFunction
{

    public float evaluate(State s)
    {
        // Se o gajo que chama a função não tiver unidades, é porque perdeu
        if (s.PlayersUnits.Count == 0)
            return -1;
        // Caso contrário devolve 1, visto não haver empates
        else
            return 1;
    }
}
