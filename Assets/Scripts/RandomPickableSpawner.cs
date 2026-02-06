using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomPickableSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("Vloû sem vöetky prefaby z Assets/Prefabs")]
    public GameObject[] pickablePrefabs;

    [Header("Scene Settings")]
    [Tooltip("N·zov scÈny, kde sa m· spawnovaù (default: GatherScene)")]
    public string targetSceneName = "GatherScene";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Skontroluj Ëi je to spr·vna scÈna
        if (scene.name == targetSceneName)
        {
            ReplacePickablesWithRandomPrefabs();
        }
    }

    private void ReplacePickablesWithRandomPrefabs()
    {
        // Kontrola Ëi m·me prefaby
        if (pickablePrefabs == null || pickablePrefabs.Length == 0)
        {
            Debug.LogError("Nie s˙ nastavenÈ ûiadne prefaby v RandomPickableSpawner!");
            return;
        }

        // N·jdi Pickables rodiËovsk˝ objekt
        GameObject pickablesParent = GameObject.Find("Pickables");

        if (pickablesParent == null)
        {
            Debug.LogError("Nepodarilo sa n·jsù objekt 'Pickables' v scÈne!");
            return;
        }

        // Vytvor list objektov na nahradenie (aby sme sa vyhli problÈmom s indexom)
        List<Transform> pickablesToReplace = new List<Transform>();

        // Najprv n·jdi vöetky objekty ktorÈ treba nahradiù
        foreach (Transform child in pickablesParent.transform)
        {
            if (child.name.StartsWith("Pickable"))
            {
                pickablesToReplace.Add(child);
            }
        }

        // Teraz nahradzuj objekty
        foreach (Transform child in pickablesToReplace)
        {
            // Uloû pozÌciu a rot·ciu pÙvodnÈho objektu
            Vector3 position = child.position;
            Quaternion rotation = child.rotation;
            string originalName = child.name;

            // Vyber n·hodn˝ prefab
            int randomIndex = Random.Range(0, pickablePrefabs.Length);
            GameObject selectedPrefab = pickablePrefabs[randomIndex];

            // Vytvor nov˝ objekt z prefabu (ponech· svoju pÙvodn˙ veækosù z assetu)
            GameObject newPickable = Instantiate(selectedPrefab, position, rotation, pickablesParent.transform);

            // Zachovaj n·zov pre konzistenciu (voliteænÈ)
            newPickable.name = originalName;

            // ZniËit pÙvodn˝ objekt
            Destroy(child.gameObject);

            Debug.Log($"Nahraden˝ {originalName} za {selectedPrefab.name}");
        }

        Debug.Log($"Nahraden˝ch {pickablesToReplace.Count} Pickable objektov!");
    }

    // Pre testovanie - mÙûeö zavolaù manu·lne v editore
    [ContextMenu("Test Replace Pickables")]
    public void TestReplacePickables()
    {
        ReplacePickablesWithRandomPrefabs();
    }
}