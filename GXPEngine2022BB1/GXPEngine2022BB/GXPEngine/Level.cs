using System;
using GXPEngine;
using TiledMapParser;


//------------------------Level-----------------------------------//
// Inherits from GameObject
// Creates level instances
// Can only exist one at a time
// Used to create the whole level structure and includes all objects
//------------------------------------------------------------------------//

public class Level: GameObject
{
    String currentLevelName;
    TiledLoader loader;
    Player player;
    HUD hud;
    int currentLvl;

    public Level(String pFileName = null)
    {
        currentLevelName = pFileName;
        loader = new TiledLoader(pFileName);
        Map levelData = MapParser.ReadMap(pFileName);
        CreateLevel();
    }

    void CreateLevel()
    {       
        switch(currentLevelName)
        {
            case "tiledMaps/mainMenu.tmx":
                loader.LoadImageLayers();
                loader.LoadTileLayers(0);
                loader.autoInstance = true;
                loader.LoadObjectGroups();

                break;

            case "tiledMaps/level1.tmx":
                currentLvl = 1;
                loader.addColliders = false;
                loader.LoadImageLayers(); //sky
                loader.autoInstance = true;
                loader.LoadObjectGroups(1); //quit button

                loader.rootObject = this; //level class becomes parent to add scrolling 
                for (int i = 0; i<loader.NumTileLayers; i++)
                {
                    if (i == 2 || i == 4) loader.addColliders = true;
                    else loader.addColliders = false;
                    loader.LoadTileLayers(i); //background decoration                   
                }

                loader.addColliders = true;
                loader.LoadObjectGroups(0); //enemies, players, and all other game objects
                
                loader.addColliders = false;
                for(int i = 8; i < loader.NumTileLayers; i++)
                {
                    loader.LoadTileLayers(i); //frontground decoration
                }

                break;

            case "tiledMaps/level2.tmx":
                currentLvl = 2;
                loader.addColliders = false;
                loader.LoadImageLayers(); //sky
                loader.autoInstance = true;
                loader.LoadObjectGroups(1); //quit button

                loader.rootObject = this;

                for(int i = 0; i < loader.NumTileLayers - 1; i++)
                {
                    loader.LoadTileLayers(i);
                }

                loader.addColliders = true;
                loader.LoadTileLayers(2); //collision tiles

                loader.LoadObjectGroups(0); //enemies, players, etc

                break;

            case "tiledMaps/level3.tmx":
                currentLvl = 3;
                loader.addColliders = false;
                loader.LoadImageLayers(); //Cellar
                loader.autoInstance = true;
                loader.LoadObjectGroups(1); //quit button

                loader.rootObject = this;

                loader.LoadTileLayers(0);

                loader.addColliders= true;
                loader.LoadTileLayers(1);
                loader.LoadObjectGroups(0); //enemies, players, etc

                break;
            
            case "tiledMaps/level4.tmx":
                currentLvl = 4;
                loader.addColliders = false;
                loader.LoadImageLayers();
                loader.autoInstance= true;
                loader.LoadObjectGroups();
                break;

            default:
                Console.WriteLine("Map not found");
                break;
        }

        //find player
        if (currentLevelName != "tiledMaps/mainMenu.tmx")
        {
            player = FindObjectOfType<Player>();
            if(player != null)
            {
                CreateHUD(player);
                AssignTargetToEnemies(player);
            }   
        }
    }

    void Scrolling(GameObject target)
    {
        int boundary = 150;
        if (target.x + x < boundary)
        {
            x = boundary - target.x;
        }
        if (target.x + x > game.width - boundary)
        {
            x = game.width - boundary - target.x;
        }
    }

    void Update()
    {
        if (player == null) return;
        Scrolling(player);
    }

    void CreateHUD(Player pPlayer)
    {
        if (player == null) return;
        hud = new HUD(pPlayer);
        hud.gameCurrentLevel = currentLvl;
        game.AddChild(hud);
    }

    void AssignTargetToEnemies(GameObject player)
    {
        EnemyBehaviour[] enemies = FindObjectsOfType<EnemyBehaviour>();
        foreach (EnemyBehaviour enemy in enemies)
        {
            enemy.target = player;
        }
    }

}
