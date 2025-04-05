using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    NoSQLDatabase database;
    [SerializeField] TMP_Text timeText;

    async void Start()
    {
        database = new NoSQLDatabase("GameDatabase", "HighScores");

        await database.LoadDatabase(); // Ensure database is loaded before fetching high score

        float? highScore = database.GetHighScore();
        timeText.text = $"Record: {highScore} seconds";
    }
    // Called when we click the "Play" button.
    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }
    // Called when we click the "Quit" button.
    public void OnQuitButton()
    {
        Application.Quit();
    }

}