using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
[SerializeField] private GameObject menu;

    public void OpenOptionsMenu() {
        menu.SetActive(true);
    }

    public void CloseOptionsMenu() {
        menu.SetActive(false);
    }
}
