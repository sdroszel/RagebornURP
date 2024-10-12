using UnityEngine;
using TMPro;  // If you're using TextMeshPro

public class ScrollingText : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private float scrollSpeedZ = 5f;
    private RectTransform textTransform;

    void Start()
    {
        // Get the RectTransform component of the text
        textTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Move the text upward over time
        transform.position += new Vector3(0, scrollSpeed * Time.deltaTime, scrollSpeedZ * Time.deltaTime);    }
}
