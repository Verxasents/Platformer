using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    public string FirstLevelName;

    [SerializeField] private GameObject[] uiElements;

    [SerializeField] private GameObject continueButton;

    [Header("Interactive Camera")]
    [SerializeField] private MenuCharacter menuCharacter;
    [SerializeField] private CinemachineVirtualCamera cinemachine;
    [SerializeField] private Transform mainMenuPoint;
    [SerializeField] private Transform skinSelectionPoint;


    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }
    private void Start()
    {
        if(HasLevelProgression())
            continueButton.SetActive(true);

        fadeEffect.ScreenFade(0, 1.5f);
    }

    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (GameObject ui in uiElements)
        {
            ui.SetActive(false);
        }

        uiToEnable.SetActive(true);

        AudioManager.instance.PlaySFX(4);
    }
    public void NewGame()
    {
        fadeEffect.ScreenFade(1, 1.5f, LoadLevelScene);
        AudioManager.instance.PlaySFX(4);
    }

    private void LoadLevelScene() => SceneManager.LoadScene(FirstLevelName);

    private bool HasLevelProgression()
    {
        bool hasLevelProgression = PlayerPrefs.GetInt("ContinueLevelNumber", 0) > 0;

        return hasLevelProgression;
    }

    public void ContinueGame()
    {
         int difficultyIndex = PlayerPrefs.GetInt("GameDifficulty", 1);

        int levelToLoad = PlayerPrefs.GetInt("ContinueLevelNumber", 0);
        int lastSavedSkin = PlayerPrefs.GetInt("LastUsedSkin");
        SkinManager.instance.SetSkinId(lastSavedSkin);

        DifficultyManager.instance.LoadDifficulty(difficultyIndex);
        SceneManager.LoadScene("Level_" + levelToLoad);
        AudioManager.instance.PlaySFX(4);
         
    }

    public void MoveCameraToMainMenu()
    {
        menuCharacter.MoveTo(mainMenuPoint);
        cinemachine.Follow = mainMenuPoint;
    }

    public void MoveCameraToSkinMenu()
    {
        menuCharacter.MoveTo(skinSelectionPoint);
        cinemachine.Follow = skinSelectionPoint;
    }
}
