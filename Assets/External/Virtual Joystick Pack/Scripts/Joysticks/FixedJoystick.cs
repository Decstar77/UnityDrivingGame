﻿using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    Vector2 joystickPosition = Vector2.zero;
    private Camera cam = new Camera();

    void Start()
    {
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
		handle.anchoredPosition = Vector3.zero;
    }
	public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickPosition;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

	public Vector3 GetJoyStickDirection()
	{
		return handle.anchoredPosition.normalized;
	}

}