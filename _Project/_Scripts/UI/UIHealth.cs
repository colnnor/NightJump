using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHealth : MonoBehaviour, IDependencyProvider
{
    [SerializeField] private GameObject heartsTemplate;

    private PlayerController playerHealth;
    private List<GameObject> hearts = new();
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<UIHealth>(this);
    }
    private void OnDisable()
    {
        ServiceLocator.Instance.DeregisterService<UIHealth>(this);
    }

    private void Start()
    {
        playerHealth = ServiceLocator.Instance.GetService<PlayerController>(this);  
        for (int i = 0; i < playerHealth.Health; i++)
        {
            GameObject heart = Instantiate(heartsTemplate, transform);
            hearts.Add(heart);
        }

        heartsTemplate.SetActive(false);
    }

    public void UpdateHealth(int health)
    {
        for (int i = 0; i < hearts.Count; i++)
            hearts[i].SetActive(i < health);
        
    }
}
