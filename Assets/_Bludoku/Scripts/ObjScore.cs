using System.Collections;
using TMPro;
using UnityEngine;

namespace _Bludoku.Scripts
{
    public class ObjScore : MonoBehaviour
    {

        public void InitObj(int _score, Vector3 _position)
        {
            transform.localScale = Vector3.zero;
            transform.position = _position;
            GetComponent<TextMeshPro>().SetText("+" + _score);
            StartCoroutine(SelfRoutine());
        }

        private IEnumerator SelfRoutine()
        {
            Transform selftransform = transform;

            float lerp = 0f;

            while (lerp < 1.2f)
            {
                lerp += Time.deltaTime * 5f;
                if (lerp > 1.2f) lerp = 1.2f;
                selftransform.localScale = Vector3.one * lerp * .1f;
                yield return null;
            }

            while (lerp > 1f)
            {
                lerp -= Time.deltaTime * 5f;
                if (lerp < 1f) lerp = 1f;
                selftransform.localScale = Vector3.one * lerp * .1f;
                yield return null;
            }

            yield return new WaitForSeconds(.5f);

            while (lerp > 0f)
            {
                lerp -= Time.deltaTime * 5f;
                if (lerp < 0f) lerp = 0f;
                selftransform.localScale = Vector3.one * lerp * .1f;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
