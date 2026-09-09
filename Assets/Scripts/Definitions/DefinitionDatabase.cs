
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefinitionDatabase", menuName = "Scriptable Objects/DefinitionDatabase")]
public class DefinitionDatabase : ScriptableObject
{
    public BuildingDefinition[] BuildingDefinitions;

    private Dictionary<BuildingId, BuildingDefinition> _buildingDefinitionLookup;

    public void Initialize()
    {
        _buildingDefinitionLookup = new Dictionary<BuildingId, BuildingDefinition>();
        foreach (BuildingDefinition buildingDefinition in BuildingDefinitions)
        {
            _buildingDefinitionLookup.Add(buildingDefinition.Id, buildingDefinition);
        }
    }

    public BuildingDefinition GetBuildingDefinition(BuildingId id) => _buildingDefinitionLookup[id];
}
