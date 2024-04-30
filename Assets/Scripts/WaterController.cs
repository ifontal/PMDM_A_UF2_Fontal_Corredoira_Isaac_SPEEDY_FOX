using UnityEngine;

public class WaterController : MonoBehaviour
{
  [SerializeField] AudioClip waterInSound, waterOutSound;

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player")) {
      AudioSource.PlayClipAtPoint(waterInSound, Camera.main.transform.position);
    }
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("Player")) {
      AudioSource.PlayClipAtPoint(waterOutSound, Camera.main.transform.position);
    }
  }
}
