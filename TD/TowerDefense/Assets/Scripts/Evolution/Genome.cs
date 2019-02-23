using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionGameFramework.Health;
using Redzen.Numerics.Distributions.Float;
using Encog.Engine.Network.Activation;
using TowerDefense.Level;

namespace Evolution
{
    public class Genome
    {
        private readonly static float HEALTH_SCALE = 0.3f;
        private readonly static float DAMAGE_SCALE = 1;
        private readonly static float MOVEMENT_SCALE = 0.4f;
        private readonly static float HEALTH_OFFSET = 0.5f;
        private readonly static float DAMAGE_OFFSET = 0.5f;
        private readonly static float MOVEMENT_OFFSET = 1;

        private static ActivationSoftMax softMax;

        private static int current_id = 0;
        private static int id_counter = 0;

        // ID # Makes sure that all genomes hash differently
        private int id;
        public int Id
        {
            get
            {
                return id;
            }
        }

        private float health_points;
        private float health_scalar;
        public float Health {
            get {
                return HEALTH_SCALE * health_scalar * totalStats + HEALTH_OFFSET;
            }
        }

        private float damage_points;
        private float damage_scalar;
        public float Damage {
            get {
                return DAMAGE_SCALE * damage_scalar * totalStats + DAMAGE_OFFSET;
            }
        }

        private float speed_points;
        private float speed_scalar;
        public float MovementSpeed {
            get {
                return MOVEMENT_SCALE * speed_scalar * totalStats + MOVEMENT_OFFSET;
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
                double[] stat_array = new double[] { speed_points, damage_points, health_points };
                softMax.ActivationFunction(stat_array, 0, stat_array.Length);
                speed_scalar = (float)stat_array[0];
                damage_scalar = (float)stat_array[1];
                health_scalar = (float)stat_array[2];
                //MonoBehaviour.print($"Health: ({health_scalar}, {Health}), Damage: ({damage_scalar}, {Damage}), Speed: ({speed_scalar}, {MovementSpeed})");
            }
        }


        // Insert NN here


        public Genome(float health, float damage, float movementSpeed, float totalStats)
        {
            if (softMax == null) softMax = new ActivationSoftMax();
            this.health_points = health;
            this.damage_points = damage;
            this.speed_points = movementSpeed;
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
            return new Genome(
                health_points + sampler.Sample(), 
                damage_points + sampler.Sample(), 
                speed_points + sampler.Sample(), 
                this.totalStats + difficultyModifier
                );
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
        public float CalculateDirection(
            PolarVector previous_direction,
            int time_alive,
            float health,
            Dictionary<Targetable, PolarVector> nearby
            )
        {
            KeyValuePair<Targetable, PolarVector> target;
            bool seen = false;
            var count = 0;
            foreach (var pair in nearby)
            {
                if (pair.Key.GetType() == typeof(PlayerHomeBase)) ++count;
            }

            foreach (var pair in nearby)
            {
                var targetableType = pair.Key.GetType();
                if (targetableType == typeof(PlayerHomeBase))
                {
                    target = pair;
                    seen = true;
                    break;
                }
            }
            if (seen)
            {
                return target.Value.direction;// - previous_direction.direction;
            }
            return previous_direction.direction;
        }
    }
}