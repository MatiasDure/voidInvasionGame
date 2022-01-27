using System;
using System.Collections.Generic;
using GXPEngine;
using TiledMapParser;

public class Button : Sprite
{

    string target;
    Sprite[] buttonImgs;
    int img;

    public Button(TiledObject obj = null):base("objectImgs/hitbox.jpg",false,false)
    {
        buttonImgs = new Sprite[] { new Sprite("buttonImgs/playButton.png"), new Sprite("buttonImgs/QuitButton.png") };
        alpha = 0;
        //SetScaleXY(2,2);
        if (obj != null) target = obj.GetStringProperty("target");
        img = target == "tiledMaps/level1.tmx" ? 0 : 1;
        if (target == "tiledMaps/mainMenu.tmx") buttonImgs[img].SetScaleXY(.5f,.5f);
        buttonImgs[img].SetOrigin(buttonImgs[img].width/2,buttonImgs[img].height/2);
        AddChild(buttonImgs[img]);

    }
    void Update()
    {
        if (buttonImgs[img].HitTestPoint(Input.mouseX, Input.mouseY))
        {
            buttonImgs[img].SetColor(255, 0, 0);
            if (Input.GetMouseButtonDown(0) && target != "") ((MyGame)game).LoadLevel(target);
            else if(Input.GetMouseButtonDown(0) && target == "") ((MyGame)game).Destroy();
        }
        else buttonImgs[img].SetColor(255, 255, 255);
    }
}