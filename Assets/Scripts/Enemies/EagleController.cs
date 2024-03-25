using System.Collections;
using UnityEngine;

public class EagleController : MonoBehaviour
{
    const int POINTS = 250;
    [SerializeField] float moveDistance, attackDistance, speed, attackForce;
    [SerializeField] GameObject death;
    Rigidbody2D rb;
    Animator anim;
    GameObject player;
    Vector2 distance;
    GameController gc;
    bool attack = false;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        gc = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    void Update() {
        distance = transform.position - player.transform.position;
        if(!attack) {
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
        while(distance.magnitude > attackDistance) {
            yield return null;
        }
        rb.AddForce(-distance/distance.magnitude * attackForce, ForceMode2D.Impulse);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
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
