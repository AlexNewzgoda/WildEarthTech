using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSettingsActionChanger : MonoBehaviour
{
    public string ActionName = "";
    public Text ButtonNameView;
    bool first = true;
    private ChangeResult _result;

    public class ChangeResult
    {
        public ChangeResult(InputSettingsActionChanger o)
        {
            owner = o;
        }

        public KeyCode key;
        private InputSettingsActionChanger owner;
        public void Set (KeyCode v)
        {
            key = v;
            owner.UIUpdate();
        }
    }

    public void Action ()
    {
        InputSettings.Instance.StartKeyEdit(ref _result, ActionName);
    }

    public void UIUpdate ()
    {
        if (ButtonNameView != null)
        {
            if (_result.key == KeyCode.None)
            {
                ButtonNameView.text = "<" + _result.key.ToString() + ">";
            }
            else
            {
                ButtonNameView.text = _result.key.ToString();

                if (ButtonNameView.text.Contains("Alpha"))
                    ButtonNameView.text = ButtonNameView.text.Replace("Alpha", null);

                if (this.gameObject.activeInHierarchy && !first)
                {

                }

                first = false;
            }
        }
    }

    private void Start()
    {
        _result = new ChangeResult(this);
        _result.Set(InputSettings.Instance.GetKeyValue(ActionName, KeyCode.None));
    }
}
