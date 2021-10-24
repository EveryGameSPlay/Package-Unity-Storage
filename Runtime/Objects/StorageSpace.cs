using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Egsp.Core;

// todo: добавить инструмент индексирования файлов, для последующего кеширования путей к ним или данных в них. 

namespace Egsp.Core
{
    /// <summary>
    /// Предоставляет методы для доступа к данным в определенном профиле.
    /// </summary>
    public partial class StorageSpace
    {
        public readonly StorageId Id;

        public readonly string PathToSpace;

        public StorageSpace(StorageId id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException();
            
            Id = id;
            PathToSpace = Path.Combine(Storage.Path, id);
        }
    }
}