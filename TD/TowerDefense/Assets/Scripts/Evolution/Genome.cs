using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionGameFramework.Health;
using Redzen.Numerics.Distributions.Float;
using Encog.Engine.Network.Activation;

namespace Evolution
{
    public class Genome
    {
        private readonly static int HEALTH_SCALE = 1;
        private readonly static int DAMAGE_SCALE = 1;
        private readonly static float MOVEMENT_SCALE = 0.4f;
        
        private static int current_id = 0;

        private static int id_counter = 0;

        private int id;
        public int Id
        {
            get
            {
                return id;
            }
        }

        // TODO: Use SoftMax for these distributions

        private float health;
        public float Health { 
            get {
                return HEALTH_SCALE * (1 + health) * divisor;
            }
        }

        private float damage;
        public float Damage {
            get {
                return DAMAGE_SCALE * (1 + damage) * divisor;
            }
        }

        private float movementSpeed;
        public float MovementSpeed { 
            get {
                return MOVEMENT_SCALE * (1 + movementSpeed) * divisor;
            }
        }

        private float divisor;

        private float totalStats;
        public float TotalStats {
            get {
                return totalStats;
            }
            set {
                totalStats = value;
                var NUM_STATS = 3;
                var sum = movementSpeed + damage + health + NUM_STATS - Mathf.Min(movementSpeed, damage, health);
                divisor = totalStats / (sum);
                MonoBehaviour.print("Movement Speed: " + MovementSpeed);
            }
        }


        // Insert NN here


        public Genome(float health, float damage, float movementSpeed, float totalStats)
        {
            this.health = health;
            this.damage = damage;
            this.movementSpeed = movementSpeed;
            this.TotalStats = totalStats;
            id = current_id;
            current_id++;
        }

        public static Genome Zero() {
            return new Genome(0,0,0, 1);
        }

        public static Genome RandomGenome(ZigguratGaussianSampler sampler, float totalStats)
        {
            return new Genome(sampler.Sample(), sampler.Sample(), sampler.Sample(), totalStats);
        }

        public Genome mutate(ZigguratGaussianSampler sampler, float difficultyModifier)
        {
            return new Genome(health + sampler.Sample(), damage + sampler.Sample(), movementSpeed + sampler.Sample(), this.totalStats + difficultyModifier);
        }

        /// <summary>
        /// Calcualtes the direction that the genome would have the zombie move
        /// </summary>
        /// <param name="time_alive"></param>
        /// <param name="is_dealing_damage"></param>
        /// <param name="previous_direction">The last direction the zombie was moving</param>
        /// <param name="health">current health of the zombie</param>
        /// <param name="nearby">dictionary from nearby entities mapped to the vector to them</param>
        /// <returns>direction, The direction to move in</returns>
        public float CalculateDirection(PolarVector previous_direction, int time_alive, float health, Dictionary<Targetable, PolarVector> nearby)
        {
            return 0f;
        }
    }
}