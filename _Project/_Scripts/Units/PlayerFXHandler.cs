using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerFXHandler : MonoBehaviour, IDependencyProvider
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private Transform deathFX;
    [SerializeField] private GameObject playerMesh;
    [SerializeField] private ParticleSystem shakeParticles;

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += InstantiateDeathFX;
        PlayerController.OnPlayerDamage += PlayerDamage;
    }
    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= InstantiateDeathFX;
        PlayerController.OnPlayerDamage -= PlayerDamage;
    }
    private void Start()
    {
        playerController = ServiceLocator.Instance.GetService<PlayerController>(this);
    }
    public void InstantiateDeathFX()
    {
        Instantiate(deathFX, transform.position, Quaternion.identity);
    }
    public void PlayShakeParticles()
    {
        shakeParticles.Play();
    }

    public void EnablePlayerMesh(bool visible = true)
    {
        playerMesh.SetActive(visible);
    }
    public void FlickerPlayerVisibility()
    {
        FlickerPlayerVisibilityAsync(); 
    }
    public async void FlickerPlayerVisibilityAsync()
    {
        for (int i = 0; i < 5; i++)
        {
            playerMesh.SetActive(!playerMesh.activeSelf);
            await Awaitable.WaitForSecondsAsync(0.15f);
        }
        playerMesh.SetActive(true);
    }
    
    /// <summary>
    /// Plays the death FX and disables the player mesh for a short duration. If the player is still alive after the duration, the player mesh will flicker.
    /// </summary>
    /// <returns></returns>
    void PlayerDamage(int i)
    {
        EnablePlayerMesh(false);
        InstantiateDeathFX();
    }
}
