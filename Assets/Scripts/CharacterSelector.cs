using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelector : MonoBehaviour
{
    public Button previousButton;               // Button für vorherigen Charakter
    public Button nextButton;                   // Button für nächsten Charakter
    public Transform displayPosition;           // Position, an der der Charakter angezeigt wird
    private List<GameObject> characterPrefabs;  // Liste mit allen geladenen Prefabs
    private GameObject currentCharacter;        // Das aktuell angezeigte Charakter-Objekt
    private int currentIndex = 0;               // Der Index des aktuell angezeigten Charakters

    void Start()
    {
        // Lade alle Charakter-Modelle aus den Unterordnern
        LoadCharacterPrefabs();

        // Zeige das erste Charakter-Model an
        if (characterPrefabs.Count > 0)
        {
            UpdateCharacter();
        }

        // Buttons mit Funktionen verbinden
        previousButton.onClick.AddListener(ShowPreviousCharacter);
        nextButton.onClick.AddListener(ShowNextCharacter);
    }

    void LoadCharacterPrefabs()
    {
        // Initialisiere die Liste
        characterPrefabs = new List<GameObject>();

        // Lade alle Prefabs aus den Unterordnern
        string[] folders = { "Devil", "Elf", "Human", "Skeleton" };
        foreach (string folder in folders)
        {
            GameObject[] prefabsInFolder = Resources.LoadAll<GameObject>($"Addons/BasicPack/2_Prefab/{folder}");
            foreach (var prefab in prefabsInFolder)
            {
                characterPrefabs.Add(prefab);
            }
        }

        if (characterPrefabs.Count == 0)
        {
            Debug.LogError("Keine Charakter-Prefabs gefunden! Stelle sicher, dass die Prefabs im richtigen Resources-Ordner liegen.");
        }
    }

    void UpdateCharacter()
    {
        // Entferne den aktuellen Charakter, falls vorhanden
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
        }

        // Instanziiere den neuen Charakter
        currentCharacter = Instantiate(characterPrefabs[currentIndex], displayPosition.position, Quaternion.identity);
        currentCharacter.transform.SetParent(displayPosition); // Optional, um Ordnung zu halten
    }

    void ShowPreviousCharacter()
    {
        if (characterPrefabs.Count == 0) return;

        currentIndex = (currentIndex - 1 + characterPrefabs.Count) % characterPrefabs.Count;
        UpdateCharacter();
    }

    void ShowNextCharacter()
    {
        if (characterPrefabs.Count == 0) return;

        currentIndex = (currentIndex + 1) % characterPrefabs.Count;
        UpdateCharacter();
    }
}
