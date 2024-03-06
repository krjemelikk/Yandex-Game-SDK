﻿using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using YaSDK.Source.SDK.Interfaces;
using Progress = YaSDK.Source.Data.Progress;

namespace YaSDK.Source.SDK.EditorServices
{
   public class EditorProgress : IProgressService
   {
      public void SaveProgress(Progress progress)
      {
         var json = JsonConvert.SerializeObject(progress);
         PlayerPrefs.SetString("Progress", json);
      }

      public IEnumerator LoadProgress()
      {
         if (PlayerPrefs.HasKey("Progress"))
         {
            YandexSDKData.Instance.Progress =
               JsonConvert.DeserializeObject<Progress>(PlayerPrefs.GetString("Progress"));
            yield return null;
         }

         YandexSDKData.Instance.Progress = new Progress();
         yield return null;
      }

      [MenuItem("YandexSDK/Clean editor progress")]
      public static void CleanUp()
      {
         PlayerPrefs.DeleteAll();
      }
   }
}