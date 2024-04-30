using System;
using System.Collections;
using UnityEngine;

public class EagleController : MonoBehaviour
{
    const int POINTS = 250;
    [SerializeField] float moveDistance, attackDistance, speed, attackForce;
    [SerializeField] GameObject death;
    Rigidbody2D rb;
    Collider2D col;
    Animator anim;
    GameObject player;
    Vector2 distance, direction, engageDirection;
    GameController gc;
    bool attack = false;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        gc = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    void Update() {
        if(!attack) {
            distance = transform.position - player.transform.position;
            if(distance.magnitude < moveDistance) {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack() {
        attack = true;
        if(transform.position.x < player.transform.position.x) {
            GetComponent<SpriteRenderer>().flipX = true;
            rb.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
        } else {
            rb.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
        }
        direction = new Vector2(Mathf.Sign(rb.velocity.x), -1);
        engageDirection = new Vector2(Mathf.Sign(rb.velocity.x) * 1.1f, -1);
        DateTime startTime = DateTime.Now;
        while(!PlayerEngaged()) {
            yield return null;
            if(DateTime.Now - startTime > TimeSpan.Parse("00:00:20")) {
                Destroy(gameObject);
            }
        }
        rb.AddForce(direction * attackForce, ForceMode2D.Impulse);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private bool PlayerEngaged() {
        RaycastHit2D rc = Physics2D.Raycast(
            col.bounds.center,
            engageDirection,
            attackDistance,
            LayerMask.GetMask("Player"));
        return rc.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(player.GetComponent<PlayerController>().IsAttacking()) {
                gc.UpdateScore(POINTS);
                Instantiate(death, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
