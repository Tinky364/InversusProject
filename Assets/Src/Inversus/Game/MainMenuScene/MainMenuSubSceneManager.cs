using Inversus.Manager;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Game
{
    public class MainMenuSubSceneManager : SubSceneManager
    {
        protected override void OnSceneLoaded()
        {
            SMainManager.State = States.MainMenu;
        }

        public void Quit()
        {
            SMainManager.Quit();
        }
    }
}

