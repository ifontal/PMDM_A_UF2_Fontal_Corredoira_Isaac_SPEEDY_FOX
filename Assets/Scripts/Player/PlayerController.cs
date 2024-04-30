using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Cinemachine;

public class PlayerController : MonoBehaviour {

    [SerializeField] float speed, invTime, blinkNum, jumpForce;
	[SerializeField] AudioClip jumpSound, attackSound, hurtSound, deathSound;
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
	CinemachineFramingTransposer cameraOffset;

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
		cameraOffset = GameObject.Find("FollowCamera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
	}
	
	void FixedUpdate() {
		if(!dead) {
			Run();
			Jump();
		}
	}

	void Update() {
		if(!gameController.IsPaused() && !dead){
			moveX = Input.GetAxisRaw("Horizontal");
			if(!jumping && Input.GetButtonDown("Jump") && IsOnFloor()) {
				jumping = true;
			}
			if(Input.GetButtonDown("Fire1") && !attacking) {
				Attack();
			}
			if(hurting) {
				if(!invulnerable && !dead) {
					Hurt();
				}
			}
			if(gameController.GetTime() >= timeToDead) {
				Dead();
			}
		}
	}

	bool IsOnFloor() {
		RaycastHit2D rc = Physics2D.BoxCast(
			new Vector2(col.bounds.center.x,col.bounds.min.y),
			new Vector2(col.bounds.size.x*0.95f,0.1f),
			0f,
			Vector2.down,
			0.1f,
			LayerMask.GetMask("Terrain", "Platforms"));
		return rc.collider != null;
	}

	bool IsInSlope() {
		RaycastHit2D rc = Physics2D.BoxCast(
			new Vector2(col.bounds.center.x,col.bounds.min.y),
			new Vector2(col.bounds.size.x*0.95f,0.1f),
			0f,
			Vector2.down,
			0.1f,
			LayerMask.GetMask("Terrain", "Platforms"));
		return (Mathf.Abs(Vector2.Angle(rc.normal, Vector2.up)) > 5);
	}

	void Run() {
		rb.velocity = new Vector2(moveX * speed * Time.fixedDeltaTime, rb.velocity.y);
		
		if(Mathf.Abs(rb.velocity.x) > Mathf.Epsilon) {
			transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
			rb.gravityScale = g;
		} else if(IsInSlope()) {
			rb.gravityScale = 0.5f; //Para no caer en las pendientes
		} else {
			rb.gravityScale = g;
		}

		if(Mathf.Abs(rb.velocity.x) > Mathf.Epsilon) {
			anim.SetBool("Running", true);
			anim.SetBool("Crouch", false);
			cameraOffset.m_TrackedObjectOffset.y = 1;
		} else {
			anim.SetBool("Running", false);
			if(Input.GetAxisRaw("Vertical") == -1) {
				anim.SetBool("Crouch", true);
				cameraOffset.m_TrackedObjectOffset.y = -2;
			} else {
				anim.SetBool("Crouch", false);
				cameraOffset.m_TrackedObjectOffset.y = 1;
			}
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

	void Attack() {
		attacking = true;
		AudioSource.PlayClipAtPoint(attackSound, Camera.main.transform.position);
		anim.SetTrigger("Attack");
	}

	public bool IsAttacking() {
		return attacking;
	}

	public void StopAttack(float nothing) {
		attacking = false;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Traps")) {
			hurting = true;
		} else if(other.CompareTag("Enemy") && !attacking) {
			hurting = true;
		} else if(other.CompareTag("Boss")) {
			hurting = true;
		} else if(other.CompareTag("Gems")) {
			gameController.UpdateGems(1);
		} else if(other.gameObject.CompareTag("RedGems")) {
			gameController.UpdateGems(10);
		} else if(other.CompareTag("Cherries")) {
			gameController.UpdateLifes(1);
		} else if(other.CompareTag("Continue")) {
			gameController.AddContinues();
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if(other.CompareTag("Enemy") || other.CompareTag("Traps")) {
			hurting = false;
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
		dead = true;
		Camera.main.GetComponent<AudioSource>().Stop();
		AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
		rb.velocity = Vector2.zero;
		anim.SetBool("Dead", true);
		gameController.UpdateLifes(-1);
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
