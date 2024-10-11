using TMPro;
using UnityEngine;

public class TextPulse : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Reference to your AudioSource
    [SerializeField] private TextMeshProUGUI uiText; // TextMeshProUGUI for UI Text
    [SerializeField] private float scaleMultiplier = 2.0f; // Controls how much the text scales
    [SerializeField] private float smoothSpeed = 0.5f; // Controls how quickly the scale changes
    private float[] spectrumData = new float[64]; // Array to store audio frequency data
    private Vector3 originalScale;

    void Start()
    {
        // Store the original scale of the text to modify later
        originalScale = uiText.transform.localScale;
    }

    void Update()
    {
        // Get spectrum data from the audio source
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        // Get the intensity of the audio by averaging some of the spectrum data
        float intensity = 0f;
        for (int i = 0; i < 10; i++) // Analyzing the lower frequency range (bass and mid tones)
        {
            intensity += spectrumData[i];
        }

        // Normalize the intensity value
        intensity = Mathf.Clamp(intensity * scaleMultiplier, 0.5f, 1.5f);

        // Smoothly adjust the scale of the text based on the audio intensity
        Vector3 targetScale = originalScale * intensity;
        uiText.transform.localScale = Vector3.Lerp(uiText.transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
    }
}
