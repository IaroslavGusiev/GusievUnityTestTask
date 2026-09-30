using UnityEngine;

namespace _Bludoku.Scripts
{
    public class UI_LoadingScreen : MonoBehaviour
    {

        private static UI_LoadingScreen instance;

        private static UI_LoadingScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = GameObject.Find("LoadingScreen")?.GetComponent<UI_LoadingScreen>();

                return instance;
            }
        }

        public static void Open()
        {
            Instance.gameObject.SetActive(true);
        }

        public static void Close()
        {
            Instance.gameObject.SetActive(false);
        }


    }
}
