using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFreezeManager : MonoBehaviour
{
    public static EnemyFreezeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void FreezeEnemies(float duration)
    {
        StartCoroutine(FreezeEnemiesCoroutine(duration));
    }

    private IEnumerator FreezeEnemiesCoroutine(float duration)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // uložíme pôvodné constraints (ak chceš ešte aj fyzicky locknúť)
        Dictionary<Rigidbody, RigidbodyConstraints> saved = new();

        List<Enemy> enemyScripts = new();

        foreach (GameObject go in enemies)
        {
            Enemy e = go.GetComponent<Enemy>();
            if (e != null)
            {
                e.SetFrozen(true);
                enemyScripts.Add(e);
            }

            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb != null)
            {
                saved[rb] = rb.constraints;

                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.constraints =
                    RigidbodyConstraints.FreezePositionX |
                    RigidbodyConstraints.FreezePositionY |
                    RigidbodyConstraints.FreezePositionZ |
                    RigidbodyConstraints.FreezeRotation;
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (Enemy e in enemyScripts)
        {
            if (e != null) e.SetFrozen(false);
        }

        foreach (var pair in saved)
        {
            if (pair.Key != null) pair.Key.constraints = pair.Value;
        }
    }
}
