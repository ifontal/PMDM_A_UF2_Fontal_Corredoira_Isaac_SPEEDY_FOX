using System.Collections;
using UnityEngine;

public class ItemRemainsController : MonoBehaviour
{
    [SerializeField] int blinkNum;
    [SerializeField] float lifeTime, force;
    [SerializeField] GameObject feed;
    float x;
    Rigidbody2D rb;
    private void Start() {
        x = Random.Range(-1f,1f);
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(x,5)*force, ForceMode2D.Impulse);
        StartCoroutine(Expire());
        Invoke("EnableCol", 1);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            Instantiate(feed, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    IEnumerator Expire() {
		float t = 0, t2 = 0;
		Material mat = GetComponent<SpriteRenderer>().material; 
		Color color = mat.color;
		while (t < lifeTime) {
			t += Time.deltaTime;
			t2 += Time.deltaTime;
			float newAlpha = blinkNum * (t2/lifeTime);
			if (newAlpha > 1) {
				t2 = 0;
			}
			color.a = newAlpha;
			mat.color = color;
			yield return null;
		}
        Destroy(gameObject);
	}

    void EnableCol() {
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
