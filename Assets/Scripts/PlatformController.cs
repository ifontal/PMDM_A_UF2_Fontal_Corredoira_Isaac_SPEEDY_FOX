using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] GameObject[] waypoints;
    [SerializeField] float speed;
    int i = 0;


    void Update() {
        Move();
    }

    void Move() {
        if (Vector2.Distance(transform.position, waypoints[i].transform.position) < 0.1f) {
            i++;
            if (i >= waypoints.Length) {
                i = 0;
            }
        }
        
        transform.position = Vector2.MoveTowards(transform.position, waypoints[i].transform.position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")) {
            other.gameObject.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")) {
            other.gameObject.transform.SetParent(null);
        }
    }
}
