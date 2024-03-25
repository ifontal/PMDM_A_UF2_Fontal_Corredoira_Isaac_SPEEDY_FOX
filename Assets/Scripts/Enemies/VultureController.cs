using System.Collections;
using UnityEngine;

public class VultureController : MonoBehaviour
{
    const int POINTS = 150;
    [SerializeField] float moveRange, duration, detectDistance;
    [SerializeField] GameObject death;
    Vector2 distance;
    Vector3 initPosition;
    float finalPosition;
    bool playerNear, isMoving;
    SpriteRenderer sr;
    Animator anim;
    GameController gc;
    GameObject player;
    
    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        initPosition = transform.position;
        finalPosition = initPosition.y + moveRange;
        playerNear = false;
        isMoving = false;
        player = GameObject.Find("Player");
        gc = GameObject.Find("GameManager").GetComponent<GameController>();     
    }

    private void Update() {
        distance = initPosition - player.transform.position;
        if(distance.magnitude < detectDistance) {
            playerNear = true;
        } else {
            playerNear = false;
        }
        if(initPosition.x > player.transform.position.x) {
            sr.flipX = true;
        } else {
            sr.flipX = false;
        }
    }

    private void FixedUpdate() {
        if (playerNear && !isMoving) {
            StartCoroutine(Move());
        }
    }

    IEnumerator Move() {
        isMoving = true;
        anim.SetBool("Attack", true);
        float t;
        while(playerNear) {
            t = 0;
            while (t < duration) {
                t += Time.deltaTime;
                transform.position = new Vector3(transform.position.x, Mathf.Lerp(initPosition.y, finalPosition, t / duration), transform.position.z);
                yield return null;
            }
            t = 0;
            while (t < duration) {
                t += Time.deltaTime;
                transform.position = new Vector3(transform.position.x, Mathf.Lerp(finalPosition, initPosition.y, t / duration), transform.position.z);
                yield return null;
            }
        }
        anim.SetBool("Attack", false);
        isMoving = false;
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
