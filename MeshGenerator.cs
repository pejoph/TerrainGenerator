/// ================================
/// Peter Phillips, 2022
/// ================================


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private GenerationSettings genSettings;
    [SerializeField] private Transform water;
    [SerializeField] private GameObject tree;

    private int xSize, zSize = 200;
    private float perlinScale = .3f;
    internal float xOffset, zOffset = 150f;
    private float height = 1f;
    private int octaves = 3;
    private float lacunarity = 2f;
    private float persistence = .5f;
    private float waterLevel = .6f;
    private float gradient = 1f;
    private int numOfTrees = 100;
    private int numOfRocks = 100;
    private Gradient colourGradient;

    private Mesh mesh;
    private Mesh collisionMesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colours;
    private Coroutine createShape;
    private Coroutine spawnTrees;
    private MeshCollider meshCollider;
    private Vector3[] collisionVertices;
    private int[] collisionTriangles;

    private TreeSpawner treeSpawner;
    private GameObject[] trees;
    private GameObject[] rocks;

    private void Awake()
    {
        //Initialise()
    }

    public void Initialise()
    {
        mesh = new Mesh();
        collisionMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        //meshCollider = GetComponent<MeshCollider>();
        treeSpawner = genSettings.gameObject.GetComponent<TreeSpawner>();
    }


    //private void OnEnable()
    //{
    //    mesh = new Mesh();
    //    collisionMesh = new Mesh();
    //    GetComponent<MeshFilter>().mesh = mesh;
    //    //meshCollider = GetComponent<MeshCollider>();
    //    treeSpawner = genSettings.gameObject.GetComponent<TreeSpawner>();
    //}

    public void SetInitialValues(bool includeOffset = false)
    {
        xSize = genSettings.xSize;
        zSize = genSettings.zSize;
        perlinScale = genSettings.perlinScale;
        if (includeOffset)
        {
            xOffset = genSettings.xOffset;
            zOffset = genSettings.zOffset;
        }
        height = genSettings.height;
        octaves = genSettings.octaves;
        lacunarity = genSettings.lacunarity;
        persistence = genSettings.persistence;
        waterLevel = genSettings.waterLevel;
        gradient = genSettings.gradient;
        colourGradient = genSettings.colourGradient;
        Vector3 temp = water.localPosition;
        temp.y = -10f + height * waterLevel / 10f;
        water.localPosition = temp;
        numOfTrees = genSettings.numOfTrees;
        numOfRocks = genSettings.numOfRocks;
    }

    // This will instantly generate the starting terrain, as opposed
    // to the coroutine method which will take a few seconds.
    public void InitialGeneration(int xShift = 0, int zShift = 0)
    {
        CreateShape(xShift, zShift);
        treeSpawner.Initialise();
        GenerateObjects();
    }

    public void GenerateObjects()
    {
        treeSpawner.Initialise();
        if (trees != null)
            treeSpawner.DestroyObjects(trees);
        if (rocks != null)
            treeSpawner.DestroyObjects(rocks); 
        trees = treeSpawner.SpawnTrees(numOfTrees, transform.position, xSize, zSize, vertices);
        rocks = treeSpawner.SpawnRocks(numOfRocks, transform.position, xSize, zSize, vertices);
    }

    //public void ConstantlyUpdate(int xShift = 0, int zShift = 0)
    //{
    //    CreateShape(xShift, zShift);
    //}

    public void GenerateTerrain(int xShift = 0, int zShift = 0)
    {
        if (createShape != null) StopCoroutine(createShape);
        createShape = StartCoroutine(CreateShapeEnum(xShift, zShift));
    }

    //private void CreateCollisionMesh(int xShift = 0, int zShift = 0, int simplificationFactor = 1)
    //{
    //    int xSizeCollisions = xSize / simplificationFactor;
    //    int zSizeCollisions = zSize / simplificationFactor;
    //    // Create vec3 array.
    //    collisionVertices = new Vector3[(xSizeCollisions + 1) * (zSizeCollisions + 1)];
    //    // Calculate min and max values of Z.
    //    int minZ = zShift * zSize;
    //    int maxZ = zSize + zShift * zSize;
    //    // Calculate min and max values of X.
    //    int minX = xShift * xSize;
    //    int maxX = xSize + xShift * xSize;
    //    // Loop through vertices.
    //    for (int i = 0, z = minZ; z <= maxZ; z+=simplificationFactor)
    //    {
    //        for (int x = minX; x <= maxX; x+=simplificationFactor, i++)
    //        {
    //            // Calculate co-ordinates at each point, using the rendering mesh.
    //            collisionVertices[i] = vertices[(x + (z * (zSize + 1)))];
    //        }
    //    }
    //
    //    // Create int array.
    //    triangles = new int[xSizeCollisions * zSizeCollisions * 6];
    //    int vert = 0;
    //    int tris = 0;
    //    // Loop through triangles.
    //    for (int z = 0; z < zSizeCollisions; z++, vert++)
    //    {
    //        for (int x = 0; x < xSizeCollisions; x++, vert++, tris += 6)
    //        {
    //            // Assign vertex position of both triangles in each quad.
    //            // Make sure triangles are defined clockwise (so that up-face is drawn).
    //            triangles[tris + 0] = vert + 0;
    //            triangles[tris + 1] = vert + xSize + 1;
    //            triangles[tris + 2] = vert + 1;
    //            triangles[tris + 3] = vert + 1;
    //            triangles[tris + 4] = vert + xSize + 1;
    //            triangles[tris + 5] = vert + xSize + 2;
    //        }
    //    }
    //    UpdateCollisionMesh();
    //}

    private void CreateShape(int xShift = 0, int zShift = 0)
    {
        // Create vec3 array.
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        // Reset min/max height values.
        //minHeight = height;
        //maxHeight = 0;
        // Calculate min and max values of Z.
        int minZ = (-zSize / 2) + zShift * zSize;
        int maxZ = (zSize - zSize / 2) + zShift * zSize;
        // Calculate min and max values of X.
        int minX = (-xSize / 2) + xShift * xSize;
        int maxX = (xSize  - xSize / 2) + xShift * xSize;
        //// Calculate min and max values of Z.
        //int minZ = 0 + zShift * zSize;
        //int maxZ = zSize + zShift * zSize;
        //// Calculate min and max values of X.
        //int minX = 0 + xShift * xSize;
        //int maxX = xSize + xShift * xSize;
        // Loop through vertices.
        for (int i = 0, z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++, i++)
            {
                // Calculate y at each point according to noise.
                float y = CalculateHeight(x, z);
                // Adjust y according to the gradient.
                y = Mathf.Pow((y / (height * 1.3f)), gradient) * height;
                // Clamp y at the water level.
                //if (y < height * waterLevel)
                //    y = height * waterLevel;
                vertices[i] = new Vector3(x, y, z);
                // Update min/max height values.
                //if (y < minHeight)
                //    minHeight = y;
                //if (y > maxHeight)
                //    maxHeight = y;
            }
        }

        // Create int array.
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        // Loop through triangles.
        for (int z = 0; z < zSize; z++, vert++)
        {
            for (int x = 0; x < xSize; x++, vert++, tris+=6)
            {
                // Assign vertex position of both triangles in each quad.
                // Make sure triangles are defined clockwise (so that up-face is drawn).
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
            }
        }

        // Create colour array.
        colours = new Color[vertices.Length];
        // Loop through colours.
        for (int i = 0, z = 0; z < zSize + 1; z++)
        {
            for (int x = 0; x < xSize + 1; x++, i++)
            {
                // Set colour according to point on a colour gradient.
                // Inverse lerp returns a value between 0 and 1 that corresponds to the vertex height
                // relative to the minimum and maximum vertex heights.
                //float point = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
                float point = Mathf.Clamp01(((vertices[i].y / height) - waterLevel) / (1.2f - waterLevel));
                colours[i] = colourGradient.Evaluate(point);
            }
        }
        UpdateMesh();
    }

    // This function is run as a co-routine to reduce computational overhead, instead of calculating all the vertex data in 1 frame,
    // it will take as many frames as there are quads in the Z-axis of each terrain tile, e.g. 200.
    // So long as this happens quicker than the player can see the tile, there is no issue.
    private IEnumerator CreateShapeEnum(int xShift = 0, int zShift = 0)
    {
        // Create vec3 array.
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        // Calculate min and max values of Z.
        int minZ = (-zSize / 2) + zShift * zSize;
        int maxZ = (zSize - zSize / 2) + zShift * zSize;
        // Calculate min and max values of X.
        int minX = (-xSize / 2) + xShift * xSize;
        int maxX = (xSize - xSize / 2) + xShift * xSize;
        float heightMod = height * 1.3f;
        // Loop through vertices.
        for (int i = 0, z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++, i++)
            {
                // Calculate y at each point according to noise.
                float y = CalculateHeight(x, z);
                // Adjust y according to the gradient.
                y = Mathf.Pow((y / heightMod), gradient) * height;
                //// Clamp y at the water level.
                //if (y < height * waterLevel)
                //    y = height * waterLevel;
                vertices[i] = new Vector3(x, y, z);
                //if (x % 100 == 0)
                //    yield return null;
            }
            yield return null;
        }

        // Create int array.
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        // Loop through triangles.
        for (int z = 0; z < zSize; z++, vert++)
        {
            for (int x = 0; x < xSize; x++, vert++, tris += 6)
            {
                // Assign vertex position of both triangles in each quad.
                // Make sure triangles are defined clockwise (so that up-face is drawn).
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
            }
            //yield return null;
        }
        yield return null;

        // Create colour array.
        colours = new Color[vertices.Length];
        // Loop through colours.
        float maxHeight = 1.2f - waterLevel;
        for (int i = 0, z = 0; z < zSize + 1; z++)
        {
            for (int x = 0; x < xSize + 1; x++, i++)
            {
                // Set colour according to point on a colour gradient.
                float point = Mathf.Clamp01(((vertices[i].y / height) - waterLevel) / maxHeight);
                colours[i] = colourGradient.Evaluate(point);
            }
        }
        yield return null;
        UpdateMesh();
        yield return null;
        DestroyTrees();
        yield return null;
        DestroyRocks();
        yield return new WaitForEndOfFrame();
        yield return null;
        trees = treeSpawner.SpawnTrees(numOfTrees, transform.position, xSize, zSize, vertices);
        yield return null;
        rocks = treeSpawner.SpawnRocks(numOfRocks, transform.position, xSize, zSize, vertices);
    }

    public void DestroyTrees()
    {
        treeSpawner.DestroyObjects(trees);
    }
    public void DestroyRocks()
    {
        treeSpawner.DestroyObjects(rocks);
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colours;

        mesh.RecalculateNormals();
        //if (meshCollider != null)
        //    Destroy(gameObject.GetComponent<MeshCollider>());
        //meshCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        //meshCollider.sharedMesh = null;
        //meshCollider.sharedMesh = mesh;
    }

    //private void UpdateCollisionMesh()
    //{
    //    collisionMesh.Clear();
    //
    //    collisionMesh.vertices = collisionVertices;
    //    collisionMesh.triangles = collisionTriangles;
    //    collisionMesh.RecalculateNormals();
    //
    //    if (meshCollider != null)
    //        Destroy(gameObject.GetComponent<MeshCollider>());
    //    meshCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
    //    meshCollider.sharedMesh = null;
    //    meshCollider.sharedMesh = collisionMesh;
    //}

    public float CalculateHeight(float xPos, float zPos)
    {
        float yPos = 0;
        // These horribly long named variables drastically reduce the number of calculations involved,
        // which in turn reduces lag.
        float persistenceModifierWithHeight = height;
        float xPosWithOffsetScaledAndLacunarity = (xPos + xOffset * xSize) * perlinScale;
        float zPosWithOffsetScaledAndLacunarity = (zPos + zOffset * zSize) * perlinScale;
        // Loop through the octaves of our fractal noise.
        for (int i = 0; i < octaves; i++, xPosWithOffsetScaledAndLacunarity *= lacunarity, zPosWithOffsetScaledAndLacunarity *= lacunarity, persistenceModifierWithHeight *= persistence)
        {
            // At each octave, calculate xz co-ords based on the perlin scale and the lacunarity.
            float compoundX = xPosWithOffsetScaledAndLacunarity;
            float compoundZ = zPosWithOffsetScaledAndLacunarity;
            // Calculate y position using these modified xz co-ords, as well as the height and persistence parameters.
            yPos += Mathf.PerlinNoise(compoundX, compoundZ) * persistenceModifierWithHeight;
        }
        return yPos;
    }
}
