using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private float scrollSpeedZ = 5f;

    void Update()
    {
        // Move the text upward over time
        transform.position += new Vector3(0, scrollSpeed * Time.deltaTime, scrollSpeedZ * Time.deltaTime);
    }
}
