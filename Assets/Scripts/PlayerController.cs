using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    [SerializeField] float speed, invTime, blinkNum, jumpForce;
	[SerializeField] AudioClip jumpSound, attackSound, hurtSound, deathSound, splashSound;
	[SerializeField] GameObject gem;
	[SerializeField] GameController gameController;
	bool jumping, dead, attacking, invulnerable, hurting;
    Rigidbody2D rb;
	Collider2D col;
	SpriteRenderer sp;
	Animator anim;
	float moveX;
	float g;
	float timeToDead;

	void Start() {
        rb = GetComponent<Rigidbody2D>();
		col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
		sp = GetComponent<SpriteRenderer>();
		gameController.ResetClock();
		gameController.ResetGems();
		jumping = false;
		dead = false;
		attacking = false;
		invulnerable = false;
		hurting = false;
		g = rb.gravityScale;
		timeToDead = GameController.TIME_TO_DEAD;
	}
	
	void FixedUpdate() {
		if(dead == false) {
			Run();
			Jump();
		}
	}

	void Update() {
		if(!gameController.IsPaused()){
			moveX = Input.GetAxisRaw("Horizontal");
			if(!jumping && Input.GetButtonDown("Jump") && IsOnFloor()) {
				jumping = true;
			}
			if(Input.GetButtonDown("Fire1")) {
				StartCoroutine(Attack());
			}
			if(hurting) {
				if(!invulnerable && !dead) {
					Hurt();
				}
			}
			if(gameController.GetTime() >= timeToDead && !dead) {
				Dead();
			}
		}
	}

	bool IsOnFloor() {
		RaycastHit2D rc = Physics2D.BoxCast(
			col.bounds.center + Vector3.down,
			col.bounds.size / 2,
			0f,
			Vector2.down,
			0.01f,
			LayerMask.GetMask("Terrain", "Platforms"));
		return rc.collider != null; 
	}

	void Run() {
		rb.velocity = new Vector2(moveX * speed * Time.fixedDeltaTime, rb.velocity.y);
		
		if(rb.velocity.x < 0) {
			sp.flipX = true;
			rb.gravityScale = g;
		} else if(rb.velocity.x > 0) {
			sp.flipX = false;
			rb.gravityScale = g;
		} else if(rb.velocity.x == 0 && IsOnFloor()) {
			rb.gravityScale = 0.01f; //Para no caer en las pendientes
		} else {
			rb.gravityScale = g;
		}

		if(rb.velocity.x != 0) {
			anim.SetBool("Running", true);
		} else {
			anim.SetBool("Running", false);
		}
	}

	void Jump() {
		if(jumping) {
			AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position);
			rb.velocity = new Vector2(rb.velocity.x, 0); //Evita doble salto al pulsar salto justo al entrar en plataformas
			rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			anim.ResetTrigger("Ground");
			anim.SetTrigger("Up");
			jumping = false;
		} else

		if(!attacking) {
			if(IsOnFloor()) {
				anim.SetTrigger("Ground");
				anim.ResetTrigger("Up");
				anim.ResetTrigger("Down");
			} else if(rb.velocity.y < 0) {
				anim.SetTrigger("Down");
			} else if(rb.velocity.y > 0) {
				anim.SetTrigger("Up");
			}
		}
	}

	IEnumerator Attack() {
		attacking = true;
		AudioSource.PlayClipAtPoint(attackSound, Camera.main.transform.position);
		anim.SetTrigger("Attack");
		yield return new WaitForSeconds(1);
		attacking = false;
	}

	public bool IsAttacking() {
		return attacking;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Traps")) {
			hurting = true;
		} else if(other.CompareTag("Enemy") && !attacking) {
			hurting = true;
		} else if(other.CompareTag("Gems")) {
			gameController.UpdateGems(1);
		} else if(other.CompareTag("Cherries")) {
			gameController.UpdateLifes(1);
		} else if(other.CompareTag("Water")) {
			AudioSource.PlayClipAtPoint(splashSound, Camera.main.transform.position);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if(other.CompareTag("Enemy") || other.CompareTag("Traps")) {
			hurting = false;
		} else if(other.CompareTag("Water")) {
			AudioSource.PlayClipAtPoint(splashSound, Camera.main.transform.position);
		}
	}							

	void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.CompareTag("Gems")) {
			gameController.UpdateGems(1);
		} else if(other.gameObject.CompareTag("Finish")) {
			if(GameObject.Find("FollowCamera")) {
				GameObject.Find("FollowCamera").SetActive(false);
			}
		}
	}

	void Hurt() {
		if(gameController.GetGemsCount() == 0) {
			Dead();
		} else {
			AudioSource.PlayClipAtPoint(hurtSound, Camera.main.transform.position);
			anim.SetTrigger("Damage");
			for(int i = 0; i < gameController.GetGemsCount()/10; i++) {
				Instantiate(gem, transform.position, Quaternion.identity);
			}
			gameController.ResetGems();
			StartCoroutine(Invulnerability());
		}
	}

	void Dead() {
		Camera.main.GetComponent<AudioSource>().Stop();
		AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
		rb.velocity = Vector2.zero;
		anim.SetTrigger("Dead");
		gameController.UpdateLifes(-1);
		dead = true;
		if(gameController.GetLifesCount() == 0) {
			gameController.GameOver();
		} else {
			Invoke("ReloadScene", 6);
		}
	}

	void ReloadScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	
	IEnumerator Invulnerability() {
		invulnerable = true;
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
		invulnerable = false;
	}
}
