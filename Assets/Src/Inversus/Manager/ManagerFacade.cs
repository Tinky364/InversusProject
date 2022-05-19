namespace Inversus.Manager
{
    public static class ManagerFacade
    {
        public static MainManager SMainManager => MainManager.Instance;
        public static SceneCreator SSceneCreator => SceneCreator.Instance;
        public static SubSceneManager SSubSceneManager => SubSceneManager.Instance;
        public static EventBus SEventBus => EventBus.Instance;
        public static CanvasManager SCanvasManager => CanvasManager.Instance;
        public static InputManager SInputManager => InputManager.Instance;
    }
}
