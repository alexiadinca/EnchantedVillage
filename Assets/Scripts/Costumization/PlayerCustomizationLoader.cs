using UnityEngine;

public class PlayerCustomizationLoader : MonoBehaviour
{
    public MeshFilter bodyRenderer;
    public Mesh[] animalModels;
    public Material[] baseMaterials;
    public Color[] furColors;
    public GameObject[] hats;
    public Transform hatAnchor;

    public void LoadFromPrefs()
    {
        if (bodyRenderer == null || hatAnchor == null)
        {
            Debug.LogError("Missing references on PlayerCustomizationLoader: Please assign all fields in the Inspector (especially bodyRenderer and hatAnchor).");
            return;
        }

        int animalIndex = PlayerPrefs.GetInt("AnimalIndex", 0);
        int colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        int hatIndex = PlayerPrefs.GetInt("HatIndex", -1);

        // Apply mesh
        bodyRenderer.sharedMesh = animalModels[animalIndex];

        // Apply material
        Renderer rend = bodyRenderer.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("No Renderer found on bodyRenderer object!");
            return;
        }
        rend.sharedMaterial = baseMaterials[animalIndex];

        // Apply fur color
        if (rend.sharedMaterial.HasProperty("_FurColor"))
        {
            rend.sharedMaterial.SetColor("_FurColor", furColors[colorIndex]);
        }
        else
        {
            Debug.LogWarning("Material does not have _FurColor property.");
        }

        // Apply hat (if any)
        if (hatIndex >= 0 && hatIndex < hats.Length)
        {
            // parent under hatAnchor and preserve the prefab’s own local transform:
            Instantiate(hats[hatIndex], hatAnchor, false);
        }
    }
}






/*
using UnityEngine;

public class PlayerCustomizationLoader : MonoBehaviour
{
    public MeshFilter bodyRenderer;
    public Mesh[] animalModels;
    public Material[] baseMaterials;
    public Color[] furColors;
    public GameObject[] hats;
    public Transform hatAnchor;

    private Material[] activeMaterials;

    public void LoadFromPrefs()
    {
        if (bodyRenderer == null || hatAnchor == null)
        {
            Debug.LogError("Missing references on PlayerCustomizationLoader: Please assign all fields in the Inspector (especially bodyRenderer and hatAnchor).");
            return;
        }

        int animalIndex = PlayerPrefs.GetInt("AnimalIndex", 0);
        int colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        int hatIndex = PlayerPrefs.GetInt("HatIndex", -1);

        // Apply mesh
        bodyRenderer.sharedMesh = animalModels[animalIndex];

        Renderer rend = bodyRenderer.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("No Renderer found on bodyRenderer object!");
            return;
        }

        rend.sharedMaterial = baseMaterials[animalIndex]; // safer for prefab setup

        // Apply fur color
        if (rend.sharedMaterial.HasProperty("_FurColor"))
        {
            rend.sharedMaterial.SetColor("_FurColor", furColors[colorIndex]);
        }
        else
        {
            Debug.LogWarning("Material does not have _FurColor property.");
        }

        // Apply hat
        if (hatIndex >= 0 && hatIndex < hats.Length)
        {
            GameObject hat = Instantiate(hats[hatIndex], hatAnchor);
            hat.transform.localPosition = Vector3.zero;
            hat.transform.localRotation = Quaternion.identity;
            hat.transform.localScale = Vector3.one;
        }
    }
}
*/