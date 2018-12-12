using ActionGameFramework.Health;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Agents;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZombieAttacker : MonoBehaviour {

   public ZombieAgent zombieAgent;

    private HashSet<Targetable> toAttack;

    // Use this for initialization
    void Start()
    {
        zombieAgent = GetComponentInParent<ZombieAgent>();
        toAttack = new HashSet<Targetable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject != null && other.gameObject.GetComponent<Targetable>() != null)
            try
            {
                toAttack.Add(other.gameObject.GetComponent<Targetable>());
            }
            catch { }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.gameObject != null && other.gameObject.GetComponent<Targetable>() != null)
        {
            Targetable leavingTargetable = other.gameObject.GetComponent<Targetable>();
            toAttack.Remove(leavingTargetable);
        }
    }

    private void Update()
    {
        foreach (Targetable item in toAttack)
        {
            if (item != null)
            {
                float health = item.configuration.currentHealth;
                float damage_to_do = zombieAgent.Genome.Damage * Time.deltaTime;
                if (item.TakeDamage(
                    damage_to_do,
                    zombieAgent.transform.position,
                    zombieAgent.configuration.alignmentProvider
                ))
                {
                    zombieAgent.addDamageDone(damage_to_do);
                    zombieAgent.ResetStarvation();
                }
            }
        }
    }
}
