using System.Collections;
using UnityEngine;

public class BatController : MonoBehaviour
{
    const int POINTS = 200;
    [SerializeField] float attackDistance, downForce, attackForce, offset;
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
        if(!attack) {
            distance = transform.position - player.transform.position;
            if(distance.sqrMagnitude < attackDistance * attackDistance) {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack() {
        attack = true;
        if(transform.position.x > player.transform.position.x) {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        rb.AddForce(Vector2.down * downForce, ForceMode2D.Impulse);
        while(transform.position.y > player.transform.position.y + offset) {
            yield return null;
        }
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.left * attackForce, ForceMode2D.Impulse);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(5);
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
