using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public static void Log(string message, bool enableLogging)
    {
        if (!enableLogging) return;

        Debug.Log(message);
    }
    public static void LogError(string message, bool enableLogging)
    {
        if (!enableLogging) return;

        Debug.LogError(message);
    }

    public static void LogWarning(string message, bool enableLogging)
    {
        if (!enableLogging) return;

        Debug.LogWarning(message);
    }
}
