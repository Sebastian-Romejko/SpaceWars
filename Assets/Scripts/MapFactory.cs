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
            { DifficultyLevel.EASY, 
                new DifficultyLevelConfiguration(20, 10, 8, new Dictionary<EnemyStrategy, decimal>() 
                { 
                    { EnemyStrategy.PEACEFUL, 60 }, 
                    { EnemyStrategy.BALANCED, 30 }, 
                    { EnemyStrategy.AGGRESSIVE, 10 }
                }) 
            },
            { DifficultyLevel.NORMAL, 
                new DifficultyLevelConfiguration(30, 8, 10, new Dictionary<EnemyStrategy, decimal>()
                {
                    { EnemyStrategy.PEACEFUL, 40 },
                    { EnemyStrategy.BALANCED, 40 },
                    { EnemyStrategy.AGGRESSIVE, 20 }
                }) 
            },
            { DifficultyLevel.HARD, 
                new DifficultyLevelConfiguration(40, 5, 10, new Dictionary<EnemyStrategy, decimal>() 
                { 
                    { EnemyStrategy.PEACEFUL, 20 }, 
                    { EnemyStrategy.BALANCED, 50 }, 
                    { EnemyStrategy.AGGRESSIVE, 30 }
                }) 
            },
            { DifficultyLevel.HARDCORE, 
                new DifficultyLevelConfiguration(50, 5, 12, new Dictionary<EnemyStrategy, decimal>()
                {
                    { EnemyStrategy.PEACEFUL, 0 },
                    { EnemyStrategy.BALANCED, 50 },
                    { EnemyStrategy.AGGRESSIVE, 50 }
                }) 
            }
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
        setEnemyPlanetsStrategy(difficultyLevel);
        //setCameraOnPlayersPlanet();
    }

    private static void renderPlanets(GameObject plane, int numberOfPlanets)
    {
        for (int i = 0; i < numberOfPlanets; i++)
        {
            Vector3 randomPos;
            do
            {
                Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * plane.GetComponent<Renderer>().bounds.size.z / 2;
                randomPos = plane.transform.position + new Vector3(randomCirclePoint.x, plane.transform.position.y, randomCirclePoint.y);
            }
            while(planets.Find(planet => Vector3.Distance(randomPos, planet.transform.position) < 40));

            planets.Add(Instantiate(planetPrefab, randomPos, Quaternion.identity));
        }
    }

    //TODO: remove if not needed
    private static int randomXModifier(GameObject plane)
    {
        System.Random random = new System.Random();
        return random.Next(-1, 1) * random.Next(0, Convert.ToInt32(plane.GetComponent<Renderer>().bounds.size.x / 4));
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
        int numerOfEnemyPlanets = Convert.ToInt32(planets.Count * difficulty.percentOfEnemyPlanets / 100);
        List<int> selectedPlanetsNumbers = new List<int>(Enumerable.Range(1, 50).OrderBy(x => new System.Random().Next()).Take(numerOfEnemyPlanets));
        selectedPlanetsNumbers.ForEach(number => 
        {
            planets[number].GetComponent<PlanetManager>().Init(Fraction.ENEMY, difficulty.secondsToProduceUnit, difficulty.unitsHP);
            planets[number].AddComponent<EnemyAI>();
        });
    }

    private static void setPlayerPlanet()
    {
        GameObject selectedPlanet = planets.Where(planet => planet.GetComponent<PlanetManager>().owner != Fraction.ENEMY).FirstOrDefault();
        selectedPlanet.GetComponent<PlanetManager>().Init(Fraction.PLAYER, 5, 10);
    }

    private static void setEnemyPlanetsStrategy(DifficultyLevel difficultyLevel)
    {
        List<GameObject> enemyPlanets = planets.Where(p => p.GetComponent<PlanetManager>().owner == Fraction.ENEMY).ToList();
        int peacefulPlanets = Convert.ToInt32(enemyPlanets.Count * difficultyLevelConfiguration[difficultyLevel].enemyStrategyToPresence[EnemyStrategy.PEACEFUL] / 100);
        int balancedPlanets = Convert.ToInt32(enemyPlanets.Count * difficultyLevelConfiguration[difficultyLevel].enemyStrategyToPresence[EnemyStrategy.BALANCED] / 100);
        int aggresivePlanets = Convert.ToInt32(enemyPlanets.Count * difficultyLevelConfiguration[difficultyLevel].enemyStrategyToPresence[EnemyStrategy.AGGRESSIVE] / 100);
        foreach (GameObject planet in enemyPlanets) 
        {
            if (planet.GetComponent<EnemyAI>() != null)
            {
                Destroy(planet.GetComponent<EnemyAI>());
            }

            if (peacefulPlanets-- > 0)
            {
                planet.GetComponent<PlanetManager>().enemyStrategy = EnemyStrategy.PEACEFUL;
            }
            else if (balancedPlanets-- > 0)
            {
                planet.GetComponent<PlanetManager>().enemyStrategy = EnemyStrategy.BALANCED;
            }
            else if(aggresivePlanets-- > 0)
            {
                planet.GetComponent<PlanetManager>().enemyStrategy = EnemyStrategy.AGGRESSIVE;
            }
        }
    }

    private static void setCameraOnPlayersPlanet()
    {
        GameObject playersPlanet = planets.Where(planet => planet.GetComponent<PlanetManager>().owner == Fraction.PLAYER).FirstOrDefault();
        GameObject.Find("Main Camera").transform.position = new Vector3(
            playersPlanet.transform.position.x,
            playersPlanet.transform.position.y + 150,
            playersPlanet.transform.position.z + 10);
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

                planet.GetComponent<PlanetManager>().AddConnectedPlanet(planetToConnect);
                planetToConnect.GetComponent<PlanetManager>().AddConnectedPlanet(planet);
            }
        }
    }

    private static bool isOtherPlanetHit(Vector3 point1, Vector3 point2)
    {
        RaycastHit hit;
        Physics.Linecast(Vector3.MoveTowards(point1, point2, 20), Vector3.MoveTowards(point2, point1, 20), out hit);
        return hit.collider != null && hit.collider.gameObject != null;
    }
}
