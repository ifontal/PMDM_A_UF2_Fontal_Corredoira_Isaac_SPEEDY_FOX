using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{
    [SerializeField] float speed, maxPosition;
    [SerializeField] Text header, msg, score, bottom;
    [SerializeField] GameController gameController;
    Rigidbody2D rb;
    Animator anim;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("Running", true);
        score.text = "Your Score: " + gameController.GetScore();
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
        if(Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadScene(0);
        }
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(speed * Time.fixedDeltaTime, rb.velocity.y);
    }

    private void OnGUI() {
        msg.text = gameController.GetMessage();
    }

    IEnumerator Decoration() {
        float t;
        float duration = 3;
        while(true) {
            t = 0;
            while (t < duration) {
                t += Time.deltaTime;
                header.color = Color.Lerp(Color.blue, Color.magenta, t / duration);
                score.color = Color.Lerp(Color.green, Color.red, t / duration);
                bottom.color = Color.Lerp(Color.red, Color.cyan, t / duration);
                yield return null;
            }
            t = 0;
            while (t < duration) {
                t += Time.deltaTime;
                header.color = Color.Lerp(Color.magenta, Color.blue, t / duration);
                score.color = Color.Lerp(Color.red, Color.green, t / duration);
                bottom.color = Color.Lerp(Color.cyan, Color.red, t / duration);
                yield return null;
            }
        }
    }

    IEnumerator Reload() {
        yield return new WaitForSeconds(Camera.main.GetComponent<AudioSource>().clip.length);
        SceneManager.LoadScene(0);
    }
}
