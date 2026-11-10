using UnityEngine;

public class movement_and_bullet_patterns : MonoBehaviour
{
    public struct linear_v
    {
        public Vector2 stop_pos;
        public float wait_time;

        public linear_v(Vector2 stop, float wait)
        {
            stop_pos = stop;
            wait_time = wait;
        }
    }
}
