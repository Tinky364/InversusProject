using Oppositum.Data;
using Oppositum.Manager;
using static Oppositum.Facade;

namespace Oppositum.Game
{
    public class MainMenuSubSceneManager : SubSceneManager
    {
        protected override void OnSceneLoaded(SceneData sceneData)
        {
            SMainManager.State = States.MainMenu;
            SInputProfileManager.Disable();
        }

        public void Quit()
        {
            SMainManager.Quit();
        }
    }
}

