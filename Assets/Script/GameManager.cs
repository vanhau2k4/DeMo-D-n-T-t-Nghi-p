using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum GameState
    {
        GamePlay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;

    public GameState previosState;

    [Header("Damage Text Setting")]
    public Canvas damageCanvas;
    public float textFontsize = 30;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;
    int stackdLevelUps = 0;

    [Header("Current Stats Displays")]
    public TMP_Text currentHealthDisplay;
    public TMP_Text currentRecoveryDisplay;
    public TMP_Text currentMoveSpeedDisplay;
    public TMP_Text currentMightDisplay;
    public TMP_Text currentProjecttileSpeedDisplay;
    public TMP_Text currentMagnetDisplay;

    [Header("Results Screen Displays")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurvivedDisplay;

    [Header("Stopwatch")]
    public float timeLimit;
    float stopwatchTime;
    public TMP_Text stopwatchDisplay;


    PlayerStats[] players;

    public bool isGameOver { get { return currentState == GameState.Paused; } }
    public bool choosingUpgrade { get { return currentState == GameState.LevelUp; } }

    public float GetElapsedTime() { return stopwatchTime; }

    public static float GetCumulativeCurse()
    {
        if (!instance) return 1;
        float totalCurse = 0;
        foreach(PlayerStats p in instance.players)
        {
            totalCurse += p.Actual.curse;
        }
        return Mathf.Max(1,1 + totalCurse);
    }
    public static float GetCumulativeLevels()
    {
        if (!instance) return 1;
        float totalLevel = 0;
        foreach (PlayerStats p in instance.players)
        {
            totalLevel += p.Actual.curse;
        }
        return Mathf.Max(1, 1 + totalLevel);
    }

    private void Awake()
    {
        players = FindObjectsOfType<PlayerStats>();
        if (instance == null)
        {
            instance = this;
        }
        else { Debug.LogWarning("EXTRA " + this + " DELETED"); }

        DisableScreens();
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.GamePlay:
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;

            case GameState.Paused:
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
                break;
            case GameState.LevelUp:
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }
    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!instance.damageCanvas) return;
        if(!instance.referenceCamera)instance.referenceCamera = Camera.main;
        instance.StartCoroutine(instance.GenerateFloatingTextCorortine(text,target,duration,speed));
    }
    public void ChangeState(GameState newState)
    {
        previosState = currentState;
        currentState = newState;
    }
    public void PauseGame()
    {
        if(currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
        }
    }
    public void ResumeGame()
    {
        if(currentState == GameState.Paused)
        {
            ChangeState(previosState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
        }
    }
    private void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    private void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }
    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text;
        ChangeState(GameState.GameOver);
        Time.timeScale = 0f;
        DisPlayResults();
    }
    private void DisPlayResults()
    {
        resultsScreen.SetActive(true);
    }
    public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;
    }
    public void AssignLevelReacheUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }
    private void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopwatchDisplay();

        if (stopwatchTime >= timeLimit)
        {
            foreach (PlayerStats p in players)
            {
                p.SendMessage("Kill");
            }
        }
    }
    private void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        if (levelUpScreen.activeSelf) stackdLevelUps++;
        else
        {
            levelUpScreen.SetActive(true);
            Time.timeScale = 0f;
            foreach (PlayerStats p in players)
            {
                p.SendMessage("RemoveAndApplyUpgrades");
            }
        }
    }
    public void EndLevelUp()
    {
        Time.timeScale = 1;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.GamePlay);
        if(stackdLevelUps > 0)
        {
            stackdLevelUps--;
            StartLevelUp();
        }
    }
    IEnumerator GenerateFloatingTextCorortine(string text,Transform target,float duration = 1f, float speed = 50f)
    {
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontsize;
        if(textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(textObj,duration);

        textObj.transform.SetParent(instance.damageCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnowPosition = target.position;
        while(t < duration)
        {
            if (!rect) break;

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            if (target)
            {
                lastKnowPosition = target.position;
            }

            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnowPosition + new Vector3(0, yOffset));

            yield return w;
            t += Time.deltaTime;

        }
    }
}
