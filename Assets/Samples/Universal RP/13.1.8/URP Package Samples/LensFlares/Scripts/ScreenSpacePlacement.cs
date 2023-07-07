using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ScreenSpacePlacement : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform flareObject;

    private bool m_MouseDown;

    void OnGUI()
    {
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        if (currentEvent.type == EventType.MouseDown) m_MouseDown = true;
        if (currentEvent.type == EventType.MouseUp) m_MouseDown = false;

        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        if (flareObject != null && mousePos.x > 0 && mousePos.y > 0 && mousePos.x < cam.pixelWidth && mousePos.y < cam.pixelHeight)
        {
            Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

            if (m_MouseDown)
            {
                flareObject.position = point;
            }
        }
    }
}
