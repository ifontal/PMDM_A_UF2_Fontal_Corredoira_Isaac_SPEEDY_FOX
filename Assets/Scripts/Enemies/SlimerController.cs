using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimerController : MonoBehaviour
{
    const int POINTS = 100;
    [SerializeField] float detectDistance;
    [SerializeField] GameObject death;
    SpriteRenderer sr;
    Animator anim;
    GameObject player;
    GameController gc;
    Vector2 distance;

    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        gc = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    void Update() {
        distance = transform.position - player.transform.position;
        if(distance.sqrMagnitude < detectDistance * detectDistance) {
            anim.SetTrigger("Attack");
            if(transform.position.x < player.transform.position.x) {
                sr.flipX = true;
            } else {
                sr.flipX = false;
            }
        } else {
            anim.ResetTrigger("Attack");
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
