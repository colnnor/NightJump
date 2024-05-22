using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CommandManager : SerializedMonoBehaviour
{
    public PlayerController playerMovement;
    public ICommand singleCommand;

    bool isExecuting;

    public List<ICommand> commands = new();

    readonly CommandInvoker commandInvoker = new();

    private void Start()
    {
        playerMovement = GetComponent<PlayerController>();
    }

    public async Task ExecuteCommand(ICommand command)
    {
        commands.Add(command);
        await commandInvoker.ExecuteCommand(command);
    }

}

public class CommandInvoker
{
    public async Task ExecuteCommand(ICommand command)
    {
        await command.Execute();
    }
    public async Task ExecuteCommands(List<ICommand> commands)
    {
        foreach (var command in commands)
        {
            await command.Execute();
        }
    }
}