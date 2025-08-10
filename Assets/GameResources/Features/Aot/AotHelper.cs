using GameResources.Features.Factories;
using GameResources.Features.GameControllers;
using GameResources.Features.GameStates;
using GameResources.Features.GameStates.Core;
using GameResources.Features.PersistentProgress;
using GameResources.Features.SaveLoadSystem;
using GameResources.Features.UISystem.SO;
using UnityEngine;
using Zenject;

namespace GameResources.Features.Aot
{
    public sealed class AotHelper : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private UIConfig _uiConfig = default;
        
        [Header("Controllers")]
        [SerializeField] private GameFacade _gameFacade = default;
        
        private void Start()
        {
            var diContainer = new DiContainer();
            var managerInstaller = new ZenjectManagersInstaller();
            var projectContext = new SceneContextRegistry();
            var goContext = new GameObjectContext();
            var sceneContextRegistry =  new SceneContextRegistryAdderAndRemover(new SceneContext(), new SceneContextRegistry());

            var ballFactory = new BallFactory();
            var vfxFactory = new VFXFactory();

            var uisystem = new UISystem.UISystem(_uiConfig);
            var saveload = new SaveLoadService();
            var persistentProgressService = new PersistentProgressService(saveload);
            var stateMachine = new GameStateMachine();
            var bootstrapState = new BootstrapState(stateMachine, uisystem, _gameFacade, persistentProgressService);
            var menuState = new MenuState(stateMachine, uisystem, persistentProgressService);
            var gameState = new GameState(stateMachine, uisystem, _gameFacade, persistentProgressService);
            var resultState = new ResultState(stateMachine, uisystem, persistentProgressService);
        }
    }
}
