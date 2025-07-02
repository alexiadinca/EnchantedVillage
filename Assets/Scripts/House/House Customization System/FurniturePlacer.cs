using UnityEngine;

public class FurniturePlacer : MonoBehaviour
{
    public static FurniturePlacer Instance;

    [Header("Furniture Settings")]
    public GameObject[] placeableFurniturePrefabs;
    public Transform furnitureParent;
    public LayerMask placementLayer;

    [Header("Scale Settings")]
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 15f;
    [SerializeField] private float scaleStep = 0.5f;

    private GameObject currentPreview;
    private GameObject selectedPrefab;
    private Vector3 currentScale = Vector3.one * 3f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!BuildModeManager.IsInBuildMode)
            return;

        // Left click on existing placed furniture to pick it up
        if (currentPreview == null && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.CompareTag("PlacedFurniture"))
                {
                    GameObject toPickUp = hit.collider.gameObject;

                    // Get the original prefab safely
                    GameObject prefabToUse = toPickUp.GetComponent<PlacedFurnitureData>()?.originalPrefab;

                    if (prefabToUse == null)
                    {
                        Debug.LogWarning("No original prefab found. Using object itself.");
                        prefabToUse = toPickUp;
                    }

                    Vector3 oldScale = toPickUp.transform.localScale;
                    Quaternion oldRotation = toPickUp.transform.rotation;
                    Vector3 oldPosition = toPickUp.transform.position;

                    // Now safe to destroy
                    Destroy(toPickUp);

                    // Instantiate fresh preview from real prefab
                    selectedPrefab = prefabToUse;
                    currentPreview = Instantiate(prefabToUse);
                    MakeTransparent(currentPreview);

                    currentScale = oldScale;
                    currentPreview.transform.localScale = currentScale;
                    currentPreview.transform.rotation = oldRotation;
                    currentPreview.transform.position = oldPosition;

                    return;
                }


            }
        }

        // Preview movement + input
        if (currentPreview != null)
        {
            Ray moveRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(moveRay, out RaycastHit moveHit, 100f, placementLayer))
            {
                currentPreview.transform.position = moveHit.point;
            }
            else
            {
                currentPreview.transform.position = new Vector3(200f, 1f, 390f);
            }

            if (Input.GetKeyDown(KeyCode.R))
                currentPreview.transform.Rotate(Vector3.up, 45f);

            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
            {
                float delta = scroll * scaleStep;
                currentScale += new Vector3(delta, delta, delta);
                currentScale = Vector3.Max(Vector3.one * minScale, Vector3.Min(currentScale, Vector3.one * maxScale));
                currentPreview.transform.localScale = currentScale;
            }

            if (Input.GetMouseButtonDown(0))
                PlaceFurniture();

            if (Input.GetKeyDown(KeyCode.Escape))
                CancelPlacing();
        }
    }

    public void StartPlacing(GameObject furniturePrefab)
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        selectedPrefab = furniturePrefab;

        currentPreview = Instantiate(selectedPrefab);
        currentPreview.transform.localScale = Vector3.one;
        currentPreview.transform.rotation = Quaternion.identity;

        currentScale = Vector3.one * 3f;
        currentPreview.transform.localScale = currentScale;

        MakeTransparent(currentPreview);
    }

    void PlaceFurniture()
    {
        Vector3 placePos = currentPreview.transform.position;
        if (placePos == Vector3.zero)
            placePos = new Vector3(200f, 1f, 390f);

        GameObject obj = Instantiate(
            selectedPrefab,
            placePos,
            currentPreview.transform.rotation,
            furnitureParent
        );

        obj.transform.localScale = currentScale;
        obj.tag = "PlacedFurniture";

        // Store original prefab so it can be re-picked properly
        var data = obj.AddComponent<PlacedFurnitureData>();
        data.originalPrefab = selectedPrefab;

        // Add collider so it can be clicked
        if (obj.GetComponent<Collider>() == null && obj.GetComponentInChildren<Collider>() == null)
        {
            var col = obj.AddComponent<BoxCollider>();
            col.isTrigger = true;
        }

        Destroy(currentPreview);
        currentPreview = null;
    }

    public void CancelPlacing()
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        currentPreview = null;
    }

    void MakeTransparent(GameObject obj)
    {
        foreach (var rend in obj.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in rend.materials)
            {
                Color color = mat.color;
                color.a = 0.5f;
                mat.color = color;
                mat.SetFloat("_Mode", 2);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }

        foreach (var col in obj.GetComponentsInChildren<Collider>())
            col.enabled = false;
    }
}
