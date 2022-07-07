/// ================================
/// Peter Phillips, 2022
/// ================================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerationSettings : MonoBehaviour
{
    [Range(1, 255)]
    public int xSize, zSize = 200;
    [Range(.005f, .02f)]
    public float perlinScale = .01f;
    public float xOffset, zOffset = 1000f;
    [Range(1f, 120f)]
    public float height = 80f;
    [Range(1, 8)]
    public int octaves = 5;
    [Range(1f, 3f)]
    public float lacunarity = 2f;
    [Range(.1f, .6f)]
    public float persistence = .4f;
    [Range(0f, 1f)]
    public float waterLevel = .6f;
    [Range(.2f, 8f)]
    public float gradient = 1f;
    public int numOfTrees = 100;
    public int numOfRocks = 100;
    [Range(0f, 1f)]
    public float curvatureAmount = .25f;
    public Gradient colourGradient;
    public Material[] worldBending;

    [SerializeField] private MeshGenerator testTile;
    [SerializeField] private Slider[] sliders;
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private PlayerController player;
    [SerializeField] private Canvas generationUI;
    [SerializeField] private Canvas loadingUI;
    [SerializeField] private Canvas inGameUI;
    [SerializeField] private Canvas hideUI;
    [SerializeField] private Canvas exitUI;
    [SerializeField] private Image loadingBG;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private GameObject textAndSlider;
    [SerializeField] private Transform lightSource;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Text hideText;

    private TreeSpawner treeSpawner;
    private Vector2 direction = Vector3.zero;
    private bool inGame = false;
    private Vector3 startPos;
    private Coroutine startGameCo;
    private int numOfTimes = 120;

    #region MonoBehaviour
    private void Awake()
    {
        treeSpawner = GetComponent<TreeSpawner>();
    }
    private void Start()
    {
        SetCurvature(true);
        // Activate our test tile.
        testTile.transform.parent.gameObject.SetActive(true);
        startPos = player.transform.position;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            exitUI.enabled = true;

        // If not in game and pressing any WASD keys.
        if (!inGame &&
            (Input.GetAxisRaw("Horizontal") != 0 ||
            Input.GetAxisRaw("Vertical") != 0))
        {
            // Create normalised direction vector.
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            direction.Normalize();
            // Increment our offset values.
            xOffset += direction.x * Time.deltaTime;
            zOffset += direction.y * Time.deltaTime;
            testTile.xOffset += direction.x * Time.deltaTime;
            testTile.zOffset += direction.y * Time.deltaTime;
            // Generate the tile.
            GenerateTerrain();
        }
    }
    #endregion

    #region Functions
    private void GenerateTerrain()
    {
        testTile.SetInitialValues();
        testTile.InitialGeneration();
    }
    private void GenerateObjects()
    {
        testTile.SetInitialValues();
        testTile.GenerateObjects();
    }
    private void SetCurvature(bool inSettings = false)
    {
        foreach (Material mat in worldBending)
        {
            // Set the curvature to 0 if the settings menu is open,
            // otherwise set to the value on the slider.
            mat.SetFloat("CurvatureAmount", (inSettings) ? 0 : curvatureAmount);
        }
    }
    private void SetDefaultValues()
    {
        sliders[0].value = height = 100f;
        sliders[1].value = octaves = 8;
        sliders[2].value = waterLevel = .03f;
        sliders[3].value = 0.0175f;
        perlinScale = .005f;
        sliders[4].value = gradient = 8f;
        sliders[5].value = lacunarity = 2f;
        sliders[6].value = numOfTrees = 100;
        sliders[7].value = numOfRocks = 25;
        xOffset = 1002f;
        zOffset = 1000.8f;
        persistence = .4f;
    }
    private void StartGame()
    {
        inGame = true;
        loadingUI.enabled = true;

        if (startGameCo != null) StopCoroutine(startGameCo);
        startGameCo = StartCoroutine(LoadTiles());
    }
    private IEnumerator LoadTiles()
    {
        // Fade in the loading UI background.
        loadingSlider.value = 0;
        float timer = 0f;
        float fadeTime = .25f;
        Color temp = loadingBG.color;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            temp = loadingBG.color;
            temp.a = timer / fadeTime;
            loadingBG.color = temp;
            yield return null;
        }
        temp.a = 1;
        loadingBG.color = temp;
        textAndSlider.SetActive(true);
        // Turn off generation UI.
        generationUI.enabled = false;
        // Spawn the tiles.
        foreach (GameObject go in tiles)
        {
            go.SetActive(true);
            yield return null;
            loadingSlider.value++;
        }
        // Set bool and curvature, activate player, and turn on in-game UI.
        inGame = true;
        SetCurvature();
        player.enabled = true;
        inGameUI.enabled = true;
        hideUI.enabled = true;
        textAndSlider.SetActive(false);
        // Fade out loading BG.
        timer = fadeTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            temp = loadingBG.color;
            temp.a = timer / fadeTime;
            loadingBG.color = temp;
            yield return null;
        }
        // Turn off loading UI.
        loadingUI.enabled = false;
    }
    private void ResetWorld()
    {
        inGame = false;

        foreach (GameObject go in tiles)
        {
            go.GetComponent<TilePosition>().ResetPosition();
            go.GetComponentInChildren<MeshGenerator>().DestroyTrees();
            go.GetComponentInChildren<MeshGenerator>().DestroyRocks();
            go.SetActive(false);
        }
        testTile.transform.parent.gameObject.SetActive(true);

        player.transform.position = startPos;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.enabled = false;
        // Reset time of day and curvature.
        ResetInGameValues();
        // Swap UI.
        generationUI.enabled = true;
        inGameUI.enabled = false;
        hideUI.enabled = false;
        SetCurvature(true);
    }
    private void ResetInGameValues()
    {
        // Reset time of day and curvature.
        sliders[8].value = 90f;
        Vector3 tempColour;
        Color.RGBToHSV(playerCamera.backgroundColor, out tempColour.x, out tempColour.y, out tempColour.z);
        tempColour.x = 180f / 360f;
        tempColour.z = 1f;
        RenderSettings.fogColor = playerCamera.backgroundColor = Color.HSVToRGB(tempColour.x, tempColour.y, tempColour.z);
        sliders[9].value = .25f;
        curvatureAmount = .25f;
    }
    #endregion

    #region UI
    public void HeightSlider(float newHeight)
    {
        height = newHeight;
        GenerateTerrain();
    }
    public void DetailSlider(float newOctaves)
    {
        octaves = (int)newOctaves;
        GenerateTerrain();
    }
    public void WaterLevelSlider(float newWaterLevel)
    {
        waterLevel = newWaterLevel;
        GenerateTerrain();
    }
    public void ScaleSlider(float newScale)
    {
        perlinScale = .0225f - newScale;
        GenerateTerrain();
    }
    public void GradientSlider(float newGradient)
    {
        gradient = newGradient;
        GenerateTerrain();
    }
    public void TreesSlider(float newTrees)
    {
        numOfTrees = (int)newTrees;
        GenerateObjects();
    }
    public void RocksSlider(float newRocks)
    {
        numOfRocks = (int)newRocks;
        GenerateObjects();
    }
    public void LacunaritySlider(float newLacunarity)
    {
        lacunarity = newLacunarity;
        GenerateTerrain();
    }
    public void PersistenceSlider(float newPersistence)
    {
        persistence = newPersistence;
        GenerateTerrain();
    }
    public void ResetButton()
    {
        SetDefaultValues();
        testTile.GetComponentInParent<TilePosition>().Initialise();
    }
    public void GenerateWorldButton()
    {
        StartGame();
    }
    public void ExitButton()
    {
        exitUI.enabled = true;
    }
    public void ExitConfirmButton()
    {
        Application.Quit();
    }
    public void ExitCancelButton()
    {
        exitUI.enabled = false;
    }
    public void ReturnToSettingsButton()
    {
        ResetWorld();
    }
    public void TimeOfDaySlider(float newTime)
    {
        Vector3 tempColour;
        Color.RGBToHSV(playerCamera.backgroundColor, out tempColour.x, out tempColour.y, out tempColour.z);
        tempColour.x = (180f + Mathf.Abs(90 - newTime) / 3f) / 360f;
        tempColour.z = (100f - Mathf.Abs(90 - newTime) / 3f) / 100f;
        RenderSettings.fogColor = playerCamera.backgroundColor = Color.HSVToRGB(tempColour.x, tempColour.y, tempColour.z);

        Vector3 tempAngles = lightSource.eulerAngles;
        if (newTime > 90f)
        {
            tempAngles.x = 180f - newTime;
            tempAngles.y = 150f;
        }
        else
        {
            tempAngles.x = newTime;
            tempAngles.y = -30f;
        }
        lightSource.eulerAngles = tempAngles;
    }
    public void CurvatureSlider(float newCurvature)
    {
        curvatureAmount = newCurvature;
        SetCurvature();
    }
    public void InGameResetButton()
    {
        ResetInGameValues();
    }
    public void HideUIButton()
    {
        hideText.enabled = inGameUI.enabled = inGameUI.isActiveAndEnabled ? false : true;
    }
    #endregion
}
