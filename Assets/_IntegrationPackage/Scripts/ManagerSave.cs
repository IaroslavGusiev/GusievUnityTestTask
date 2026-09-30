using System.Collections.Generic;
using UnityEngine;

namespace _IntegrationPackage.Scripts
{
    public class ManagerSave : MonoBehaviour
    {

        public static bool isInit = false;
        private static string saveString;
        private static List<PlayerVar> playerVars = new List<PlayerVar>();


        public static void Init()
        {
            if (isInit) return;

            if (PlayerPrefs.HasKey("RadSav"))
            {
                Debug.Log("RadSav found, Loading");
                saveString = PlayerPrefs.GetString("RadSav");
                playerVars = SaveStringToPlayerVars();
            }

            //else
            //    CreateBaseSave();

            isInit = true;
        }

        public static void Save()
        {
            Debug.Log("Saving PlayerVars");
            string _saveString = PlayerVarsToSaveString();
            PlayerPrefs.SetString("RadSav", _saveString);
        }


        //========CONVERSION

        private static string PlayerVarsToSaveString()
        {
            List<string> _stringArray = new List<string>();

            foreach (PlayerVar _var in playerVars)
                _stringArray.Add(_var.GetSaveString());

            return string.Join("||", _stringArray);
        }


        private static List<PlayerVar> SaveStringToPlayerVars()
        {
            Debug.Log("Start init SaveString to PlayerVar");

            List<PlayerVar> _returnList = new List<PlayerVar>();

            string[] _splitSav = saveString.Split("||");

            foreach (string _splitted in _splitSav)
            {
                if (_splitted == "") continue;

                string[] _nameValue = _splitted.Split("::");

                if (_nameValue.Length != 2) continue;

                Debug.Log($"New player Var : {_nameValue[0]}, value : {_nameValue[1]}");

                PlayerVar _newVar = new PlayerVar();

                _newVar.varName = _nameValue[0];
                _newVar.varValue = _nameValue[1];

                _returnList.Add(_newVar);
            }

            return _returnList;
        }


        //========== GET

        private static PlayerVar GetVar(string _name)
        {
            foreach (PlayerVar _var in playerVars)
                if (_var.varName == _name) return _var;
            return null;
        }

        private static bool TryGetVar(string _name, out PlayerVar _playerVar)
        {
            PlayerVar _selected = GetVar(_name);

            if (_selected == null)
            {
                Debug.Log($"ManagerSave: var {_name} doesn't exist, creating one");
                PlayerVar _newVar = new PlayerVar();
                _newVar.varName = _name;
                playerVars.Add(_newVar);
                _playerVar = _newVar;
                return false;
            }

            _playerVar = _selected;
            return true;
        }

        public static bool GetBool(string _varName, bool _defaultValue = false)
        {
            if (!TryGetVar(_varName, out PlayerVar _select))
            {
                _select.BoolValue = _defaultValue;
                return _defaultValue;
            }

            return _select.BoolValue;
        }

        public static string GetString(string _varName, string _defaultValue = "")
        {
            if (!TryGetVar(_varName, out PlayerVar _select))
            {
                _select.StringValue = _defaultValue;
                return _defaultValue;
            }

            return _select.StringValue;
        }

        public static int GetInt(string _varName, int _defaultValue = 0)
        {
            if (!TryGetVar(_varName, out PlayerVar _select))
            {
                _select.IntValue = _defaultValue;
                return _defaultValue;
            }

            return _select.IntValue;
        }

        //==== SET

        public static void SetBool(string _varName, bool _value, bool _shouldSave = true)
        {
            TryGetVar(_varName, out PlayerVar _selectedVar);
            _selectedVar.BoolValue = _value;
            if (_shouldSave) Save();
        }

        public static void SetInt(string _varName, int _value, bool _shouldSave = true)
        {
            Debug.Log("setint");
            TryGetVar(_varName, out PlayerVar _selectedVar);
            _selectedVar.IntValue = _value;
            if (_shouldSave) Save();
        }

        public static void IncreaseInt(string _varName, int _value, bool _shouldSave = true)
        {
            TryGetVar(_varName, out PlayerVar _selectedVar);
            int _oldValue = _selectedVar.IntValue;
            _selectedVar.IntValue = _oldValue + _value;
            if (_shouldSave) Save();
        }

        public static void SetString(string _varName, string _value, bool _shouldSave = true)
        {
            TryGetVar(_varName, out PlayerVar _selectedVar);
            _selectedVar.StringValue = _value;
            if (_shouldSave) Save();
        }

        //private static void CreateBaseSave()
        //{
        //    GetBool("haptics", true);
        //    GetBool("audio", false);
        //    GetBool("cookie", false);
        //}


        // ------------------------- CREATION VARIABLES STATIQUES

        public static int MoneyTime20Reset
        {
            get => GetInt("MoneyReset");
            set => SetInt("MoneyReset", value);
        }

        public static int MoneyTime20Timer
        {
            get => GetInt("MoneyTimer");
            set => SetInt("MoneyTimer", value);
        }

        public static string MoneyTimeID
        {
            get => GetString("MoneyTimeID", "-1");
            set => SetString("MoneyTimeID", value);
        }
    }

    public class PlayerVar
    {

        public string varName;
        public string varValue;
        public string GetSaveString()
        {
            return varName + "::" + varValue;
        }

        //BOOL
        public bool BoolValue
        {
            get { return varValue == "t"; }
            set { varValue = value ? "t" : "f"; }
        }

        //INT
        public int IntValue
        {
            get
            {
                if (int.TryParse(varValue, out int _result)) return _result;
                return -1;
            }

            set { varValue = value.ToString(); }
        }

        //STRING
        public string StringValue
        {
            get { return varValue; }
            set { varValue = value; }
        }
    }
}