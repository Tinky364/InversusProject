﻿using Inversus.Game;
using Inversus.Manager;

namespace Inversus
{
    public static class Facade
    {
        public static MainManager SMainManager => MainManager.Instance;
        public static SceneCreator SSceneCreator => SceneCreator.Instance;
        public static EventBus SEventBus => EventBus.Instance;
        public static InputProfileManager SInputProfileManager => InputProfileManager.Instance;
        public static GameCreator SGameCreator => GameCreator.Instance;
        public static Database SDatabase => Database.Instance;
        public static OnlineGameManager SOnlineGameManager => OnlineGameManager.Instance;

        public static SubSceneManager SSubSceneManager => SubSceneManager.Instance;
        public static MainMenuSubSceneManager SMainMenuSubSceneManager => 
            SubSceneManager.Instance as MainMenuSubSceneManager;
        
        public static CanvasManager SCanvasManager => CanvasManager.Instance;
        public static MainMenuCanvasManager SMainMenuCanvasManager =>
            CanvasManager.Instance as MainMenuCanvasManager;
    }
}
