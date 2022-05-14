namespace Inversus.Manager
{
    public static class ManagerFacade
    {
        public static MainManager SMainManager => MainManager.Instance;
        public static SubSceneCreator SSubSceneCreator => SubSceneCreator.Instance;
        public static SubSceneManager SSubSceneManager => SubSceneManager.Instance;
        public static EventBus SEventBus => EventBus.Instance;
        public static CanvasManager SCanvasManager => CanvasManager.Instance;
    }
}
