using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject planet { get; set; }
    public UnitState state { get; private set; } = UnitState.ORBITING;
    public Fraction owner { get; private set; }


    private UnitState previousState;
    private GameObject targetPlanet;
    private GameObject targetEnemy;
    private int healthPoints;
    private int attackPoints = 1;

    private void Update()
    {
        targetEnemy = EnemyInRange();
        if(targetEnemy != null && state != UnitState.FIGHTING)
        {
            Debug.Log("Our owner: " + owner);
            Debug.Log("Enemy owner: " + targetEnemy.GetComponent<UnitManager>().owner);
            Debug.Log("Distance between: " + Vector3.Distance(targetEnemy.transform.position, gameObject.transform.position));
            previousState = state;
            state = UnitState.FIGHTING;
            InvokeRepeating("DealDamageToUnit", 0f, 1);
        }
        else if (targetPlanet != null && state == UnitState.MOVING)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlanet.transform.position, 10f * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPlanet.transform.position) <= 7f)
            {
                targetPlanet.GetComponent<PlanetManager>().AddUnit(gameObject);

                if (targetPlanet.GetComponent<PlanetManager>().owner == owner)
                {
                    SetState(UnitState.ORBITING);
                }
                else
                {
                    SetState(UnitState.ATTACKING);
                    InvokeRepeating("DealDamageToPlanet", 0f, 1);
                }
            }
        }
    }

    private GameObject EnemyInRange()
    {
        return targetEnemy != null ? targetEnemy : 
            new List<GameObject>(GameObject.FindGameObjectsWithTag("Unit")).Where(unit => unit.GetComponent<UnitManager>().owner != owner 
                && Vector3.Distance(unit.transform.position, transform.position) < 15).FirstOrDefault();
    }

    private void DealDamageToPlanet()
    {
        targetPlanet.GetComponent<PlanetManager>().TakeDamage(owner, attackPoints);
    }

    private void DealDamageToUnit()
    {
        if (targetEnemy == null || targetEnemy.GetComponent<UnitManager>().TakeDamage(attackPoints))
        {
            Debug.Log("Enemy destroyed!");
            CancelInvoke("DealDamageToUnit");
            state = previousState;
        }
    }

    public void SetOwner(Fraction owner)
    {
        this.owner = owner;
        gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/" + owner.ToString());
    }

    public void SetHealthPoints(int hp)
    {
        if (healthPoints == 0)
        {
            healthPoints = hp;
        }
    }

    public bool TakeDamage(int damage)
    {
        healthPoints -= damage;
        Debug.Log("Damage taken: " + damage + ", remaining hp: " + healthPoints);
        if (healthPoints <= 0)
        {
            DestroyUnit();
            return true;
        }
        return false;
    }

    private void DestroyUnit()
    {
        if (planet != null) 
        {
            planet.GetComponent<PlanetManager>().RemoveUnit(gameObject);
        }
        Destroy(gameObject);
    }

    public void MoveTo(GameObject planetToMove)
    {
        this.targetPlanet = planetToMove;
        state = UnitState.MOVING;
    }

    public void SetState(UnitState newState)
    {
        state = newState;
        CancelInvoke("DealDamageToPlanet");
    }
}
