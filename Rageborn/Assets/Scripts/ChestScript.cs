using System.Collections;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class ChestScript : MonoBehaviour
{
    [SerializeField] private GameObject chestDialogBox;
    [SerializeField] private TextMeshProUGUI chestDialogTextObject;
    [SerializeField] private string checkDialogText;
    [SerializeField] private GameObject prompt;
    [SerializeField] private Image healthPotionSlot;
    [SerializeField] private Image staminaPotionSlot;
    [SerializeField] private bool isHealthPotion;
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

        prompt.SetActive(false);
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
            if (!isChestOpened)
            {
                prompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            chestDialogBox.SetActive(false);
            prompt.SetActive(false);
        }
    }

    private void OpenChest()
    {
        prompt.SetActive(false);
        isChestOpened = true;
        meshFilter.mesh = openChestMesh;
        audioSource.PlayOneShot(chestAudio);

        if (chestDialogBox != null && chestDialogTextObject != null)
        {
            chestDialogBox.SetActive(true);
            if (isHealthPotion)
            {
                SceneManagerScript.instance.numOfHealthPotions++;

                healthPotionSlot.gameObject.SetActive(true);
                staminaPotionSlot.gameObject.SetActive(false);
            }
            else
            {
                SceneManagerScript.instance.numOfStaminaPotions++;

                healthPotionSlot.gameObject.SetActive(false);
                staminaPotionSlot.gameObject.SetActive(true);
            }
            
            chestDialogTextObject.text = checkDialogText;
        }
    }
}
