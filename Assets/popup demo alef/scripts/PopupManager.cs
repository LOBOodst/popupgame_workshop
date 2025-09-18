using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    [Header("Popup Settings")]
    public GameObject popupPrefab;
    public RectTransform popupPanel;

    [Header("Spawn Settings")]
    public int minPopups = 4;
    public int maxPopups = 10;
    public KeyCode spawnKey = KeyCode.Space;
    public float spawnDelay = 0.3f;

    [Header("Position Settings")]
    public float minDistance = 150f;
    public Vector2 popupSize = new Vector2(200, 150);

    [Header("Content Settings")]
    public Texture[] popupTextures;

    private List<GameObject> activePopups = new List<GameObject>();
    private List<Vector2> usedPositions = new List<Vector2>();

    void Start()
    {
        if (popupPanel == null)
        {
            Debug.LogError("Popup Panel not assigned!");
            return;
        }

        // Verificar se o popupPanel está configurado corretamente
        Debug.Log($"Popup Panel size: {popupPanel.rect.size}");
        Debug.Log($"Popup Panel position: {popupPanel.anchoredPosition}");

        StartCoroutine(CreateInitialPopups());
    }

    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            StartCoroutine(CreateAdditionalPopups(Random.Range(1, 4)));
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllPopups();
        }

        // Debug: Mostrar informações de posição
        if (Input.GetKeyDown(KeyCode.P))
        {
            DebugPopupPositions();
        }
    }

    IEnumerator CreateInitialPopups()
    {
        int popupCount = Random.Range(minPopups, maxPopups + 1);
        for (int i = 0; i < popupCount; i++)
        {
            CreatePopupAtRandomPosition();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator CreateAdditionalPopups(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreatePopupAtRandomPosition();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void CreatePopupAtRandomPosition()
    {
        if (popupPrefab == null || popupPanel == null)
        {
            Debug.LogError("Popup prefab or panel not assigned!");
            return;
        }

        Vector2 randomPosition = FindAvailablePosition();
        if (randomPosition == Vector2.zero)
        {
            Debug.LogWarning("No available positions found!");
            return;
        }

        // Criar o popup
        GameObject popup = Instantiate(popupPrefab, popupPanel);
        activePopups.Add(popup);
        usedPositions.Add(randomPosition);

        // Configurar o popup IMEDIATAMENTE após a instanciação
        SetupPopup(popup, randomPosition);
    }

    void SetupPopup(GameObject popup, Vector2 position)
    {
        // Forçar a posição primeiro
        SimplePopup popupScript = popup.GetComponent<SimplePopup>();
        if (popupScript != null)
        {
            popupScript.ForcePosition(position);
        }

        // Configurar o RectTransform
        RectTransform popupRect = popup.GetComponent<RectTransform>();
        if (popupRect != null)
        {
            popupRect.sizeDelta = popupSize;
            popupRect.anchoredPosition = position;

            // Reset de transformações indesejadas
            popupRect.localScale = Vector3.one;
            popupRect.localRotation = Quaternion.identity;
        }

        // Configurar conteúdo
        if (popupScript != null && popupTextures != null && popupTextures.Length > 0)
        {
            Texture randomTexture = popupTextures[Random.Range(0, popupTextures.Length)];
            popupScript.SetupPopup(randomTexture);
        }

        Debug.Log($"Popup created at position: {position}");
    }

    Vector2 FindAvailablePosition()
    {
        if (popupPanel == null) return Vector2.zero;

        Vector2 panelSize = popupPanel.rect.size;
        Debug.Log($"Panel Size: {panelSize}");

        // Calcular área disponível considerando o tamanho do popup
        float availableWidth = panelSize.x - popupSize.x;
        float availableHeight = panelSize.y - popupSize.y;

        float minX = -availableWidth / 2;
        float maxX = availableWidth / 2;
        float minY = -availableHeight / 2;
        float maxY = availableHeight / 2;

        Debug.Log($"Available area: X({minX} to {maxX}), Y({minY} to {maxY})");

        int maxAttempts = 50;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            if (IsPositionAvailable(randomPos))
            {
                return randomPos;
            }

            attempts++;
        }

        return Vector2.zero;
    }

    bool IsPositionAvailable(Vector2 position)
    {
        foreach (Vector2 usedPos in usedPositions)
        {
            if (Vector2.Distance(position, usedPos) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    public void ClearAllPopups()
    {
        for (int i = activePopups.Count - 1; i >= 0; i--)
        {
            if (activePopups[i] != null)
            {
                Destroy(activePopups[i]);
            }
        }
        activePopups.Clear();
        usedPositions.Clear();
        Debug.Log("All popups cleared!");
    }

    void DebugPopupPositions()
    {
        Debug.Log("=== POPUP POSITIONS ===");
        for (int i = 0; i < activePopups.Count; i++)
        {
            if (activePopups[i] != null)
            {
                RectTransform rect = activePopups[i].GetComponent<RectTransform>();
                if (rect != null)
                {
                    Debug.Log($"Popup {i}: {rect.anchoredPosition}");
                }
            }
        }
    }
}