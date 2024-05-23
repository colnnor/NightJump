using UnityEngine;

public class PrefabsProvider : MonoBehaviour
{
    public Transform GemPrefab;
    public Transform EnemyPrefab;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<PrefabsProvider>(this);
    }

}
