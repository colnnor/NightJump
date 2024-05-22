using UnityEngine;

public class EnvironmentSystem : MonoBehaviour, IDependencyProvider, IEnvironmentSystem
{
    [Provide]
    public IEnvironmentSystem ProvideEnvironmentSystem()
    {
        return this;
    }

    public void Initialize()
    {
        Debug.Log("EnvironmentSystem.Initialize()");
    }
}
