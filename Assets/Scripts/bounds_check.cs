using UnityEditor;
using UnityEngine;

public class bounds_check
{
    //Fixed player area boundaries
    public static float left_x_edge = -4.25f;
    public static float right_x_edge = 4.25f;
    public static float top_y_edge = 4.428f;
    public static float bottom_y_edge = -4.428f;

    //Used to delete objects when offscreen - enemies, items, bullets
    static public bool check_boundaries(Vector2 pos, float offset)
    {
        return !(pos.x < left_x_edge - offset || pos.x > right_x_edge + offset ||
                 pos.y > top_y_edge + offset || pos.y < bottom_y_edge - offset);
    }

    //Used to correct player position to screen bounds
    static public Vector3 fix_to_boundaries(Vector3 pos)
    {
        return new Vector3(Mathf.Clamp(pos.x, left_x_edge, right_x_edge), Mathf.Clamp(pos.y, bottom_y_edge,  top_y_edge), pos.z);
    }
}
