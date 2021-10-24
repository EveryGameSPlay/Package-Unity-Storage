using System.IO;

namespace Egsp.Core
{
    public partial class StorageSpace
    {
        private string ApplyExtension(string file, string ext, bool changeExtension = false)
        {
            if (Path.HasExtension(file))
            {
                if (changeExtension)
                    return Path.ChangeExtension(file, ext);
                return file;
            }
            
            return Path.Combine(file, ext);
        }
    }
}