using System.Collections.Generic;
using GXPEngine;                                

public class MyGame : Game
{
	string startLevel = "tiledMaps/level3.tmx";
	string nextLevel = null;
	Sound[] soundtrack;
	SoundChannel soundtrackChannel;

	public MyGame() : base(400, 400, false, false, 900, 750,true)        
	{
		soundtrack = new Sound[] { new Sound("sounds/mainMenuSoundtrack.mp3",true,true), 
									new Sound("sounds/gameplaySoundtrack.mp3",true,true) };
		OnAfterStep += CheckNextLevel;
		LoadLevel(startLevel);
	}

	static void Main()							
	{
		MyGame game = new MyGame();
		game.Start(); // starts the game			
	}

	void Update()
    {
		if (Input.GetKeyDown(Key.LEFT_SHIFT)) LoadLevel(startLevel);
	}
	public void LoadLevel(string pLevel)
	{
		nextLevel = pLevel;
	}

	void CheckNextLevel()
    {
		if(nextLevel != null)
        {
			if(soundtrackChannel != null) soundtrackChannel.Stop();
			Sound soundToPlay;
			DestroyAll();
			AddChild(new Level(nextLevel));
			soundToPlay = nextLevel == "tiledMaps/mainMenu.tmx" ? soundtrack[0]:soundtrack[1];
			soundtrackChannel = soundToPlay.Play();
			nextLevel = null;
        }
    }

	void DestroyAll()
	{
		List<GameObject> children = GetChildren();
		foreach (GameObject child in children)
		{
			child.Destroy();
		}
	}

}