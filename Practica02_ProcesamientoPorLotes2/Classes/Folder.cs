using Serilog;

namespace Practica02_ProcesamientoPorLotes2.Classes
{
    public class Folder
    {
        private Guid _guid;
        private string _path;
        private string _savePath;

        public Folder()
        {
            _guid = Guid.NewGuid();
            _path = Directory.GetCurrentDirectory();
            _savePath = Directory.GetCurrentDirectory();
        }

        public Folder(string path)
        {
            _path = isAValidFolder(path) && CreateFolderIfNotExists(path) ? path : Directory.GetCurrentDirectory();
            _savePath = Directory.GetCurrentDirectory();
        }

        public Folder(string path, string savePath)
        {
            _path = isAValidFolder(path) && CreateFolderIfNotExists(path) ? path : Directory.GetCurrentDirectory();
            _savePath = isAValidFolder(savePath) && CreateFolderIfNotExists(savePath) ? savePath : Directory.GetCurrentDirectory();
        }

        public string Path
        {
            set
            {
                _path = isAValidFolder(value) && CreateFolderIfNotExists(value) ? value : Directory.GetCurrentDirectory();
            }

            get => _path;
        }

        public string SavePath
        {
            set
            {
                _savePath = isAValidFolder(value) && CreateFolderIfNotExists(value) ? value : Directory.GetCurrentDirectory();
            }

            get => _savePath;
        }

        private bool isAValidFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                _ = System.IO.Path.GetFullPath(path);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"{_guid} - Something went wrong while validating folder path - Exception: {ex.Message}");
            }

            return false;
        }

        private bool CreateFolderIfNotExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                if (Directory.Exists(path))
                    return true;

                _ = Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"{_guid} - Something went wrong when checking if folder exists - Exception: {ex.Message}");
            }

            return false;
        }
    }
}
