using TaleKit.Game;
using TaleKit.Game.Combat;
using TaleKit.Game.Entities;
using PhoenixWrapped;
using PhoenixWrapped.Messaging.Combat;
using PhoenixWrapped.Messaging.Interaction;
using PhoenixWrapped.Messaging.Movement;
using PartnerSkill = TaleKit.Game.Combat.PartnerSkill;

namespace TaleKit.Phoenix;

public class PhoenixActionBridge : IActionBridge
{
    private readonly PhoenixClient client;

    public PhoenixActionBridge(PhoenixClient client)
    {
        this.client = client;
    }

    public Session? Session { get; set; }

    public void Walk(Position position, int speed)
    {
        this.client.SendMessage(new PlayerWalk
        {
            X = position.X,
            Y = position.Y
        });
    }

    public void Attack(LivingEntity entity)
    {
        this.client.SendMessage(new Attack
        {
            MonsterId = entity.Id
        });
    }

    public void Attack(LivingEntity entity, Skill skill)
    {
        this.client.SendMessage(new PlayerSkill
        {
            MonsterId = entity.Id,
            SkillId = skill.CastId
        });
    }

    public void PickUp(Drop drop)
    {
        this.client.SendMessage(new PickUp
        {
            ItemId = drop.Id
        });
    }
}