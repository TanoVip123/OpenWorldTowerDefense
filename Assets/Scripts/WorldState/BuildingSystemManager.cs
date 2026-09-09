using UnityEngine;

public class BuildingSystemManager : MonoBehaviour
{
    [SerializeField] private GameObject gridShadowObject;
    [SerializeField] private GameObject gridRendererObject;

    private GameObject _gridShadowInstance;
    private GameObject _gridRendererInstance;
    public BuildingId? BuildingId;
    private void Awake()
    {
        BuildingId = null;
        InfiniteGridRenderer infiniteGridRenderer = FindFirstObjectByType<InfiniteGridRenderer>();
        GridShadow gridShadow = FindFirstObjectByType<GridShadow>();

        if (infiniteGridRenderer)
        {
            _gridRendererInstance = infiniteGridRenderer.gameObject;
        }
        else
        {
            _gridRendererInstance = Instantiate(gridRendererObject, Vector3.zero, Quaternion.identity);
        }

        if (gridShadow)
        {
            _gridShadowInstance = gridShadow.gameObject;
        }
        else
        {
            _gridShadowInstance = Instantiate(gridShadowObject, Vector3.zero, Quaternion.identity);
        }

        _gridShadowInstance.SetActive(false);
        _gridRendererInstance.SetActive(false);
    }

    public void SetBuildingId(BuildingId buildingId)
    {
        BuildingId = buildingId;
        BuildingDefinition buildingDefinition = GameBootstrap.DefinitionDatabase.GetBuildingDefinition(buildingId);
        _gridShadowInstance.SetActive(true);
        _gridShadowInstance.GetComponent<GridShadow>().setShadowObject(buildingDefinition.BuildingShadow, buildingDefinition.GridOccupancy);
        _gridRendererInstance.SetActive(true);
    }

    public Vector3 GetBuildingPlacementWorldPosition() => _gridShadowInstance.transform.position;

    public void UnsetBuildingId()
    {
        BuildingId = null;
        _gridShadowInstance.GetComponent<GridShadow>().ClearShadowObject();
        _gridShadowInstance.SetActive(false);
        _gridRendererInstance.SetActive(false);
    }
}
