using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleController : MonoBehaviour
{
    [SerializeField] float moveDistance, attackDistance, speed, attackForce;
    [SerializeField] GameObject death;
    Rigidbody2D rb;
    Animator anim;
    GameObject player;
    Vector2 distance;
    bool attack = false;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
    }

    void Update() {
        distance = transform.position - player.transform.position;
        if(!attack) {
            if(distance.sqrMagnitude < moveDistance * moveDistance) {
                rb.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack() {
        attack = true;
        if(transform.position.x < player.transform.position.x) {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        while(distance.sqrMagnitude > attackDistance * attackDistance) {
            yield return null;
        }
        rb.AddForce(Vector2.down * attackForce, ForceMode2D.Impulse);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(player.GetComponent<PlayerController>().IsAttacking()) {
                Instantiate(death, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
