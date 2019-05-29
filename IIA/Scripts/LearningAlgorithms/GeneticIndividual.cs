using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticIndividual : Individual {


	public GeneticIndividual(int[] topology) : base(topology) {
	}

    // Copiado do HillClimber
	public override void Initialize () 
	{
        for (int i = 0; i < totalSize; i++)
        {
            genotype[i] = Random.Range(-1.0f, 1.0f);
        }
    }
		
	public override void Crossover (Individual partner, float probability)
	{
        int n = partner.geno.Length;
        int indice = (int) Random.Range(0, n);
        for (int i = indice; i < n; i++)
        {
            if (Random.Range(0.0f, 1.0f) < probability)
            {
                //Debug.Log("Vou dar crossover neste gene");
                this.geno[i] = partner.geno[i];
            }
        }
    }

    // Copiado do HillClimber
	public override void Mutate (float probability)
	{
        for (int i = 0; i < totalSize; i++)
        {
            if (Random.Range(0.0f, 1.0f) < probability)
            {
                //Debug.Log("Vou mutar este gene");
                genotype[i] = Random.Range(-1.0f, 1.0f);
            }
        }
    }

	public override Individual Clone ()
	{
		GeneticIndividual new_ind = new GeneticIndividual(this.topology);

		genotype.CopyTo (new_ind.genotype, 0);
		new_ind.fitness = this.Fitness;
		new_ind.evaluated = false;

		return new_ind;
	}
    // Funçoes anteriores
    //public override void Initialize()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public override void Crossover(Individual partner, float probability)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public override void Mutate(float probability)
    //{
    //    throw new System.NotImplementedException();
    //}
    //public override Individual Clone()
    //{
    //    GeneticIndividual new_ind = new GeneticIndividual(this.topology);

    //    genotype.CopyTo(new_ind.genotype, 0);
    //    new_ind.fitness = this.Fitness;
    //    new_ind.evaluated = false;

    //    return new_ind;
    //}
}
