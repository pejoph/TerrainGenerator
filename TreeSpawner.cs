using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject tree;
    [SerializeField] private Transform treeParent;
    [SerializeField] private GameObject rock;
    //[SerializeField] private GameObject rock2;
    [SerializeField] private Transform rockParent;

    //private LayerMask layerMask;
    private float waterLevel;
    private float height;
    private float grassMin;
    private float grassMax;
    private float sandMin;
    private float sandMax;
    private GenerationSettings genSettings;
    //private GameObject[] trees;

    //private void Start()
    //{
    //    //layerMask = ~LayerMask.GetMask("Tree");
    //    genSettings = GetComponent<GenerationSettings>();
    //    waterLevel = genSettings.waterLevel;
    //    height = genSettings.height;
    //    grassMin = waterLevel * height + height * (1 - waterLevel) * .05f;
    //    grassMax = waterLevel * height + height * (1 - waterLevel) * .15f;
    //    sandMin = waterLevel * height;
    //    sandMax = waterLevel * height + height * (1 - waterLevel) * .04f;
    //}

    public void Initialise()
    {
        genSettings = GetComponent<GenerationSettings>();
        waterLevel = genSettings.waterLevel;
        height = genSettings.height;
        grassMin = waterLevel * height + height * (1 - waterLevel) * .05f;
        grassMax = waterLevel * height + height * (1 - waterLevel) * .15f;
        sandMin = waterLevel * height;
        sandMax = waterLevel * height + height * (1 - waterLevel) * .03f;
    }

    public GameObject[] SpawnTrees(int numOfTrees, Vector3 tilePos, int xSize, int zSize, Vector3[] vertices)
    {
        GameObject[] trees = new GameObject[numOfTrees];
        Vector3 treePosition = Vector3.zero;
        //float rayDist = 200f;
        //RaycastHit hit;
        for (int i = 0; i < numOfTrees; i++)
        {
            treePosition = tilePos;
            float randX = Random.Range(0, xSize);
            float randZ = Random.Range(0, zSize);
            treePosition.x += randX - xSize / 2;// Random.Range(-xSize / 2, xSize / 2);
            treePosition.z += randZ - zSize / 2;// Random.Range(-zSize / 2, zSize / 2);
            treePosition.y = vertices[(int)randX + (int)randZ * (zSize + 1)].y;
            //treePosition.y = meshGenScript.CalculateHeight(treePosition.x, treePosition.y);
            if (treePosition.y > grassMin && treePosition.y < grassMax)
            {
                trees[i] = Instantiate(tree, treePosition, Quaternion.identity, treeParent);
                trees[i].transform.localScale *= Random.Range(.75f, 1.25f);
            }
            //treePosition.y += rayDist;
            //if (Physics.Raycast(treePosition, Vector3.down, out hit, rayDist, layerMask))
            //{
            //    if (hit.point.y > grassMin && hit.point.y < grassMax)
            //    {
            //        treePosition.y = hit.point.y;
            //        trees[i] = Instantiate(tree, treePosition, Quaternion.identity, treeParent);
            //    }
            //}
        }
        return trees;
    }

    public void DestroyObjects(GameObject[] objectsToDestroy)
    {
        if (objectsToDestroy != null)
            foreach (GameObject obj in objectsToDestroy)
                if (obj != null)
                    Destroy(obj);
    }

    public GameObject[] SpawnRocks(int numOfRocks, Vector3 tilePos, int xSize, int zSize, Vector3[] vertices)
    {
        GameObject[] rocks = new GameObject[numOfRocks];
        Vector3 rockPosition = Vector3.zero;

        for (int i = 0; i < numOfRocks; i++)
        {
            rockPosition = tilePos;
            float randX = Random.Range(0, xSize);
            float randZ = Random.Range(0, zSize);
            rockPosition.x += randX - xSize / 2;
            rockPosition.z += randZ - zSize / 2;
            rockPosition.y = vertices[(int)randX + (int)randZ * (zSize + 1)].y;
            if (/*rockPosition.y > sandMin && */rockPosition.y < sandMax)
            {
                rocks[i] = Instantiate(/*(Random.Range(0, 2) == 0) ? */rock/* : rock2*/, rockPosition, Quaternion.identity, rockParent);
                rocks[i].transform.localScale *= Random.Range(.6f, 1.2f);
            }
        }
        return rocks;
    }
}
