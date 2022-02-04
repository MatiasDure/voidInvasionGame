using GXPEngine;
using GXPEngine.Core;
using TiledMapParser;

public abstract class EnemyBehaviour:Entity
{
    GameObject _target;
    public GameObject Target { get => _target; set => _target = value; }

    protected AnimationSprite enemyExplode;

    protected int _giveExp;
    public int GiveExp { get => _giveExp; set => _giveExp = value; }

    EasyDraw healthBar;
    
    //enemy states (for all enemies)
    protected enum State
    {
        IDLE,
        ATTACK,
        INJURED,
        DIE
    }

    protected State _state;

    protected override void Initialize(TiledObject obj = null)
    {
        
        if (obj != null)
        {
            Hp = obj.GetIntProperty("hp", 30);
            Damage = obj.GetIntProperty("damage", 1);
            GiveExp = obj.GetIntProperty("giveExp", 1);
            animationSpeed = obj.GetFloatProperty("animationSpeed", .22f);
            SpeedX = obj.GetFloatProperty("speedX", 2f);
        }
        entityImg.SetOrigin(entityImg.width / 2, entityImg.height / 2);
        AddChild(entityImg);

        healthBar = new EasyDraw(80,80,false);
        healthBar.SetOrigin(width / 2, height / 2);
        healthBar.SetXY(0,-46);
        healthBar.TextSize(14);
        AddChild(healthBar);
    }

    //constructor for abstract class
    public EnemyBehaviour(string pAnimationImgPath, int pCol, int pRow):base(pAnimationImgPath,pCol,pRow)
    {
        enemyExplode = new AnimationSprite("particleFX/entityExplode.png", 8, 1, -1, false);
        enemyExplode.collider.isTrigger = true;
        animationSounds = new Sound[2];
        animationSounds[0] = new Sound("sounds/enemyDeathSound.mp3");
    }

    //enemy shoots bullet (for shooter enemies)
    protected override void Shoot(GameObject pOwner,string pStringImgPath=null, int pShootFrame=0, Sound pShotSound = null)
    {
        if (entityImg.currentFrame == pShootFrame && !isAttacking)
        {
            isAttacking = true;
            if (pShotSound != null) pShotSound.Play();
            parent.AddChild(new Bullet(pStringImgPath,pOwner,1,SpeedX,0,8,12));
        }
    }

    //enemy moves towards location provided (for mover enemies. A quick implementation could be added for flying mover enemies)
    virtual protected void MoveTowards(float pLocation, float pSpeed)
    {
        float velocityX = 0;
        float velocityY = 3f;
        if (pLocation < x - pSpeed)
        {
            velocityX -= pSpeed;
            entityImg.Mirror(false, false);
        }
        if (pLocation > x + pSpeed)
        {
            velocityX += pSpeed;
            entityImg.Mirror(true, false);
        }

        Collision moverColsY = MoveUntilCollision(0, velocityY);
        Collision moverColsX = MoveUntilCollision(velocityX, 0);

    } 

    //checks enemy's hp (for all enemies)
    protected override void CheckStatus()
    {
        if (Hp <= 0 || y > game.height - height / 2)
        {
            ModifyState(State.DIE);
        }
    }

    //health UI for all enemies
    protected void UpdateHealthUI()
    {
        healthBar.ClearTransparent();
        healthBar.Text(""+Hp,0,healthBar.height/2);
    }

    //Manages the different states (for all enemies). 
    //parameter name <pStartFrame> is the frame number when the enemy injury animation starts
    //parameter name <pFrameCount> is the count of frames in the enemy's injury animation
    //parameter name <pDeathSound> is the sound that the enemy plays when it dies
    //parameter name <pHasInjuredAnim> is for in case the enemy does not have an injured animation
    virtual protected void ManageState(int pDeathSound = -1, bool pHasInjuredAnim=false ,int pStartFrame=0, int pFrameCount=0) 
    {
        switch (_state)
        {
            case State.IDLE:
                ManageIdle();
                break;
            case State.ATTACK:
                ManageAttack();
                break;
            case State.DIE:
                ManageDeath(pDeathSound);
                break;
            case State.INJURED:
                if (pHasInjuredAnim) ManageInjured(pStartFrame, pFrameCount);
                else ManageInjured();
                break;
        }
    }

    //switches between states (for all enemies)
    virtual protected void ModifyState(State pNewState) 
    {
        if (_state != pNewState) _state = pNewState;
    }

    //handles death state of an enemy (for all enemies)
    virtual protected void ManageDeath(int pDeathSound = -1) 
    {
        if (!isDead)
        {
            SetCollisionOff(); //Customized method in the gxp engine's Sprite class which turn collisions "off" by making the width and height 0
            entityImg.alpha = 0; //set the animationsprite of the enemy transparent to make place for the explosion animation
            isDead = true;
            if(pDeathSound >= 0) animationSounds[pDeathSound].Play(); //enemy death sound
            enemyExplode.SetOrigin(enemyExplode.width/2, height / 2 + 20);
            AddChild(enemyExplode);
        }
        else
        {
            enemyExplode.Animate(animationSpeed);
            if (enemyExplode.currentFrame == enemyExplode.frameCount - 1) Death();
        }
    }

    //handles injured state of an enemy (for all enemies that have an injured animation)
    virtual protected void ManageInjured(int pStartFrame, int pFrameCount) 
    {
        if (entityImg.currentFrame == (pStartFrame + pFrameCount - 1))
        {
            ModifyState(State.IDLE);
            return;
        }
        entityImg.SetCycle(pStartFrame, pFrameCount);
    }

    //handles injured state of an enemy (for all enemies that DON'T have an injured animation)
    virtual protected void ManageInjured() { }


    //handles idle state of an enemy (for all enemies)
    virtual protected void ManageIdle() { }

    //handles attack state of an enemy (for all enemies)
    virtual protected void ManageAttack() { }

    virtual protected void OnCollision(GameObject other)
    {
        if (other is Bullet bullet && bullet.Owner is Player p)
        {
            ModifyState(State.INJURED);
            Hp -= bullet.Damage;
            bullet.SetCollided();
            p.Exp += GiveExp;
        }
    }
}
