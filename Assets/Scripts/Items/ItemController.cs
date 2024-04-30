using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] GameObject feed;
    [SerializeField] GameController gameController;
    
    private void Start() {
        if(gameObject.CompareTag("Cherries") && gameController.IsExtraLifeTaken()) {
            Destroy(gameObject);
        }    
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(gameObject.CompareTag("Cherries")) {
                gameController.ExtraLifeTaken();
            }
            Instantiate(feed, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
