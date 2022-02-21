using System.Collections.Generic;
using GXPEngine;

//-------------------------------------------What--was--done----------------------------------
//removed all public getters/setters. Most of them stayed as public variables, but managed to make some of them private or protected
//created an updatelasttimeshot in the entity class to change the access modifier of lastTimeShot from public to private
//in StillEnemy and Player class
//Got rid of the gate private getter/setter (no need)
//Managed to change a public setter to a protected setter for HP in the entity class
//Added healthUI for enemies
//Created AddAnim() and RemoveAnim() methods for obstacle enemies to make code more readable 
//Created a hot reload for the game (left-shift)
//Removed duplicated lastTimeHit from player and obstaclesEnemy, and moved it to Entity
//Currently there are 3 public variables: target(enemyBehaviour), EXP (player), currentLevel (HUD)
//Added a check to avoid throwing null exceptions when there is no player. Game works without any player
//Added class documentation
//-----------------------------------------------------------------------------------------------

//------------------------MyGame-----------------------------------//
// Inherits from Game
// Starts and destroys the game
// Used as a level manager
// Creates and destroys all objects 
//------------------------------------------------------------------------//

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

	static void Main()							
	{
		MyGame game = new MyGame();
		game.Start(); // starts the game			
	}

	void Update()
    {	
		if (Input.GetKeyDown(Key.LEFT_SHIFT)) LoadLevel(startLevel); //Hot reload
		//if (Input.GetKeyDown(Key.F)) System.Console.WriteLine(currentFps); //displays current fps
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