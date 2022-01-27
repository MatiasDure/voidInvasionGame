using System;
using GXPEngine;
using GXPEngine.Core;
using TiledMapParser;
using System.IO;
using Newtonsoft.Json.Linq;

public class Player : Entity //Inherits from Entity class which was created to avoid code duplication between the Enemy and Player class
{
    //json file path
    const string jsonPath = @"jsonFiles\PlayerSave.json"; //C:\Users\matid\Documents\CMGT_Saxion\year1\term2\gameDesign\game\myGame\GXPEngine2022BB1\GXPEngine2022BB\GXPEngine\bin\Debug\

    //game physics
    float jumpForce;
    float gravity;

    //player attributes
    int _abilityCount;
    int _EXP;
    int _LVL;
    float movementSpeed;
    float velocityY;
    string weakAbility, normalAbility, strongAbility;

    //starting positions when player falls to his death
    readonly float startingPositionX = 50;
    readonly float startingPositionY = 200;

    //set cooldown for when mover enemy hits player
    int lastTimeHit = 0;

    public int Exp { get => _EXP; set => _EXP = value; }
    public int AbilityCount { get => _abilityCount; private set => _abilityCount = value; }
    public int Lvl { get => _LVL; private set => _LVL = value; }

    //player's actions over multiple frames
    bool isJumping, isFalling;
    bool mirror;
    bool isMoving;
    bool isInjured;

    //json object to save player's properties
    JObject playerJson;

    //propeties which decide what ability to fire
    int shootFrame = 0;
    string currentAbilityPath = "";
    Sound currentAbilitySound = null;

    public Player(TiledObject obj = null) : base("objectImgs/spritesheetHero.png",9,13)
    {
        Initialize(obj);
    }
    void Update()
    {
        animationSpeed = 0.25f;

        if (Input.GetMouseButtonDown(0)) Console.WriteLine(Lvl); ; 
        CheckSave();
        CheckStatus();
        if (isDead)
        {
            animationSpeed = 0.1f;
            if (entityImg.currentFrame == 51) Death();
        }
        else
        {
            HorizontalMove();
            if (!ShotCoolDown()) PrepareToShoot();
            Shoot(this,currentAbilityPath,shootFrame,currentAbilitySound);
        }
        Jumping();
        SetAnimation();
        entityImg.Animate(animationSpeed);
    }

    protected override void Initialize(TiledObject obj = null)
    {
        Hp = 5;
        animationSounds = new Sound[7];
        for (int i = 0; i < animationSounds.Length; i++)
        {
            string soundFile = "";
            switch (i)
            {
                case 0:
                    soundFile = "sounds/weakAbilitySound.mp3";
                    break;
                case 1:
                    soundFile = "sounds/normalAbilitySound.mp3";
                    break;
                case 2:
                    soundFile = "sounds/strongAbilitySound.mp3";
                    break;
                case 3:
                    soundFile = "sounds/jumpSound.mp3";
                    break;
                case 4:
                    soundFile = "sounds/deathSound.mp3";
                    break;
                case 5:
                    soundFile = "sounds/hitSound.mp3";
                    break;
                case 6:
                    soundFile = "sounds/heroHurtSound.mp3";
                    break;
            }
            animationSounds[i] = new Sound(soundFile, false, false);
        }

        //reading json file
        playerJson = JObject.Parse(File.ReadAllText(jsonPath));

        //setting origin and scaling character sprite
        entityImg.SetOrigin(entityImg.width / 2 + 1, entityImg.height * 0.78f);
        entityImg.SetScaleXY(1.2f,0.85f);
        AddChild(entityImg);

        //loading players fields
        Load();
    }
    void Jumping()
    {
        velocityY += gravity;
        isFalling = velocityY > 0 && isJumping;
        Collision col = MoveUntilCollision(0, velocityY);

        if (col != null)
        {
            if (col.normal.y < 0) { isJumping = isFalling = false; }
            velocityY = 0;
        }

        if (!isJumping && Input.GetKey(Key.UP))
        {
            velocityY -= jumpForce;
            animationSounds[3].Play();
            isJumping = true;
        }
    }
    void HorizontalMove()
    {
        float velocityX = 0;

        bool isPressingRight = Input.GetKey(Key.RIGHT);
        bool isPressingLeft = Input.GetKey(Key.LEFT);

        if (isPressingRight)
        {
            velocityX += movementSpeed;
            entityImg.Mirror(false, false);
            mirror = false;
        }
        if (isPressingLeft)
        {
            velocityX -= movementSpeed;
            entityImg.Mirror(true, false);
            mirror = true;
        }

        //Uses 35 if deltaTime is greater, else it uses deltaTime
        int deltaTimeClamp = Mathf.Min(Time.deltaTime,40);

        //regulates velocity depending on the frame rates 
        //ex: 204 (pixels/second) * 16 (60fps) / 1000 (1 sec) =  3,3 pixels per frame.
        //ex: If frameRate is 30, then it would be 204 * 32 (double previous deltaTime) / 1000 = 6,5 pixels per frame.
        velocityX = velocityX * deltaTimeClamp / 1000;

        isMoving = Math.Abs(velocityX) > 0.1f;
        if (MoveUntilCollision(velocityX, 0) != null) isMoving = false;

    }
    protected override void Shoot(GameObject pOwner,string pImgPath = null, int pShootFrame = 0, Sound pShotSound = null)
    {
        if (Damage != 0 && entityImg.currentFrame == pShootFrame)
        {
            Console.WriteLine("shot fired");
            if (pShotSound != null) pShotSound.Play();
            parent.AddChild(new Bullet(pImgPath, this, Damage, mirror ? -SpeedX : SpeedX,0));
            _abilityCount--;
            lastTimeShot = Time.time + shotCooldown; //sets cooldown
            Damage = 0;
            currentAbilityPath = "";
        }
    }
    void PrepareToShoot()
    {
        //returns if there are no abilities left or no input keys for attacking are pressed
        if (_abilityCount < 1 || isAttacking || (!Input.GetKey(Key.A) && !Input.GetKey(Key.S) && !Input.GetKey(Key.D))) return;
        if ( (Lvl < 3 && Input.GetKey(Key.S)) || (Lvl < 5 && Input.GetKey(Key.D)) ) return;
        Console.WriteLine("Prepare to shoot");
        if (Input.GetKey(Key.A))
        {
            currentAbilityPath = weakAbility;
            Damage = 5;
            shootFrame = 3; //frame when bullet will appear
            SpeedX = 7;
            entityImg.SetCycle(0, 10); //weak attack animation
            currentAbilitySound = animationSounds[0]; //attack SFX
        }
        else if (Lvl > 2 && Input.GetKey(Key.S))
        {
            currentAbilityPath = normalAbility;
            Damage = 10;
            shootFrame = 22; //frame when bullet will appear
            SpeedX = 5;
            entityImg.SetCycle(18, 11); //normal attack animation
            currentAbilitySound = animationSounds[1]; //attack SFX
        }
        else if (Lvl > 4 && Input.GetKey(Key.D))
        {
            currentAbilityPath = strongAbility;
            Damage = 15;
            shootFrame = 33; //frame when bullet will appear
            SpeedX = 2f;
            entityImg.SetCycle(29, 10); //strong attack animation
            currentAbilitySound = animationSounds[2]; //attack SFX
        }

        if(currentAbilityPath != "") isAttacking = true;
    }
    protected override bool ShotCoolDown()
    {
        //sets attacking false at the end of attack animation
        if (isAttacking && (entityImg.currentFrame == 9 || entityImg.currentFrame == 28 || entityImg.currentFrame == 38)) isAttacking = false;
        return base.ShotCoolDown();
    }
    void SetAnimation()
    {
        if (isDead)
        {
            entityImg.SetCycle(40, 12); //die animaiton
            return;
        }
        if(isInjured)
        {
            entityImg.SetCycle(107,5); //injured animation
            isAttacking = false;
            if (entityImg.currentFrame == 111) isInjured = false;
            return;
        }
        if (isAttacking) return;
        if (isJumping && !isFalling) entityImg.SetCycle(65, 1); //jumping animation
        else if (isJumping && isFalling) entityImg.SetCycle(72, 1); //falling animation
        else if (isMoving) entityImg.SetCycle(79, 9); //walking animation
        else entityImg.SetCycle(52, 7); //idle animation
    }
    protected override void CheckStatus()
    {
        Lvl = Exp / 4;

        if (Hp <= 0 && !isDead)
        {
            animationSounds[4].Play();
            isDead = true;
        }
        if (y > game.height + height / 2)
        {
            TakeDamage(1);
            SetXY(startingPositionX, startingPositionY);
        }
    }
    void OnCollision(GameObject other)
    {
        //collision with collectable
        if (other is Collectable collectable)
        {
            _abilityCount += collectable.GiveBullets();
            collectable.Grab();
        }
        //collision with bullet
        if (other is Bullet bullet && bullet.Owner is StillEnemy)
        {
            TakeDamage(bullet.Damage);
            bullet.SetCollided();
        }
        //collision with moverEnemy
        if (other is MovingEnemy mover)
        {
            if (CanGetInjured())
            {
                TakeDamage(mover.Damage,true);
            }
        }
        if(other is ObstacleEnemy obstacle)
        {
            if (CanGetInjured())
            {
                TakeDamage(obstacle.Damage,true);
            }
        }
        if(other is Reader board)
        {
            board.ReadText();
        }
    }
    void TakeDamage(int pDamage, bool pIsObstacleOrMovingEnemy=false, int pAttackCooldown = 1000)
    {
        if (pIsObstacleOrMovingEnemy)
        {
            lastTimeHit = Time.time + pAttackCooldown;
        }
        Hp -= pDamage;
        if (Hp > 0) //avoids injured states if player is dead
        {
            animationSounds[6].Play(false, 0, 0.6f); 
            isInjured = true;
        }
    }

    protected override void Death()
    {
        ((MyGame)game).LoadLevel("tiledMaps/mainMenu.tmx");
    }

    //For enemies that have a cooldown before attacking again
    bool CanGetInjured()
    {
        return (lastTimeHit < Time.time);
    }
    void CheckSave()
    {
        if (Input.GetKey(Key.ONE))
        {
            Console.WriteLine("Game Saved");
            Save();
        }
    }
    public void Save()
    {
        try
        {
            //attributes that change throughout game
            playerJson.GetValue("EXP").Replace(Exp);
            playerJson.GetValue("LVL").Replace(Lvl);
            playerJson.GetValue("abilityCount").Replace(AbilityCount);

            File.WriteAllText(jsonPath, playerJson.ToString());
            Console.WriteLine("Game Saved!");
        }
        catch (Exception) { Console.WriteLine("Error while saving! Game could not be saved."); }
    }
    void Load()
    {
        try
        {
            //using setters
            AbilityCount = (int)playerJson["abilityCount"];
            Exp = (int)playerJson["EXP"];
            Lvl = (int)playerJson["LVL"];

            //not using setters
            movementSpeed = (float)playerJson["movementSpeed"];
            jumpForce = (float)playerJson["jumpForce"];
            gravity = (float)playerJson["gravity"];
            shotCooldown = (int)playerJson["shotCooldown"];
            weakAbility = (string)playerJson["weakAbility"];
            normalAbility = (string)playerJson["normalAbility"];
            strongAbility = (string)playerJson["strongAbility"];
        }
        catch (Exception) { Console.WriteLine("Error while loading! Game could not be saved."); }
    }
}
