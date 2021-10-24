using System.Collections.Generic;
using System.IO;

namespace Egsp.Core
{
    // todo: реализовать настоящий интерпретатор для чтения подобного рода данных.
    public class StorageInterpreter
    {
        public static IEnumerator<string> ReadBlock(FileInfo file, string blockId)
        {
            var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream);

            string str = reader.ReadLine();
            while(!string.IsNullOrWhiteSpace(str))
            {
                
                
                yield return str;

                str = reader.ReadLine();
            }
        } 
    }
}