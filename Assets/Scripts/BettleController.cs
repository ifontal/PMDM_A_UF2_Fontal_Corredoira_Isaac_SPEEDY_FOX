using System.Collections;
using UnityEngine;

public class BettleController : MonoBehaviour
{
    [SerializeField] float attackDistance, attackForce;
    [SerializeField] GameObject death;
    Rigidbody2D rb;
    GameObject player;
    Vector2 distance;
    bool attack = false;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
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
        if(transform.position.x < player.transform.position.x) {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        rb.AddForce(Vector2.left * attackForce, ForceMode2D.Impulse);
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
