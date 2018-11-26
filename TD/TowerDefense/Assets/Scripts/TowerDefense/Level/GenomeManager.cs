using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evolution;
using TowerDefense.Agents;
using Redzen.Numerics.Distributions.Float;

namespace TowerDefense.Level
{
    public class GenomeManager : MonoBehaviour {
        /// <summary>
        /// How many zombunnies to spawn
        /// </summary>
        public readonly int POPULATION_SIZE = 10;

        /// <summary>
        /// The mutation rate
        /// </summary>
        public readonly float MUTATION_RATE = 0.2f;

        public readonly float GROWTH_RATE = 0.3f;

        public readonly float SPAWN_DELAY = 2f;

        private readonly static float STARTING_TOTAL = 16f;

        private readonly static double INITIAL_FITNESS = 0.0;

        /// <summary>
        /// The prefab to spawn as an enemy
        /// </summary>
        public GameObject ZombiePrefab;

        /// <summary>
        /// Where the Zombunnies spawn from
        /// </summary>
        public List<Transform> SpawnPoints;

        private int remaining_alive;
        private List<Dictionary<Genome, double>> genomeMaps;
        private Dictionary<Genome, double> currentMap;

        // TODO
        private Stack<Genome> toSpawn;
        private ZombieAgent prefabAgent;

        private ZigguratGaussianSampler sampler;

        private void Start()
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
            print(currentMap.Count);
            prefabAgent = ZombiePrefab.GetComponent<ZombieAgent>();
        }

        private void NextWave()
        {
            var nextWave = new Stack<Genome>(POPULATION_SIZE);
            // Reproduce Here! dict -> list
            foreach (var pair in currentMap)
            {
                if (pair.Key == null) continue;
                nextWave.Push(pair.Key.mutate(sampler, GROWTH_RATE));
            }
            //Possibly scramble list here?

            //Reset values
            remaining_alive = POPULATION_SIZE;
            var newMap = new Dictionary<Genome, double>(POPULATION_SIZE);
            genomeMaps.Add(newMap);
            currentMap = genomeMaps[genomeMaps.Count - 1];

            //TODO
            toSpawn = nextWave;

            //Spawn next wave
            InvokeRepeating("Spawn", SPAWN_DELAY, SPAWN_DELAY);
        }

        private void Spawn()
        {
            if(toSpawn.Count == 1)
            {
                CancelInvoke("Spawn");
            }
            int spawn_index = Random.Range(0, SpawnPoints.Count);

            // TODO: Figure out pools!
            var prefabInstance = Instantiate(ZombiePrefab, SpawnPoints[spawn_index]);
            var agentInstance = prefabInstance.GetComponent<ZombieAgent>();
            agentInstance.Genome = toSpawn.Pop();
            agentInstance.removed += (_) =>
            {
                ZombieDied(agentInstance);
            };
        }

        //Callback function on zombunny death
        public void ZombieDied(ZombieAgent agent)
        {
            --remaining_alive;
            currentMap.Add(agent.Genome, agent.Fitness);
            print(remaining_alive);
            if (remaining_alive == 0)
            {
                NextWave();
            }
        }
    }
}
