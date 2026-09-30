using UnityEngine;

namespace _Bludoku.Scripts
{
    public class UIElement_Gauge : MonoBehaviour
    {
        public RectTransform parentTransform;
        public RectTransform fillTransform;


        private float refFloat;
        private float currentPercent;
        private float percentGoal;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {

            currentPercent = Mathf.SmoothDamp(currentPercent, percentGoal, ref refFloat, 0.2f);

        

            fillTransform.sizeDelta = new Vector2(parentTransform.rect.size.x * currentPercent, fillTransform.rect.size.y);
        }


        public void SetGaugeValue(float _percent)
        {
            percentGoal = _percent;
        }
    }
}
