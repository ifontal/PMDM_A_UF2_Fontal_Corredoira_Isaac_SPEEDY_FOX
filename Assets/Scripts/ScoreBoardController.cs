using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoardController : MonoBehaviour
{
    [SerializeField] Text title, score, timeBonus, gemsBonus;
    [SerializeField] GameController gameController;
    
    void Start() {
        int actNumber = SceneManager.GetActiveScene().buildIndex;
        title.text = String.Format("FOX HAS PASSED ACT {0,2:D2}", actNumber);
        score.text = gameController.GetScore().ToString();
        timeBonus.text = String.Format("{0}", gameController.GetBonusTime());
        gemsBonus.text = String.Format("{0,2:D2} x {1:D}", gameController.GetGemsCount(), GameController.GEM_BONUS_MULT);
    }

    private void OnGUI() {
        score.text = gameController.GetScore().ToString();
        timeBonus.text = String.Format("{0}", gameController.GetBonusTime());
        gemsBonus.text = String.Format("{0,2:D2} x {1:D}", gameController.GetGemsCount(), GameController.GEM_BONUS_MULT);
    }
}
