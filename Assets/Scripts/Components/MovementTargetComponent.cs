using UnityEngine;

public struct MovementTargetComponent : IComponent
{
    public Vector3 TargetPosition { get; set; } // Position the entity is moving towards
    public int Version; // Version number to track updates to the target position

    public MovementTargetComponent(Vector3 targetPosition)
    {
        TargetPosition = targetPosition;
        Version = 0; // Initialize version to 0 when the component is created
    }
}
