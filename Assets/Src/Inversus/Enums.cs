namespace Inversus
{
    public enum FrameRate
    {
        Default = 0, Rate30 = 30, Rate45 = 45,
        Rate60 = 60, Rate100 = 100, Rate120 = 120
    }

    public enum States
    {
        MainMenu, PlayLocallyMenu, PlayOnlineMenu, 
        Loading, InGame, GamePauseMenu
    }

    public enum SubSceneLoadMode
    {
        Single, Additive
    }
}
