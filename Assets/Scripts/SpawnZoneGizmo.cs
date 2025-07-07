using UnityEngine;

public class SpawnZoneGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.green;
    public Vector3 size = Vector3.one;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.1f);
    }
}