using GXPEngine;
using TiledMapParser;

public class StillEnemy : EnemyBehaviour
{
    string bulletImg;

    public StillEnemy(TiledObject obj = null):base("objectImgs/spritesheetPlant.png", 8, 3)
    {
        Initialize(obj);
    }
    void Update()
    {
        CheckStatus();
        ManageState(0,true,16,5);
        if(!isDead)entityImg.Animate(animationSpeed);   
    }
    protected override void Initialize(TiledObject obj = null)
    {
        bulletImg = "bullets/enemyBullet.png";
        collider.isTrigger = true;
        animationSounds[1] = new Sound("sounds/ping.wav");

        //configure from tiles
        base.Initialize(obj);
        entityImg.SetXY(-5,-5);
        if (obj != null) shotCooldown = obj.GetIntProperty("cooldownMs", 2000);
    }
    protected override void ManageIdle()
    {  
        if ((DistanceTo(Target) < 200) && !ShotCoolDown())
        {
            ModifyState(State.ATTACK);
            lastTimeShot = Time.time + shotCooldown;
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

        if (Target.x > x)
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