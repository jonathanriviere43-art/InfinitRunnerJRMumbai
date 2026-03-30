using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public PlayerMouvement playerMovement;

    [Header("Offsets")]
    public Vector3 groundOffset = new Vector3(0, 2, -5);
    public Vector3 flyingOffset = new Vector3(0, 6, -10);

    [Header("Rotation")]
    public Vector3 groundRotation = new Vector3(10f, 0f, 0f);
    public float flyingTiltAngle = 30f;

    [Header("Movement")]
    public float smoothSpeed = 5f;

    [Header("Blend Settings")]
    public float blendStartHeight = 0.5f;
    public float blendEndHeight = 2.5f;

    private float basePlayerY;

    void Start()
    {
        basePlayerY = player.position.y;
    }

    void LateUpdate()
    {
        if (player == null || playerMovement == null)
            return;

        float y = player.position.y;

        // 🔹 Blend sol → vol
        float t = Mathf.InverseLerp(blendStartHeight, blendEndHeight, y);

        // 🔹 Position cible selon mode
        Vector3 groundPos = player.position + groundOffset;
        Vector3 flyingPos = player.position + flyingOffset;

        Vector3 targetPosition = Vector3.Lerp(groundPos, flyingPos, t);

        // 🔹 Smooth position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // 🔹 Rotation :
        // Sol = rotation fixe
        Quaternion groundRot = Quaternion.Euler(groundRotation);

        // Vol = tilt vers le bas
        Quaternion flyingRot = Quaternion.Euler(flyingTiltAngle, 0f, 0f);

        // Blend rotation
        transform.rotation = Quaternion.Slerp(groundRot, flyingRot, t);
    }
}
