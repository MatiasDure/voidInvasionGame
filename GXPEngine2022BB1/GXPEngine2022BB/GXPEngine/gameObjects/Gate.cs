using GXPEngine;
using TiledMapParser;


//------------------------Gate-----------------------------------//
// Inherits from Sprite to use a white hitbox image
// Contains an AnimationSprite to create the portal animation
// Creates Gate objects
// Contains a target variable which points to the next fileName which has to be loaded
// Used to travel between levels
//------------------------------------------------------------------------//

public class Gate:Sprite
{
    string target;

    AnimationSprite gateAnimation;

    public Gate(TiledObject obj = null):base("objectImgs/hitbox.jpg")
    {
        Initialize(obj);
    }
    
    void Initialize(TiledObject obj = null)
    {
        alpha = 0;
        SetOrigin(width/2,height/2);
        gateAnimation = new AnimationSprite("objectImgs/portal.png", 4, 1,-1,false,false);
        gateAnimation.SetOrigin(gateAnimation.width,gateAnimation.height/2);
        gateAnimation.scaleX = 2f;
        AddChild(gateAnimation);
        collider.isTrigger = true;
        if(obj != null)
        {
            target = obj.GetStringProperty("target","tiledMaps/mainMenu.tmx");
        }
    }

    void Update()
    {
        gateAnimation.Animate(0.12f);
    }
    void OnCollision(GameObject other)
    {
        //saves player stats and loads next level
        if (other is Player p)
        {
            p.Save(); 
            ((MyGame)game).LoadLevel(target);
        } 
    }
}