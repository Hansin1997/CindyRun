using Cindy.Logic.ReferenceValues;
using UnityEngine;

namespace CindyRun.Util
{
    /// <summary>
    /// 计算能完全显示Canva的坐标
    /// </summary>
    public class CanvaPointFitter : MonoBehaviour
    {
        public Canvas canvas;
        public new Camera camera;
        public ReferenceFloat scale = new ReferenceFloat() { value = 1f };

        private Canvas _canvas;
        protected RectTransform rectTransform;

        private void Awake()
        {
            if (camera == null)
                camera = Camera.main;
            canvas = GetComponentInParent<Canvas>();
        }

        protected virtual void Update()
        {
            if(_canvas != canvas)
            {
                _canvas = canvas;
                if (canvas != null)
                    rectTransform = canvas.GetComponent<RectTransform>();
            }
            if(canvas!= null && rectTransform != null)
            {
                float d = rectTransform.sizeDelta.y / 2 * rectTransform.localScale.y;
                if (camera != null)
                {
                    float angle = (camera.fieldOfView / 2) * Mathf.PI / 180;
                    // tan(angle) = d / x , x = d / tan(angle);
                    float x = d / Mathf.Tan(angle);

                    transform.position = canvas.transform .position - canvas.transform.forward * scale.Value * x;
                }
            }
        }
    }
}