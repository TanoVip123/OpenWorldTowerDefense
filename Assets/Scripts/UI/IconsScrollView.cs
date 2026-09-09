using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconsScrollView : MonoBehaviour
{
    public Transform sidePanelParent;   // ScrollView Content
    public GameObject iconPrefab;       // Prefab with Image + Button
    private BuildingDefinition[] buildingDefinitions; // Array of building definitions to create icons for

    public void Start()
    {
        buildingDefinitions = GameBootstrap.DefinitionDatabase.BuildingDefinitions;
        PopulateBuildingSidePanel();
    }

    private void PopulateBuildingSidePanel()
    {
        foreach (BuildingDefinition buildingDef in buildingDefinitions)
        {
            GameObject newIcon = Instantiate(iconPrefab, sidePanelParent);

            // Set icon image
            newIcon.GetComponent<IconButton>().iconImage.sprite = buildingDef.BuildingIcon;

            // Add click event
            newIcon.GetComponent<IconButton>().button.onClick.AddListener(() => BuildingIconClicked(buildingDef));
        }
    }

    public void BuildingIconClicked(BuildingDefinition buildingDef)
    {
        Debug.Log("BuildingIconClicked: " + buildingDef.Id.Value);
        GameBootstrap.World.Commands.AddCommand(new SelectBuildingToBuildCommand(buildingDef.Id));
    }
}
