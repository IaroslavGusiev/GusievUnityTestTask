using UnityEngine;

namespace _Bludoku.Scripts.Extentions
{
    public static class RectTransformExtensions
    {
        public static bool TryWorldToLocalPoint(this RectTransform container, Vector3 worldPosition,
            UnityEngine.Camera worldCamera, Canvas canvas, out Vector2 localPoint)
        {
            Vector2 screenPoint = worldCamera.WorldToScreenPoint(worldPosition);
            return container.TryScreenToLocalPoint(screenPoint, canvas, out localPoint);
        }

        public static bool TryUIToLocalPoint(this RectTransform container, RectTransform target,
            Canvas canvas, out Vector2 localPoint)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(GetUICamera(canvas), target.position);
            return container.TryScreenToLocalPoint(screenPoint, canvas, out localPoint);
        }

        private static bool TryScreenToLocalPoint(this RectTransform container, Vector2 screenPoint,
            Canvas canvas, out Vector2 localPoint) =>
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                container, screenPoint, GetUICamera(canvas), out localPoint);

        private static UnityEngine.Camera GetUICamera(Canvas canvas) =>
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }
}
