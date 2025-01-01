using TaleKit.Game;
using TaleKit.Game.Combat;
using TaleKit.Game.Entities;
using PhoenixWrapped;
using PhoenixWrapped.Messaging.Combat;
using PhoenixWrapped.Messaging.Interaction;
using PhoenixWrapped.Messaging.Movement;

namespace TaleKit.Phoenix;

public class PhoenixActionBridge(PhoenixClient client) : IActionBridge
{
    public Session? Session { get; set; }

    public void Walk(Character character, Position position)
    {
        client.SendMessage(new PlayerWalk
        {
            X = position.X,
            Y = position.Y
        });
    }

    public void WalkNosmate(SummonedNosmate nosmate, Position position)
    {
        client.SendMessage(new PetsWalk
        {
            X = position.X,
            Y = position.Y
        });
    }

    public void Attack(LivingEntity entity)
    {
        client.SendMessage(new Attack
        {
            MonsterId = entity.Id
        });
    }

    public void Attack(LivingEntity entity, Skill skill)
    {
        client.SendMessage(new PlayerSkill
        {
            MonsterId = entity.Id,
            SkillId = skill.CastId
        });
    }

    public void PickUp(Drop drop)
    {
        client.SendMessage(new PickUp
        {
            ItemId = drop.Id
        });
    }
}