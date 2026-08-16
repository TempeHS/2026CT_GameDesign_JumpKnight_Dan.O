using UnityEngine;

public class CameraSnap : MonoBehaviour
{
    public Transform player;
    public float screenHeight = 13f;   // camera height (6.5 * 2)
    public float snapSpeed = 10f;
    public float snapBuffer = 2f;

    private float currentScreenY;

    void Start()
    {
        // Find the nearest screen the player starts in
        currentScreenY = transform.position.y;

        // Snap camera to that screen immediately
        transform.position = new Vector3(
            transform.position.x,
            currentScreenY,
            transform.position.z
        );
    }

    void Update()
    {
        float playerY = player.position.y;

        float screenTop = currentScreenY + screenHeight;
        float screenBottom = currentScreenY;

        // SNAP UP
        if (playerY > screenTop + snapBuffer)
        {
            currentScreenY += screenHeight;
        }

        // SNAP DOWN
        if (playerY < screenBottom - snapBuffer)
        {
            currentScreenY -= screenHeight;
        }

        // Smooth movement toward the target screen
        float targetY = currentScreenY;

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(transform.position.x, targetY, transform.position.z),
            snapSpeed * Time.deltaTime
        );
    }
}