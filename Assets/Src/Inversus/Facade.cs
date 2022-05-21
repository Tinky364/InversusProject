using Inversus.Manager;

namespace Inversus
{
    public static class Facade
    {
        public static MainManager SMainManager => MainManager.Instance;
        public static SceneCreator SSceneCreator => SceneCreator.Instance;
        public static SubSceneManager SSubSceneManager => SubSceneManager.Instance;
        public static EventBus SEventBus => EventBus.Instance;
        public static CanvasManager SCanvasManager => CanvasManager.Instance;
        public static InputManager SInputManager => InputManager.Instance;
        public static GameCreator SGameCreator => GameCreator.Instance;
        public static Database SDatabase => Database.Instance;
    }
}
