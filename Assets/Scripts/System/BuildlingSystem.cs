using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingSystem : IUpdatableSystem, IGameSystem
{
    private World _world;
    public void Initialize(World world)
    {
        _world = world;
        _world.AddSystem(this);
        Debug.Log("BuildingSystem initialized");
    }

    public void Shutdown() => _world.highlightEntities.Clear();

    public void Update(float deltaTime)
    {
        if (_world.Commands.GetCommands<SelectBuildingToBuildCommand>(out List<SelectBuildingToBuildCommand> selectBuildingCommands))
        {
            foreach (SelectBuildingToBuildCommand command in selectBuildingCommands)
            {
                Debug.Log($"BuildingSystem received SelectBuildingToBuildCommand for BuildingID: {command.BuildingID}");
                GameBootstrap.BuildingSystemManager.SetBuildingId(command.BuildingID);
                GameBootstrap.PlayerInputHandler.CurrentInputMode = PlayerInputHandler.InputMode.BuildingPlacement;
            }
        }

        if (_world.Commands.GetCommands<CancelBuildingToBuildCommand>(out List<CancelBuildingToBuildCommand> _))
        {
            GameBootstrap.PlayerInputHandler.CurrentInputMode = PlayerInputHandler.InputMode.BuildingPlacement;
            Debug.Log($"BuildingSystem received CancelBuildingToBuildCommand ");
            GameBootstrap.BuildingSystemManager.UnsetBuildingId();
            GameBootstrap.PlayerInputHandler.CurrentInputMode = PlayerInputHandler.InputMode.None;
        }

        if (_world.Commands.GetCommands<PlaceBuildingCommand>(out List<PlaceBuildingCommand> placeBuildingCommands))
        {
            foreach (PlaceBuildingCommand command in placeBuildingCommands)
            {
                Debug.Log($"BuildingSystem received PlaceBuildingCommand for BuildingID: {command.BuildingID} at Position: {command.Position}");
                BuildingDefinition buildingDefinition = GameBootstrap.DefinitionDatabase.GetBuildingDefinition(command.BuildingID);
                // Raise if building prefab does not have BuildingComponentAuthor
                if (!buildingDefinition.BuildingPrefab.TryGetComponent<BuildingComponentAuthor>(out BuildingComponentAuthor buildingAuthor))
                {
                    Debug.LogError($"BuildingPrefab for BuildingID: {command.BuildingID} does not have a BuildingComponentAuthor component.");
                    return;
                }

                if (buildingAuthor.buildingId != command.BuildingID)
                {
                    Debug.LogError($"BuildingPrefab's BuildingComponentAuthor buildingId ({buildingAuthor.buildingId}) does not match the command's BuildingID ({command.BuildingID.Value}).");
                    return;
                }
                Vector2 gridPosition = GridUtils.WorldToGrid(command.Position);
                Vector2 worldPositionFromGrid = GridUtils.GridToWorld(gridPosition);
                Debug.Log($"BuildingSystem received Final placement PlaceBuildingCommand for BuildingID: {command.BuildingID} at Position: {worldPositionFromGrid}");
                GameObject.Instantiate(buildingDefinition.BuildingPrefab, new Vector3(worldPositionFromGrid.x, worldPositionFromGrid.y, 0), Quaternion.identity);
            }
        }
    }
}

