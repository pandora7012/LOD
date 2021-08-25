using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GamePlay_UI : MonoBehaviour
{
    //Play Field UI 
    public Text scoreText;
    public Text highScoreText;
    [SerializeField]
    private GridManager grid;

    //GameOver UI
    public GameObject gameOverUI;
    public Image dragon;
    public Text YourScore;
    public Text highScore;
    //
    public GameObject pauseButton;
    public static bool paused; 

    public void Start()
    {
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = grid.score.ToString();
        highScoreText.text = "Highscore: "+  PlayerPrefs.GetInt("highscore").ToString();
        //

        if (grid.gameIsOver)
        {
            PlayerPrefs.SetInt("highscore", Mathf.Max( PlayerPrefs.GetInt("highscore"), grid.score));   
            gameOverUI.SetActive(true);
            dragon.sprite = grid.dragons[grid.maxInit].GetComponent<SpriteRenderer>().sprite;
            dragon.SetNativeSize();
            YourScore.text = grid.score.ToString();
            highScore.text =  PlayerPrefs.GetInt("highscore").ToString();
        }
    }

    public void HomeButton()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1; 

    }

    public void ReplayButton()
    {
        SceneManager.LoadScene("GamePlay");
        Time.timeScale = 1;
    }

    public void PauseButton()
    {
        if (paused)
        {
            Time.timeScale = 1;
            pauseButton.GetComponent<Image>().color = Color.white ;
            paused = false;
        }
        else
        {
            paused = true;
            pauseButton.GetComponent<Image>().color = Color.red;

        }
    }
}
