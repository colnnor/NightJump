using System;
using System.Collections.Generic;
using UnityEngine;
using Eflatun.SceneReference;
using System.Linq;

[Serializable]
public class SceneGroup
{
    public string GroupName = "New Scene Group";
    public List<SceneData> Scenes;

    public string FindSceneNameByType(SceneType sceneType)
    {
        return Scenes.FirstOrDefault(scene => scene.SceneType == sceneType)?.Reference.Name;
    }
}

[Serializable]
public class SceneData
{
    public SceneReference Reference;
    public string Name => Reference.Name;
    public SceneType SceneType;
}

public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Environment, Tooling }
