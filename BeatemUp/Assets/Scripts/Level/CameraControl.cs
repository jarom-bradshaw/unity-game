using System.Collections.Generic;
using Unity.Mathematics;
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
    [SerializeField] private float deadZone;
    [SerializeField] private float followMultiplier;

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

        Vector3 targetPosition = target.position; // Player current position
        targetPosition.y = cameraY;
        targetPosition.z = cameraZ;

        Vector3 cameraPosition = transform.position; // Camera current position
        cameraPosition.y = cameraY;
        cameraPosition.z = cameraZ;

        var deadZoneTest = targetPosition.x - cameraPosition.x;
        if (Mathf.Abs(deadZoneTest) < deadZone)
            return;

        if (targetPosition.x > cameraPosition.x)
        {
            targetPosition.x -= deadZone;
        }
        else if (targetPosition.x < cameraPosition.x)
        {
            targetPosition.x += deadZone;
        }

        int maxRoomI = roomState switch { // changes room limit when cleared
            RoomTransition.Cleared => currRoomI + 1,
            _ => currRoomI
        };

        Vector3 smoothPosition = Vector3.SmoothDamp(cameraPosition, targetPosition, ref velocity, smoothTime / followMultiplier); // smooth movement
        var boundMax = maxRoomI >= roomLimits.Count ? roomLimits[currRoomI].y : roomLimits[maxRoomI].y; // prevents out of index
        smoothPosition.x = Mathf.Clamp(smoothPosition.x, roomLimits[currRoomI].x, boundMax); // limits camera movement at the edge of rooms
        transform.position = smoothPosition;

        var isInRange = maxRoomI < roomLimits.Count; // checks if room number is within limits
        var exceedsRoomLimit = transform.position.x > roomLimits[maxRoomI].x; // checks if player is beyond room limits
        var isCleared = roomState == RoomTransition.Cleared; // checks if room is cleared

        if (isCleared && isInRange && exceedsRoomLimit) // switches room from cleared to active
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