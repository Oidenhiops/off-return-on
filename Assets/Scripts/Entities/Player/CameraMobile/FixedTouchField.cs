using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerInputs playerInputs;
    public Vector2 TouchDist;
    [HideInInspector]
    public Vector2 PointerOld;
    [HideInInspector]
    protected int PointerId;
    [HideInInspector]
    public bool Pressed;
    void Update()
    {
        if (Pressed)
        {
            Touch touch = default;
            bool found = false;

            foreach (Touch t in Input.touches)
            {
                if (t.fingerId == PointerId)
                {
                    touch = t;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                TouchDist = touch.position - PointerOld;
                PointerOld = touch.position;
                print("touch");
            }
            else
            {
                TouchDist = (Vector2)Input.mousePosition - PointerOld;
                PointerOld = Input.mousePosition;
                print("mouse");
            }
        }
        else
        {
            TouchDist = Vector2.zero;
        }

        playerInputs.playerInputsInfo.lookInput = TouchDist.normalized;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        PointerId = eventData.pointerId;
        PointerOld = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}