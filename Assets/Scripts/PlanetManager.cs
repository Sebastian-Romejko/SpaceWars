using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public GameObject unitPrefab;
    public Fraction initPlanetOwner;

    public Fraction planetOwner { get; private set; }

    private int controlPoints = 100;
    private List<GameObject> units = new List<GameObject>();
    private int secondsToProduceUnit = 5;
    private float radius = 7f;
    private float rotationSpeed = 90f;

    void Start()
    {
        SetOwner(initPlanetOwner);
        if (gameObject != null && unitPrefab != null)
        {
            InvokeRepeating("ProduceUnit", 0f, secondsToProduceUnit);
        }
    }

    void Update()
    {
        float angleBetweenObjects = 360.0f / units.Count;
        int unitsCounter = 1;

        units.FindAll(unit => unit.GetComponent<UnitManager>().state == UnitState.ORBITING).ForEach(unit =>
        {
            float angle = (Time.time * rotationSpeed + angleBetweenObjects * unitsCounter++) % 360.0f;
            float xPos = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            float zPos = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            Vector3 newPos = new Vector3(xPos, 0, zPos);

            unit.transform.position = gameObject.transform.position + newPos;
        });
    }

    void ProduceUnit()
    {
        GameObject newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity) as GameObject;
        newUnit.AddComponent<UnitManager>().planet = gameObject;
        units.Add(newUnit);
        PlaceUnits();
    }

    void PlaceUnits()
    {
        float angleBetweenObjects = 360.0f / units.Count;
        int unitsCounter = 1;

        units.ForEach(unit =>
        {
            float angle = angleBetweenObjects * unitsCounter++;
            float xPos = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            float zPos = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            Vector3 pos = new Vector3(xPos, 0, zPos);

            unit.transform.position = gameObject.transform.position + pos;
        });
    }

    public void AddUnit(GameObject unitToAdd)
    {
        units.Add(unitToAdd);
    }

    public void MoveUnits(GameObject planetToMove)
    {
        SetUnitsState(UnitState.MOVING);
        units.ForEach(unit => unit.GetComponent<UnitManager>().MoveTo(planetToMove));
        units = new List<GameObject>();
    }

    public void TakeControl(Fraction fraction, int controlPoints)
    {
        this.controlPoints -= controlPoints;
        if(this.controlPoints == 0)
        {
            SetOwner(fraction);
            this.controlPoints = 100;
            SetUnitsState(UnitState.ORBITING);
        }
    }

    private void SetOwner(Fraction owner)
    {
        planetOwner = owner;
        gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/" + owner.ToString());
    }

    private void SetUnitsState(UnitState unitState)
    {
        units.ForEach(unit => unit.GetComponent<UnitManager>().SetState(unitState));
    }
}
