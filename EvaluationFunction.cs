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
        //nº de peças
        int pieces1 = 0;
        int pieces2 = 0;
        int fpieces, fataque;
        // Vida total
        float acumHP1=0, acumHP2=0;
        // Pontos total
        int acumPt1 = 0, acumPt2 = 0;
        // Atk total
        float acumAtk1 = 0, acumAtk2 = 0;
        // Pesos
        int wP=1, wHP=2, wPT=3, wATK = 1;
        // Fatores
        int fPts;
        float fHP, fATK;
        foreach (Unit t in s.PlayersUnits)
        {
            acum1 += t.hp * t.pontos;
            pieces1++;
            acumHP1 += t.hp+t.hpbonus;
            acumPt1 += t.pontos;
            acumAtk1 += t.attack + t.attackbonus;
        }
        foreach (Unit t in s.AdversaryUnits)
        {
            acum2 += t.hp * t.pontos;
            pieces2++;
            acumHP2 += t.hp + t.hpbonus;
            acumPt2 += t.pontos;
            acumAtk2 += t.attack + t.attackbonus;
        }
        fpieces = (pieces1 - pieces2);
        fataque = 5;
        fPts = acumPt1 - acumPt2;
        fHP = acumHP1 - acumHP2;
        fATK = acumAtk1 - acumAtk2;
        // Para favorecer o ataque
        // Se o estado for de ataque
        if (s.isAttack)
        {
            return acum1 * fpieces * fataque - acum2;
        }
        // Se for para mover uma peça
        else
        {
            return wHP * fHP + wPT * fPts + wP * fpieces + wATK * fATK;
        }

    }

    /*
    public float evaluate(State s)
    {

        float acum1 = 0;
        float acum2 = 0;
        //nº de peças
        int pieces1 = 0;
        int pieces2 = 0;
        int fpieces, fataque;

        foreach (Unit t in s.PlayersUnits)
        {
            acum1 += t.hp * t.pontos;
            pieces1++;
        }
        foreach (Unit t in s.AdversaryUnits)
        {
            acum2 += t.hp * t.pontos;
            pieces2++;

        }
        fpieces = (pieces1 - pieces2);
        fataque = 50;

        //Debug.Log("Valor do acum1:" + acum1 + " Valor do acum2:" + acum2 + "Valor avaliado"+(acum1-acum2));

        //Se for um estado de ataque
        if (s.isAttack)
        {
            return acum1 * fpieces * fataque - acum2;
        }
        // Se for para mover uma unidade
        else
        {
            return MaisProximo(s);
        }

    }
    */   

    // Vai receber a peça que se vai mover no determinado estado e devolve a distância à peça mais perto do inimigo
    public float MaisProximo(State s)
    {
        // Unidade que vai andar
        Unit u = s.unitToPermormAction;
        float aux = 99999;
        foreach (Unit t in s.AdversaryUnits)
        {
            if (distancia(s.unitToPermormAction, t) < aux)
            {
                aux = distancia(s.unitToPermormAction, t);
            }

        }
        return aux;


    }
    public float distancia(Unit u1, Unit u2)
    {
        return (float)Math.Sqrt(Math.Pow(u2.x - u1.x, 2) + Math.Pow(u2.y - u1.y, 2));
    }
    /*
    public float evaluate(State s)
    {
        float acum1 = 0;
        float acum2 = 0;
        //nº de peças
        int pieces1 = 0;
        int pieces2 = 0;
        int fpieces, fataque;

        foreach (Unit t in s.PlayersUnits)
        {
            acum1 += t.hp * t.pontos;
            pieces1++;
        }
        foreach (Unit t in s.AdversaryUnits)
        {
            acum2 += t.hp * t.pontos;
            pieces2++;

        }
        fpieces = (pieces1 - pieces2) * 3;
        fataque = 5;
        //Debug.Log("Valor do acum1:" + acum1 + " Valor do acum2:" + acum2 + "Valor avaliado"+(acum1-acum2));

        // Para favorecer o ataque
        if (s.isAttack)
        {
            return acum1 * fpieces * fataque - acum2;
        }

        // Se estiver em desvantagem de peças, tentar fugir
        if (fpieces < 0 && s.isAttack == false)
        {
            return acum1 - acum2 * fpieces * 5;// estes fatores a multiplicar são deste lado?
        }

        return acum1 - acum2;

    }
    */   

}
