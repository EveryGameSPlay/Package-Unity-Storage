using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Egsp.Core
 {
     // todo: Создать пакет с сериализатором от Sirenix (Odin Serializer).
     
     public partial class Storage : SingletonRaw<Storage>
     {
         // Профиль и идентификатор это одно и то же. Просто профиль звучит удобнее для понимания.
         
         // STATIC
         // properties
         // settings
         public static string Path => Instance._baseRootPath;

         // objects
         private static ILogger Logger => Debug.unityLogger;
         public static ISerializer Serializer => Instance._defaultSerializer;

         // todo: добавить проверку на наличие пространств.
         // spaces
         public static StorageSpace General => Instance._generalSpace;
         public static StorageSpace Specified => Instance._specifiedSpace;
         
         // profiles
         public static IReadOnlyCollection<StorageId> Profiles => Instance._profiles;

         // INSTANCE
         // settings
         private string _baseRootPath;
         
         // serializing
         private ISerializer _defaultSerializer;
         
         // profiles
         private List<StorageId> _profiles;
         private Option<StorageId> _generalProfile;
         private Option<StorageId> _specifiedProfile;
         
         // spaces
         private Option<StorageSpace> _generalSpace;
         private Option<StorageSpace> _specifiedSpace;

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
             _baseRootPath = Application.platform switch
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
             var commonProvider = new StorageSpace(commonProfile, _baseRootPath, _defaultSerializer, DefaultExtension);

             General = commonProvider;

             var profiles = General.GetObjects<StorageId>("Profiles/profiles");

             if (profiles.IsSome)
             {
                 var list = profiles.option;

                 if (list.Count == 0)
                 {
                     _profiles = list;
                     Specified = General;
                 }
                 else
                 {
                     var localProfile = _profiles[0];
                     Specified = new StorageSpace(localProfile, _baseRootPath, _defaultSerializer, DefaultExtension);
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
                 Specified = General;
             }
             else
             {
                 _profiles = profiles.option;
                 var localProfile = _profiles[0];
                 Specified = new StorageSpace(localProfile, _baseRootPath, _defaultSerializer, DefaultExtension);
             }
         }

         /// <summary>
         /// Установка локального профиля.
         /// Если профиль не будет найдет в списке, то вылетит исключение. 
         /// </summary>
         public static void SwitchCurrentProfile(StorageId profile)
         {
             if(!_profiles.Contains(profile))
                 throw new Exception($"Profile {profile.Value} not exist in current list of profiles!");
             
             Specified = new StorageSpace(profile, _baseRootPath, new UnitySerializer(), DefaultExtension);
         }

     }
}




