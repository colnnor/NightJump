using UnityEngine;

public class ClassB : MonoBehaviour
{
    ServiceA serviceA;
    [Inject] ServiceB serviceB;
    [Inject] FactoryA factoryA;

    [Inject]
    void GetServiceA(ServiceA serviceA) => this.serviceA = serviceA;

    private void Start()
    {
        serviceA.Initialize(" from ClassB");
        serviceB.Initialize(" from ClassB");
        factoryA.CreateServceA().Initialize(" from ClassB");
    }
}


