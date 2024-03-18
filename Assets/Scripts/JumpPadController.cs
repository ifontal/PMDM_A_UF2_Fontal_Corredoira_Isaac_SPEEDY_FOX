using UnityEngine;

public class JumpPadController : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] AudioClip sound;

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")) {
            if(Vector3.Angle(other.contacts[0].normal, Vector2.down) < 10) {
                AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
                other.rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            }
        }
    }
}
