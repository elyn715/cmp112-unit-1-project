using UnityEditor;
using UnityEngine;

public class bounds_check
{
    public static float left_x_edge = -4.25f;
    public static float right_x_edge = 4.25f;
    public static float top_y_edge = 4.428f;
    public static float bottom_y_edge = -4.428f;

    static public bool check_boundaries(Vector2 pos, float offset)
    {
        return !(pos.x < left_x_edge - offset || pos.x > right_x_edge + offset ||
                 pos.y > top_y_edge + offset || pos.y < bottom_y_edge - offset);
    }

    static public Vector3 fix_to_boundaries(Vector3 pos)
    {
        Vector3 boundary_fixed_pos = pos;
        
        if(pos.x < left_x_edge) boundary_fixed_pos.x = left_x_edge;
        else if(pos.x > right_x_edge) boundary_fixed_pos.x = right_x_edge;
        
        if(pos.y > top_y_edge) boundary_fixed_pos.y = top_y_edge;
        else if(pos.y < bottom_y_edge) boundary_fixed_pos.y = bottom_y_edge;
        
        return boundary_fixed_pos;
    }
}
