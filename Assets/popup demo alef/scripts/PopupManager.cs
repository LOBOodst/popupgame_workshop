using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    [Header("Popup Settings")]
    public GameObject popupPrefab;
    public GameObject movingPopupPrefab;
    public GameObject adminPopupPrefab;
    public RectTransform popupPanel;

    [Header("Spawn Settings")]
    public int minPopups = 4;
    public int maxPopups = 10;
    public KeyCode spawnKey = KeyCode.Space;
    public KeyCode spawnMovingKey = KeyCode.M;
    public KeyCode spawnAdminKey = KeyCode.A;
    public float spawnDelay = 0.3f;
    public float adminPopupChance = 0.2f; // 20% de chance de spawnar AdminPopup

    [Header("Position Settings")]
    public float minDistance = 150f;
    public Vector2 popupSize = new Vector2(200, 150);
    public Vector2 adminPopupSize = new Vector2(400, 300);

    [Header("Moving Popup Settings")]
    public int movesPerPopup = 3;
    public float moveDuration = 0.5f;
    public float timeBetweenMoves = 1f;
    public Vector2 moveAreaMin = new Vector2(-400, -200);
    public Vector2 moveAreaMax = new Vector2(400, 200);

    [Header("Content Settings")]
    public Texture[] popupTextures;

    private List<GameObject> activePopups = new List<GameObject>();
    private List<Vector2> usedPositions = new List<Vector2>();

    private bool hasAdminPopup = false;

    void Start()
    {
        if (popupPanel == null)
        {
            Debug.LogError("Popup Panel not assigned!");
            return;
        }

        StartCoroutine(CreateInitialPopups());
    }

    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            StartCoroutine(CreateAdditionalPopups(Random.Range(1, 4), false));
        }

        if (Input.GetKeyDown(spawnMovingKey))
        {
            StartCoroutine(CreateAdditionalPopups(Random.Range(1, 3), true));
        }

        if (Input.GetKeyDown(spawnAdminKey))
        {
            SpawnAdminPopup();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllPopups();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DebugPopupInfo();
        }
    }

    IEnumerator CreateInitialPopups()
    {
        int popupCount = Random.Range(minPopups, maxPopups + 1);

        for (int i = 0; i < popupCount; i++)
        {
            bool isMoving = Random.value < 0.3f;
            CreatePopupAtRandomPosition(isMoving);
            yield return new WaitForSeconds(spawnDelay);
        }

        // Chance de spawnar AdminPopup automaticamente no início
        if (Random.value < adminPopupChance && !hasAdminPopup)
        {
            yield return new WaitForSeconds(spawnDelay * 2);
            SpawnAdminPopup();
        }
    }

    IEnumerator CreateAdditionalPopups(int count, bool isMovingPopup)
    {
        for (int i = 0; i < count; i++)
        {
            CreatePopupAtRandomPosition(isMovingPopup);
            yield return new WaitForSeconds(spawnDelay);
        }

        // Chance de spawnar AdminPopup quando spawnar popups adicionais
        if (Random.value < adminPopupChance / 2 && !hasAdminPopup)
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnAdminPopup();
        }
    }

    void CreatePopupAtRandomPosition(bool isMovingPopup)
    {
        GameObject prefabToUse = isMovingPopup ? movingPopupPrefab : popupPrefab;

        if (prefabToUse == null || popupPanel == null)
        {
            Debug.LogError("Popup prefab or panel not assigned!");
            return;
        }

        Vector2 randomPosition = FindAvailablePosition();
        if (randomPosition == Vector2.zero) return;

        GameObject popup = Instantiate(prefabToUse, popupPanel);
        activePopups.Add(popup);
        usedPositions.Add(randomPosition);

        if (isMovingPopup)
            SetupMovingPopup(popup, randomPosition);
        else
            SetupNormalPopup(popup, randomPosition);
    }

    void SetupNormalPopup(GameObject popup, Vector2 position)
    {
        RectTransform popupRect = popup.GetComponent<RectTransform>();
        if (popupRect != null)
        {
            popupRect.sizeDelta = popupSize;
            popupRect.anchoredPosition = position;
        }

        SimplePopup popupScript = popup.GetComponent<SimplePopup>();
        if (popupScript != null && popupTextures != null && popupTextures.Length > 0)
        {
            Texture randomTexture = popupTextures[Random.Range(0, popupTextures.Length)];
            popupScript.SetupPopup(randomTexture);
        }
    }

    void SetupMovingPopup(GameObject popup, Vector2 position)
    {
        RectTransform popupRect = popup.GetComponent<RectTransform>();
        if (popupRect != null)
        {
            popupRect.sizeDelta = popupSize;
            popupRect.anchoredPosition = position;
        }

        MovingPopup movingPopup = popup.GetComponent<MovingPopup>();
        if (movingPopup != null)
        {
            movingPopup.SetInitialPosition(position);
            movingPopup.maxMoves = movesPerPopup;
            movingPopup.moveDuration = moveDuration;
            movingPopup.timeBetweenMoves = timeBetweenMoves;
            movingPopup.moveAreaMin = moveAreaMin;
            movingPopup.moveAreaMax = moveAreaMax;

            if (popupTextures != null && popupTextures.Length > 0)
            {
                Texture randomTexture = popupTextures[Random.Range(0, popupTextures.Length)];
                movingPopup.SetupPopup(randomTexture);
            }
        }
    }

    Vector2 FindAvailablePosition()
    {
        if (popupPanel == null) return Vector2.zero;

        Vector2 panelSize = popupPanel.rect.size;
        Vector2 halfPanelSize = panelSize / 2f;
        Vector2 halfPopupSize = popupSize / 2f;

        float minX = -halfPanelSize.x + halfPopupSize.x;
        float maxX = halfPanelSize.x - halfPopupSize.x;
        float minY = -halfPanelSize.y + halfPopupSize.y;
        float maxY = halfPanelSize.y - halfPopupSize.y;

        int maxAttempts = 50;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            if (IsPositionAvailable(randomPos)) return randomPos;
            attempts++;
        }

        return Vector2.zero;
    }

    bool IsPositionAvailable(Vector2 position)
    {
        foreach (Vector2 usedPos in usedPositions)
        {
            if (Vector2.Distance(position, usedPos) < minDistance)
                return false;
        }
        return true;
    }

    public void RemovePositionFromList(Vector2 position)
    {
        usedPositions.Remove(position);
    }

    public void ClearAllPopups()
    {
        for (int i = activePopups.Count - 1; i >= 0; i--)
        {
            if (activePopups[i] != null) Destroy(activePopups[i]);
        }
        activePopups.Clear();
        usedPositions.Clear();
        hasAdminPopup = false;
    }

    public void DebugPopupInfo()
    {
        Debug.Log("=== POPUP INFORMATION ===");
        Debug.Log($"Total Popups: {activePopups.Count}");
        Debug.Log($"AdminPopup Active: {hasAdminPopup}");
    }

    public void SpawnAdminPopup()
    {
        if (hasAdminPopup)
        {
            Debug.Log("Já existe um AdminPopup aberto!");
            return;
        }
        if (adminPopupPrefab == null || popupPanel == null) return;

        Vector2 randomPosition = FindAvailablePositionForAdminPopup();
        if (randomPosition == Vector2.zero)
        {
            Debug.Log("Não foi possível encontrar posição para AdminPopup!");
            return;
        }

        GameObject popup = Instantiate(adminPopupPrefab, popupPanel);
        activePopups.Add(popup);
        usedPositions.Add(randomPosition);

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        if (popupRect != null)
        {
            popupRect.sizeDelta = adminPopupSize;
            popupRect.anchoredPosition = randomPosition;
        }

        hasAdminPopup = true;
        Debug.Log("AdminPopup spawnado!");
    }

    Vector2 FindAvailablePositionForAdminPopup()
    {
        if (popupPanel == null) return Vector2.zero;

        Vector2 panelSize = popupPanel.rect.size;
        Vector2 halfPanelSize = panelSize / 2f;
        Vector2 halfPopupSize = adminPopupSize / 2f;

        float minX = -halfPanelSize.x + halfPopupSize.x;
        float maxX = halfPanelSize.x - halfPopupSize.x;
        float minY = -halfPanelSize.y + halfPopupSize.y;
        float maxY = halfPanelSize.y - halfPopupSize.y;

        int maxAttempts = 100; // Aumentei para 100 tentativas
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            if (IsPositionAvailableForAdminPopup(randomPos)) return randomPos;
            attempts++;
        }

        return Vector2.zero;
    }

    bool IsPositionAvailableForAdminPopup(Vector2 position)
    {
        foreach (Vector2 usedPos in usedPositions)
        {
            if (Vector2.Distance(position, usedPos) < (minDistance * 2f)) // Distância maior para AdminPopup
                return false;
        }
        return true;
    }

    public void AdminPopupClosed()
    {
        hasAdminPopup = false;
        Debug.Log("AdminPopup fechado!");
    }

    public void SpawnExtraPopups()
    {
        StartCoroutine(CreateAdditionalPopups(Random.Range(1, 3), false));
        StartCoroutine(CreateAdditionalPopups(Random.Range(1, 2), true));
    }

    public void RemoveRandomPopups(int count)
    {
        int popupsToRemove = Mathf.Min(count, activePopups.Count);
        int removedCount = 0;

        // Lista temporária para evitar modificar a lista durante a iteração
        List<GameObject> popupsToRemoveList = new List<GameObject>();

        // Encontra popups para remover (excluindo AdminPopup)
        foreach (GameObject popup in activePopups)
        {
            if (popup != null && popup.GetComponent<AdminPopup>() == null && removedCount < count)
            {
                popupsToRemoveList.Add(popup);
                removedCount++;
            }
        }

        // Remove os popups selecionados
        foreach (GameObject popupToRemove in popupsToRemoveList)
        {
            // Remove a posição da lista de posições usadas
            RectTransform popupRect = popupToRemove.GetComponent<RectTransform>();
            if (popupRect != null)
            {
                RemovePositionFromList(popupRect.anchoredPosition);
            }

            // Destroi o popup
            Destroy(popupToRemove);
            activePopups.Remove(popupToRemove);

            Debug.Log("Popup removido por código admin correto!");
        }
    }
}