using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathController : MonoBehaviour
{
    private const float DELAY = .20f;
    [SerializeField] AudioClip deathSound;

    void Start() {
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
        Destroy(gameObject, DELAY);
    }
}
