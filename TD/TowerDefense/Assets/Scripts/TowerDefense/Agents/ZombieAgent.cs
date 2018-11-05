using System;
using ActionGameFramework.Health;
using Core.Utilities;
using TowerDefense.Affectors;
using TowerDefense.Level;
using TowerDefense.Nodes;
using UnityEngine;
using UnityEngine.AI;
using Evolution;
// Will include neural net

namespace TowerDefense.Agents
{
    public class ZombieAgent : Targetable
    {
        // Fix whatever this is mad about
        public override Vector3 velocity
        {
            // Previous velocity set by Neural Net
            get { return lastVelocity; }
        }
        private Vector3 lastVelocity;

        protected LevelManager m_LevelManager;

        public double Fitness {
            get;
            protected set;
        }

        private Genome genome;
        private int time_alive = 0;

		/// <summary>	
		/// Setup all the necessary parameters for this agent from configuration data
		/// </summary>
        public void Initialize() {
            lastVelocity = new Vector3(0,0,0);
        }

        public void FixedUpdate()
        {
            this.time_alive += 1;
        }
        
    }
}