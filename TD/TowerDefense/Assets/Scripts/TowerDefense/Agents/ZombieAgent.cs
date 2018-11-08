using System;
using ActionGameFramework.Health;
using Core.Utilities;
using TowerDefense.Affectors;
using TowerDefense.Level;
using TowerDefense.Nodes;
using UnityEngine;
using UnityEngine.AI;
using Evolution;
using Core.Health;
// Will include neural net

namespace TowerDefense.Agents
{
    [RequireComponent(typeof(AttackAffector)), 
        RequireComponent(typeof(Collider)), 
        RequireComponent(typeof(Collider)),
        RequireComponent(typeof(Rigidbody))]
    public class ZombieAgent : Targetable
    {
        // Fix whatever this is mad about
        public override Vector3 velocity
        {
            // Previous velocity set by Neural Net
            get { return lastVelocity.toVector3(); }
        }
        private PolarVector lastVelocity;

        protected LevelManager m_LevelManager;

        public double Fitness {
            get;
            protected set;
        }

        public readonly IAlignmentProvider alignment;
        public Collider visionCollider;
        public Collider damageCollider;
        public Rigidbody rigidBody;

        // Get alignment of zombie with this.configuration.alignment

        private Genome genome;
        private int time_alive = 0;

		/// <summary>	
		/// Setup all the necessary parameters for this agent from configuration data
		/// </summary>
        public void Start() {
            this.lastVelocity = new PolarVector(0, 1f);
            genome = new Genome(50, 1, 1);

        }

        // Damage hits the Damageable behavior corresponding to the enitiy being attacked
        //
        // Also need to calculate the damage point on the object, look into damager for
        // how to calculate the damagepoint

        public void FixedUpdate()
        {
            time_alive += 1;
            lastVelocity.Rotate(genome.CalculateDirection(
                lastVelocity, 
                time_alive, 
                configuration.currentHealth, 
                new System.Collections.Generic.Dictionary<Targetable, PolarVector>()));
            rigidBody.MoveRotation(Quaternion.LookRotation(lastVelocity.toVector3()));
            rigidBody.MovePosition(
                transform.position + 
                (lastVelocity.toVector3() * Time.deltaTime));
        }
    }
}