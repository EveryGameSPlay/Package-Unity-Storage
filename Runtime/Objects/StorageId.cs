using System;

namespace Egsp.Core
{
    [Serializable]
    public readonly struct StorageId
    {
        public readonly string Id;
        
        public StorageId(Option<string> id)
        {
            if (!id)
            {
                Id = StorageDefines.DefaultStorageId;
                return;
            }
            Id = id;
        }
    }
}