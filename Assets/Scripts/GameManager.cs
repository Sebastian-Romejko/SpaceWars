using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject selectedPlanet;

    void Start()
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended && touch.tapCount == 1)
            {
                Vector2 touchPos = touch.position;

                Ray ray = Camera.main.ScreenPointToRay(touchPos);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touchedObject = hit.collider.gameObject;

                    if (touchedObject.layer == 8)
                    {
                        MoveUnits(touchedObject);
                    }
                }
            }
        }
    }

    void MoveUnits(GameObject clickedPlanet)
    {
        if (clickedPlanet.GetComponent<PlanetManager>().owner == Fraction.PLAYER)
        {
            selectedPlanet = clickedPlanet;
        }

        if (selectedPlanet != null)
        {
            selectedPlanet.GetComponent<PlanetManager>().MoveUnits(clickedPlanet);
            
        }
    }
}
