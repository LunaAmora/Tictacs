﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public EquipmentDisplay equipmentDisplay;
    public GameObject cameraBase;
    public Camera cam;

    public bool follow = false;
    public bool camFollow = false;
    public Vector3 newCameraPos;

    private void Start()
    {
        equipmentDisplay = Resources.FindObjectsOfTypeAll<EquipmentDisplay>()[0].GetComponent<EquipmentDisplay>();
        TurnManager.gm = this;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cameraBase = GameObject.FindGameObjectWithTag("CameraBase");
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == TurnManager.CurrentTeam)
                {
                    if (!hit.collider.GetComponent<TacticsMove>().turn && !hit.collider.GetComponent<TacticsMove>().passedTurn)
                    {
                        TurnManager.CurrentSelected.EndTurn();
                        hit.collider.GetComponent<TacticsMove>().BeginTurn();
                        TacticsMove currentTm = hit.collider.GetComponent<TacticsMove>();
                        TurnManager.CurrentSelected = currentTm;
                        currentTm.BeginTurn();
                        currentTm.MoveAction();

                        if (equipmentDisplay.isActiveAndEnabled)
                        {
                            equipmentDisplay.UpdateEquipment(true);
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CraftManager.UpdateRecipes();
            GetComponent<Recipe>().Craft();
        }

        if (camFollow && TurnManager.CurrentTeam == "Player")
        {
            newCameraPos = new Vector3(newCameraPos.x, cameraBase.transform.position.y, newCameraPos.z);
            cameraBase.transform.position = Vector3.MoveTowards(cameraBase.transform.position, newCameraPos, cameraBase.GetComponent<CameraControl>().panSpeed);
            if (cameraBase.transform.position == newCameraPos)
            {
                camFollow = false;
            }
        }

        if (!TurnManager.combatInitialized)
        {
            if (TurnManager.UnitList.Count > 0)
            {
                TurnManager.InitCombat();
                follow = true;
            }
            else
            {
                follow = false;
            }
        }

        if (follow)
        {
            transform.position = TurnManager.CurrentSelected.transform.position + new Vector3(0, 2, 0);
        }
    }

    public void Attack()
    {
        Character currentChar = TurnManager.CurrentSelected.GetComponent<Character>();
        currentChar.CauseDamage();
    }

    public void EndTurn()
    {
        TurnManager.CurrentSelected.EndTurn();
        TurnManager.NextTeam();
    }
}
