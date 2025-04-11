using Cysharp.Threading.Tasks;
using TEngine;
using UnityEditor.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameLogic
{
    public partial class MainView
    {
        public Button StartBtn;
    }

    [Window(UILayer.UI, fromResources: false)]
    public partial class MainView : UIWindow
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Log.Info("MainView OnCreate");
        }

        protected override void BindMemberProperty()
        {

            var uHubComponent = transform.GetOrAddComponent<UHubComponent>();
            uHubComponent.BindUI(this, gameObject);
            // Log.Info($"结束绑定 + {StartBtn}");
            uHubComponent.BindClick(StartBtn, () => OnClickStartBtn().Forget());
        }

        public async UniTask OnClickStartBtn()
        {
            // Log.Info("OnClickStartBtn");
            GameModule.UI.HideUI<MainView>();
            await GameModule.Scene.LoadScene("Game", LoadSceneMode.Single, false, 100, (handle) =>
            {
                // Log.Info($"加载场景 {handle} {handle.SceneObject}");
                if (handle.IsValid)
                {
                    Log.Info($"加载场景 {handle.SceneName} {handle.SceneObject}");
                    GameSceneManager.Instance.Init();
                }
                else
                {
                    Log.Error("加载场景失败");
                }

            }, true, (progress) =>
            {
                Log.Info($"加载进度 {progress}");
            });
            // Log.Info($"加载场景 {s} {s?.SceneObject}");
        }
    }
}
