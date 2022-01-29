using GXPEngine;
using TiledMapParser;

public class ObstacleEnemy : EnemyBehaviour
{
    bool flip;
    bool gotHit;
    int lastTimeHit = 0;
    public ObstacleEnemy(TiledObject obj = null):base("objectImgs/saw.png", 8, 1)
    {
        Initialize(obj);
    }

    protected override void Initialize(TiledObject obj = null)
    {
        base.Initialize(obj);
        collider.isTrigger = false;
        if(obj != null) SpeedY = obj.GetFloatProperty("speedY",2f);
    }

    void Update()
    {
        CheckStatus();
        ManageState(0);
        if (!isDead) entityImg.Animate(animationSpeed);
    }

    protected override void ManageIdle()
    {
        if(gotHit)
        {
            entityImg.SetColor(255, 255, 255);
            gotHit = false;
        }
        if(DistanceTo(Target) < 300)
        {
            ModifyState(State.ATTACK);
            entityImg.SetCycle(0,8);
        }
    }
    protected override void ManageAttack()
    {
        if (y > game.height - height) flip = true;
        else if (y < game.height / 2) flip = false;

        if (flip) y -= SpeedY;
        else y += SpeedY;

        if (DistanceTo(Target) > 300)
        {
            ModifyState(State.IDLE);
            entityImg.SetCycle(1, 1);
        }
    }
    protected override void ManageInjured()
    {
        if(!gotHit)
        {
            entityImg.SetColor(255, 0, 0);
            gotHit = true;
            lastTimeHit = Time.time + 200;
        }

        if (lastTimeHit < Time.time)
        {
            ModifyState(State.IDLE);
            entityImg.SetCycle(1, 1);
        }
    }

    void GotHit(int pDamage)
    {
        Hp -= pDamage;
        entityImg.SetColor(255, 0, 0);
        lastTimeHit = Time.time + 200;
        gotHit = true;
    }
    void GotHitAnimation()
    {
        if (gotHit && lastTimeHit < Time.time)
        {
            entityImg.SetColor(255, 255, 255);
            gotHit = false;
        }
    }
}

