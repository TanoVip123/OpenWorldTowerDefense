using UnityEngine;

public class InfiniteGridRenderer : MonoBehaviour
{
    private void LateUpdate() => transform.position = new Vector3(
             Camera.main.transform.position.x,
             Camera.main.transform.position.y,
            0);
}
