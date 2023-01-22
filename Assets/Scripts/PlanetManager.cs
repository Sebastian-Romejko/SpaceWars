using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public Fraction owner { get; private set; } = Fraction.NEUTRAL;
    public EnemyStrategy enemyStrategy { get; set; }
    public List<GameObject> connectedPlanets { get; set; } = new List<GameObject>();

    private List<GameObject> units = new List<GameObject>();
    private int healthPoints = 100;
    private int secondsToProduceUnit = 5;
    private int unitsHP = 10;
    private float radius = 15f;
    private float rotationSpeed = 90f;

    void Start()
    {
    }

    public void Init(Fraction owner, int secondsToProduceUnit, int unitsHP)
    {
        this.secondsToProduceUnit = secondsToProduceUnit;
        this.unitsHP = unitsHP;
        SetOwner(owner);
    }
    public void AddConnectedPlanet(GameObject planetToConnect)
    {
        if(!connectedPlanets.Contains(planetToConnect))
        {
            connectedPlanets.Add(planetToConnect);
        }
    }

    void Update()
    {
        float angleBetweenObjects = 360.0f / units.Count;
        int unitsCounter = 1;

        // TODO: find out why it is needed (some units aren't removed from units list when they should)
        units.RemoveAll(unit => unit == null);

        units.FindAll(unit => unit.GetComponent<UnitManager>().state == UnitState.ORBITING).ForEach(unit =>
        {
            float angle = (Time.time * rotationSpeed + angleBetweenObjects * unitsCounter++) % 360.0f;
            float xPos = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            float zPos = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            Vector3 newPos = new Vector3(xPos, 0, zPos);

            //unit.transform.RotateAround(gameObject.transform.position, new Vector3(0, 20, 0), Time.time * 1);
            unit.transform.position = Vector3.MoveTowards(unit.transform.position, gameObject.transform.position + newPos, 20 * Time.deltaTime);
        });
    }

    void ProduceUnit()
    {
        GameObject newUnit = Instantiate(Resources.Load<GameObject>("Prefabs/Unit"), transform.position, Quaternion.identity);
        newUnit.GetComponent<UnitManager>().planet = gameObject;
        newUnit.GetComponent<UnitManager>().SetHealthPoints(unitsHP);
        newUnit.GetComponent<UnitManager>().SetOwner(owner);
        units.Add(newUnit);
    }

    public int GetUnitsCount()
    {
        return units.Count;
    }

    public void AddUnit(GameObject unitToAdd)
    {
        units.Add(unitToAdd);
    }

    public void RemoveUnit(GameObject unitToRemove)
    {
        units.Remove(unitToRemove);
    }

    public void MoveUnits(GameObject planetToMove)
    {
        SetUnitsState(UnitState.MOVING);
        Debug.Log("-----------------------");
        units.ForEach(unit => Debug.Log("OWNER: " + unit.GetComponent<UnitManager>().owner));
        Debug.Log("-----------------------");
        units.Where(unit => unit.GetComponent<UnitManager>().owner == owner).ToList()
            .ForEach(unit => unit.GetComponent<UnitManager>().MoveTo(planetToMove));
        units = new List<GameObject>();
    }

    public void TakeDamage
        (Fraction fraction, int controlPoints)
    {
        this.healthPoints -= controlPoints;
        if(this.healthPoints == 0)
        {
            SetOwner(fraction);
            this.healthPoints = 100;
            SetUnitsState(UnitState.ORBITING);
        }
    }

    private void SetOwner(Fraction owner)
    {
        this.owner = owner;
        gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/" + owner.ToString());

        CancelInvoke("ProduceUnit");
        if (gameObject.GetComponent<EnemyAI>() != null)
        {
            Destroy(gameObject.GetComponent<EnemyAI>());
        }

        switch (owner)
        {
            case Fraction.PLAYER:
                InvokeRepeating("ProduceUnit", 1f, secondsToProduceUnit);
                return;
            case Fraction.ENEMY:
                gameObject.AddComponent<EnemyAI>();
                InvokeRepeating("ProduceUnit", 1f, secondsToProduceUnit);
                return;
            case Fraction.NEUTRAL:
                return;
        }
        if (owner != Fraction.NEUTRAL)
        {
            InvokeRepeating("ProduceUnit", 1f, secondsToProduceUnit);
        }
    }

    private void SetUnitsState(UnitState unitState)
    {
        units.ForEach(unit => unit.GetComponent<UnitManager>().SetState(unitState));
    }
}
