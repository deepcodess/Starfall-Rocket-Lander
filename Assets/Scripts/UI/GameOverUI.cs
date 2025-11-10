using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button mainMenubutton;
    [SerializeField] private TextMeshProUGUI scoreTextMesh;


    private void Awake()
    {
        mainMenubutton.onClick.AddListener(() => {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        scoreTextMesh.text =  GameManager.Instance.GetTotalScore().ToString();

        mainMenubutton.Select();
    }
}
