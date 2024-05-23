using UnityEngine;

public class PlayerAudio : MonoBehaviour, IDependencyProvider
{
    [Provide] PlayerAudio ProvidePlayerAudio() => this;

    [SerializeField] private AudioManager audioManager;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;
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
        PlayerController.OnPlayerDeath += PlayDeathSound;
        PlayerController.OnPlayerDamage += PlayDamageSound;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.DeregisterService<PlayerAudio>(this);
        PlayerController.OnPlayerDeath -= PlayDeathSound;
        PlayerController.OnPlayerDamage -= PlayDamageSound;
    }
    public void PlayJumpSound() => audioManager.PlayOneShot(SFXType.Jump);
    public void PlayDamageSound(int value) => audioManager.PlayOneShot(SFXType.Damage);
    public void PlayDeathSound()
    {
        Debug.Log("Playing death sound");
        audioManager.PlayOneShot(SFXType.Death);
    }
}
