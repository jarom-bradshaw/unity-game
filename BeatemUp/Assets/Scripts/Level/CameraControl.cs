using System.Collections.Generic;
using UnityEngine;

public enum RoomTransition{
    Active,
    Cleared
}
public class CameraControl : MonoBehaviour
{
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    private float cameraZ;
    private float cameraY;

    private int currRoomI = 0;

    [SerializeField] private Transform target;

    [SerializeField] private List<Vector2> roomLimits;

    public RoomTransition roomState = RoomTransition.Active;
    public bool clearRoom = false;

    void Start()
    {
        cameraZ = transform.position.z;
        cameraY = transform.position.y;
    }
    void FixedUpdate()
    {
        DebugClear();

        Vector3 targetPosition = target.position;
        targetPosition.y = cameraY;
        targetPosition.z = cameraZ;

        Vector3 cameraPosition = transform.position;
        cameraPosition.y = cameraY;
        cameraPosition.z = cameraZ;

        Vector3 smoothPosition = Vector3.SmoothDamp(cameraPosition, targetPosition, ref velocity, smoothTime);

        int maxRoomI = roomState switch {
            RoomTransition.Cleared => currRoomI + 1,
            _ => currRoomI
        };

        var boundMax = maxRoomI >= roomLimits.Count ? roomLimits[currRoomI].y : roomLimits[maxRoomI].y;

        smoothPosition.x = Mathf.Clamp(smoothPosition.x, roomLimits[currRoomI].x, boundMax);
        transform.position = smoothPosition;

        var isInRange = maxRoomI < roomLimits.Count;
        var exceedsRoomLimit = transform.position.x > roomLimits[maxRoomI].x;
        var isCleared = roomState == RoomTransition.Cleared;

        if (isCleared && isInRange && exceedsRoomLimit)
        {
            currRoomI++;
            roomState = RoomTransition.Active;
        }
    }

    void DebugClear() {
        if (clearRoom){
            roomState = RoomTransition.Cleared;
            clearRoom = false;
        }
    }
}