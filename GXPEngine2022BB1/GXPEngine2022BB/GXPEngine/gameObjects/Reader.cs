using GXPEngine;
using TiledMapParser;
using System.Drawing;


//------------------------Reader-----------------------------------//
// Inherits from Sprite
// Create reader board objects
// Only active while colliding with player 
// Used to instruct users about the game
//------------------------------------------------------------------------//

public class Reader : Sprite
{
    string text, text2;
    EasyDraw textBoard;
    public Reader(TiledObject obj = null) : base("objectImgs/conversationBoard.png")
    {
        textBoard = new EasyDraw(400,70,false);
        textBoard.SetOrigin(textBoard.width/2, -60);
        AddChild(textBoard);
        collider.isTrigger = true;
        if(obj != null)
        {
            text = obj.GetStringProperty("text","You can do it!");
            text2 = obj.GetStringProperty("text2", "");
        }
    }
    void Update()
    {
        textBoard.ClearTransparent();
    }

    public void ReadText()
    {
        textBoard.Fill(Color.Black, 150);
        textBoard.Rect(0, 0, textBoard.width*2, textBoard.height*2);
        textBoard.Fill(Color.FloralWhite,255);
        textBoard.TextSize(20f);
        textBoard.TextAlign(CenterMode.Min,CenterMode.Min);
        textBoard.Text(text, 0, 0);
        if (text2 != "") textBoard.Text(text2, 0, 30);
    }

}
