using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
public sealed class InjectAttribute : Attribute{ }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
public sealed class ProvideAttribute : Attribute { }

public interface IDependencyProvider { }

[DefaultExecutionOrder(-1000)]
public class Injector : Singleton<Injector>
{
    readonly bool useDebug = false;
    const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    readonly Dictionary<Type, object> registry = new();

    protected override void Awake()
    {
        base.Awake();

        //find all modules with IDependencyProvider

        var providers = FindMonobehaviours().OfType<IDependencyProvider>();

        foreach ( var provider in providers)
        {
            RegisterProvider(provider);

        }

        var injectables = FindMonobehaviours().Where(IsInjectable);
        foreach (var injectable in injectables)
        {
            Inject(injectable);
        }

    }
     
    void Inject(object instance)
    {
        Debug.Log(instance);
        var type = instance.GetType();

        var injectableFields = type.GetFields(BINDING_FLAGS).Where(field => Attribute.IsDefined(field, typeof(InjectAttribute)));

        foreach(var injectableField in injectableFields)
        {
            var fieldType = injectableField.FieldType;
            var resolvedInstance = Resolve(fieldType);
            Debug.Log(fieldType);
            if(resolvedInstance != null)
            {
                injectableField.SetValue(instance, resolvedInstance);
                if(useDebug)
                    Debug.Log($"Field Injected {fieldType.Name} into {type.Name}.{injectableField.Name}");
            }
            else throw new Exception($"Failed to resolve {fieldType.Name} for {type.Name}");
        }

        var injectableMethods = type.GetMethods(BINDING_FLAGS).Where(method => Attribute.IsDefined(method, typeof(InjectAttribute)));
        foreach(var injectableMethod in injectableMethods)
        {
            var requiredParameters = injectableMethod.GetParameters().Select(paramateter => paramateter.ParameterType).ToArray();
            var resolvedInstances = requiredParameters.Select(Resolve).ToArray();

            if(resolvedInstances.Any(resolvedInstances => resolvedInstances == null))
            {
                throw new Exception($"Failed to resolve {requiredParameters.Length} requiredParameters for {type.Name}.{injectableMethod.Name}");
            }
            injectableMethod.Invoke(instance, resolvedInstances);
                if(useDebug)
                    Debug.Log($"Method Injected {type.Name} into {type.Name}.{injectableMethod.Name}");
        }

        var injectableProperties = type.GetProperties(BINDING_FLAGS).Where(property => Attribute.IsDefined(property, typeof(InjectAttribute)));
        foreach (var injectableProperty in injectableProperties)
        {
            var propertyType = injectableProperty.PropertyType;
            var resolvedInstance = Resolve(propertyType);

            if(resolvedInstance != null)
            {
                injectableProperty.SetValue(instance, resolvedInstance);
                if(useDebug)
                    Debug.Log($"Property Injected {propertyType.Name} into {type.Name}.{injectableProperty.Name}");
            }
            else throw new Exception($"Failed to resolve {propertyType.Name} for {type.Name}");
        }
    }

    object Resolve(Type type)
    {
        registry.TryGetValue(type, out var resolvedInstance);
        return resolvedInstance;
    }

    static bool IsInjectable(MonoBehaviour obj)
    {
        var members = obj.GetType().GetMembers(BINDING_FLAGS);
        return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
    }

    private void RegisterProvider(IDependencyProvider provider)
    {
        var methods = provider.GetType().GetMethods(BINDING_FLAGS);

        foreach ( var method in methods )
        {
            if(!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

            var returnType = method.ReturnType;
            var providedInstance = method.Invoke(provider, null);

            if(providedInstance != null )
            {
                registry.Add(returnType, providedInstance);
                if(useDebug)
                    Debug.Log($"Registered {returnType.Name} from {provider.GetType().Name}");
            }
            else throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
        }
    }
    static MonoBehaviour[] FindMonobehaviours()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
    }
}
