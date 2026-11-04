using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed = 5.0f;

    void Update()
    {
        if (transform.position.y > 4.3f)
        {
            Destroy(gameObject);
        }
    }
    
    void FixedUpdate()
    {
        Vector3 movement = new Vector2(0.0f, speed * Time.deltaTime);
        transform.position += movement;
    }
}
