/// ================================
/// Peter Phillips, 2022
/// ================================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePosition : MonoBehaviour
{
    public float xOffset, zOffset = 0;

    [SerializeField] private GenerationSettings genSettings;
    [SerializeField] private Transform player;
    [SerializeField] private MeshGenerator tile;

    private int tileWidth;
    private int tileHeight;
    private int xTiles = 12;
    private int zTiles = 10;
    private Vector3 startPos;
    //private bool isGameRunning = false;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnEnable()
    {
        tile.Initialise();
        Initialise();
    }

    void Update()
    {
        //if (!isGameRunning)
        //{
        //    tile.SetInitialValues();
        //    tile.ConstantlyUpdate();
        //}

        // Check if this object's x-position is a certain distance from the player object.
        if (Mathf.Abs(player.position.x - transform.position.x) > tileWidth * (float)xTiles / 2)
        {
            // If this object is to the right of the player object, wrap around to the left and modify
            // the child tile's x-offset value.
            if (player.position.x < transform.position.x)
            {
                transform.position += Vector3.left * xTiles * tileWidth;
                tile.xOffset -= xTiles;
            }
            // Likewise, if this object is to the left of the player object, do the opposite.
            else
            {
                transform.position += Vector3.right * xTiles * tileWidth;
                tile.xOffset += xTiles;
            }
            // Instruct child tile to generate terrain based on the new offset.
            tile.GenerateTerrain();
        }
        // Repeat the above but for the z-axis.
        if (Mathf.Abs(player.position.z - transform.position.z) > tileHeight * (float)zTiles / 2)
        {
            if (player.position.z < transform.position.z)
            {
                transform.position += Vector3.back * zTiles * tileWidth;
                tile.zOffset -= zTiles;
            }
            else
            {
                transform.position += Vector3.forward * zTiles * tileWidth;
                tile.zOffset += zTiles;
            }
            tile.GenerateTerrain();
        }
    }

    public void ResetPosition()
    {
        transform.position = startPos;
    }

    public void Initialise()
    {
        // Get some values from the generation settings.
        tileWidth = genSettings.xSize;
        tileHeight = genSettings.zSize;
        // instruct the child tile to fetch the generation settings.
        tile.SetInitialValues(true);
        // Modify the offset based on this objects relative location.
        tile.xOffset += xOffset;
        tile.zOffset += zOffset;
        // Generate the terrain immediately.
        tile.InitialGeneration();
    }
}
