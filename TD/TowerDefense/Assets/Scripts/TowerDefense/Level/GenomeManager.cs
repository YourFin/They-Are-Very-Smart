using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evolution;
using TowerDefense.Agents;

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

        public readonly float SPAWN_DELAY = 2f;

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
            for (int index = 0; index < POPULATION_SIZE; ++index)
            {
                currentMap.Add(new Genome(20, 4, 2), 0.0);
            }
            print(currentMap.Count);
        }

        private void NextWave()
        {
            var nextWave = new Stack<Genome>(POPULATION_SIZE);
            // Reproduce Here! dict -> list
            foreach (var pair in currentMap)
            {
                nextWave.Push(pair.Key);
            }
            //Possibly scramble list here?
            print(nextWave.Count);

            //Reset values
            remaining_alive = POPULATION_SIZE;
            var newMap = new Dictionary<Genome, double>(POPULATION_SIZE);
            genomeMaps.Add(newMap);
            currentMap = genomeMaps[genomeMaps.Count - 1];

            //TODO
            toSpawn = nextWave;

            prefabAgent = ZombiePrefab.GetComponent<ZombieAgent>();
            //Spawn next wave
            InvokeRepeating("Spawn", SPAWN_DELAY, SPAWN_DELAY);
            //foreach (var genome in nextWave)
            //{
            //    Spawn(genome, prefabAgent);
            //}
        }

        //private void Spawn(Genome genome, ZombieAgent prefabAgent)
        private void Spawn()
        {
            if(toSpawn.Count == 0)
            {
                CancelInvoke("Spawn");
            }
            int spawn_index = Random.Range(0, SpawnPoints.Count - 1);

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
