using Assets.Scripts;
using Assets.Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapFactory : MonoBehaviour
{
    private static GameObject planetPrefab = Resources.Load<GameObject>("Prefabs/Planet");
    private static List<GameObject> planets;
    private static Dictionary<DifficultyLevel, DifficultyLevelConfiguration> difficultyLevelConfiguration =
        new Dictionary<DifficultyLevel, DifficultyLevelConfiguration>()
        { 
            { DifficultyLevel.EASY, new DifficultyLevelConfiguration(40, 10) },
            { DifficultyLevel.NORMAL, new DifficultyLevelConfiguration(50, 8) },
            { DifficultyLevel.HARD, new DifficultyLevelConfiguration(60, 5) },
            { DifficultyLevel.HARDCORE, new DifficultyLevelConfiguration(70, 5) }
        };

    private MapFactory()
    {
    }

    public static void renderMap(GameObject plane, int numberOfPlanets, DifficultyLevel difficultyLevel)
    {
        planets = new List<GameObject>();
        renderPlanets(plane, numberOfPlanets);
        renderLinesBetweenPlanets();
        setPlanetsOwners(difficultyLevel);
    }

    private static void renderPlanets(GameObject plane, int numberOfPlanets)
    {
        for (int i = 0; i < numberOfPlanets; i++)
        {
            Vector3 randomPos;
            do
            {
                Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * plane.GetComponent<Renderer>().bounds.size.x / 2;
                randomPos = plane.transform.position + new Vector3(randomCirclePoint.x, plane.transform.position.y, randomCirclePoint.y);
            }
            while(planets.Find(planet => Vector3.Distance(randomPos, planet.transform.position) < 50));

            planets.Add(Instantiate(planetPrefab, randomPos, Quaternion.identity));
        }
    }

    private static void renderLinesBetweenPlanets()
    {
        foreach(GameObject planet in planets)
        {
            List<GameObject> otherPlanets = new List<GameObject>(planets).FindAll(otherPlanet => otherPlanet != planet);
            otherPlanets.Sort((x, y) => Vector3.Distance(x.transform.position, planet.transform.position).CompareTo(
                Vector3.Distance(y.transform.position, planet.transform.position)));
            renderLineBetweenPlanets(planet, new List<GameObject>(new [] { otherPlanets[0], otherPlanets[1], otherPlanets[2] }));
        }
    }

    private static void setPlanetsOwners(DifficultyLevel difficultyLevel)
    {
        setEnemyPlanets(difficultyLevel);
        setPlayerPlanet();
    }

    private static void setEnemyPlanets(DifficultyLevel difficultyLevel)
    {
        DifficultyLevelConfiguration difficulty = difficultyLevelConfiguration[difficultyLevel];
        int numerOfEnemyPlanets = Convert.ToInt32(difficulty.percentOfEnemyPlanets);
        List<int> selectedPlanets = new List<int>(Enumerable.Range(1, 50).OrderBy(x => new System.Random().Next()).Take(5));
        selectedPlanets.ForEach(number => planets[number].GetComponent<PlanetManager>().Init(Fraction.ENEMY, difficulty.secondsToProduceUnit));
    }

    private static void setPlayerPlanet()
    {
        GameObject selectedPlanet = planets.Where(planet => planet.GetComponent<PlanetManager>().owner != Fraction.ENEMY).FirstOrDefault();
        selectedPlanet.GetComponent<PlanetManager>().Init(Fraction.PLAYER, 5);
        GameObject.Find("Main Camera").transform.position = new Vector3(
            selectedPlanet.transform.position.x,
            selectedPlanet.transform.position.y + 100,
            selectedPlanet.transform.position.z - 20);
    }

    private static void renderLineBetweenPlanets(GameObject planet, List<GameObject> planetsToConnect)
    {
        foreach (GameObject planetToConnect in planetsToConnect)
        {
            if (!isOtherPlanetHit(planet.transform.position, planetToConnect.transform.position))
            {
                GameObject newConnection = new GameObject("Connection");
                newConnection.transform.parent = GameObject.Find("Connections").transform;
                LineRenderer lineRenderer = newConnection.AddComponent<LineRenderer>();
                lineRenderer.material = Resources.Load<Material>("Materials/White");
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, planet.transform.position);
                lineRenderer.SetPosition(1, planetToConnect.transform.position);
            }
        }
    }

    private static bool isOtherPlanetHit(Vector3 point1, Vector3 point2)
    {
        RaycastHit hit;
        Physics.Linecast(Vector3.MoveTowards(point1, point2, 10), Vector3.MoveTowards(point2, point1, 10), out hit);
        return hit.collider != null && hit.collider.gameObject != null;
    }
}
