using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject planet { get; set; }
    public UnitState state { get; private set; } = UnitState.ORBITING;
    public Fraction owner { get; set; }


    private GameObject targetPlanet;
    private int attackPoints = 1;

    private void Update()
    {
        if (targetPlanet != null && state == UnitState.MOVING)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlanet.transform.position, 10f * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPlanet.transform.position) <= 7f)
            {
                targetPlanet.GetComponent<PlanetManager>().AddUnit(gameObject);
                state = UnitState.ATTACKING;
                InvokeRepeating("TakeControl", 0f, 1);
            }
        }
    }

    private void TakeControl()
    {
        targetPlanet.GetComponent<PlanetManager>().TakeControl(owner, attackPoints);
    }

    public void MoveTo(GameObject planetToMove)
    {
        this.targetPlanet = planetToMove;
        state = UnitState.MOVING;
    }

    public void SetState(UnitState newState)
    {
        state = newState;
        CancelInvoke("TakeControl");

    }
}
