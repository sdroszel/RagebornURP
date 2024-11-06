using System.Collections;
using TMPro;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    [SerializeField] private GameObject chestDialogBox;
    [SerializeField] private GameObject potion;
    [SerializeField] private TextMeshProUGUI chestDialogText;
    [SerializeField] private Mesh openChestMesh;

    private MeshFilter meshFilter;
    private bool isChestOpened = false;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        if (isPlayerInRange && !isChestOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            chestDialogBox.SetActive(false);
        }
    }

    private void OpenChest()
    {
        isChestOpened = true;
        meshFilter.mesh = openChestMesh;

        SceneManagerScript.instance.numOfHealthPotions++;

        if (chestDialogBox != null && chestDialogText != null)
        {
            chestDialogBox.SetActive(true);
            chestDialogText.text = "You found a health potion!";
        }
    }
}
