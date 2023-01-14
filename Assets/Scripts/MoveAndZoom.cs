using UnityEngine;

class MoveAndZoom : MonoBehaviour
{
#if UNITY_IOS || UNITY_ANDROID
    public Camera Camera;
    public bool Rotate;
    protected Plane Plane;

    private float minX = -250.0f;
    private float maxX = 250.0f;
    private float minY = 0.0f;
    private float maxY = 200.0f;
    private float minZ = -250.0f;
    private float maxZ = 250.0f;

    private void Awake()
    {
        if (Camera == null)
            Camera = Camera.main;
    }

    private void Update()
    {
        //Update Plane
        if (Input.touchCount >= 1)
            Plane.SetNormalAndPosition(transform.up, transform.position);

        var Delta1 = Vector3.zero;
        var Delta2 = Vector3.zero;

        //Scroll
        if (Input.touchCount >= 1)
        {
            Delta1 = PlanePositionDelta(Input.GetTouch(0));
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
                Camera.transform.Translate(Delta1 / 5, Space.World);
        }

        //Pinch
        /*if (Input.touchCount >= 2)
        {
            var pos1  = PlanePosition(Input.GetTouch(0).position);
            var pos2  = PlanePosition(Input.GetTouch(1).position);
            var pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            //calc zoom
            var zoom = Vector3.Distance(pos1, pos2) /
                       Vector3.Distance(pos1b, pos2b);

            //edge case - do not know what it's for, maybe could be delete later
            //if (zoom == 0 || zoom > 10)
            //    return;

            //Move cam amount the mid ray
            Camera.transform.position = ConvertToValidPoint(Vector3.LerpUnclamped(pos1, Camera.transform.position, 1 / zoom));
            
            if (Rotate && pos2b != pos2)
                Camera.transform.RotateAround(pos1, Plane.normal, Vector3.SignedAngle((pos2 - pos1) / 5, (pos2b - pos1b) / 5, Plane.normal));
    }*/

    }

    /*private Vector3 ConvertToValidPoint(Vector3 newPosition)
    {
        Debug.Log("TEST0: " + newPosition);

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(newPosition);

        viewportPos.x = Mathf.Clamp01(viewportPos.x);
        viewportPos.y = Mathf.Clamp01(viewportPos.y);
        viewportPos.z = Mathf.Clamp01(viewportPos.z);

        Debug.Log("TEST1: " + viewportPos);

        Vector3 clampedCamPos = Camera.main.ViewportToWorldPoint(viewportPos);

        Debug.Log("TEST2: " + clampedCamPos);

        Debug.Log("TEST2.1: " + Mathf.Clamp(clampedCamPos.x, minX, maxX));
        Debug.Log("TEST2.2: " + Mathf.Clamp(clampedCamPos.y, minY, maxY));
        Debug.Log("TEST2.3: " + Mathf.Clamp(clampedCamPos.z, minZ, maxZ));

        clampedCamPos.x = Mathf.Clamp(clampedCamPos.x, minX, maxX);
        clampedCamPos.y = Mathf.Clamp(clampedCamPos.y, minY, maxY);
        clampedCamPos.y = Mathf.Clamp(clampedCamPos.z, minZ, maxZ);

        Debug.Log("TEST3: " + clampedCamPos);

        return clampedCamPos;
    }*/

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;

        var rayBefore = Camera.ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = Camera.ScreenPointToRay(touch.position);
        if (Plane.Raycast(rayBefore, out var enterBefore) && Plane.Raycast(rayNow, out var enterNow))
            return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

        return Vector3.zero;
    }

    protected Vector3 PlanePosition(Vector2 screenPos)
    {
        var rayNow = Camera.ScreenPointToRay(screenPos);
        if (Plane.Raycast(rayNow, out var enterNow))
            return rayNow.GetPoint(enterNow);

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.up);
    }
#endif
}
