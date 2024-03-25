using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{
    [SerializeField] float speed, maxPosition;
    [SerializeField] Text msg;
    [SerializeField] GameController gameController;
    Rigidbody2D rb;
    Animator anim;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("Running", true);
        rb.velocity = new Vector2(speed * Time.fixedDeltaTime, rb.velocity.y);
        msg.text = "Congratulations!\nThanks for playing\n\nYour Score: " + gameController.GetScore() + "\n\n\nPress <ESC> to Quit";
        StartCoroutine(Decoration());
        StartCoroutine(Reload());
    }

    void Update() {
        if(transform.position.x > maxPosition) {
            transform.position = new Vector2(0,0);
        }
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
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

    IEnumerator Reload() {
        yield return new WaitForSeconds(Camera.main.GetComponent<AudioSource>().clip.length);
        SceneManager.LoadScene(0);
    }
}
