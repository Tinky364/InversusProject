using Oppositum.Game;
using Oppositum.Manager;
using Oppositum.UI;

namespace Oppositum
{
    public static class Facade
    {
        public static MainManager SMainManager => MainManager.Instance;
        public static SceneCreator SSceneCreator => SceneCreator.Instance;
        public static EventBus SEventBus => EventBus.Instance;
        public static InputProfileManager SInputProfileManager => InputProfileManager.Instance;
        public static GameCreator SGameCreator => GameCreator.Instance;
        public static Database SDatabase => Database.Instance;
        public static OnlineManager SOnlineManager => OnlineManager.Instance;

        public static SubSceneManager SSubSceneManager => SubSceneManager.Instance;
        public static MainMenuSubSceneManager SMainMenuSubSceneManager => 
            SubSceneManager.Instance as MainMenuSubSceneManager; 
        public static GameSubSceneManager SGameSubSceneManager => 
            SubSceneManager.Instance as GameSubSceneManager;
        
        public static CanvasManager SCanvasManager => CanvasManager.Instance;
        public static MainMenuCanvasManager SMainMenuCanvasManager =>
            CanvasManager.Instance as MainMenuCanvasManager;
        public static GameCanvasManager SGameCanvasManager =>
            CanvasManager.Instance as GameCanvasManager;
    }
}
