using Inversus.Data;
using Inversus.Manager;

using static Inversus.Facade;

namespace Inversus.Game
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

