using System;
using UnityEngine;
using UnityEngine.UI;


public class GUIController : MonoBehaviour
{
    [SerializeField] Text textGems, textLifes, textClock, textMessage;
    [SerializeField] GameController gameController;

    
    private void OnGUI() {
        textGems.text = String.Format("{0,2:D2}", gameController.GetGemsCount());
        textLifes.text = String.Format("x {0,2:D2}", gameController.GetLifesCount());
        textClock.text = gameController.CalculateClock();
        textMessage.text = gameController.GetMessage();
    }
}
