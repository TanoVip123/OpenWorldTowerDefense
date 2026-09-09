using System.Collections.Generic;
using UnityEngine;

public readonly struct SelectBuildingToBuildCommand : ICommand
{
    public BuildingId BuildingID { get; }

    public SelectBuildingToBuildCommand(BuildingId buildingID) => BuildingID = buildingID;
}
