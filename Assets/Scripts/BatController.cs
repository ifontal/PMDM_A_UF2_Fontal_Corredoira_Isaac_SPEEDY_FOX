using System.Collections;
using UnityEngine;

public class BatController : MonoBehaviour
{
    [SerializeField] float attackDistance, downForce, attackForce, offset;
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
                Instantiate(death, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
