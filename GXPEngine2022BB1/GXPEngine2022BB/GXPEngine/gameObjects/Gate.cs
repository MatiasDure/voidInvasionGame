using System;
using System.Collections.Generic;
using GXPEngine;
using TiledMapParser;

public class Gate:Sprite
{
    string _target;
    public string Target { get => _target; set => _target = value; }

    AnimationSprite gateAnimation;

    public Gate(TiledObject obj = null):base("objectImgs/hitbox.jpg")
    {
        Initialize(obj);
    }

    void Initialize(TiledObject obj = null)
    {
        SetOrigin(width/2,height/2);
        gateAnimation = new AnimationSprite("objectImgs/portal.png", 4, 1,-1,false,false);
        gateAnimation.SetOrigin(gateAnimation.width,gateAnimation.height/2);
        gateAnimation.scaleX = 2f;
        AddChild(gateAnimation);
        collider.isTrigger = true;
        if(obj != null)
        {
            Target = obj.GetStringProperty("target","tiledMaps/mainMenu.tmx");
        }
    }

    void Update()
    {
        gateAnimation.Animate(0.12f);
    }
    void OnCollision(GameObject other)
    {
        if (other is Player p)
        {
            p.Save();
            ((MyGame)game).LoadLevel(Target);
        } 
    }
}