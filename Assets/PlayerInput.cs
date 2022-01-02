using UnityEngine;
using UnityEngine.EventSystems;

class PlayerInput : MonoBehaviour
{
    public delegate void OnPlayerInputDelegate(InputInfo info);
    public static event OnPlayerInputDelegate OnPlayerInput;
    [SerializeField] float doubleClickTime = 0.3f;
    [SerializeField] int clicks;
    private bool triggered;
    private float time;
    private void Update()
    {
        if (time > 0 && clicks >= 2 && !triggered)
        {
            OnDoubleClick();
            triggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            time = doubleClickTime;
            clicks++;
            if (clicks == 1)
            {
                OnSingleClick();

            }
        }
        else if (time < 0)
        {
            clicks = 0;
            triggered = false;

        }
        time -= Time.deltaTime;
    }

    private void OnSingleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Unit")))
        {
            if(hit.collider.TryGetComponent(out ISelectable selectable))
            {
                selectable.OnSelect();
            }
        }
    }
    private void OnDoubleClick()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue,LayerMask.GetMask("Obstacle")))
        {
            OnPlayerInput?.Invoke(new InputInfo(hit.point,InputInfo.CommandType.Move));

        }
    }
}
