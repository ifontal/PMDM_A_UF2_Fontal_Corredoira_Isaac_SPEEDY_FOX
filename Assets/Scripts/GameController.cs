using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    const int LIFES = 3;
    public const float TIME_TO_DEAD = 600;
    public const int GEM_BONUS_MULT = 100;
    private static int lifesCount = LIFES;
    private static int score = 0;
    private int bonusTime;
    private int gemsCount;
    private float timeInSeconds;
    private bool finished;
    private static String msg = "";
    private static bool gameOver = false;
    private static bool paused = false;
    private static bool extraLifeTaken = false;

    private void Start() {
        gemsCount = 0;
        timeInSeconds = 0;
        bonusTime = 0;
        finished = false;
    }
    
    private void Update() {
        if(!paused && !gameOver && !finished) {
            timeInSeconds += Time.deltaTime;
        }

        if(!gameOver && Input.GetKeyDown(KeyCode.P)) {
            if(paused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }

        if(gameOver && Input.GetKeyDown(KeyCode.C)) {
            lifesCount = LIFES;
            score = 0;
            msg = "";
            gameOver = false;
            extraLifeTaken = false;
            ReloadScene();
        }

        if(gameOver && Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    private void PauseGame() {
        paused = true;
        AudioListener.pause = true;
        msg = "PAUSED\nPRESS <P> TO RESUME";
        Time.timeScale = 0;
    }

    private void ResumeGame() {
        paused = false;
        AudioListener.pause = false;
        msg = "";
        Time.timeScale = 1;
    }

    private void NextScene() {
        extraLifeTaken = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator CalculateScore() {
        float t = 0;
        int duration = 4;
        int initialScore = score;
        int partial = score + bonusTime;
        int init = bonusTime;
        yield return new WaitForSeconds(2);
        GameObject.Find("ScoreBoard").GetComponent<AudioSource>().Play();
        while (t < duration) {
            t += Time.deltaTime;
            bonusTime = ((int)Mathf.Lerp(init, 0, t / duration));
            score = ((int)Mathf.Lerp(initialScore, partial, t / duration));
            yield return null;
        }
        t = 0;
        partial = score + (gemsCount * GEM_BONUS_MULT);
        init = gemsCount;
        initialScore = score;
        while (t < duration) {
            t += Time.deltaTime;
            gemsCount = ((int)Mathf.Lerp(init, 0, t / duration));
            score = ((int)Mathf.Lerp(initialScore, partial, t / duration));
            yield return null;
        }
        GameObject.Find("ScoreBoard").GetComponent<AudioSource>().Stop();
        Invoke("NextScene", 3);
    }

    public void ExtraLifeTaken() {
        extraLifeTaken = true;
    }

    public bool IsExtraLifeTaken() {
        return extraLifeTaken;
    }

    public void GameOver() {
        gameOver = true;
        msg = "GAME OVER\nPRESS <C> TO CONTINUE OR <ESC> TO QUIT";
    }

    public String GetMessage() {
        return msg;
    }

    public bool IsPaused() {
        return paused;
    }
    
    public String CalculateClock() {
        int minutes = (int)timeInSeconds / 60;
        int seconds = (int)timeInSeconds % 60;
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void ResetClock() {
        timeInSeconds = 0;
    }

    public float GetTime() {
        return timeInSeconds;
    }

    public void UpdateGems(int gems) {
        gemsCount += gems;
        if(gemsCount > 99) {
            ResetGems();
            UpdateLifes(1);
        }
    }

    public void UpdateLifes(int lifes) {
        lifesCount += lifes;
    }

    public void UpdateScore(int newScore) {
        score += newScore;
    }

    public void ResetGems() {
        gemsCount = 0;
    }

    public int GetGemsCount() {
        return gemsCount;
    }

    public int GetLifesCount() {
        return lifesCount;
    }

    public int GetScore() {
        return score;
    }

    public int GetBonusTime() {
        return bonusTime;
    }

    public void SceneClear() {
        finished = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject scoreBoard = GameObject.FindGameObjectWithTag("GUI").transform.Find("ScoreBoard").gameObject;
        bonusTime = (int)(TIME_TO_DEAD - timeInSeconds) * 10;
        
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Animator>().SetBool("Running", true);
        player.GetComponent<Rigidbody2D>().velocity = Vector2.right * 10;
        player.GetComponent<SpriteRenderer>().flipX = false;

        scoreBoard.SetActive(true);
        StartCoroutine(CalculateScore());
    }
}
