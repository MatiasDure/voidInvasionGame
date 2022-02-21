using GXPEngine;
using TiledMapParser;

//------------------------ObstacleEnemy-----------------------------------//
// Inherits from EnemyBehaviour
// Has declared the attack, idle, and injured states for the obstacle enemies
// Creates obstacle enemies
//------------------------------------------------------------------------//

public class ObstacleEnemy : EnemyBehaviour
{
    bool flip;
    bool gotHit;

    public ObstacleEnemy(TiledObject obj = null):base("objectImgs/saw.png", 8, 1)
    {
        Initialize(obj);
    }

    //Initializes obstacle enemies
    protected override void Initialize(TiledObject obj = null)
    {
        base.Initialize(obj); //calls enemyBehaviour initialize method
        collider.isTrigger = false;
        if(obj != null) SpeedY = obj.GetFloatProperty("speedY",2f);
    }

    void Update()
    {
        UpdateHealthUI();
        CheckStatus();
        ManageState(0);
        if (!isDead) entityImg.Animate(animationSpeed);
    }

    protected override void ManageIdle()
    {
        RemoveAnim();
        if(DistanceTo(target) < 300)
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

        if (DistanceTo(target) > 300)
        {
            ModifyState(State.IDLE);
            entityImg.SetCycle(1, 1);
        }
    }

    protected override void ManageInjured()
    {
        if(!gotHit)
        {
            SetAnim();
        }

        if (lastTimeHit < Time.time)
        {
            ModifyState(State.IDLE);
            entityImg.SetCycle(1, 1);
        }
    }

    void SetAnim()
    {
        entityImg.SetColor(255, 0, 0);
        lastTimeHit = Time.time + 200;
        gotHit = true;
    }

    void RemoveAnim()
    {
        if (gotHit)
        {
            entityImg.SetColor(255, 255, 255);
            gotHit = false;
        }
    }
}

