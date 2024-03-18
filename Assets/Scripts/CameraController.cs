using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static CinemachineConfiner2D cc;
    static Collider2D col;

    private void Start() {
        cc = GetComponent<CinemachineConfiner2D>();
        col = GameObject.Find("CameraConfiner").GetComponent<PolygonCollider2D>();
    }

    public static void ChangeConfiner() {
        cc.m_BoundingShape2D = col;
        cc.InvalidateCache();
    }
}
