using UnityEngine;

namespace _Bludoku.Scripts.Blocks
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] sprites;

        public int State { get; private set; }

        public void SetState(int value)
        {
            if (value < 0 || value >= sprites.Length)
                return;

            State = value;
            spriteRenderer.sprite = sprites[value];
        }
        
        public void SetAlpha(float value)
        {
            Color color = spriteRenderer.color;
            color.a = value;
            spriteRenderer.color = color;
        }
    }
}