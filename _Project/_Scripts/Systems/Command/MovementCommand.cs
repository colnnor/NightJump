using System.Threading.Tasks;
using UnityEngine;
using static System.Activator;

public abstract class MovementCommand : ICommand
{
    protected PlayerController playerMovement;

    protected MovementCommand(PlayerController playerMovement)
    {
        this.playerMovement = playerMovement;
    }

    public abstract Task Execute();

    public static T Create<T>(PlayerController playerMovement) where T : MovementCommand
    {
        return (T) CreateInstance(typeof(T), playerMovement);
    }
}

public class MoveLeftCommand : MovementCommand
{
    public MoveLeftCommand(PlayerController playerMovement) : base(playerMovement) { }

    public override async Task Execute()
    {
        playerMovement.JumpLeft();
        await Awaitable.WaitForSecondsAsync(playerMovement.GetJumpDuration());
    }
}

public class MoveRightCommand : MovementCommand
{
    public MoveRightCommand(PlayerController playerMovement) : base(playerMovement) { }

    public override async Task Execute()
    {
        playerMovement.JumpRight();
        await Awaitable.WaitForSecondsAsync(playerMovement.GetJumpDuration());

    }
}
public class MoveForwardCommand : MovementCommand
{
    public MoveForwardCommand(PlayerController playerMovement) : base(playerMovement) { }

    public override async Task Execute()
    {
        playerMovement.JumpForward();
        await Awaitable.WaitForSecondsAsync(playerMovement.GetJumpDuration());

    }
}

public class MoveBackwardCommand : MovementCommand
{
    public MoveBackwardCommand(PlayerController playerMovement) : base(playerMovement) { }

    public override async  Task Execute()
    {
        playerMovement.JumpBackward();
        await Awaitable.WaitForSecondsAsync(playerMovement.GetJumpDuration());

    }
}
