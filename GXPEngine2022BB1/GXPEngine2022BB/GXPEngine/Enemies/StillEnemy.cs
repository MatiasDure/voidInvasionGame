using GXPEngine;
using TiledMapParser;

//------------------------StillEnemy-----------------------------------//
// Inherits from EnemyBehaviour
// Has declared the attack and idle states for the shooter enemies
// Creates shooter enemies
//------------------------------------------------------------------------//

public class StillEnemy : EnemyBehaviour
{
    string bulletImg;

    public StillEnemy(TiledObject obj = null):base("objectImgs/spritesheetPlant.png", 8, 3)
    {
        Initialize(obj);
    }

    void Update()
    {
        UpdateHealthUI();
        CheckStatus();
        ManageState(0,true,16,5);
        if(!isDead)entityImg.Animate(animationSpeed);   
    }

    //Initialize shooter enemies
    protected override void Initialize(TiledObject obj = null)
    {
        bulletImg = "bullets/enemyBullet.png";
        animationSounds[1] = new Sound("sounds/ping.wav");

        base.Initialize(obj); //calls enemyBehaviour initialize method
        entityImg.SetXY(-5,-5);
        if (obj != null) shotCooldown = obj.GetIntProperty("cooldownMs", 2000);
    }

    protected override void ManageIdle()
    {
        if ((DistanceTo(target) < 200) && !ShotCoolDown())
        {
            ModifyState(State.ATTACK);
            UpdateLastTimeShot();
            return;
        }
        entityImg.SetCycle(8, 8); //idle 
    }

    protected override void ManageAttack()
    {
        if(entityImg.currentFrame == 7)
        {
            ModifyState(State.IDLE);
            isAttacking = false;
            return;
        }

        if (target.x > x)
        {
            entityImg.Mirror(true, false);
            SpeedX = SpeedX < 0 ? SpeedX * -1 : SpeedX;
        }
        else
        {
            entityImg.Mirror(false, false);
            SpeedX = SpeedX >= 0 ? SpeedX * -1 : SpeedX; 
        }
        entityImg.SetCycle(0, 8); //shooting
        Shoot(this,bulletImg, 4,animationSounds[1]);
    }

}