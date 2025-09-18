using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    [Header("Popup Settings")]
    public GameObject popupPrefab;
    public GameObject movingPopupPrefab;
    public RectTransform popupPanel;

    [Header("Spawn Settings")]
    public int minPopups = 4;
    public int maxPopups = 10;
    public KeyCode spawnKey = KeyCode.Space;
    public KeyCode spawnMovingKey = KeyCode.M;
    public float spawnDelay = 0.3f;

    [Header("Position Settings")]
    public float minDistance = 150f;
    public Vector2 popupSize = new Vector2(200, 150);

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
            // 70% chance normal, 30% chance móvel
            bool isMoving = Random.value < 0.3f;
            CreatePopupAtRandomPosition(isMoving);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator CreateAdditionalPopups(int count, bool isMovingPopup)
    {
        for (int i = 0; i < count; i++)
        {
            CreatePopupAtRandomPosition(isMovingPopup);
            yield return new WaitForSeconds(spawnDelay);
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
        if (randomPosition == Vector2.zero)
        {
            Debug.LogWarning("No available positions found!");
            return;
        }

        GameObject popup = Instantiate(prefabToUse, popupPanel);
        activePopups.Add(popup);
        usedPositions.Add(randomPosition);

        if (isMovingPopup)
        {
            SetupMovingPopup(popup, randomPosition);
        }
        else
        {
            SetupNormalPopup(popup, randomPosition);
        }

        Debug.Log($"{(isMovingPopup ? "Moving" : "Normal")} popup created at position: {randomPosition}");
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

    void DebugPopupInfo()
    {
        Debug.Log("=== POPUP INFORMATION ===");
        Debug.Log($"Total Popups: {activePopups.Count}");

        int movingCount = 0;
        int normalCount = 0;

        foreach (GameObject popup in activePopups)
        {
            if (popup != null)
            {
                if (popup.GetComponent<MovingPopup>() != null)
                    movingCount++;
                else
                    normalCount++;
            }
        }

        Debug.Log($"Normal Popups: {normalCount}");
        Debug.Log($"Moving Popups: {movingCount}");
    }

    public void RemovePositionFromList(Vector2 position)
    {
        usedPositions.Remove(position);
    }
}
