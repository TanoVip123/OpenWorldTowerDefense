using System.Collections.Generic;
using UnityEngine;

public readonly struct PlaceBuildingCommand : ICommand
{
    public BuildingId BuildingID { get; }
    public Vector2 Position { get; }

    public PlaceBuildingCommand(BuildingId buildingID, Vector2 position)
    {
        BuildingID = buildingID;
        Position = position;
    }
}
