using System;
using System.IO;

namespace ListaDeCompras.SqLite
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(path, filename);
        }
    }
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
    }
}
