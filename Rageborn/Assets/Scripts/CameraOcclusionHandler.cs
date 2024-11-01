using System.Collections.Generic;
using UnityEngine;

public class CameraOcclusionHandler : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player or target object
    [SerializeField] private LayerMask occlusionLayers; // Layer mask for objects that can cause occlusion
    [SerializeField] private float transparency = 0.3f; // Transparency level to apply to occluding objects
    [SerializeField] private float fadeSpeed = 2f; // Speed at which objects fade

    private List<Renderer> currentOccluders = new List<Renderer>();
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    void Update()
    {
        // Restore any previously occluded objects
        RestoreOccluders();

        // Cast a ray from the camera to the player
        Vector3 directionToPlayer = player.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToPlayer, directionToPlayer.magnitude, occlusionLayers);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null && !currentOccluders.Contains(renderer))
            {
                // Save the original materials
                if (!originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.materials;
                }

                // Create transparent versions of the materials
                Material[] transparentMaterials = CreateTransparentMaterials(renderer.materials);
                renderer.materials = transparentMaterials;

                currentOccluders.Add(renderer);
            }
        }
    }

    private Material[] CreateTransparentMaterials(Material[] originalMats)
    {
        Material[] transparentMats = new Material[originalMats.Length];
        for (int i = 0; i < originalMats.Length; i++)
        {
            Material mat = new Material(originalMats[i]);
            mat.SetFloat("_Mode", 2); // Set material mode to transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            Color color = mat.color;
            color.a = transparency;
            mat.color = color;

            transparentMats[i] = mat;
        }
        return transparentMats;
    }

    private void RestoreOccluders()
    {
        foreach (Renderer renderer in currentOccluders)
        {
            if (renderer != null && originalMaterials.ContainsKey(renderer))
            {
                renderer.materials = originalMaterials[renderer];
            }
        }
        currentOccluders.Clear();
    }
}
