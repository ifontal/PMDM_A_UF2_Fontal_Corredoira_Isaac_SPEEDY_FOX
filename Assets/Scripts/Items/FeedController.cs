using UnityEngine;

public class FeedController : MonoBehaviour
{
    private const float DELAY = .20f;
    void Start() {
        Destroy(gameObject, DELAY);
    }
}
