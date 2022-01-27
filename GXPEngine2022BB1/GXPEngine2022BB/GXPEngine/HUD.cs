using System;
using System.Drawing;
using System.Collections.Generic;
using GXPEngine;
public class HUD:Canvas
{
    Player player;
    AnimationSprite lives;
    Sprite bulletCount, playerLvl;
    int currentLife;
    int _gameCurrentLevel;
    int framesToAnimate;

    public int GameCurrentLevel { get => _gameCurrentLevel; set => _gameCurrentLevel = value; }  

    public HUD(Player pPlayer):base(300,200,false)
    {
        player = pPlayer;
        currentLife = pPlayer.Hp;
        //Player bullet count
        bulletCount = new Sprite("hudImgs/bulletCount.png");
        bulletCount.SetXY(5, 20);
        AddChild(bulletCount);
        //Player lvl
        playerLvl = new Sprite("hudImgs/levelImg.png");
        playerLvl.SetXY(3, 45);
        AddChild(playerLvl);
        //Player health
        lives = new AnimationSprite("hudImgs/heartsAnimation.png",1,21,-1,false,false);
        lives.SetCycle(0, 21);
        lives.SetScaleXY(0.15f, 0.15f);
        AddChild(lives);
        framesToAnimate = 0;
    }

    void Update()
    {
        if (framesToAnimate > 20) framesToAnimate = 0;
            if (currentLife > player.Hp)
            {
                currentLife = player.Hp;
                framesToAnimate += 4;
            }
            if (framesToAnimate > lives.currentFrame)
            {
                lives.Animate(0.12f);
            }

        graphics.Clear(Color.Empty);
        graphics.DrawString("X  "+player.AbilityCount, SystemFonts.DefaultFont, Brushes.AntiqueWhite, 18, 22);
        graphics.DrawString("LEVEL   " + player.Lvl, SystemFonts.DefaultFont, Brushes.BlanchedAlmond, 20, 45);
        graphics.DrawString("LVL: " + GameCurrentLevel, SystemFonts.DefaultFont, Brushes.BlanchedAlmond, 180, 10);
    }

}

