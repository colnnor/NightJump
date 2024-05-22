using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public Transform gemParticles;
    public GameObject gemModel;

    public static event Action<bool> GemCollected;

    bool enemyCollected;

    private void OnEnable()
    {
        GameManager.OnGameEnd += DestroyGem;
        LightManager.LightEnabled += ShowMesh;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd -= DestroyGem;
        LightManager.LightEnabled -= ShowMesh;
    }
    public void CollectGem()
    {
        Instantiate(gemParticles, transform.position, Quaternion.identity);
        SFXType collectSFX = enemyCollected ? SFXType.EnemyCollect : SFXType.Collect;
        AudioManager.Instance.PlayOneShot(collectSFX);
        enemyCollected = false;
    }

    public void ShowMesh(bool value)
    {
        gemModel.SetActive(value);
    }

    public void DestroyGem()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            enemyCollected = false;
            CollectGem();
            GemCollected?.Invoke(true);
        }
        else if(other.GetComponent<EnemyController>())
        {
            enemyCollected = true;
            CollectGem();
            GemCollected?.Invoke(false);
        }
    }
}
