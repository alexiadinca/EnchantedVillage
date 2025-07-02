using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCustomizer : MonoBehaviour
{
    public TMP_InputField nameInput;
    public MeshFilter bodyRenderer;
    public Mesh[] animalModels;
    public Material[] baseMaterials;
    public Color[] furColors;
    public GameObject[] hats;

    private int currentAnimalIndex = 0;
    private int currentColorIndex = 0;
    private GameObject[] hatInstances;

    private Material[] activeMaterials;

    private int currentHatIndex = -1;
    private GameObject currentHatInstance;
    public Transform hatAnchor;

    void Start()
    {
        // Copy & instantiate your materials so we don’t overwrite the originals
        activeMaterials = new Material[baseMaterials.Length];
        for (int i = 0; i < baseMaterials.Length; i++)
        {
            activeMaterials[i] = new Material(baseMaterials[i]);
        }

        // Apply the mesh/material/color
        ApplyCustomization();

        // ——— NEW HAT SETUP ———
        // Instantiate all hat prefabs as children of hatAnchor,
        // but preserve their authored local pos/rot/scale.
        hatInstances = new GameObject[hats.Length];
        for (int i = 0; i < hats.Length; i++)
        {
            // the 'false' flag = worldPositionStays → keep each hat's prefab-local transform
            hatInstances[i] = Instantiate(hats[i], hatAnchor, false);
            hatInstances[i].SetActive(false);
        }
        // ——— end NEW HAT SETUP ———
    }

    public void NextAnimal()
    {
        currentAnimalIndex = (currentAnimalIndex + 1) % animalModels.Length;
        currentColorIndex = 0;
        ApplyCustomization();
    }

    public void NextColor()
    {
        currentColorIndex = (currentColorIndex + 1) % furColors.Length;
        ApplyColor();
    }

    private void ApplyCustomization()
    {
        bodyRenderer.sharedMesh = animalModels[currentAnimalIndex];
        bodyRenderer.GetComponent<Renderer>().material = activeMaterials[currentAnimalIndex];
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (currentColorIndex < furColors.Length)
        {
            Renderer renderer = bodyRenderer.GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("Renderer not found on bodyRenderer GameObject!");
                return;
            }

            Material mat = renderer.material;
            if (!mat.HasProperty("_FurColor"))
            {
                Debug.LogError("Material does NOT have _FurColor property!");
                return;
            }

            mat.SetColor("_FurColor", furColors[currentColorIndex]);
            Debug.Log($"Applied _FurColor: {furColors[currentColorIndex]}");
        }
    }

    // ——— Outfit Section ———

    public void NextHat()
    {
        if (hatInstances.Length == 0)
        {
            Debug.LogWarning("No hats available.");
            return;
        }

        // Turn off current hat (if any)
        if (currentHatIndex >= 0 && currentHatIndex < hatInstances.Length)
            hatInstances[currentHatIndex].SetActive(false);

        // Cycle index (allowing one step to “no hat”)
        currentHatIndex = (currentHatIndex + 1) % (hatInstances.Length + 1);

        // Turn on new hat (if not “no hat”)
        if (currentHatIndex < hatInstances.Length)
        {
            hatInstances[currentHatIndex].SetActive(true);
            Debug.Log("Hat " + hatInstances[currentHatIndex].name + " enabled.");
        }
        else
        {
            Debug.Log("No hat selected.");
        }
    }

    public void SaveCharacter()
    {
        PlayerPrefs.SetString("CharacterName", nameInput.text);
        PlayerPrefs.SetInt("AnimalIndex", currentAnimalIndex);
        PlayerPrefs.SetInt("ColorIndex", currentColorIndex);
        PlayerPrefs.SetInt("HatIndex", currentHatIndex);
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Village");
    }
}



/*
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CharacterCustomizer : MonoBehaviour
{
    public TMP_InputField nameInput;
    public MeshFilter bodyRenderer;
    public Mesh[] animalModels;
    public Material[] baseMaterials;
    public Color[] furColors;
    public GameObject[] hats;

    private int currentAnimalIndex = 0;
    private int currentColorIndex = 0;
    private GameObject[] hatInstances;

    private Material[] activeMaterials;

    private int currentHatIndex = -1;
    private GameObject currentHatInstance;
    public Transform hatAnchor;

    void Start()
    {
        activeMaterials = new Material[baseMaterials.Length];
        for (int i = 0; i < baseMaterials.Length; i++)
        {
            // Create a copy so we don't overwrite the base material
            activeMaterials[i] = new Material(baseMaterials[i]);
        }

        ApplyCustomization();
        // Instantiate hats once, and disable all
        hatInstances = new GameObject[hats.Length];
        for (int i = 0; i < hats.Length; i++)
        {
            hatInstances[i] = Instantiate(hats[i], hatAnchor);
            hatInstances[i].transform.localPosition = Vector3.zero;
            hatInstances[i].transform.localRotation = Quaternion.identity;
            hatInstances[i].transform.localScale = Vector3.one;
            hatInstances[i].SetActive(false); // hide all at start
        }
    }

    public void NextAnimal()
    {
        currentAnimalIndex = (currentAnimalIndex + 1) % animalModels.Length;
        currentColorIndex = 0;
        ApplyCustomization();
    }

    public void NextColor()
    {
        currentColorIndex = (currentColorIndex + 1) % furColors.Length;
        ApplyColor();
    }

    private void ApplyCustomization()
    {
        bodyRenderer.sharedMesh = animalModels[currentAnimalIndex];
        bodyRenderer.GetComponent<Renderer>().material = activeMaterials[currentAnimalIndex];
        ApplyColor();

    }

    private void ApplyColor()
    {
        if (currentColorIndex < furColors.Length)
        {
            Renderer renderer = bodyRenderer.GetComponent<Renderer>();

            if (renderer == null)
            {
                Debug.LogError("Renderer not found on bodyRenderer GameObject!");
                return;
            }

            Material mat = renderer.material;
            Debug.Log($"Current Material: {mat.name}");

            if (!mat.HasProperty("_FurColor"))
            {
                Debug.LogError("Material does NOT have _FurColor property!");
                return;
            }

            Color newColor = furColors[currentColorIndex];
            mat.SetColor("_FurColor", newColor);

            Debug.Log($"Applied _FurColor: {newColor}");
        }
    }

    // Outfit Section


    public void NextHat()
    {
        if (hatInstances.Length == 0)
        {
            Debug.LogWarning("No hats available.");
            return;
        }

        // Turn off current hat
        if (currentHatIndex >= 0 && currentHatIndex < hatInstances.Length)
            hatInstances[currentHatIndex].SetActive(false);

        // Cycle index (including "no hat" state)
        currentHatIndex = (currentHatIndex + 1) % (hatInstances.Length + 1); // +1 = no hat

        // Turn on new hat
        if (currentHatIndex >= 0 && currentHatIndex < hatInstances.Length)
        {
            hatInstances[currentHatIndex].SetActive(true);
            Debug.Log("Hat " + hatInstances[currentHatIndex].name + " enabled.");
        }
        else
        {
            Debug.Log("No hat selected.");
        }
    }


    public void SaveCharacter()
    {
        PlayerPrefs.SetString("CharacterName", nameInput.text);
        PlayerPrefs.SetInt("AnimalIndex", currentAnimalIndex);
        PlayerPrefs.SetInt("ColorIndex", currentColorIndex);
        PlayerPrefs.SetInt("HatIndex", currentHatIndex);

        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Village");
    }

}
*/