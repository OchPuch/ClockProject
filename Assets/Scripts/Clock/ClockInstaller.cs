using UnityEngine;
using Zenject;

namespace Clock
{
    public class ClockInstaller : MonoInstaller
    {
        [SerializeField] [Min(1)] 
        private int retrieveInternetTimeInMinutes;
        private ClockService _clockService;

        public override void InstallBindings()
        {
            _clockService = new ClockService();
            _clockService.SetInternetRetrieveTimeUpdater(retrieveInternetTimeInMinutes, gameObject);
            Container.BindInterfacesAndSelfTo<ClockService>().FromInstance(_clockService).AsSingle().NonLazy();

        }
    }
}