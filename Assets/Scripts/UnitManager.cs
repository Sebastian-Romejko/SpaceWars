using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject planet { get; set; }
    public UnitState state { get; private set; } = UnitState.ORBITING;
    public Fraction owner { get; private set; }


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

                if (targetPlanet.GetComponent<PlanetManager>().owner == owner)
                {
                    SetState(UnitState.ORBITING);
                }
                else
                {
                    SetState(UnitState.ATTACKING);
                    InvokeRepeating("TakeControl", 0f, 1);

                }
            }
        }
    }

    private void TakeControl()
    {
        targetPlanet.GetComponent<PlanetManager>().TakeControl(owner, attackPoints);
    }

    public void SetOwner(Fraction owner)
    {
        this.owner = owner;
        gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/" + owner.ToString());
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
