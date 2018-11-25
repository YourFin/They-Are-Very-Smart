using ActionGameFramework.Health;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Agents;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZombieAttacker : MonoBehaviour {

   public ZombieAgent ZombieAgent;

    private HashSet<Targetable> toAttack;

    // Use this for initialization
    void Start()
    {
        ZombieAgent = GetComponentInParent<ZombieAgent>();
        toAttack = new HashSet<Targetable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        toAttack.Add(other.gameObject.GetComponent<Targetable>());
    }

    private void OnTriggerExit(Collider other)
    {
        Targetable leavingTargetable = other.gameObject.GetComponent<Targetable>();
        toAttack.Remove(leavingTargetable);
    }

    private void Update()
    {
        foreach (var item in toAttack)
        {
            if (item != null)
            {
                var health = item.configuration.currentHealth;
                item.TakeDamage(
                    ZombieAgent.Genome.Damage * Time.deltaTime,
                    ZombieAgent.transform.position,
                    ZombieAgent.configuration.alignmentProvider
                );

                this.ZombieAgent.addDamageDone(health - item.configuration.currentHealth);
            }
        }
    }
}
