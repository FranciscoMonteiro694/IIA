using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MetaHeuristic {
	public float mutationProbability;
	public float crossoverProbability;
	public int tournamentSize; // 
	public bool elitist;//número de individuos que são preservados de uma geração para a outra
    public int numeroValoresPreservados;


    // Função alterada, original em baixo
	public override void InitPopulation() {
        population = new List<Individual>();
        Debug.Log("Estou a inicializar a população no Genetic");
        // jncor 
        while (population.Count < populationSize)
        {
            GeneticIndividual new_ind = new GeneticIndividual(topology);
            new_ind.Initialize();
            population.Add(new_ind);
        }
        Debug.Log("População inicializada");
        Debug.Log("Valor"+numeroValoresPreservados);
	}
    // Função alterada, original em baixo
    public override void Step()
    {

        // You should implement the code runs in each generation here
        // Nova população
        int contador;
        contador = 0;
        List<Individual> new_pop = new List<Individual>();
        updateReport();
        for (int i = 0; i < populationSize; i++)
        {
            // Agora vamos fazer a seleção por torneio
            Individual x = torneio();
            Individual y = torneio();
            x.Crossover(y, crossoverProbability);
            x.Mutate(mutationProbability);
            new_pop.Add(x);
        }

        if (elitist)
        {
            while (contador<numeroValoresPreservados)
            {
                new_pop[contador] = overallBest.Clone();
                contador++;
            }
        }

        population = new_pop;

        generation++;
    }

    public Individual torneio()
    {
        int contador;
        contador = 0;
        int indice;
        int indiceAux=0;
        List<Individual> auxiliar = new List<Individual>();
        Individual aux;
        float best=-999999;
        while (contador < tournamentSize)
        {
            indice = (int)Random.Range(0, populationSize);
            //Debug.Log("Indice do torneio"+indice);
            aux = population[indice].Clone();
            auxiliar.Add(aux);
            if (aux.Fitness > best)
            {
                best = aux.Fitness;
                indiceAux = contador;
            }
            contador++;
        }

        return auxiliar[indiceAux].Clone();
    }


    // Função inicial
    //public override void InitPopulation()
    //{
    //    //You should implement the code to initialize the population here
    //    // 
    //    throw new System.NotImplementedException();
    //}

    //The Step function assumes that the fitness values of all the individuals in the population have been calculated.
    //   public override void Step() {
    //	//You should implement the code runs in each generation here
    //       // Usar o código do livro (?)
    //	throw new System.NotImplementedException ();
    //}

}
