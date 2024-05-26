using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioManager audioManager;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip landedSound;
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<PlayerAudio>(this);
    }
    private void Start()
    {
        audioManager = AudioManager.Instance;
    }
    private void OnEnable()
    {
        PlatformMovement.OnPlatformMovementComplete += PlayLandedSound;
        PlayerController.OnPlayerDeath += PlayDeathSound;
        PlayerController.OnPlayerDamage += PlayDamageSound;
    }

    private void OnDisable()
    {
        PlatformMovement.OnPlatformMovementComplete -= PlayLandedSound;
        ServiceLocator.Instance.DeregisterService<PlayerAudio>(this);
        PlayerController.OnPlayerDeath -= PlayDeathSound;
        PlayerController.OnPlayerDamage -= PlayDamageSound;
    }

    public void PlayLandedSound() => audioManager?.PlayOneShot(landedSound);
    public void PlayJumpSound() => audioManager?.PlayOneShot(SFXType.Jump);
    public void PlayDamageSound(int value) => audioManager.PlayOneShot(SFXType.Damage);
    public void PlayDeathSound()
    {
        Debug.Log("Playing death sound");
        audioManager.PlayOneShot(SFXType.Death);
    }
}
