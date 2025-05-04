using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private UI_InGame inGameUI;


    [Header("Level Management")]
    [SerializeField] private float levelTimer;
    [SerializeField] private int currentLevelIndex;
    private int nextLevelIndex;



    [Header("Fruits Management")]
    public bool fruitsAreRandom;
    public int fruitsCollected;
    public int totalFruits;
    public Transform fruitParent;

    [Header("Checkpoints")]
    public bool canReactivate;



    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private SkinManager skinManager;
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private ObjectCreator objectCreator;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inGameUI = UI_InGame.instance;

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        nextLevelIndex = currentLevelIndex + 1;
        CollectFruitsInfo();
        CreateManagersIfNeeded();
    }

    private void Update()
    {
        levelTimer += Time.deltaTime;

        inGameUI.UpdateTimerUI(levelTimer);
    }

    private void CreateManagersIfNeeded()
    {
        if(AudioManager.instance == null)
            Instantiate(audioManager);

        if(PlayerManager.instance == null)
            Instantiate(playerManager);

        if(SkinManager.instance == null)
            Instantiate(skinManager);

        if(DifficultyManager.instance == null)
            Instantiate(difficultyManager);

        if(ObjectCreator.instance == null)
            Instantiate(objectCreator);
    }

    private void CollectFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalFruits = allFruits.Length;

        inGameUI.UpdateFruitUI(fruitsCollected, totalFruits);


        PlayerPrefs.SetInt("Level" + currentLevelIndex + "TotalFruits", totalFruits);
    }

    [ContextMenu("Parent All Fruits")]
    private void ParentAllTheFruits()
    {

        if (fruitParent == null)
            return;

        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);

        foreach (Fruit fruit in allFruits)
        {
            fruit.transform.parent = fruitParent;
        }
    }

    public void AddFruit()
    {
        fruitsCollected++;
        inGameUI.UpdateFruitUI(fruitsCollected,totalFruits);
    }

    public void RemoveFruit()
    {
        fruitsCollected--;
        inGameUI.UpdateFruitUI(fruitsCollected, totalFruits);
    }

    public int FruitsCollected() => fruitsCollected;
    public bool FruitsHaveRandomLook() => fruitsAreRandom;

    public void LevelFinished()
    {
        SaveLevelProgression();
        SaveBestTime();
        SaveFruitsInfo();
        LoadNextScene();
    }

    private void SaveFruitsInfo()
    {
        int fruitsCollectedBefore = PlayerPrefs.GetInt("Level" + currentLevelIndex + "FruitsCollected");

        if (fruitsCollectedBefore < fruitsCollected)
            PlayerPrefs.SetInt("Level" + currentLevelIndex + "FruitsCollected", fruitsCollected);

        int totalFruitsInBank = PlayerPrefs.GetInt("TotalFruitsAmount");
        PlayerPrefs.SetInt("TotalFruitsAmount", totalFruitsInBank + fruitsCollected);
    }

    private void SaveBestTime()
    {
        float lastTime = PlayerPrefs.GetFloat("Level" + currentLevelIndex + "BestTime", 99);

        if(levelTimer < lastTime)
        PlayerPrefs.SetFloat("Level" + currentLevelIndex + "BestTime", levelTimer);


    }

  private void SaveLevelProgression()
  {
        PlayerPrefs.SetInt("Level" + nextLevelIndex + "Unlocked", 1);

        if (NoMoreLevels() == false)
        {
            PlayerPrefs.SetInt("ContinueLevelNumber",nextLevelIndex);
            PlayerPrefs.SetInt("LastUsedSkin", SkinManager.instance.GetSkinId);
        }
  }

    public void RestartLevel()
    {
        UI_InGame.instance.fadeEffect.ScreenFade(1, .75f, LoadCurrentScene);
    }

    private void LoadCurrentScene() => SceneManager.LoadScene("Level_" + currentLevelIndex);

    private void LoadTheEndScene() => SceneManager.LoadScene("TheEnd");

    private void LoadNextLevel()
    {
        SceneManager.LoadScene("Level_" + nextLevelIndex);
    }

    private void LoadNextScene()
    {
        UI_FadeEffect fadeEffect = UI_InGame.instance.fadeEffect;

        if (NoMoreLevels())
            fadeEffect.ScreenFade(1, 1.5f, LoadTheEndScene);
        else
            fadeEffect.ScreenFade(1, 1.5f, LoadNextLevel);
    }

    private bool NoMoreLevels()
    {
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 2; //Ana Menü ve The End sahneleri de olduðundan 2 kullandým
        bool noMoreLevels = currentLevelIndex == lastLevelIndex;

        return noMoreLevels;
    }
}
