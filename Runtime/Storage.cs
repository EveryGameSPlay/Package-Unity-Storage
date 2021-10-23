using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Egsp.Core
 {
     // todo: Создать пакет с сериализатором от Sirenix (Odin Serializer).
     
     /// <summary>
     /// <para>Через данный класс осуществляется доступ к данным вне приложения.
     /// Учитывает платформу и инструментарий.</para>
     /// <para>Корневая папка на разных устройствах отличается.</para>
     /// <para>
     /// Mobile - Application.persistentDataPath/Storage/;
     /// PC - Application.dataPath/Storage/
     /// </para>
     /// </summary>
     public partial class Storage : SingletonRaw<Storage>
     {
         private string _defaultRootPath;
         private ISerializer _defaultSerialzier;
         
         private static List<StorageId> _profiles;

         // todo: Добавить используемый путь как вариант возвращаемого значения.
         public static string RootPath { get; private set; } = _defaultRootPath;

         [NotNull]
         public static StorageData Common { get; private set; }
         
         [NotNull]
         public static StorageData Current { get; set; }

         private static ILogger Logger => Debug.unityLogger;

         [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
         private static void InitializeSingleton()
         {
             Logger.Log("Инициализация менеджера сцен.");
             // Создает экземпляр перед полной загрузкой сцены.
             if (!Exist)
                 CreateInstance();
         }

         protected override void OnInstanceCreatedInternal()
         {
             SetupRootFolderPath();
             SetupSerializer();
             
             // todo: refactor
             LoadProfiles();
         }

         private void SetupRootFolderPath()
         {
             _defaultRootPath = Application.platform switch
             {
                 RuntimePlatform.Android => Application.persistentDataPath,
                 RuntimePlatform.OSXPlayer => Application.persistentDataPath,
                 RuntimePlatform.IPhonePlayer => Application.persistentDataPath,
                 RuntimePlatform.WindowsPlayer => Application.dataPath,
                 RuntimePlatform.LinuxPlayer => Application.dataPath,
                 
                 RuntimePlatform.WebGLPlayer => Application.persistentDataPath,
                 
                 RuntimePlatform.OSXEditor => Application.dataPath,
                 RuntimePlatform.WindowsEditor => Application.dataPath,
                 RuntimePlatform.LinuxEditor => Application.dataPath,
                 
                 
                 RuntimePlatform.PS4 => Application.persistentDataPath,
                 RuntimePlatform.XboxOne => Application.persistentDataPath,
                 RuntimePlatform.tvOS => Application.persistentDataPath,
                 RuntimePlatform.Switch => Application.persistentDataPath,
                 RuntimePlatform.Lumin => Application.persistentDataPath,
                 RuntimePlatform.Stadia => Application.persistentDataPath,
                 RuntimePlatform.CloudRendering => Application.persistentDataPath,
                 RuntimePlatform.GameCoreXboxOne => Application.persistentDataPath,
                 RuntimePlatform.PS5 => Application.persistentDataPath,
                 _ => Application.persistentDataPath
             };
         }

         private void SetupSerializer()
         {
             _defaultSerializer = new UnitySerializer();
         }

         private static void LoadProfiles()
         {
             var commonProfile = new StorageId(CommonProviderName);
             var commonProvider = new StorageData(commonProfile, _defaultRootPath, _defaultSerializer, DefaultExtension);

             Common = commonProvider;

             var profiles = Common.GetObjects<StorageId>("Profiles/profiles");

             if (profiles.IsSome)
             {
                 var list = profiles.option;

                 if (list.Count == 0)
                 {
                     _profiles = list;
                     Current = Common;
                 }
                 else
                 {
                     var localProfile = _profiles[0];
                     Current = new StorageData(localProfile, _defaultRootPath, _defaultSerializer, DefaultExtension);
                 }
             }
             else
             {
                 _profiles = null;
             }
             
             // Если локальный профиль не найден, то он будет ссылаться на глобальный профиль
             if (!profiles.IsSome || profiles.option.Count == 0)
             {
                 _profiles = profiles.option;
                 Current = Common;
             }
             else
             {
                 _profiles = profiles.option;
                 var localProfile = _profiles[0];
                 Current = new StorageData(localProfile, _defaultRootPath, _defaultSerializer, DefaultExtension);
             }
         }

         /// <summary>
         /// Получение всех существующих профилей.
         /// Возвращается копия списка. Однако элементы списка НЕ копии!
         /// </summary>
         public static List<StorageId> GetProfiles()
         {
             return _profiles.ToList();
         }

         /// <summary>
         /// Установка локального профиля.
         /// Если профиль не будет найдет в списке, то вылетит исключение. 
         /// </summary>
         public static void SwitchCurrentProfile(StorageId profile)
         {
             if(!_profiles.Contains(profile))
                 throw new Exception($"Profile {profile.Id} not exist in current list of profiles!");
             
             Current = new StorageData(profile, _defaultRootPath, new UnitySerializer(), DefaultExtension);
         }

     }
}




