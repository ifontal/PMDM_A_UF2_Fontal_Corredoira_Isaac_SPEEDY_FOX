using System.Collections;
using UnityEngine;


public class DinoController : MonoBehaviour
{
    const int POINTS = 150;
    [SerializeField] float moveRange, duration;
    [SerializeField] GameObject death;
    Vector3 initPosition;
    float finalPosition;
    SpriteRenderer sr;
    GameController gc;
    GameObject player;

    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        initPosition = transform.position;
        finalPosition = initPosition.x + moveRange;
        player = GameObject.Find("Player");
        gc = GameObject.Find("GameManager").GetComponent<GameController>();
        StartCoroutine(Move());     
    }

    IEnumerator Move() {
        while(true) {
            float t;
            t = 0;
            sr.flipX = true;
            while (t < duration) {
                t += Time.deltaTime;
                transform.position = new Vector3(Mathf.Lerp(initPosition.x, finalPosition, t / duration), transform.position.y, transform.position.z);
                yield return null;
            }
            t = 0;
            sr.flipX = false;
            while (t < duration) {
                t += Time.deltaTime;
                transform.position = new Vector3(Mathf.Lerp(finalPosition, initPosition.x, t / duration), transform.position.y, transform.position.z);
                yield return null;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(player.GetComponent<PlayerController>().IsAttacking()) {
                gc.UpdateScore(POINTS);
                Instantiate(death, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
