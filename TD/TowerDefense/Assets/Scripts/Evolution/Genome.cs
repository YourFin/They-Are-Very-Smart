using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionGameFramework.Health;

namespace Evolution
{
    public class Genome
    {
        private readonly static int HEALTH_SCALE = 1;
        private readonly static int DAMAGE_SCALE = 1;
        private readonly static float MOVEMENT_SCALE = 0.4f;
        
        private readonly int total;

        private static uint current_id = 0;

        private int health;
        public int Health { 
            get {
                return HEALTH_SCALE * this.health;
            }
        }

        public uint Id
        {
            get; private set;
        }

        private int damage;
        public int Damage { 
            get {
                return DAMAGE_SCALE * this.damage;
            }
        }

        private int movementSpeed;
        public float MovementSpeed { 
            get {
                return MOVEMENT_SCALE * (float)this.movementSpeed; 
            }
        }
        // Insert NN here

        Genome(int health, int damage, int movementSpeed)
        {
            this.health = health;
            this.damage = damage;
            this.movementSpeed = movementSpeed;
            this.total = damage + health + movementSpeed;
            this.Id = current_id;
            current_id++;
        }

        public static Genome Zero() {
            return new Genome(0,0,0);
        }

        public static Genome RandomGenome(int totalStats)
        {
            return Genome.Zero();
        }

        public Genome mutate(float mutationRate)
        {
            return new Genome(this.health, this.damage, this.movementSpeed);
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
        public float CalculateDirection(int time_alive, bool is_dealing_damage, float previous_direction, int health, Dictionary<Targetable, Vector3> nearby)
        {
            return 0f;
        }
    }
}