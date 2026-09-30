using System.Collections.Generic;
using UnityEngine;

namespace _IntegrationPackage.Scripts
{
    public class UI_Menu : MonoBehaviour
    {
        public static string previousMenu = "";
        public static string currentMenu = "";


        public static List<UI_Menu> menuList = new List<UI_Menu>();
        private Animation animSelf;

        string menuName = "";
        public string MenuName { get => menuName == "" ? gameObject.name : menuName; }

        // Start is called before the first frame update
        void Awake()
        {
            if (!menuList.Contains(this))
            {
                menuList.Add(this);
                gameObject.SetActive(false);
            }
        }

        public static UI_Menu GetMenu(string _menuName)
        {
            foreach (UI_Menu _menu in menuList) if (_menu.MenuName == _menuName) return _menu;
            Debug.LogError("No menu found for : " + _menuName);
            return null;
        }

        public static void Open(string _menuName)
        {
            foreach (UI_Menu _menu in menuList)
                if (_menu.MenuName != _menuName) _menu.CloseMenu();
                else
                {
                    previousMenu = currentMenu;
                    currentMenu = _menuName;
                    _menu.OpenMenu();
                }
        }

        public virtual void OpenMenu()
        {
            gameObject.SetActive(true);
            if (animSelf != null) animSelf.Play();
        }

        public void CloseMenu()
        {
            gameObject.SetActive(false);
        }
    }
}