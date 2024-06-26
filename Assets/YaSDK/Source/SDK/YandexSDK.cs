using System.Collections;
using UnityEngine;
using YaSDK.Source.SDK.Services;
using YaSDK.Source.SDK.Services.EditorServices;
using YaSDK.Source.SDK.Services.EditorServices.Utilities;
using YaSDK.Source.SDK.Services.Interfaces;
using YaSDK.Source.SDK.Services.YandexServices;

namespace YaSDK.Source.SDK
{
   public class YandexSDK : SingletonBehaviour<YandexSDK>
   {
#if UNITY_EDITOR
      [SerializeField] private EditorSDKSettings _editorSDKSettings;
#endif

      private IPurchaseHandler _purchaseHandler;

      public IConsole Console { get; private set; }
      public IGameReadyAPIService GameReadyService { get; private set; }
      public IAdvertisementService AdvertisementService { get; private set; }
      public IEnvironmentService EnvironmentService { get; private set; }
      public ILeaderboardService LeaderboardService { get; private set; }
      public IPurchaseService PurchaseService { get; private set; }
      public IProgressService ProgressService { get; private set; }
      public IProductDataService ProductsService { get; private set; }

      private void Awake() =>
         DontDestroyOnLoad(this);

      public IEnumerator Initialize()
      {
#if UNITY_EDITOR
         Console = new EditorConsole();
         GameReadyService = new EditorGameReadyAPI();
         AdvertisementService = new EditorAdvertisement();
         EnvironmentService = new EditorEnvironment(_editorSDKSettings.EnvironmentData);
         LeaderboardService = new EditorLeaderboard();
         PurchaseService = new EditorPurchases();
         ProgressService = new EditorProgress();
         ProductsService = new EditorProducts(_editorSDKSettings.Products);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
         Console = gameObject.AddComponent<YandexSDKConsole>();
         GameReadyService = gameObject.AddComponent<YandexSDKGameReadyAPI>();
         AdvertisementService = gameObject.AddComponent<YandexSDKAdvertisement>();
         EnvironmentService = gameObject.AddComponent<YandexSDKEnvironment>();
         LeaderboardService = gameObject.AddComponent<YandexSDKLeaderboard>();
         PurchaseService = gameObject.AddComponent<YandexSDKPurchases>();
         ProgressService = gameObject.AddComponent<YandexSDKProgress>();
         ProductsService = gameObject.AddComponent<YandexSDKProducts>();
#endif
         _purchaseHandler = new PurchaseHandler(PurchaseService);
         _purchaseHandler.Initialize();

         yield return LoadAllData();
      }

      public void PauseGame()
      {
         Time.timeScale = 0;
         AudioListener.pause = true;
      }

      public void UnPauseGame()
      {
         Time.timeScale = 1;
         AudioListener.pause = false;
      }

      private void OnDisable() =>
         _purchaseHandler.CleanUp();

      private IEnumerator LoadAllData()
      {
         yield return ProductsService.LoadProductData();
         yield return ProgressService.LoadProgress();
         yield return EnvironmentService.LoadEnvironmentData();
      }
   }
}