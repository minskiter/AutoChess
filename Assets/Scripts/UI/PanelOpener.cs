using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelOpener : MonoBehaviour
{
    public GameObject Panel;

    private bool isOpen = false;
  
    public void openPanel() {
        if(Panel != null) {
            Panel.SetActive(!isOpen);
            isOpen = !isOpen;
        }
    }
}
