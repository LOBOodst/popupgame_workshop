using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    [Header("Paramètres des Popups")]
    public GameObject popupPrefab;
    public GameObject movingPopupPrefab;
    public GameObject adminPopupPrefab;
    public RectTransform popupPanel;

    [Header("Paramètres d'Apparition")]
    public int minPopups = 4;
    public int maxPopups = 10;
    public KeyCode spawnKey = KeyCode.Space;
    public KeyCode spawnMovingKey = KeyCode.M;
    public KeyCode spawnAdminKey = KeyCode.A;
    public float spawnDelay = 0.3f;
    public float adminPopupChance = 0.2f; // 20% de chance de faire apparaître un AdminPopup

    [Header("Paramètres de Position")]
    public float minDistance = 150f;
    public Vector2 popupSize = new Vector2(200, 150);
    public Vector2 adminPopupSize = new Vector2(400, 300);

    [Header("Paramètres des Popups Mouvants")]
    public int movesPerPopup = 3;
    public float moveDuration = 0.5f;
    public float timeBetweenMoves = 1f;
    public Vector2 moveAreaMin = new Vector2(-400, -200);
    public Vector2 moveAreaMax = new Vector2(400, 200);

    [Header("Paramètres de Contenu")]
    public Texture[] popupTextures;

    private List<GameObject> activePopups = new List<GameObject>();
    private List<Vector2> usedPositions = new List<Vector2>();

    private bool hasAdminPopup = false;

    void Start()
    {
        if (popupPanel == null)
        {
            Debug.LogError("Panel Popup non assigné !");
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

        // Chance de faire apparaître automatiquement un AdminPopup au début
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

        // Chance de faire apparaître un AdminPopup lors de l'ajout de popups
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
            Debug.LogError("Prefab Popup ou panel non assigné !");
            return;
        }

        Vector2 randomPosition = FindAvailablePosition();
        if (randomPosition == Vector2.zero) return;

        GameObject popup = Instantiate(prefabToUse, popupPanel);

        // Met le nouveau popup au-dessus des précédents
        popup.transform.SetAsLastSibling();

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

        // Retourne une position aléatoire sans vérifier les chevauchements
        return new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
    }

    bool IsPositionAvailable(Vector2 position)
    {
        // Permet le chevauchement des popups - toujours disponible
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
        Debug.Log("=== INFORMATIONS SUR LES POPUPS ===");
        Debug.Log($"Total des Popups: {activePopups.Count}");
        Debug.Log($"AdminPopup Actif: {hasAdminPopup}");
    }

    public void SpawnAdminPopup()
    {
        if (hasAdminPopup)
        {
            Debug.Log("Il y a déjà un AdminPopup ouvert !");
            return;
        }
        if (adminPopupPrefab == null || popupPanel == null) return;

        Vector2 randomPosition = FindAvailablePositionForAdminPopup();
        if (randomPosition == Vector2.zero)
        {
            Debug.Log("Impossible de trouver une position pour l'AdminPopup !");
            return;
        }

        GameObject popup = Instantiate(adminPopupPrefab, popupPanel);

        // Met l'AdminPopup au-dessus de tous les autres popups
        popup.transform.SetAsLastSibling();

        activePopups.Add(popup);
        usedPositions.Add(randomPosition);

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        if (popupRect != null)
        {
            popupRect.sizeDelta = adminPopupSize;
            popupRect.anchoredPosition = randomPosition;
        }

        hasAdminPopup = true;
        Debug.Log("AdminPopup apparu !");
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

        // Retourne une position aléatoire pour l'AdminPopup
        return new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
    }

    bool IsPositionAvailableForAdminPopup(Vector2 position)
    {
        // Permet le chevauchement même pour l'AdminPopup
        return true;
    }

    public void AdminPopupClosed()
    {
        hasAdminPopup = false;
        Debug.Log("AdminPopup fermé !");
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

        // Liste temporaire pour éviter de modifier la liste pendant l'itération
        List<GameObject> popupsToRemoveList = new List<GameObject>();

        // Trouve les popups à supprimer (en excluant l'AdminPopup)
        foreach (GameObject popup in activePopups)
        {
            if (popup != null && popup.GetComponent<AdminPopup>() == null && removedCount < count)
            {
                popupsToRemoveList.Add(popup);
                removedCount++;
            }
        }

        // Supprime les popups sélectionnés
        foreach (GameObject popupToRemove in popupsToRemoveList)
        {
            // Supprime la position de la liste des positions utilisées
            RectTransform popupRect = popupToRemove.GetComponent<RectTransform>();
            if (popupRect != null)
            {
                RemovePositionFromList(popupRect.anchoredPosition);
            }

            // Détruit le popup
            Destroy(popupToRemove);
            activePopups.Remove(popupToRemove);

            Debug.Log("Popup supprimé grâce au code admin correct !");
        }
    }
}