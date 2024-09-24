using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Clock.ClockModules.Input.Analog
{
    public class ClockAnalogHandle : MonoBehaviour, IDragHandler
    {
        [SerializeField] private Camera canvasCamera;
        [SerializeField] private RectTransform rootTransform;
        
        public event Action<float> ValueChanged;

        private void Start()
        {
            rootTransform ??= transform.parent as RectTransform;
            canvasCamera ??= Camera.main;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var direction = eventData.position - (Vector2)rootTransform.position;
            var angle = Vector2.SignedAngle(Vector2.up, direction);
            rootTransform.rotation = Quaternion.Euler(0, 0, angle);

            ValueChanged?.Invoke(angle);
        }

        public void SetRotation(float angle)
        {
            rootTransform.rotation = Quaternion.Euler(0,0,angle);
        }

        public float GetCurrentValue()
        {
            return rootTransform.rotation.eulerAngles.z;
        }
    }
}
