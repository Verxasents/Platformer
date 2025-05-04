using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_InGame : MonoBehaviour
{
    public static UI_InGame instance;
    public UI_FadeEffect fadeEffect {  get; private set; } //read-only

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fruitText;

    [SerializeField] private GameObject pauseUI;
    private bool isPaused;
    private void Awake()
    {
        instance = this;

        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }

    private void Start()
    {
        fadeEffect.ScreenFade(0, 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            PauseButton();
    }


    public void PauseButton()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1; 
            pauseUI.SetActive(false);
        }
        else
        {
            isPaused=true;
            Time.timeScale = 0;
            pauseUI.SetActive(true);
        }
    }

    public void GoToMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateFruitUI(int collectedFruits, int totalFruits)
    {
        fruitText.text = collectedFruits + "/" + totalFruits;
    }

    public void UpdateTimerUI(float timer)
    {
        timerText.text = timer.ToString("00") + " s";
    }
}
