using GXPEngine;
using TiledMapParser;

//------------------------Collectable-----------------------------------//
// Inherits from AnimationSprite
// Creates collectable objects
// Used to give bullets to the player
//------------------------------------------------------------------------//

public class Collectable:AnimationSprite
{
    const float ANIMATION_SPEED = 0.4f;
    bool grabbed;
    int amountBullets;
    Sound collectedSound;
    public Collectable(TiledObject obj=null):base("objectImgs/collectable.png",23,1)
    {
        collectedSound = new Sound("sounds/collectingItem.wav");
        if (obj != null) amountBullets = obj.GetIntProperty("amountBullets",Utils.Random(5,15));
        collider.isTrigger = true;
        SetCycle(6,17);
    }

    void Update()
    {
        if(currentFrame == 5) LateDestroy();
        Animate(ANIMATION_SPEED);
    }

    /// <summary>
    /// When this method is called it sets the collected animation and the grabbed variable to true.
    /// </summary>
    public void Grab()
    {
        if(!grabbed)
        {
            collectedSound.Play();
            SetCycle(0, 6);
            grabbed = true;
        }   
    }

    /// <returns>An amount of bullets</returns>
    public int GiveBullets()
    {
        if(grabbed) return 0;
        return amountBullets;
    }
}
