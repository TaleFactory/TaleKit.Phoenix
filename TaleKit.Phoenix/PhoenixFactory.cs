using System.Runtime.CompilerServices;
using TaleKit.Extension;
using TaleKit.Game;
using TaleKit.Game.Combat;
using TaleKit.Game.Entities;
using TaleKit.Game.Event.Maps;
using TaleKit.Game.Factory;
using TaleKit.Game.Maps;
using TaleKit.Game.Storage;
using PhoenixWrapped;
using PhoenixWrapped.Messaging;
using PhoenixWrapped.Messaging.Query;

namespace TaleKit.Phoenix;

public class PhoenixFactory
{
    public static Session CreateSession(PhoenixClient client)
    {
        var session = TaleKitFactory.CreateSession(new SessionConfiguration
        {
            Network = new PhoenixNetwork(client),
            ActionBridge = new PhoenixActionBridge(client)
        });

        // Used for initialization, everything updated here will be updated by packet processors later
        client.MessageReceived += message =>
        {
            switch (message)
            {
                case QueryInventory inv:
                    session.Character.Inventory.Gold = inv.Inventory.Gold;
                    foreach (var equip in inv.Inventory.Equip)
                    {
                        var stack = ItemFactory.CreateItemStack(equip.Vnum, equip.Quantity,  0, equip.Slot, InventoryType.Equipment);
                        if (stack is not null)
                        {
                            session.Character.Inventory.AddItem(stack);
                        }
                    }
                    
                    foreach (var equip in inv.Inventory.Main)
                    {
                        var stack = ItemFactory.CreateItemStack(equip.Vnum, equip.Quantity, 0, equip.Slot, InventoryType.Main);
                        if (stack is not null)
                        {
                            session.Character.Inventory.AddItem(stack);
                        }
                    }
                    
                    foreach (var equip in inv.Inventory.Etc)
                    {
                        var stack = ItemFactory.CreateItemStack(equip.Vnum, equip.Quantity, 0, equip.Slot, InventoryType.Etc);
                        if (stack is not null)
                        {
                            session.Character.Inventory.AddItem(stack);
                        }
                    }
                    break;
                case QueryPlayer player:
                    session.Character.Id = (int) player.PlayerInfo.Id;
                    session.Character.Name = player.PlayerInfo.Name;
                    session.Character.Position = new Position
                    {
                        X = player.PlayerInfo.X,
                        Y = player.PlayerInfo.Y
                    };
                    
                    session.Character.Map = MapFactory.CreateMap(player.PlayerInfo.MapId);
                    session.Character.Map.AddEntity(session.Character);
                    
                    break;
                case QueryMapEntities entities:
                    foreach (var monsterInfo in entities.Monsters)
                    {
                        var monster = MonsterFactory.CreateMonster(monsterInfo.Id, monsterInfo.Vnum);
                        
                        monster.HpPercentage = monsterInfo.HpPercent;
                        monster.MpPercentage = monsterInfo.HpPercent;
                        monster.Position = new Position(monsterInfo.X, monsterInfo.Y);
                        monster.Map = session.Character.Map;
                        
                        session.Character.Map.AddEntity(monster);
                    }
                    
                    foreach (var dropInfo in entities.Items)
                    {
                        var drop = DropFactory.CreateDrop(dropInfo.Id, dropInfo.Vnum, dropInfo.Quantity);

                        drop.Map = session.Character.Map;
                        drop.Position = new Position(dropInfo.X, dropInfo.Y);
                        
                        session.Character.Map.AddEntity(drop);
                    }

                    foreach (var npcInfo in entities.Npcs)
                    {
                        var npc = NpcFactory.CreateNpc((int)npcInfo.Id, npcInfo.Vnum);

                        npc.HpPercentage = npcInfo.HpPercent;
                        npc.MpPercentage = npcInfo.MpPercent;
                        npc.Position = new Position(npcInfo.X, npcInfo.Y);
                        npc.Map = session.Character.Map;
                        
                        session.Character.Map.AddEntity(npc);
                    }
                    
                    foreach (var playerInfo in entities.Players)
                    {
                        var player = new Player
                        {
                            Id = playerInfo.Id,
                            Position = new Position(playerInfo.X, playerInfo.Y),
                            HpPercentage = playerInfo.HpPercent,
                            MpPercentage = playerInfo.MpPercent,
                            Name = playerInfo.Name,
                            Map = session.Character.Map
                        };
                        
                        session.Character.Map.AddEntity(player);
                    }
                    
                    break;
                case QuerySkills skillInfos:
                    var skills = new HashSet<Skill>();
                    
                    foreach (var skillInfo in skillInfos.Skills)
                    {
                        var skill = SkillFactory.CreateSkill(skillInfo.Vnum);
                        if (skill is not null)
                        {
                            skills.Add(skill);
                        }
                    }
                    
                    session.Character.Skills = skills;
                    break;
            }
        };
        
        client.SendMessage(new Message(MessageType.QueryPlayer));
        client.SendMessage(new Message(MessageType.QueryMapEntities));
        client.SendMessage(new Message(MessageType.QuerySkills));
        client.SendMessage(new Message(MessageType.QueryInventory));

        return session;
    }

    public static Session CreateSession(string characterName)
    {
        var client = PhoenixClientFactory.CreateByCharacterName(characterName);
        var session = CreateSession(client);
        
        return session;
    }

    public static Session CreateSession(Window window)
    {
        var client = PhoenixClientFactory.CreateByPort(window.Port);
        var session = CreateSession(client);

        return session;
    }
}