using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] float speed, maxPosition;
    [SerializeField] Text msg;
    [SerializeField] GameController gameController;
    Rigidbody2D rb;
    Animator anim;

    private void Start() {
        Cursor.visible = false;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("Running", true);
        msg.text = "PRESS ENTER TO START GAME";
        gameController.NewGame();
        StartCoroutine(Decoration());
    }

    void Update() {
        if(transform.position.x > maxPosition) {
            transform.position = new Vector2(0,0);
        }
        if(Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadScene(1);
        }
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(speed * Time.fixedDeltaTime, rb.velocity.y);
    }

    IEnumerator Decoration() {
        float t;
        float duration = 3;
        while(true) {
            t = 0;
            while (t < duration) {
                t += Time.deltaTime;
                msg.color = Color.Lerp(Color.green, Color.red, t / duration);
                yield return null;
            }
            t = 0;
            while (t < duration) {
                t += Time.deltaTime;
                msg.color = Color.Lerp(Color.red, Color.green, t / duration);
                yield return null;
            }
        }
    }
}
