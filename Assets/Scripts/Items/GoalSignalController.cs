using System.Collections;
using UnityEngine;

public class GoalSignalController : MonoBehaviour
{
    bool touched;
    Vector3 startPos, finalPos;
    Animator anim;
    [SerializeField] float height, duration;
    [SerializeField] AudioClip signSound, goalSound;
    [SerializeField] GameController gameController;

    private void Start() {
        anim = GetComponent<Animator>();
        touched = false;
        startPos = transform.position;
        finalPos = startPos + Vector3.up * height;
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && !touched) {
            touched = true;
            AudioSource.PlayClipAtPoint(signSound, Camera.main.transform.position);
            StartCoroutine(Goal());
        }
    }

    IEnumerator Goal() {
        anim.SetTrigger("Spin");
        float t = 0;
        while (t < duration) {
            t += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(startPos, finalPos, t/duration);
            transform.position = newPosition;
            yield return null;
        }
        t = 0;
        while (t < duration) {
            t += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(finalPos, startPos, t/duration);
            transform.position = newPosition;
            yield return null;
        }
        switch(Random.Range(0,3)) {
            case 0:
            anim.SetTrigger("Life");
            gameController.UpdateLifes(1);
            break;
            case 1:
            anim.SetTrigger("Gem");
            gameController.UpdateGems(10);
            break;
            case 2:
            anim.SetTrigger("Nothing");
            break;
        }
        Camera.main.GetComponent<AudioSource>().Stop();
        AudioSource.PlayClipAtPoint(goalSound, Camera.main.transform.position);
        GameObject.Find("LimitWall2").SetActive(false);
        gameController.SceneClear();
    }
}
