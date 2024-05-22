using UnityEngine;

public class PrefabsProvider : MonoBehaviour, IDependencyProvider
{
    public Transform GemPrefab;
    public Transform EnemyPrefab;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<PrefabsProvider>(this);
    }
    [Provide]
    public PrefabsProvider ProviderPrefabs()
    {
        return this;
    }
}
