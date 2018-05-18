﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    bool makingPath = false;
    int layerMask;
    bool TilesChecked = false;
	public Vector3 dist;

    void Start()
    {
        Init();
        layerMask = 1 << 8;
        //layerMask = ~layerMask;
    }

    void Update()
    {
        if (!turn)
        {
            return;
        }
        switch (currentMoveType)
        {
            case MoveType.Line:
                if (!moving)
                {
                    if (!TilesChecked)
                    {
                        FindSelectableTiles(1);
                        TilesChecked = true;
                    }
                    MakePath();
                }
                else
                {
                    TilesChecked = false;
                    MoveLine();
                }
                break;
            case MoveType.Selection:
                if (!moving)
                {
                    if (!TilesChecked)
                    {
                        FindSelectableTiles(1);
                        TilesChecked = true;
                    }
					CheckMouse();
                }
                else
                {
                    TilesChecked = false;
                    MoveSelection();
                }
                break;
        }
    }

    void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tiles t = hit.collider.GetComponent<Tiles>();
                    if (t.selectable)
                    {
                        MoveToTile(t);
                    }
                }
            }
        }
    }

    void MakePath()
    {
		if (((Input.GetMouseButtonUp(0) && pathList.Count < 1)) || makingPath)
		{		
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.tag == "Tile")
                {
                    if (!currentTile.path)
                    {
                        currentTile.path = true;
                        pathList.Add(currentTile);
						Debug.Log ("1");
                    }

                    Tiles t = hit.collider.GetComponent<Tiles>();

					dist = ((pathList[pathList.Count - 1].transform.position - t.transform.position));

					if (!t.adjacencyList.Contains (t.parent)) {
						t.adjacencyList.Add (t.parent);
					}

					if (pathList.Count < (moveDistance + 1) && t.selectable && !t.path && !t.current && (t.adjacencyList.Contains (pathList [pathList.Count - 1]))) {
						//t.path = true;
						pathList.Add (t);
						Debug.Log("2");
					} 
					else if (t.path) {
						int pos = pathList.IndexOf (t);
						while (pathList.Count - 1 > pos) {
							pathList [pathList.Count - 1].path = false;
							pathList.RemoveAt (pathList.Count - 1);
							Debug.Log("-1");
						}
					} else if ((dist.x == 0 && Mathf.Abs (dist.z) > 1 || dist.z == 0 && Mathf.Abs (dist.x) > 1) && t.selectable && dist.y <= 1) {
						Vector3 lastPath = pathList [pathList.Count - 1].transform.position;
						dist.y = 0;
						bool stop = false;
						for (int i = 1; i <= Mathf.Abs (dist.x != 0 ? dist.x : dist.z); i++) {
							if (stop == true) {
								PathToTile (t);
								goto end;
							}
							RaycastHit scanner;
							Physics.Raycast (lastPath - new Vector3 (0, halfHeight*2, 0) - (dist.normalized * i), Vector3.up, out scanner, Mathf.Infinity, layerMask);
							if (scanner.collider != null) {
								Tiles scannedTile = scanner.collider.GetComponent<Tiles> ();
								if (scannedTile.selectable) {
									if (scannedTile.path) {
										while (pathList.Count - 1 > pathList.IndexOf (scannedTile)) {
											Debug.Log("-2");
											pathList [pathList.Count - 1].path = false;
											pathList.RemoveAt (pathList.Count - 1);
										}
									} else if (scannedTile.selectable) {
										//scannedTile.path = true;
										pathList.Add (scannedTile);
										Debug.Log("3");
									}
								} else {
									stop = true;
								}
							} else {
								stop = true;
							}
						}
						end:
						while (pathList.Count - 1 > moveDistance) {
							pathList [pathList.Count - 1].path = false;
							pathList.RemoveAt (pathList.Count - 1);
							Debug.Log("-3");
						}

					} else if (!t.path && pathList.Count < moveDistance) {
						PathToTile (t);
					}
                }
                if (pathList.Count >= 1)
                {
                    makingPath = true;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    makingPath = false;
                }
            }
			foreach (Tiles tile in pathList) {
				tile.path = true;
			}
            if (!makingPath && pathList.Count > 1)
            {
                MoveToTile(pathList);
            }
        }
		if ((Input.GetMouseButtonUp (1) && pathList.Count > 0) && makingPath) {
			foreach (Tiles tile in pathList) {
				tile.path = false;
			}
			pathList.Clear ();
			makingPath = false;
		}
    }

	void PathToTile(Tiles t){
		currentTile.current = false;
		currentTile = pathList [pathList.Count - 1];
		currentTile.current = true;
		FindSelectableTiles (0);
		path.Clear();
		Tiles next = t;

		while (next != null){
			path.Push(next);
			next = next.parent;
		}
		path.Pop();
		while (path.Count > 0){
			t = path.Peek();
			if (t.path)
				goto end2;
			if (t.distance <= (moveDistance - (pathList.Count - 1)) && t.selectable) {
				//t.path = true;
				pathList.Add (t);
				Debug.Log("4");
			}
			path.Pop();
		}
		end2:
		FindSelectableTiles (2);
	}
}
