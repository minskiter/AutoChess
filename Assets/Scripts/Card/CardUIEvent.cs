using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUIEvent : MonoBehaviour
{
    [Header("Description")]
    public GameObject DescriptionUI;

    public Text MoneyUI;

    public Text NameUI;

    public string name
    {
        get
        {
            return NameUI.text;
        }
        set
        {
            NameUI.text = value;
        }
    }

    public int cost
    {
        get
        {
            return int.Parse(MoneyUI.text);
        }
        set
        {
            MoneyUI.text = value.ToString();
        }
    }


    public void ToggleDescription()
    {
        DescriptionUI.SetActive(DescriptionUI.activeSelf^true);
    }
}
