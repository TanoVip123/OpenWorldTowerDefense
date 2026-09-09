using System.Collections.Generic;
using UnityEngine;

public struct PathComponent : IComponent
{
    public Vector3[] PathPoints { get; set; } // Array of points representing the path to follow
    public int CurrentPathIndex { get; set; } // Index of the current target point in the path
    public int Version; // Version number to track updates to the path

    public bool ShouldAdvanceNextFrame { get; set; } // Similar to MovementComponent, we need this to avoid overshooting

    public PathComponent(Vector3[] pathPoints, int version)
    {
        PathPoints = pathPoints;
        CurrentPathIndex = 0; // Start at the first point in the path
        Version = version;
        ShouldAdvanceNextFrame = false;
    }
}
