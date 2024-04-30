using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossBattleController : MonoBehaviour
{
    const int POINTS = 5000;
    bool touched;
    [SerializeField] AudioClip bossMusic, goalSound;
    [SerializeField] GameController gameController;
    GameObject boss;
    AudioSource cameraPlayer;

    private void Start() {
        touched = false;
        boss = GameObject.Find("Boss");
        cameraPlayer = Camera.main.GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && !touched) {
            touched = true;
            StartCoroutine(Boss());
        }
    }

    IEnumerator Boss() {
        cameraPlayer.clip = bossMusic;
        cameraPlayer.Play();
        while(!boss.IsDestroyed()) {
            yield return null;
        }
        cameraPlayer.Stop();
        AudioSource.PlayClipAtPoint(goalSound, Camera.main.transform.position);
        GameObject.Find("LimitWall2").SetActive(false);
        gameController.UpdateScore(POINTS);
        gameController.SceneClear();
    }
}
