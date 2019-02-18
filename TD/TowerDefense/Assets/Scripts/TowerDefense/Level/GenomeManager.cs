using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evolution;
using TowerDefense.Agents;
using Redzen.Numerics.Distributions.Float;
using Util;

namespace TowerDefense.Level
{
    [RequireComponent(typeof(PlayerHomeBase))]
    public class GenomeManager : MonoBehaviour {
        /// <summary>
        /// How many zombunnies to spawn
        /// </summary>
        public static readonly int POPULATION_SIZE = 500;

        /// <summary>
        /// The mutation rate
        /// </summary>
        public static readonly float MUTATION_RATE = 0.5f;

        public static readonly float GROWTH_RATE = 0.0f;

        public static readonly float SPAWN_DELAY = .1f;

        private static readonly float STARTING_TOTAL = 16f;

        private static readonly double INITIAL_FITNESS = 1.0;

        // Must be greater than 1
        private static readonly double CHANCE_BASE = 0.5; //2; //5;

        /// <summary>
        /// The prefab to spawn as an enemy
        /// </summary>
        public GameObject ZombiePrefab;

        /// <summary>
        /// Where the Zombunnies spawn from
        /// </summary>
        public List<Transform> SpawnPoints;

        /// <summary>
        /// Where the zombies start facing
        /// </summary>
        public PlayerHomeBase target;

        private int remaining_alive;
        private List<Dictionary<Genome, double>> genomeMaps;
        private Dictionary<Genome, double> currentMap;


        // TODO
        private Stack<Genome> toSpawn;
        private ZombieAgent prefabAgent;
        private ZigguratGaussianSampler sampler;
        
        public void StartSpawning()
        {
            GenerateInitialGenomes();
            NextWave();
        }

        private void GenerateInitialGenomes()
        {
            genomeMaps = new List<Dictionary<Genome, double>>();
            genomeMaps.Add(new Dictionary<Genome, double>());
            currentMap = genomeMaps[0];
            sampler = new ZigguratGaussianSampler(0, MUTATION_RATE);
            for (int index = 0; index < POPULATION_SIZE; ++index)
            {
                currentMap.Add(Genome.RandomGenome(sampler, STARTING_TOTAL), INITIAL_FITNESS);
            }
            prefabAgent = ZombiePrefab.GetComponent<ZombieAgent>();
        }

        private void NextWave()
        {
            //Cull wave
            var culled = Cull(currentMap);

            //Reproduce and scramble order
            var reproduced = Reproduce(culled);

            // Mutate
            var nextWave = mutate(reproduced);
            print($"{currentMap.Count} {culled.Count} {reproduced.Count}");

            //Calculate averages
            float avg_speed = 0;
            float avg_damage = 0;
            float avg_health = 0;
            foreach (var genome in nextWave)
            {
                avg_speed += genome.MovementSpeed;
                avg_health += genome.Health;
                avg_damage += genome.Damage;
            }
            avg_speed /= POPULATION_SIZE;
            avg_health /= POPULATION_SIZE;
            avg_damage /= POPULATION_SIZE;

            print($"Average Speed: {avg_speed}, Health: {avg_health}, Damage: {avg_damage}");

            //Reset values
            remaining_alive = POPULATION_SIZE;
            var newMap = new Dictionary<Genome, double>(POPULATION_SIZE);
            genomeMaps.Add(newMap);
            currentMap = genomeMaps[genomeMaps.Count - 1];

            toSpawn = nextWave;

            //Spawn next wave
            InvokeRepeating("Spawn", SPAWN_DELAY, SPAWN_DELAY);
        }

        private Stack<Genome> mutate(Stack<Genome> genomes)
        {
            var nextWave = new Stack<Genome>(POPULATION_SIZE);
            foreach (var genome in genomes)
            {
                if (genome == null) continue;
                nextWave.Push(genome.mutate(sampler, GROWTH_RATE));
            }
            return nextWave;
        }

        private void Spawn()
        {
            if(toSpawn.Count == 1)
            {
                CancelInvoke("Spawn");
            }
            int spawn_index = Random.Range(0, SpawnPoints.Count);
            var prefabInstance = Instantiate(ZombiePrefab, SpawnPoints[spawn_index]);
            var agentInstance = prefabInstance.GetComponent<ZombieAgent>();
            agentInstance.Initialize(toSpawn.Pop(), target.transform.position, SpawnPoints[spawn_index].position);
            agentInstance.removed += (_) =>
            {
                ZombieDied(agentInstance);
            };
        }

        private static List<Pair<double, Genome>> Cull(Dictionary<Genome, double> toCull)
        {
            double max_fitness = double.NegativeInfinity; //toCull.GetEnumerator().Current.Value;
            double min_fitness = double.PositiveInfinity; //toCull.GetEnumerator().Current.Value;
            foreach (var fitness in toCull.Values)
            {
                if (fitness > max_fitness)
                {
                    max_fitness = fitness;
                }
                if (fitness < min_fitness)
                {
                    min_fitness = fitness;
                }
            }

            var ret = new List<Pair<double, Genome>>(toCull.Count);
            MonoBehaviour.print("Min fitness = " + min_fitness + " Max fitness = " + max_fitness);
            //Handle divide by 0 case
            if (max_fitness == min_fitness)
            {
                MonoBehaviour.print("Fitness: Min == Max");
                foreach(var pair in toCull)
                {
                    ret.Add(new Pair<double, Genome>(pair.Value, pair.Key));
                }
                return ret;
            }

            
            foreach (var pair in toCull)
            {
                var squashed = (pair.Value - min_fitness) / (max_fitness - min_fitness);
                var chance = System.Math.Pow(squashed, 1/CHANCE_BASE);
                if (Random.value < chance || pair.Value == max_fitness)
                {
                    ret.Add(new Pair<double, Genome>(pair.Value, pair.Key));
                }
            }
            return ret;
        }

        private static Stack<Genome> Reproduce(List<Pair<double, Genome>> toReproduce)
        {
            var nextWave = new Stack<Genome>(POPULATION_SIZE);
            var unshuffled = new List<Genome>(POPULATION_SIZE);
            double sum_fitness = 0;
            foreach (var pair in toReproduce)
            {
                sum_fitness += pair.First;
            }
            for(int ii = 0; ii < POPULATION_SIZE - toReproduce.Count + 1; ++ii)
            {
                double rand = Random.value * sum_fitness;
                double last_pos = 0;
                int jj;
                for(jj = 0; jj < toReproduce.Count - 1; ++jj)
                {
                    last_pos += toReproduce[jj].First;
                    if (rand < last_pos) break;
                }
                //jj--;
                //print($"{jj} {toReproduce.Count}");
                unshuffled.Add(toReproduce[jj].Second);
            }
            foreach (var pair in toReproduce)
            {
                unshuffled.Add(pair.Second);
            }
            // Shuffle genome
            return Shuffle(unshuffled);
        }

        /// <summary>
        /// Simple Fisher-Yates Shuffle
        /// WARNING: CONSUMES INPUT
        /// </summary>
        /// <param name="genomes"></param>
        /// <returns></returns>
        private static Stack<Genome> Shuffle(List<Genome> genomes)
        {
            var ret = new Stack<Genome>(genomes.Count);
            int n = genomes.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n);
                Genome value = genomes[k];
                genomes[k] = genomes[n];
                genomes[n] = value;
                ret.Push(value);
            }
            return ret;
        }

        //Callback function on zombunny death
        public void ZombieDied(ZombieAgent agent)
        {
            //MonoBehaviour.print("ZombieDied Fitness: " + agent.Fitness);
            --remaining_alive;
            currentMap.Add(agent.Genome, agent.Fitness);
            if (remaining_alive == 0)
            {
                NextWave();
            }
        }
    }
}
