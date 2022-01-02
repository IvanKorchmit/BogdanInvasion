using UnityEngine;
using UnityEngine.EventSystems;

class PlayerInput : MonoBehaviour, IPointerClickHandler
{
    public delegate InputInfo OnPlayerInputDelegate();
    public static event OnPlayerInputDelegate OnPlayerInput;
    [SerializeField] float doubleClickTime = 0.3f;
    [SerializeField] int clicks;
    private bool triggered;
    private float time;
    private void Update()
    {
        if (time > 0 && clicks >= 2 && !triggered)
        {
            Debug.Log("Double clicked");
            triggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            time = doubleClickTime;
            clicks++;
        }
        else if (time < 0)
        {
            clicks = 0;
            triggered = false;

        }
        time -= Time.deltaTime;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
