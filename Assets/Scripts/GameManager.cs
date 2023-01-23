using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject fromPlanet;

    void Start()
    {
        MapFactory.renderMap(gameObject, 10, DifficultyLevel.NORMAL);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && touch.tapCount == 1)
            {
                Vector2 dragStaringPosition = touch.position;

                Ray from = Camera.main.ScreenPointToRay(dragStaringPosition);

                RaycastHit hit;
                if (Physics.Raycast(from, out hit))
                {
                    GameObject hitPlanet = hit.collider.gameObject;

                    if (hitPlanet.tag == "Planet" && hitPlanet.GetComponent<PlanetManager>().owner == Fraction.PLAYER)
                    {
                        fromPlanet = hitPlanet;
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended && touch.tapCount == 1)
            {
                Vector2 dragEndingPosition = touch.position;

                Ray to = Camera.main.ScreenPointToRay(dragEndingPosition);

                RaycastHit hit;
                if (Physics.Raycast(to, out hit))
                {
                    GameObject hitPlanet = hit.collider.gameObject;

                    if (hitPlanet.tag == "Planet")
                    {
                        MoveUnits(hitPlanet);
                    }
                }
            }
        }
    }

    void MoveUnits(GameObject toPlanet)
    {
        if (fromPlanet != null && fromPlanet != toPlanet)
        {
            fromPlanet.GetComponent<PlanetManager>().MoveUnits(toPlanet);
            fromPlanet = null;
        }
    }
}
