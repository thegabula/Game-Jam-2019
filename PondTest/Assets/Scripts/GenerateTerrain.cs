﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public GameObject theTile;
    public float creationTime;

    public Tile(GameObject t, float ct)
    {
        theTile = t;
        creationTime = ct;
    }
}
public class GenerateTerrain : MonoBehaviour
{
    public GameObject plane;
    public GameObject player;

    int planeSize = 10;
    public int halfTileX = 5 ;
    public int halfTileZ = 5;

    Vector3 startPos;

    Hashtable tiles = new Hashtable();
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.position = Vector3.zero;
        startPos = Vector3.zero;

        float updateTime = Time.realtimeSinceStartup;

        for (int x = -halfTileX; x < halfTileX; x++)
        {
            for (int z = -halfTileZ; z < halfTileZ; z++)
            {
                Vector3 pos = new Vector3((x * planeSize + startPos.x),
                                            0,
                                           (z * planeSize + startPos.z));
                GameObject t = (GameObject)Instantiate(plane, pos, Quaternion.identity);

                string tileName = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();
                t.name = tileName;
                Tile tile = new Tile(t, updateTime);
                tiles.Add(tileName, tile);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        int xMove = (int)(player.transform.position.x - startPos.x);
        int zMove = (int)(player.transform.position.z - startPos.z);

        if (Mathf.Abs(xMove) >= planeSize || Mathf.Abs(zMove) >= planeSize)
        {
            float updateTime = Time.realtimeSinceStartup;

            int playerX = (int)(Mathf.Floor(player.transform.position.x / planeSize) * planeSize);
            int playerZ = (int)(Mathf.Floor(player.transform.position.z / planeSize) * planeSize);

            for (int x = -halfTileX; x < halfTileX; x++)
            {
                for (int z = -halfTileZ; z < halfTileZ; z++)
                {
                    Vector3 pos = new Vector3((x * planeSize + playerX), 0, (z * planeSize + playerZ));

                    string tileName = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();

                    if (!tiles.ContainsKey(tileName))
                    {
                        GameObject t = (GameObject)Instantiate(plane, pos, Quaternion.identity);

                        t.name = tileName;
                        Tile tile = new Tile(t, updateTime);
                        tiles.Add(tileName, tile);
                    }
                    else
                    {
                        (tiles[tileName] as Tile).creationTime = updateTime;
                    }
                }
            }

            Hashtable newTerrain = new Hashtable();
            foreach (Tile tls in tiles.Values)
            {
                if (tls.creationTime != updateTime)
                {
                    Destroy(tls.theTile);
                }
                else
                {
                    newTerrain.Add(tls.theTile.name, tls);
                }
            }

            tiles = newTerrain;

            startPos = player.transform.position;
        }
    }
}
