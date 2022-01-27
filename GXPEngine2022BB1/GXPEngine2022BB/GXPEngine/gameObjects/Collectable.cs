using GXPEngine;
using TiledMapParser;
public class Collectable:AnimationSprite
{
    const float ANIMATION_SPEED = 0.4f;
    bool grabbed;
    int amountBullets;
    Sound collectedSound;
    public Collectable(TiledObject obj=null):base("objectImgs/collectable.png",23,1)
    {
        collectedSound = new Sound("sounds/collectingItem.wav");
        if (obj != null) amountBullets = obj.GetIntProperty("amountBullets",Utils.Random(1,20));
        collider.isTrigger = true;
        SetCycle(6,17);
    }

    void Update()
    {
        if(currentFrame == 5) LateDestroy();
        Animate(ANIMATION_SPEED);
    }
    public void Grab()
    {
        if(!grabbed)
        {
            collectedSound.Play();
            SetCycle(0, 6);
            grabbed = true;
        }   
    }
    public int GiveBullets()
    {
        if(grabbed) return 0;
        return amountBullets;
    }
}
