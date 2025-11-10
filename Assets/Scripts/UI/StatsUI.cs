using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI statsTextMesh;

    private void UpdateStatsTextMesh()
    {
        statsTextMesh.text = GameManager.Instance.GetLevelNumber() + "\n" +
            GameManager.Instance.GetScore() + "\n" +
            timecalculator();
        
    }

    private string timecalculator()
    {
        int totalSeconds = Mathf.FloorToInt(GameManager.Instance.GetTime()); //Time Conversion into 00:00:00
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void Update()
    {
        UpdateStatsTextMesh();
    }
}
