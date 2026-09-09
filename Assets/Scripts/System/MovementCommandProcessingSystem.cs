using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementCommandProcessingSystem : IGameSystem, IUpdatableSystem
{
    private World _world;
    public void Initialize(World world)
    {
        _world = world;
        _world.AddSystem(this);
        Debug.Log("MovementCommandProcessingSystem initialized");
    }

    public void Shutdown() => Debug.Log("MovementCommandProcessingSystem shutdown");

    public void Update(float deltaTime)
    {
        if (_world.Commands.GetCommands(out List<MoveCommand> movementCommands) && movementCommands.Count > 0)
        {
            Debug.Log($"MovementCommandProcessingSystem received {movementCommands.Count} MoveCommand(s)");
            foreach (MoveCommand command in movementCommands)
            {
                foreach (EntityID entityId in command.TargetEntityIDs)
                {
                    if (_world.TryGetComponentFromEntity(entityId, out MovementComponent _))
                    {
                        // Add or update the MovementTargetComponent for the entity with the target position from the command
                        if (!_world.TryGetComponentFromEntity(entityId, out MovementTargetComponent _))
                        {
                            _world.AddComponentToEntity(entityId, new MovementTargetComponent(command.TargetPosition));
                        }
                        else
                        {
                            ref MovementTargetComponent movementTargetComponentRef = ref _world.GetComponentFromEntity<MovementTargetComponent>(entityId);
                            movementTargetComponentRef.TargetPosition = command.TargetPosition;
                            movementTargetComponentRef.Version++; // Increment version to indicate an update
                        }
                    }
                }
            }
        }
    }
}
