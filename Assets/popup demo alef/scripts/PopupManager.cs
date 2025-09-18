using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    [Header("Paramètres des Popups")]
    public GameObject popupPrefab;
    public GameObject movingPopupPrefab;
    public GameObject adminPopupPrefab;
    public GameObject mathPopupPrefab;
    public RectTransform popupPanel;

    [Header("Système de Score")]
    public Text scoreText;
    public Text waveText;
    public Text countdownText;
    public int normalPopupScore = 10;
    public int movingPopupScore = 20;
    public int adminPopupScore = 50;
    public int mathPopupScore = 30;
    private int currentScore = 0;
    private int currentWave = 0;
    private int popupsInCurrentWave = 0;
    private int popupsDestroyedInWave = 0;
    private float waveCountdown = 0f;
    private float waveTimer = 0f;

    [Header("Système de Waves")]
    public int[] wavePopups = { 10, 15, 20, 25, 30, 40, 50, 60 };
    public float waveDuration = 10f; // 10 secondes par vague
    public float timeBetweenWaves = 3f; // 3 secondes entre les waves
    public bool autoStartNextWave = true;
    public float waveDifficultyMultiplier = 1.1f;
    public int minPopups = 4;
    public int maxPopups = 10;

    [Header("Chances d'Apparition")]
    [Range(0f, 1f)] public float normalPopupChance = 0.7f;
    [Range(0f, 1f)] public float movingPopupChance = 0.3f;
    [Range(0f, 1f)] public float adminPopupChance = 0.2f;
    [Range(0f, 1f)] public float mathPopupChance = 0.15f;

    [Header("Limites des Popups Spéciaux")]
    public int maxAdminPopups = 3;
    public int maxMathPopups = 2;

    [Header("Paramètres d'Apparition")]
    public float spawnDelay = 0.3f;

    [Header("Paramètres de Position")]
    public Vector2 popupSize = new Vector2(200, 150);
    public Vector2 adminPopupSize = new Vector2(400, 300);
    public Vector2 mathPopupSize = new Vector2(350, 250);

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

    private int currentAdminPopups = 0;
    private int currentMathPopups = 0;
    private bool waveInProgress = false;
    private bool isSpawningWave = false;
    private bool isWaveCountdown = false;

    void Start()
    {
        if (popupPanel == null)
        {
            Debug.LogError("Panel Popup non assigné !");
            return;
        }

        UpdateScoreDisplay();
        UpdateWaveDisplay();
        UpdateCountdownDisplay();
        StartCoroutine(GameStartRoutine());
    }

    void Update()
    {
        // Gestion du timer de vague
        if (waveInProgress)
        {
            waveTimer -= Time.deltaTime;
            UpdateCountdownDisplay();

            if (waveTimer <= 0f)
            {
                waveInProgress = false;
                Debug.Log($"Vague {currentWave} terminée (temps écoulé)!");
                StartWaveCountdown();
            }
        }

        // Gestion du compte à rebours entre les vagues
        if (isWaveCountdown)
        {
            waveCountdown -= Time.deltaTime;
            UpdateCountdownDisplay();

            if (waveCountdown <= 0f)
            {
                isWaveCountdown = false;
                UpdateCountdownDisplay();
                StartNextWave();
            }
        }
    }

    IEnumerator GameStartRoutine()
    {
        // Crée les popups initiaux
        yield return StartCoroutine(CreateInitialPopups());

        // Démarre le gestionnaire de vagues
        StartCoroutine(WaveManager());

        // Démarre le spawner de popups spéciaux avec intervalles fixes
        StartCoroutine(SpecialPopupsSpawner());
    }

    IEnumerator WaveManager()
    {
        // Attend un peu avant de commencer la première vague
        yield return new WaitForSeconds(3f);

        // Démarre automatiquement la première vague
        if (autoStartNextWave)
        {
            StartNextWave();
        }
    }

    IEnumerator SpecialPopupsSpawner()
    {
        // Attendre un peu au début du jeu
        yield return new WaitForSeconds(5f);

        while (true)
        {
            // Spawn AdminPopup toutes les 10 secondes
            yield return new WaitForSeconds(10f);

            if (currentAdminPopups < maxAdminPopups)
            {
                SpawnAdminPopup();
            }

            // Spawn MathPopup toutes les 10 secondes (décalé de 5 secondes par rapport à Admin)
            yield return new WaitForSeconds(5f);

            if (currentMathPopups < maxMathPopups)
            {
                SpawnMathPopup();
            }

            // Attendre 5 secondes supplémentaires pour compléter le cycle de 20 secondes
            yield return new WaitForSeconds(5f);
        }
    }

    public void StartNextWave()
    {
        if (currentWave >= wavePopups.Length)
        {
            Debug.Log("Toutes les vagues sont terminées!");
            if (countdownText != null) countdownText.text = "Jeu terminé!";
            return;
        }

        currentWave++;
        popupsInCurrentWave = wavePopups[currentWave - 1];
        popupsDestroyedInWave = 0;
        waveInProgress = false;
        isSpawningWave = true;
        isWaveCountdown = false;
        waveTimer = waveDuration;

        Debug.Log($"Début de la vague {currentWave}! Durée: {waveDuration} secondes");
        UpdateWaveDisplay();
        UpdateCountdownDisplay();

        // Ajuste la difficulté selon la vague
        float difficulty = 1f + ((currentWave - 1) * 0.1f);
        movingPopupChance = Mathf.Min(0.5f, 0.3f * difficulty);
        adminPopupChance = Mathf.Min(0.3f, 0.2f * difficulty);
        mathPopupChance = Mathf.Min(0.25f, 0.15f * difficulty);

        StartCoroutine(SpawnWavePopups());
    }

    IEnumerator SpawnWavePopups()
    {
        int popupsToSpawn = popupsInCurrentWave;
        int batchSize = Mathf.Min(5, popupsToSpawn);

        while (popupsToSpawn > 0)
        {
            int spawnCount = Mathf.Min(batchSize, popupsToSpawn);

            for (int i = 0; i < spawnCount; i++)
            {
                bool isMoving = Random.value < movingPopupChance;
                CreatePopupAtRandomPosition(isMoving);
                yield return new WaitForSeconds(spawnDelay);
            }

            popupsToSpawn -= spawnCount;

            if (popupsToSpawn > 0)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        Debug.Log($"Tous les popups de la vague {currentWave} ont été spawnés!");
        waveInProgress = true;
        isSpawningWave = false;
        UpdateWaveDisplay();
    }

    void StartWaveCountdown()
    {
        isWaveCountdown = true;
        waveCountdown = timeBetweenWaves;
        Debug.Log($"Début du compte à rebours: {timeBetweenWaves} secondes jusqu'à la prochaine vague...");
        UpdateCountdownDisplay();
    }

    IEnumerator CreateInitialPopups()
    {
        int popupCount = Random.Range(minPopups, maxPopups + 1);

        for (int i = 0; i < popupCount; i++)
        {
            bool isMoving = Random.value < movingPopupChance;
            CreatePopupAtRandomPosition(isMoving);
            yield return new WaitForSeconds(spawnDelay);
        }

        // Spawn initial d'AdminPopup après 2 secondes
        yield return new WaitForSeconds(2f);
        if (currentAdminPopups < maxAdminPopups)
        {
            SpawnAdminPopup();
        }

        // Spawn initial de MathPopup après 7 secondes
        yield return new WaitForSeconds(5f);
        if (currentMathPopups < maxMathPopups)
        {
            SpawnMathPopup();
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    void UpdateWaveDisplay()
    {
        if (waveText != null)
        {
            if (isSpawningWave)
            {
                waveText.text = $"Vague: {currentWave}/{wavePopups.Length}\nSpawn en cours...";
            }
            else if (waveInProgress)
            {
                waveText.text = $"Vague: {currentWave}/{wavePopups.Length}\nTemps: {Mathf.CeilToInt(waveTimer)}s";
            }
            else if (isWaveCountdown)
            {
                waveText.text = $"Vague: {currentWave}/{wavePopups.Length}\nTerminée!";
            }
            else if (currentWave > 0)
            {
                waveText.text = $"Vague: {currentWave}/{wavePopups.Length}\nPrête!";
            }
            else
            {
                waveText.text = "Prêt à commencer!";
            }
        }
    }

    void UpdateCountdownDisplay()
    {
        if (countdownText != null)
        {
            if (waveInProgress)
            {
                countdownText.text = $"Temps restant: {Mathf.CeilToInt(waveTimer)}s";
            }
            else if (isWaveCountdown)
            {
                countdownText.text = $"Prochaine vague dans: {Mathf.CeilToInt(waveCountdown)}s";
            }
            else if (currentWave >= wavePopups.Length)
            {
                countdownText.text = "Toutes les vagues terminées!";
            }
            else
            {
                countdownText.text = "";
            }
        }
    }

    public void AddScore(int points, string popupType)
    {
        currentScore += points;
        popupsDestroyedInWave++;

        Debug.Log($"+{points} points pour un {popupType}! ({popupsDestroyedInWave}/{popupsInCurrentWave})");

        UpdateScoreDisplay();
        UpdateWaveDisplay();
    }

    public void RegisterSpecialPopup(string popupType, bool isAdding)
    {
        if (popupType == "AdminPopup")
        {
            currentAdminPopups += isAdding ? 1 : -1;
            currentAdminPopups = Mathf.Max(0, currentAdminPopups);
        }
        else if (popupType == "MathPopup")
        {
            currentMathPopups += isAdding ? 1 : -1;
            currentMathPopups = Mathf.Max(0, currentMathPopups);
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

        Vector2 randomPosition = GetRandomPositionForPopup(popupSize);
        if (randomPosition == Vector2.zero) return;

        GameObject popup = Instantiate(prefabToUse, popupPanel);
        popup.transform.SetAsLastSibling();

        activePopups.Add(popup);
        usedPositions.Add(randomPosition);

        PopupScore popupScore = popup.AddComponent<PopupScore>();
        popupScore.popupManager = this;
        popupScore.scoreValue = isMovingPopup ? movingPopupScore : normalPopupScore;
        popupScore.popupType = isMovingPopup ? "Popup Mouvant" : "Popup Normal";

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

    Vector2 GetRandomPositionForPopup(Vector2 popupSize)
    {
        if (popupPanel == null) return Vector2.zero;

        Vector2 panelSize = popupPanel.rect.size;
        Vector2 halfPanelSize = panelSize / 2f;
        Vector2 halfPopupSize = popupSize / 2f;

        float minX = -halfPanelSize.x + halfPopupSize.x;
        float maxX = halfPanelSize.x - halfPopupSize.x;
        float minY = -halfPanelSize.y + halfPopupSize.y;
        float maxY = halfPanelSize.y - halfPopupSize.y;

        return new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
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
        currentAdminPopups = 0;
        currentMathPopups = 0;

        // Reset wave progress if cleared during wave
        if (waveInProgress || isSpawningWave || isWaveCountdown)
        {
            waveInProgress = false;
            isSpawningWave = false;
            isWaveCountdown = false;
            popupsDestroyedInWave = 0;
            UpdateWaveDisplay();
            UpdateCountdownDisplay();
        }
    }

    public void SpawnAdminPopup()
    {
        if (currentAdminPopups >= maxAdminPopups)
        {
            Debug.Log("Limite maximale de AdminPopups atteinte!");
            return;
        }
        if (adminPopupPrefab == null || popupPanel == null) return;

        Vector2 randomPosition = GetRandomPositionForPopup(adminPopupSize);
        if (randomPosition == Vector2.zero)
        {
            Debug.Log("Impossible de trouver une position pour l'AdminPopup !");
            return;
        }

        GameObject popup = Instantiate(adminPopupPrefab, popupPanel);
        popup.transform.SetAsLastSibling();

        PopupScore popupScore = popup.AddComponent<PopupScore>();
        popupScore.popupManager = this;
        popupScore.scoreValue = adminPopupScore;
        popupScore.popupType = "AdminPopup";

        activePopups.Add(popup);
        usedPositions.Add(randomPosition);
        currentAdminPopups++;

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        if (popupRect != null)
        {
            popupRect.sizeDelta = adminPopupSize;
            popupRect.anchoredPosition = randomPosition;
        }

        Debug.Log("AdminPopup apparu !");
    }

    public void AdminPopupClosed()
    {
        currentAdminPopups--;
        currentAdminPopups = Mathf.Max(0, currentAdminPopups);
        Debug.Log("AdminPopup fermé !");
    }

    public void SpawnMathPopup()
    {
        if (currentMathPopups >= maxMathPopups)
        {
            Debug.Log("Limite maximale de MathPopups atteinte!");
            return;
        }
        if (mathPopupPrefab == null || popupPanel == null) return;

        Vector2 randomPosition = GetRandomPositionForPopup(mathPopupSize);
        if (randomPosition == Vector2.zero)
        {
            Debug.Log("Impossible de trouver une position pour le MathPopup !");
            return;
        }

        GameObject popup = Instantiate(mathPopupPrefab, popupPanel);
        popup.transform.SetAsLastSibling();

        PopupScore popupScore = popup.AddComponent<PopupScore>();
        popupScore.popupManager = this;
        popupScore.scoreValue = mathPopupScore;
        popupScore.popupType = "MathPopup";

        activePopups.Add(popup);
        usedPositions.Add(randomPosition);
        currentMathPopups++;

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        if (popupRect != null)
        {
            popupRect.sizeDelta = mathPopupSize;
            popupRect.anchoredPosition = randomPosition;
        }

        Debug.Log("MathPopup apparu !");
    }

    public void MathPopupClosed()
    {
        currentMathPopups--;
        currentMathPopups = Mathf.Max(0, currentMathPopups);
        Debug.Log("MathPopup fermé !");
    }

    public void RemoveRandomPopups(int count)
    {
        int popupsToRemove = Mathf.Min(count, activePopups.Count);
        int removedCount = 0;

        List<GameObject> popupsToRemoveList = new List<GameObject>();

        foreach (GameObject popup in activePopups)
        {
            if (popup != null && removedCount < count)
            {
                // Vérifie si c'est un popup spécial
                AdminPopup adminPopup = popup.GetComponent<AdminPopup>();
                MathPopup mathPopup = popup.GetComponent<MathPopup>();

                if (adminPopup != null)
                {
                    currentAdminPopups--;
                }
                else if (mathPopup != null)
                {
                    currentMathPopups--;
                }

                popupsToRemoveList.Add(popup);
                removedCount++;
            }
        }

        foreach (GameObject popupToRemove in popupsToRemoveList)
        {
            RectTransform popupRect = popupToRemove.GetComponent<RectTransform>();
            if (popupRect != null)
            {
                RemovePositionFromList(popupRect.anchoredPosition);
            }

            Destroy(popupToRemove);
            activePopups.Remove(popupToRemove);
            Debug.Log("Popup supprimé !");
        }
    }

    public void SpawnExtraPopups()
    {
        // Spawn 3 popups supplémentaires quand le joueur échoue
        for (int i = 0; i < 3; i++)
        {
            bool isMoving = Random.value < 0.3f;
            CreatePopupAtRandomPosition(isMoving);
        }
    }
}