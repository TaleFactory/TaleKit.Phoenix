# PhoenixWrapped
This project is an implementation of [TaleKit](https://github.com/TaleFactory/TaleKit) for Phoenix Bot

## Installation
Using Package Manager
```shell
Install-Package TaleKit.Phoenix
```

Using .NET CLI
```shell
dotnet add package TaleKit.Phoenix
```
## Usage
```csharp
var session = PhoenixFactory.CreateSession("MySuperCharacterName");
while (!session.IsReady())
{
    await Task.Delay(100);
}

while (true)
{
    var command = Console.ReadLine();
    if (command == "exit")
    {
        return;
    }

    switch (command)
    {
        case "walk":
            session.Character.Walk(new Position(0, 0));
            break;
        case "attack":
            var monster = session.Character.GetClosestMonster();
            if (monster is not null)
            {
                session.Character.Attack(monster);
            }
            break;
        case "pickup":
            var drop = session.Character.GetClosestDrop();
            if (drop is not null)
            {
                session.Character.PickUp(drop);
            }
            break;
        default:
            Console.WriteLine("Unknown command");
            break;
    }
}
```
