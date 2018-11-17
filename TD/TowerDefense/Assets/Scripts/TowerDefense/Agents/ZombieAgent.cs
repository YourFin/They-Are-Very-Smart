using ActionGameFramework.Health;
using TowerDefense.Affectors;
using TowerDefense.Level;
using UnityEngine;
using Evolution;
using Core.Health;

namespace TowerDefense.Agents
{
    [RequireComponent(typeof(AttackAffector)), 
        RequireComponent(typeof(Collider)), 
        RequireComponent(typeof(Collider)),
        RequireComponent(typeof(Rigidbody)),
        RequireComponent(typeof(Animator))]
    public class ZombieAgent : Targetable
    {
        private readonly float SINK_SPEED = 2.5f;
        private readonly float SINK_TIME = 2.0f;

        private float speed;

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
        public Collider attackCollider;
        public Rigidbody rigidBody;
        public Animator anim;

        private bool myIsDead = false;
        // For debug purposes only
        public float CurrentHealth = 0.1f;

        // Get alignment of zombie with this.configuration.alignment
        private Genome genome;
        public Genome Genome {
            get
            {
                return genome;
            }
            set
            {
                //this.lastVelocity = PolarVector.fromVector3(transform.rotation.eulerAngles);
                lastVelocity = new PolarVector(270);
                genome = value;
                var health = genome.Health;
                configuration.SetMaxHealth(health);
                configuration.SetHealth(health);
                lastVelocity.magnitude = genome.MovementSpeed;
            }
        }
        private int time_alive = 0;
        private float time_dead = 0;

        protected override void Awake()
        {
            base.Awake();
            configuration.died += OnDeath;
        }

        /// <summary>
        /// Setup all the necessary parameters for this agent from configuration data
        /// </summary>
        public void Start() {
            genome = new Genome(50, 1, 1);
            if (lastVelocity == null)
            {
                this.lastVelocity = new PolarVector(0, 1f);
            }
        }

        // Damage hits the Damageable behavior corresponding to the enitiy being attacked
        //
        // Also need to calculate the damage point on the object, look into damager for
        // how to calculate the damagepoint

        public void FixedUpdate()
        {

            Vector3 current_pos = transform.position;
            if (myIsDead)
            {
                time_dead += Time.deltaTime;
                if (time_dead > SINK_TIME)
                {
                    //Poolable.TryPool(gameObject);
                    Destroy(gameObject);
                } else {
                    transform.Translate(
                        -Vector3.up * SINK_SPEED * Time.deltaTime
                        );
                }
                return;
            }
            // Handle attacking


            // Handle Movement
            time_alive += 1;
            lastVelocity.Rotate(
                genome.CalculateDirection(
                lastVelocity, 
                time_alive, 
                configuration.currentHealth, 
                new System.Collections.Generic.Dictionary<Targetable, PolarVector>()));

            rigidBody.MoveRotation(Quaternion.LookRotation(lastVelocity.toVector3()));
            rigidBody.MovePosition(
                transform.position + 
                (lastVelocity.toVector3() * Time.deltaTime));

            //For debug purposes only
            CurrentHealth = configuration.currentHealth;
        }

        protected virtual void OnDeath(HealthChangeInfo _)
        {
            myIsDead = true;
            rigidBody.isKinematic = true;
            damageCollider.isTrigger = false;
            anim.SetTrigger("Dead");
        }
    }
}