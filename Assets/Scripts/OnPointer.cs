using UnityEngine;
using UnityEngine.EventSystems;

public class OnPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InputManager _inputManager;

    private void Start()
    {
        _inputManager = InputManager.instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _inputManager.SetCanConfirm(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inputManager.SetCanConfirm(true);
    }
}
