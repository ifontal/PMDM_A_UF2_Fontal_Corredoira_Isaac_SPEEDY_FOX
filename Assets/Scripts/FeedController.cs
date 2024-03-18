using UnityEngine;

public class FeedController : MonoBehaviour
{
    [SerializeField] AudioClip sound;
    private float DELAY = .20f;
    void Start() {
        AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
        Destroy(gameObject, DELAY);
    }
}
