using UnityEngine;

public class CameraSnap : MonoBehaviour
{
    public Transform player;
    public float screenHeight = 10f;
    public float snapSpeed = 10f;
    public float snapBuffer = 2f;

    private float currentScreenY;

    void Start()
    {
        
        currentScreenY = Mathf.Round(player.position.y / screenHeight) * screenHeight;
    }

    void Update()
    {
        float playerY = player.position.y;

        float screenBottom = currentScreenY;
        float screenTop = currentScreenY + screenHeight;

        
        if (playerY > screenTop + snapBuffer)
        {
            currentScreenY += screenHeight;
        }

        
        if (playerY < screenBottom - snapBuffer)
        {
            currentScreenY -= screenHeight;
        }

        Vector3 targetPos = new Vector3(transform.position.x, currentScreenY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, snapSpeed * Time.deltaTime);
    }
}