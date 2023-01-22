using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private PlanetManager planetManager;
    private int requiredNumberOfUnitsToAttack;

    void Start()
    {
        planetManager = gameObject.GetComponent<PlanetManager>();
        if(planetManager.enemyStrategy == EnemyStrategy.NONE)
        {
            planetManager.enemyStrategy = (EnemyStrategy) new System.Random().Next(1, 3);
        }
        GenerateRequiredNumberOfUnitsToAttack();
    }

    void Update()
    {
        if (planetManager != null && planetManager.GetUnitsCount() >= requiredNumberOfUnitsToAttack)
        {
            GameObject planetToAttack = planetManager.connectedPlanets[new System.Random().Next(0, planetManager.connectedPlanets.Count - 1)];
            planetManager.MoveUnits(planetToAttack);
            GenerateRequiredNumberOfUnitsToAttack();
        }
    }

    private void GenerateRequiredNumberOfUnitsToAttack()
    {
        switch (planetManager.enemyStrategy)
        {
            case EnemyStrategy.PEACEFUL:
                requiredNumberOfUnitsToAttack = new System.Random().Next(10, 15);
                return;
            case EnemyStrategy.BALANCED:
                requiredNumberOfUnitsToAttack = new System.Random().Next(5, 10);
                return;
            case EnemyStrategy.AGGRESSIVE:
                requiredNumberOfUnitsToAttack = new System.Random().Next(1, 5);
                return;
            default:
                requiredNumberOfUnitsToAttack = 30;
                return;
        }
    }
}
