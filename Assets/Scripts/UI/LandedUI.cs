using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LandedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleTextMesh;
    [SerializeField] private TextMeshProUGUI statsTextMesh;
    [SerializeField] private TextMeshProUGUI nextButtonTextMesh;
    [SerializeField] private Button nextButton;


    private Action nextButtonClickAction;


    private void Awake()
    {
        nextButton.onClick.AddListener(() =>
        {
            nextButtonClickAction();
        });
    }


    private void Start()
    {
        Lander.Instance.OnLanded += Lander_OnLanded;

        Hide();
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        if(e.landingType == Lander.LandingType.Success)
        {
            titleTextMesh.text = "SUCCESSFUL LANDING!";
            nextButtonTextMesh.text = "CONTINUE";
            nextButtonClickAction = GameManager.Instance.GoToNextLevel;
        }
        else
        {
            titleTextMesh.text = "<color=#ff0000>CRASHED!</color>";
            nextButtonTextMesh.text = "RETRY";
            nextButtonClickAction = GameManager.Instance.RetryLevel;
        }



        statsTextMesh.text = timecalculator() + "\n" + e.score + "\n" + getStatsReason(e);

        Show();
    }


    private string timecalculator()
    {
        int totalSeconds = Mathf.FloorToInt(GameManager.Instance.GetTime()); //Time Conversion into 00:00:00
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
    }



    private string getStatsReason(Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case Lander.LandingType.TooFastLanding:
                return "LANDED TOO FAST!";
            case Lander.LandingType.TooSteepAngle:
                return "LANDED TOO STEEP!";
            case Lander.LandingType.WrongLandingArea:
                return "LANDED ON TERRAIN!";
            default:
                return "";
        }
    }

    private  void Show()
    {
        gameObject.SetActive(true);

        nextButton.Select();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
