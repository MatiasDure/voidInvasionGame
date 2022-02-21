using GXPEngine;
using TiledMapParser;

//------------------------MovingEnemy-----------------------------------//
// Inherits from EnemyBehaviour
// Has declared the attack and idle states for the mover enemies
// Creates mover enemies
//------------------------------------------------------------------------//

public class MovingEnemy:EnemyBehaviour
{
    float startingPositionX; 
    int distanceToAttack;

    bool soundPlayed; 

    public MovingEnemy(TiledObject obj = null):base("objectImgs/spritesheetPig.png", 47, 1)
    {
        Initialize(obj);
    }

    void Update()
    {
        UpdateHealthUI();
        CheckStatus();
        ManageState(0,true,0,5);
        if(!isDead) entityImg.Animate(animationSpeed);
    }

    //Initialize Mover enemies
    protected override void Initialize(TiledObject obj = null)
    {
        base.Initialize(obj); //calls enemyBehaviour initialize method
        if (obj != null)
        {
            startingPositionX = obj.GetFloatProperty("startingPositionX", 310);
            distanceToAttack = obj.GetIntProperty("distanceToAttack",150);
        }

        animationSounds[1] = new Sound("sounds/angryPig.mp3");
        entityImg.scaleY = 1.5f;
    }

    protected override void ManageIdle()
    {
        if (x < startingPositionX - SpeedX || x > startingPositionX + SpeedX)
        {
            entityImg.SetCycle(31,16); //happy walk
            MoveTowards(startingPositionX, SpeedX);
        }
        else entityImg.SetCycle(10, 9); //idle
        if (DistanceTo(target) < distanceToAttack)
        {
            ModifyState(State.ATTACK);
            entityImg.SetCycle(19,12); //upset walk
        }
    }

    protected override void ManageAttack()
    {        
        if(!soundPlayed)
        {
            animationSounds[1].Play(false,0,0.8f);
            soundPlayed = true;
        }
        MoveTowards(target.x, SpeedX);
        if (DistanceTo(target) > distanceToAttack)
        {
            ModifyState(State.IDLE);
            soundPlayed = false;
        }
    }

}