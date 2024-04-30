using System.Collections;
using UnityEngine;


public class BossController : MonoBehaviour
{
    [SerializeField] float speed, moveDistance, attackDistance, attackImpulse, invTime, blinkNum;
    [SerializeField] int attackTimes, hitsToDead;
    [SerializeField] GameObject bigExplosion, smallExplosion;
    [SerializeField] AudioClip swingAttackSound, stompAttackSound, poundAttackSound, snortSound, deathSound;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    GameObject player;
    int attackTime, hits;
    bool staggered, isAttacking, hurted, dead;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
        attackTime = 0;
        hits = 0;
        staggered = false;
        isAttacking = false;
        hurted = false;
        dead = false;
    }

    private void FixedUpdate() {
        if(!dead && !staggered) {
            Move();
            Attack();
        }
    }

    private bool PlayerIsNear() {
        Vector2 distance = transform.position - player.transform.position;
        if(distance.magnitude < moveDistance) {
            return true;
        } else {
            return false;
        }
    }

    private bool PlayerIsBeside() {
        Vector2 distance = transform.position - player.transform.position;
        if(distance.magnitude < attackDistance) {
            return true;
        } else {
            return false;
        }
    }

    private void Move() {
        if(isAttacking) {
            rb.velocity = Vector2.zero;
        } else if(PlayerIsNear()) {
            anim.SetBool("Moving", true);
            if(player.transform.position.x < transform.position.x) {
                rb.velocity = Vector2.left * speed;
                transform.localScale = new Vector2(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y);
            } else {
                rb.velocity = Vector2.right * speed;
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
        } else {
            anim.SetBool("Moving", false);
        }
    }

    private void Attack() {
        if(!isAttacking && PlayerIsBeside()) {
            if(attackTime < attackTimes) {
                isAttacking = true;
                attackTime++;
                switch(Random.Range(0,3)) {
                    case 0:
                    anim.SetTrigger("SwingAttack");
                    break;
                    case 1:
                    anim.SetTrigger("StompAttack");
                    break;
                    case 2:
                    anim.SetTrigger("PoundAttack");
                    break;
                }
                anim.SetBool("Moving", false);   
            }
        }
    }

    public void StopAttacks() {
        if (attackTime >= attackTimes) {
            StartCoroutine(Staggered());
        }
        isAttacking = false;
    }

    public void SwingAttackSound() {
        AudioSource.PlayClipAtPoint(swingAttackSound, Camera.main.transform.position);
    }

    public void StompAttackSound() {
        AudioSource.PlayClipAtPoint(stompAttackSound, Camera.main.transform.position);
    }

    public void PoundAttackSound() {
        AudioSource.PlayClipAtPoint(poundAttackSound, Camera.main.transform.position);
    }

    public void SnortSound() {
        AudioSource.PlayClipAtPoint(snortSound, Camera.main.transform.position);
    }

    public void StompAttack() {
        if(transform.localScale.x < 0) {
            transform.position = new Vector2(transform.position.x-attackImpulse, transform.position.y);
        } else {
            transform.position = new Vector2(transform.position.x+attackImpulse, transform.position.y);
        }
    }

    private IEnumerator Staggered() {
        staggered = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Stagger");
        yield return new WaitForSeconds(3);
        anim.SetTrigger("Restore");
        attackTime = 0;
        staggered = false;
    }

    private IEnumerator Hurted() {
        hurted = true;
        Instantiate(smallExplosion, GetComponent<BoxCollider2D>().bounds.center-Vector3.one, Quaternion.identity);
		float t = 0, t2 = 0;
		Material mat = GetComponent<SpriteRenderer>().material; 
		Color color = mat.color;
		while(t < invTime) {
			t += Time.deltaTime;
			t2 += Time.deltaTime;
			float newAlpha = blinkNum * (t2/invTime);
			if(newAlpha > 1) {
				t2 = 0;
			}
			color.a = newAlpha;
			mat.color = color;
			yield return null;
		}
		color.a = 1;
		mat.color = color;
		hurted = false;
    }

    private IEnumerator Dead() {
        dead = true;
        anim.SetTrigger("Dead");
        Instantiate(bigExplosion, GetComponent<Collider2D>().bounds.center, Quaternion.identity);
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(staggered && other.gameObject.CompareTag("Player") && player.GetComponent<PlayerController>().IsAttacking()) {
            if(!hurted) {
                hits++;
                StartCoroutine(Hurted());
                if(hits >= hitsToDead) {
                    StartCoroutine(Dead());
                }
            }
        }
    }
}
