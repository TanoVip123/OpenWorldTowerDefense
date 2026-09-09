using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))] // Fixes UNT0039
public class PlayerInputHandler : MonoBehaviour
{
    // Explicitly use Unity's namespace to avoid the naming collision (Fixes CS1061 and UNT0014)
    private UnityEngine.InputSystem.PlayerInput _playerInput;
    private InputAction _moveAction;

    public enum InputMode
    {
        None,
        BuildingPlacement,
    }

    public InputMode CurrentInputMode { get; set; } = InputMode.None;

    private void Awake()
    {
        _playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
    }

    private void Update()
    {
        Vector2 totalPan = _moveAction.ReadValue<Vector2>();

        if (Keyboard.current != null && !Keyboard.current.altKey.isPressed && Camera.main != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            float edgeSize = 25f; // Fixed physical pixel size

            // TanoVip123 PR Fix: Removed the unnecessary screen bounds wrapper.
            // A simple threshold check is sufficient.

            if (mousePos.x < edgeSize)
            {
                totalPan.x -= 1f;
            }
            else if (mousePos.x > Screen.width - edgeSize)
            {
                totalPan.x += 1f;
            }

            if (mousePos.y < edgeSize)
            {
                totalPan.y -= 1f;
            }
            else if (mousePos.y > Screen.height - edgeSize)
            {
                totalPan.y += 1f;
            }
        }

        if (totalPan != Vector2.zero && GameBootstrap.World != null)
        {
            GameBootstrap.World.Commands.AddCommand(new CameraMoveCommand(totalPan));
        }
    }

    // Epic #5 Zoom Method
    public void Zoom(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<Vector2>().y;
        Debug.Log("Zoom input detected: " + scrollValue);
        if (scrollValue != 0 && GameBootstrap.World != null)
        {
            float normalizedScroll = Mathf.Clamp(scrollValue, -1f, 1f);
            GameBootstrap.World.Commands.AddCommand(new CameraZoomCommand(normalizedScroll));
        }
    }

    public void LeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // If it is a UI click, ignore and let UI handle input
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            switch (CurrentInputMode)
            {
                case InputMode.None:
                    HandleSelectionOrMovement();
                    break;
                case InputMode.BuildingPlacement:
                    HandleByBuildingPlacement();
                    break;
                default:
                    Debug.LogWarning("Unhandled input mode: " + CurrentInputMode);
                    break;
            }

        }
    }

    private void HandleByBuildingPlacement()
    {
        if (GameBootstrap.BuildingSystemManager.BuildingId == null)
        {
            Debug.LogError("BuildingId is null. Cannot place building.");
            return;
        }
        BuildingDefinition buildingDefinition = GameBootstrap.DefinitionDatabase.GetBuildingDefinition(GameBootstrap.BuildingSystemManager.BuildingId.Value);
        Vector3 placementPosition = GameBootstrap.BuildingSystemManager.GetBuildingPlacementWorldPosition(); // Assuming the BuildingSystemManager has a position for placement
        Vector2 gridPosition = GridUtils.WorldToGrid(placementPosition);
        List<Vector2Int> occupiedTiles = GridUtils.offsetGridPoints(buildingDefinition.GridOccupancy.ToList(), new Vector2Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.y)));
        if (GameBootstrap.World.GridMap.isOccupied(occupiedTiles))
        {
            Debug.LogWarning("Cannot place building. The area is occupied.");
            return;
        }
        else
        {
            GameBootstrap.World.Commands.AddCommand(new PlaceBuildingCommand(GameBootstrap.BuildingSystemManager.BuildingId.Value, placementPosition));
            Debug.Log($"PlaceBuildingCommand to place building at position: {gridPosition}");
            GameBootstrap.BuildingSystemManager.UnsetBuildingId(); // Clear the building ID after placing the building
            CurrentInputMode = InputMode.None; // Reset to None after placing the building
        }

    }

    private void HandleSelectionOrMovement()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit &&
            hit.collider.gameObject.TryGetComponent<EntityView>(out EntityView entityView) &&
            GameBootstrap.World.TryGetComponentFromEntity<SelectableComponent>(entityView.EntityID, out SelectableComponent _))
        {
            GameBootstrap.World.Commands.AddCommand(new SelectCommand(entityView.EntityID));
        }
        else
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 targetPosition = new(worldPosition.x, worldPosition.y, 0);
            GameBootstrap.World.Commands.AddCommand(new MoveCommand(GameBootstrap.World.selectedEntities, targetPosition));
            Debug.Log($"Added MoveCommand for selected entities to move to position: {targetPosition}");
        }
    }
    public void RightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (CurrentInputMode)
            {
                case InputMode.None:
                    GameBootstrap.World.Commands.AddCommand(new SelectCommand(null));
                    break;
                case InputMode.BuildingPlacement:
                    GameBootstrap.World.Commands.AddCommand(new CancelBuildingToBuildCommand());
                    break;
                default:
                    Debug.LogWarning("Unhandled input mode: " + CurrentInputMode);
                    break;
            }
        }
    }
}
