using UnityEngine;

namespace MyTools.UI
{
    public class ProgressBar : MonoBehaviour
    {
        public RectTransform fillRectTransform;

        [Range(0f, 1f)]
        [SerializeField] private float progress;

        public float Progress => progress;

        private void Awake()
        {
            RefreshVisual();
        }

        public void SetProgress(float value)
        {
            progress = Mathf.Clamp01(value);
            RefreshVisual();
        }

        public void AddProgress(float delta)
        {
            SetProgress(progress + delta);
        }

        public void SubtractProgress(float delta)
        {
            SetProgress(progress - delta);
        }

        public void ResetProgress()
        {
            SetProgress(0f);
        }

        private void RefreshVisual()
        {
            if (fillRectTransform == null)
            {
                return;
            }

            Vector2 anchorMax = fillRectTransform.anchorMax;
            anchorMax.x = progress;
            fillRectTransform.anchorMax = anchorMax;
        }
    }
}