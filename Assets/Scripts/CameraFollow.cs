using UnityEngine;

public class CameraSnap : MonoBehaviour
{
    public Transform player;
    public float screenHeight = 10f; 
    public float snapSpeed = 10f;    

    private float currentScreenY;

    void Start()
    {
        currentScreenY = Mathf.Round(player.position.y / screenHeight) * screenHeight;
    }

    void Update()
    {
        float playerScreenY = Mathf.Round(player.position.y / screenHeight) * screenHeight;
        
        
        
        if (playerScreenY != currentScreenY)
        {
            currentScreenY = playerScreenY;
        }

        
        Vector3 targetPos = new Vector3(transform.position.x, currentScreenY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, snapSpeed * Time.deltaTime);
    }
}
