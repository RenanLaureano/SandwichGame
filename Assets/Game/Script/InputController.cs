using Lean.Common;
using Lean.Touch;
using UnityEngine;

using static MoveController;

public class InputController : MonoBehaviour
{
    private const float MIN_ARC_MOVE = 20.0f;

    private TileNodeObject selectedObject;
    private MoveController moveController;

    void Start()
    {
        ServiceLocator.Instance.Register<InputController>(this);
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerSwipe += OnFingerSwipe;
        LeanSelectable.OnAnySelected += OnSelectAny;
        LeanSelectable.OnAnyDeselected += OnDeselectedAny;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerSwipe -= OnFingerSwipe;
        LeanSelectable.OnAnySelected -= OnSelectAny;
    }

    private void OnSelectAny(LeanSelect leanSelect, LeanSelectable leanSelectable)
    {
        Debug.Log("Selectable: " + leanSelectable.gameObject.name);
        selectedObject = leanSelectable.gameObject.GetComponent<TileNodeObject>();
    }

    private void OnDeselectedAny(LeanSelect leanSelect, LeanSelectable leanSelectable)
    {
        Debug.Log("Deselected: " + leanSelectable.gameObject.name);
        selectedObject = null;
    }

    private void OnFingerSwipe(LeanFinger finger)
    {
        if(selectedObject == null)
        {
            return;
        }

        if(moveController == null)
        {
            moveController = ServiceLocator.Instance.GetComponentRegistered<MoveController>();
        }

        Vector2 fingerSwipe = finger.ScreenPosition - finger.StartScreenPosition;
        MoveDirection moveDirection = GetSideSwiper(fingerSwipe);

        Debug.Log("Move direction: " + moveDirection.ToString());

        if(moveDirection == MoveDirection.NONE)
        {
            return;
        }

        moveController.MoveToDirection(selectedObject, moveDirection);
    }

    private MoveDirection GetSideSwiper(Vector2 fingerSwipe)
    {
        if (IsInAngle(0.0f, fingerSwipe) || IsInAngle(45.0f, fingerSwipe))
        {
            return MoveDirection.UP;
        }
        else if (IsInAngle(180.0f, fingerSwipe) || IsInAngle(225.0f, fingerSwipe))
        {
            return MoveDirection.DOWN;
        }
        else if (IsInAngle(90.0f, fingerSwipe) || IsInAngle(135.0f, fingerSwipe))
        {
            return MoveDirection.RIGHT;
        }
        else if (IsInAngle(270.0f, fingerSwipe) || IsInAngle(315.0f, fingerSwipe))
        {
            return MoveDirection.LEFT;
        }

        return MoveDirection.NONE;
    }

    protected bool IsInAngle(float testAngle, Vector2 fingerSwipe)
    {
        var angle = Mathf.Atan2(fingerSwipe.x, fingerSwipe.y) * Mathf.Rad2Deg;
        var angleDelta = Mathf.DeltaAngle(angle, testAngle);

        if (angleDelta < -MIN_ARC_MOVE || angleDelta >= MIN_ARC_MOVE)
        {
            return false;
        }

        return true;
    }
}
