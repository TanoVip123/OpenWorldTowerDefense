using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshUtils
{
    public static List<Vector2> ConvexHull(List<Vector2> points)
    {
        // Remove duplicates
        points = points.Distinct().ToList();
        Debug.Log("MeshUtils: Computing Convex Hull for points: " + string.Join(", ", points));
        if (points.Count <= 1)
        {
            return new List<Vector2>(points);
        }

        // Sort by X then Y
        points.Sort((a, b) =>
        {
            int cmp = a.x.CompareTo(b.x);
            return cmp != 0 ? cmp : a.y.CompareTo(b.y);
        });

        List<Vector2> lower = new();
        foreach (Vector2 p in points)
        {
            while (lower.Count >= 2 && Cross(lower[^2], lower[^1], p) <= 0)
            {
                lower.RemoveAt(lower.Count - 1);
            }

            lower.Add(p);
        }

        List<Vector2> upper = new();
        for (int i = points.Count - 1; i >= 0; i--)
        {
            Vector2 p = points[i];
            while (upper.Count >= 2 && Cross(upper[^2], upper[^1], p) <= 0)
            {
                upper.RemoveAt(upper.Count - 1);
            }

            upper.Add(p);
        }

        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);

        lower.AddRange(upper);
        return lower;
    }

    private static float Cross(Vector2 a, Vector2 b, Vector2 c) => ((b.x - a.x) * (c.y - a.y)) - ((b.y - a.y) * (c.x - a.x));
}
