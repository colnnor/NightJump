using System;

public class MainMenu : Container
{
    private Action onPlayAction;
    private Action onSettingsAction;
    private Action onExitAction;
    //Animate out
    //call menu manager to animate correct one in
    public void Play()
    {
        AnimateContainerOut();
        onPlayAction?.Invoke();
    }
    public void Settings()
    {
        AnimateContainerOut();
        onSettingsAction?.Invoke();
    }
    public void Exit()
    {
        AnimateContainerOut();
        onExitAction?.Invoke();
    }
}