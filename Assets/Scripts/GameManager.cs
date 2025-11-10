using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int score;
    private float time;
    private bool isTimerActive;
    public static GameManager Instance { get; private set; }


    [SerializeField] private List<GameLevel> gameLevelList;
    [SerializeField] private CinemachineCamera cinemachineCamera;


    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;



    private static int levelNumber = 1;
    private static int totalScore = 0;


    public static void ResetStaticData()
    {
        levelNumber = 1;
        totalScore = 0;
    }

    
    
    
    
    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Lander.Instance.OnCoinPickup += Instance_OnCoinPickup;
        Lander.Instance.OnStageChanged += Lander_OnStageChanged;

        GameInput.Instance.OnMenuButtonPressed += GameInput_OnMenuButtonPressed;

        LoadCurrentLevel();
    }

    private void GameInput_OnMenuButtonPressed(object sender, System.EventArgs e)
    {
        PauseUnpauseGame();
    }

    private void Lander_OnStageChanged(object sender, Lander.OnStageChangedEventArgs e)
    {
        isTimerActive = e.state == Lander.State.Normal;

        if(e.state == Lander.State.Normal)
        {
            cinemachineCamera.Target.TrackingTarget = Lander.Instance.transform;
            CinemachineCameraZoom2D.Instance.SetNormalOrthographicSize();
        }
    }

    private void Update()
    {
        if (isTimerActive)
        {
            time += (Time.deltaTime);
        }
    }


    private void LoadCurrentLevel()
    {
        GameLevel gameLevel = GetGameLevel();
        GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
        Lander.Instance.transform.position = spawnedGameLevel.GetLanderStartPosition();

        cinemachineCamera.Target.TrackingTarget = spawnedGameLevel.GetCameraStartTargetTransform();
        CinemachineCameraZoom2D.Instance.SetTargetOrthographicSize(spawnedGameLevel.GetZoomedOutOrthographicSize());
    }



    private GameLevel GetGameLevel()
    {
        foreach (GameLevel gameLevel in gameLevelList)
        {
            if (gameLevel.GetLevelNumber() == levelNumber)
            {
                return gameLevel;
            }
        }
        return null;
    }




    public void GoToNextLevel()
    {
        levelNumber++;
        totalScore += score;



        if (GetGameLevel() == null)
        {
            //No more levels
            SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
        }
        else
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        }
    }

    public void RetryLevel()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }


    public int GetTotalScore()
    {
        return totalScore;
    }




    private void Instance_OnCoinPickup(object sender, System.EventArgs e)
    {
        AddScore(10);
    }
    public void AddScore(int addScoreAmount)
    {
        score += addScoreAmount;
        Debug.Log(score);
    }





    public int GetScore()
    {
        return score;
    }

    public float GetTime()
    {
        return time;
    }

    public int GetLevelNumber()
    {
        return levelNumber;
    }




    public void PauseUnpauseGame()
    {
        if(Time.timeScale == 1f)
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        OnGameUnpaused?.Invoke(this, EventArgs.Empty);
    }
}
