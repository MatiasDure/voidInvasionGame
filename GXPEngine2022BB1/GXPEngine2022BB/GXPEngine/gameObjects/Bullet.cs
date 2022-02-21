using GXPEngine;
using GXPEngine.Core;


//------------------------Bullet-----------------------------------//
// Inherits from Sprite
// Creates bullet objects
// Contains the logic for bullets
// Used for both players and enemies
//------------------------------------------------------------------------//

public class Bullet:Sprite
{
    //particle effect
    AnimationSprite bulletExplode;

    //bullet's properties
    float _velocityX, _velocityY;
    int _damage;
    GameObject _Owner;

    bool collided;

    //public getters and protected/private setters
    public int Damage { get => _damage; protected set => _damage = value; }
    public float VelocityX { get => _velocityX; protected set => _velocityX = value; }
    public float VelocityY { get => _velocityY; protected set => _velocityY = value; }
    public GameObject Owner { get => _Owner; private set => _Owner = value; }

    public Bullet(string pImgPath, GameObject pOwner, int pDamage, float pVelocityX, float pVelocityY = 0, float pOffsetX = 0, float pOffsetY = 0) : base(pImgPath) 
    {
        Initialize(pOwner, pDamage,pVelocityX,pVelocityY,pOffsetX,pOffsetY);
    }

    void Update()
    {
        Move();
        Edges();
        if (bulletExplode.currentFrame == (bulletExplode.frameCount - 1)) Collided();
        if (collided) bulletExplode.Animate(0.7f);
    }
    //runs once when object is instantiated 
    void Initialize(GameObject pOwner, int pDamage, float pVelocityX, float pVelocityY = 0, float pOffsetX = 0, float pOffsetY = 0)
    {
        collider.isTrigger = pOwner is StillEnemy;
        VelocityX = pVelocityX;
        VelocityY = pVelocityY; 
        Damage = pDamage;
        Owner = pOwner;
        
        if(pOwner is Player)
        {
            pOffsetY = height / 2;
            pOffsetX = width * 2;
        }
        if (pVelocityX > 0) SetXY(pOwner.x + pOffsetX, pOwner.y - pOffsetY);
        else
        {
            SetXY(pOwner.x - pOffsetX - 20, pOwner.y - pOffsetY);
            Mirror(true, false);
        }
        bulletExplode = new AnimationSprite("particleFX/bulletExplode.png", 7, 1, -1, false, false);
        bulletExplode.SetOrigin(width / 2, height / 2);
    }

    void Move()
    {
        if (collided) return;
        x += VelocityX;
        y += VelocityY;
    }
    void Edges()
    {
        Vector2 screenPos = TransformPoint(0, 0);
        if (screenPos.x < 0 || screenPos.x >= game.width - width || screenPos.y < 0 || screenPos.y >= game.height - height) SetCollided();
    }
    public void SetCollided()
    {
        collided = true;
        SetCollisionOff(); //customized method that I added to the Sprite class which sets the bounds of the image to 0 to avoid having a collision box
        alpha = 0;
        LateAddChild(bulletExplode);
        bulletExplode.SetCycle(0, 7);
    }
    void Collided()
    {
        LateDestroy();
    }

}