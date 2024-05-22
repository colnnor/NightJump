using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro ;
using System;

public class HudUIManager : MonoBehaviour, IDependencyProvider
{
    [Provide] HudUIManager ProvideUIManager() => this;
    [SerializeField] private UIHealth uiHealth;


    [SerializeField] private TextMeshProUGUI scoreText;
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<HudUIManager>(this);
    }
    private void Start()
    {
        uiHealth = ServiceLocator.Instance.GetService<UIHealth>(this);
    }
    private void OnEnable()
    {
        PlayerController.OnPlayerDamage += TakeDamage;
    }
    private void OnDisable()
    {
        PlayerController.OnPlayerDamage -= TakeDamage;
    }
    public void TakeDamage(int health) => uiHealth.UpdateHealth(health);



}
