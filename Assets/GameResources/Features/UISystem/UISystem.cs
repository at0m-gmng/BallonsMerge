namespace GameResources.Features.UISystem
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using SO;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public sealed class UISystem : IUISystem
    {
        public UISystem(UIConfig uiConfig)
        {
            _uiConfig = uiConfig;
        }
        private readonly UIConfig _uiConfig;

        private GameObject _windowsParent;
        private Dictionary<UIWindowID, GameObject> _windows = new Dictionary<UIWindowID, GameObject>();
        
        public async UniTask Initialize()
        {
            _windowsParent = Object.Instantiate(_uiConfig.WindowsParant);
            
            foreach (UIData window in _uiConfig.Windows)
            {
                await LoadAndCreateWindow(window.ID, window.Reference);
            }
        }
        
        public void ShowWindow(UIWindowID id)
        {
            if (_windows.TryGetValue(id, out GameObject window))
            {
                window.SetActive(true);
            }
        }
        
        public void HideWindow(UIWindowID id)
        {
            if (_windows.TryGetValue(id, out GameObject window))
            {
                window.SetActive(false);
            }
        }
        
        public async UniTask ReleaseWindow(UIWindowID id)
        {
            if (_windows.TryGetValue(id, out GameObject window))
            {
                if (window != null)
                {
                    Addressables.Release(window);
                    Object.Destroy(window);
                }
                _windows.Remove(id);
            }
        }
        
        public async UniTask ReleaseAllWindows()
        {
            UIWindowID[] windowIDs = _windows.Keys.ToArray();
            foreach (UIWindowID id in windowIDs)
            {
                await ReleaseWindow(id);
            }
        }
        
        private async UniTask LoadAndCreateWindow(UIWindowID id, AssetReference reference)
        {
            UniTask<GameObject> handle = reference.LoadAssetAsync<GameObject>().ToUniTask();
            (bool IsCanceled, GameObject Result) isResult = await handle.SuppressCancellationThrow();
            
            if (!isResult.IsCanceled)
            {
                GameObject window = Object.Instantiate(isResult.Result, _windowsParent.transform);
                window.SetActive(false);
                _windows[id] = window;
            }
        }
    }

    public interface IUISystem
    {
        public UniTask Initialize();
        public void ShowWindow(UIWindowID id);
        public void HideWindow(UIWindowID id);
        public UniTask ReleaseWindow(UIWindowID id);
        public UniTask ReleaseAllWindows();
    }
}