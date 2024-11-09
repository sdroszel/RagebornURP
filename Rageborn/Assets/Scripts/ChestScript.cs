using System.Collections;
using TMPro;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    [SerializeField] private GameObject chestDialogBox;
    [SerializeField] private TextMeshProUGUI chestDialogTextObject;
    [SerializeField] private string checkDialogText;
    [SerializeField] private Mesh openChestMesh;
    [SerializeField] private AudioClip chestAudio;

    private MeshFilter meshFilter;
    private bool isChestOpened = false;
    private bool isPlayerInRange = false;
    private AudioSource audioSource;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        audioSource = GetComponent<AudioSource>();
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
        audioSource.PlayOneShot(chestAudio);

        SceneManagerScript.instance.numOfHealthPotions++;

        if (chestDialogBox != null && chestDialogTextObject != null)
        {
            chestDialogBox.SetActive(true);
            chestDialogTextObject.text = checkDialogText;
        }
    }
}
