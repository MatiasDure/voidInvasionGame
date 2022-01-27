using GXPEngine;
using TiledMapParser;

//class for player and enemy to inherit to avoid code repetition
public abstract class Entity : Sprite
{
    protected AnimationSprite entityImg;
    protected Sound[] animationSounds;

    int _hp;
    int _damage;
    float _speedX;
    float _speedY;
    protected int shotCooldown;
    protected int lastTimeShot;
    protected float animationSpeed;

    public int Hp { get => _hp; set => _hp = value; }
    public int Damage { get => _damage; protected set => _damage = value; }
    public float SpeedX { get => _speedX; protected set => _speedX = value; }
    public float SpeedY { get => _speedY; protected set => _speedY = value; }

    protected bool isAttacking;
    protected bool isDead;

    //The parameters <pAnimationImgPath>, <pCol>, and <pRow> are for the entityImg AnimationSprite
    public Entity(string pAnimationImgPath, int pCol, int pRow, string imgPath = "objectImgs/hitbox.jpg") :base(imgPath)
    {
        entityImg = new AnimationSprite(pAnimationImgPath,pCol,pRow,-1,false,false);
        alpha = 0;
    }

    virtual protected bool ShotCoolDown()
    {
        //returns false if theres no need for cooldown
        if (Time.time > lastTimeShot) return false;
        return true;
    }
    virtual protected void Death() { LateDestroy(); }
    virtual protected void Shoot(GameObject pOwner,string imgPath, int pShootFrame=0, Sound pShotSound = null) { }
    virtual protected void CheckStatus() { }
    virtual protected void Initialize(TiledObject obj = null) { }

}