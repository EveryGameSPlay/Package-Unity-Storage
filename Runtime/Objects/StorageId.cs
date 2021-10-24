using System;

namespace Egsp.Core
{
    [Serializable]
    public readonly struct StorageId
    {
        // Использует нижний регистр всегда.
        public readonly string Value;
        
        public StorageId(Option<string> id)
        {
            if (!id)
            {
                Value = StorageDefines.GeneralStorageId;
                return;
            }

            Value = id.option.ToLower();
        }

        public static implicit operator string(StorageId id)
        {
            return id.Value;
        }
    }
}