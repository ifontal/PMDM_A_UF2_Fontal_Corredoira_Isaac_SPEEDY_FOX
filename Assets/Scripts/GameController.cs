using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    const int LIFES = 3;
    const int SCORE_CALC_TIME = 3;
    const int TIME_BONUS_MULT = 10;
    const float PLAYER_EXIT_SPEED = 15;
    public const float TIME_TO_DEAD = 300;
    public const int GEM_BONUS_MULT = 100;
    [SerializeField] AudioClip extraLifeSound, gemSound, continueSound;
    private static int lifesCount = LIFES;
    private static int score = 0;
    private static int continues = 0;
    private int bonusTime;
    private int gemsCount;
    private float timeInSeconds;
    private bool finished;
    private bool paused = false;
    private static String msg = "";
    private static bool gameOver = false;
    private static bool extraLifeTaken = false;

    private void Start() {
        Time.timeScale = 1;
        gemsCount = 0;
        timeInSeconds = 0;
        bonusTime = 0;
        finished = false;
    }
    
    private void Update() {
        if(!paused && !gameOver && !finished) {
            timeInSeconds += Time.deltaTime;
        }

        if(!gameOver && Input.GetKeyDown(KeyCode.P) && SceneManager.GetActiveScene().buildIndex != 0) {
            if(paused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }

        if(gameOver && Input.GetKeyDown(KeyCode.C) && continues > 0) {
            NewGame(continues-1);
            ReloadScene();
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
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
        float t;
        int duration;
        int initialScore = score;
        int partial = score + bonusTime;
        int init = bonusTime;

        yield return new WaitForSeconds(2);
        GameObject.Find("ScoreBoard").GetComponent<AudioSource>().Play();
        t = 0;
        duration = (bonusTime == 0) ? 0 : SCORE_CALC_TIME;
        while (t < duration) {
            t += Time.deltaTime;
            bonusTime = ((int)Mathf.Lerp(init, 0, t / duration));
            score = ((int)Mathf.Lerp(initialScore, partial, t / duration));
            yield return null;
        }
        t = 0;
        duration = (gemsCount == 0) ? 0 : SCORE_CALC_TIME;
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

    private IEnumerator PlayerExit() {
        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Animator>().SetTrigger("Ground");
        player.GetComponent<Animator>().SetBool("Running", true);
        player.transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        for(int i = 0; i < 5; i++) {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.right * PLAYER_EXIT_SPEED;
            yield return null;
        }
    }

    public void ExtraLifeTaken() {
        extraLifeTaken = true;
    }

    public bool IsExtraLifeTaken() {
        return extraLifeTaken;
    }

    public void GameOver() {
        gameOver = true;
        if (continues > 0) {
            msg = "GAME OVER\nPRESS <C> TO CONTINUE OR <ESC> TO QUIT";
        } else {
            msg = "GAME OVER\nPRESS <ESC> TO QUIT";
        }
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
        AudioSource.PlayClipAtPoint(gemSound, Camera.main.transform.position);
        gemsCount += gems;
        if(gemsCount > 99) {
            gemsCount -= 100;
            UpdateLifes(1);
        }
    }

    public void UpdateLifes(int lifes) {
        if(lifes > 0) {
            AudioSource.PlayClipAtPoint(extraLifeSound, Camera.main.transform.position);
        }
        lifesCount += lifes;
    }

    public void UpdateScore(int newScore) {
        score += newScore;
    }

    public void AddContinues() {
        AudioSource.PlayClipAtPoint(continueSound, Camera.main.transform.position);
        continues++;
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
        StartCoroutine(PlayerExit());
        GameObject scoreBoard = GameObject.Find("GUI").transform.Find("ScoreBoard").gameObject;
        bonusTime = (int)(TIME_TO_DEAD - timeInSeconds) * TIME_BONUS_MULT;
        scoreBoard.SetActive(true);
        StartCoroutine(CalculateScore());
    }

    public void NewGame() {
        lifesCount = LIFES;
        score = 0;
        continues = 0;
        msg = "";
        gameOver = false;
        paused = false;
        extraLifeTaken = false;
    }

    public void NewGame(int newContinues) {
        lifesCount = LIFES;
        score = 0;
        continues = newContinues;
        msg = "";
        gameOver = false;
        paused = false;
        extraLifeTaken = false;
    }
}
