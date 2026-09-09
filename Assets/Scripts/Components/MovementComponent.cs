using UnityEngine;

public struct MovementComponent : IComponent
{
    public float MaxSpeed { get; set; } // Maximum speed the entity can move at
    public float ArrivalRadius { get; set; } // How close should
    public Vector3 CurrSpeed { get; set; } // Current speed of the entity, can

    public bool ShouldStopNextFrame { get; set; }

    public MovementComponent(float maxSpeed, float arrivalRadius)
    {
        MaxSpeed = maxSpeed;
        ArrivalRadius = arrivalRadius;
        CurrSpeed = Vector3.zero; // Initialize current speed to 0
        ShouldStopNextFrame = false; // This is to avoid overshooting
    }
}
