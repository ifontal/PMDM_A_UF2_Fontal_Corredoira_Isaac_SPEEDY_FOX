using System.Collections;
using UnityEngine;

public class BettleController : MonoBehaviour
{
    const int POINTS = 100;
    [SerializeField] float attackDistance, attackForce;
    [SerializeField] GameObject death;
    Rigidbody2D rb;
    GameObject player;
    Vector2 distance;
    GameController gc;
    bool attack = false;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
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
                gc.UpdateScore(POINTS);
                Instantiate(death, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
