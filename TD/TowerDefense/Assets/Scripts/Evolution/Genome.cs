using UnityEngine;

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

        public Vector3 CalculateDirection(int time_alive, bool is_dealing_damage, float previous_direction, int health, List<Targetable> nearby)
        {
            return new Vector3();
        }
    }
}