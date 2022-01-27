using System;                                   // System contains a lot of default C# libraries 
using System.Collections.Generic;
using GXPEngine;                                // GXPEngine contains the engine

//weird player movement bug (?)
//add soundtrack
//refactor code
//animation for when the player lvls up

public class MyGame : Game
{
	string startLevel = "tiledMaps/mainMenu.tmx";
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

    // For every game object, Update is called every frame, by the engine:
    void Update()
	{
		if (Input.GetKeyDown(Key.Q) && Input.GetKey(Key.LEFT_SHIFT))
        {
			LoadLevel(startLevel);
        }
		if (Input.GetKey(Key.L)) Console.WriteLine(GetDiagnostics());
        if (Input.GetKey(Key.F)) Console.WriteLine(currentFps);	
	}

	// Main() is the first method that's called when the program is run
	static void Main()							
	{
		// Create a "MyGame" and start it
		MyGame game = new MyGame();
		game.Start();
					
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
            Console.WriteLine(soundToPlay.ToString());
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