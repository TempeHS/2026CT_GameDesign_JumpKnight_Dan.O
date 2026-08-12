using UnityEngine;

public class CameraSnap : MonoBehaviour
{
    public Transform player;
    public float screenHeight = 13f;   // MUST match camera height (6.5 * 2)
    public float snapSpeed = 10f;
    public float snapBuffer = 2f;

    private float currentScreenY;

    void Start()
    {
        
        currentScreenY = Mathf.Round(player.position.y / screenHeight) * screenHeight;

        
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

        
        if (playerY > screenTop + snapBuffer)
        {
            currentScreenY += screenHeight;
        }

        
        float targetY = currentScreenY;

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(transform.position.x, targetY, transform.position.z),
            snapSpeed * Time.deltaTime
        );
    }
}