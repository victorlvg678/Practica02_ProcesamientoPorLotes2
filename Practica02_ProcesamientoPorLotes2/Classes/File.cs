using Serilog;
using System.Text;

namespace Practica02_ProcesamientoPorLotes2.Classes
{
    public class File
    {
        private readonly Guid _guid;
        private string _path;
        private string _name;
        private string _savePath;
        private byte[] _content;
        private long _size;
        private bool _isLoaded;
        private bool _isProcessed;

        public File()
        {
            _guid = Guid.NewGuid();
            _path = string.Empty;
            _savePath = string.Empty;
            _name = string.Empty;
            _content = new byte[0];
            _size = 0;
            _isLoaded = false;
        }

        public File(string name)
        {
            _guid = Guid.NewGuid();
            _path = string.Empty;
            _savePath = string.Empty;
            _name = !string.IsNullOrWhiteSpace(name) ? name : string.Empty;
            _content = new byte[0];
            _size = 0;
            _isLoaded = false;
            _isProcessed = false;
        }

        public File(string name, string path)
        {
            _guid = Guid.NewGuid();
            _path = isAValidFolder(path) && CreateFolderIfNotExists(path) ? path : string.Empty;
            _savePath = string.Empty;
            _name = !string.IsNullOrWhiteSpace(name) ? name : string.Empty;
            _content = new byte[0];
            _size = 0;
            _isLoaded = false;
            _isProcessed = false;
        }

        public File(string name, string path, string savePath)
        {
            _guid = Guid.NewGuid();
            _path = isAValidFolder(path) && CreateFolderIfNotExists(path) ? path : string.Empty;
            _savePath = isAValidFolder(savePath) && CreateFolderIfNotExists(savePath) ? savePath : string.Empty;
            _name = !string.IsNullOrWhiteSpace(name) ? name : string.Empty;
            _content = new byte[0];
            _size = 0;
            _isLoaded = false;
            _isProcessed = false;
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

        public string Name
        {
            set
            {
                _name = !string.IsNullOrWhiteSpace(value) ? value : string.Empty;
            }

            get => _name;
        }

        public byte[] Content
        {
            set
            {
                if (value.Length == 0)
                    return;

                _content = value;
            }

            get => _content;
        }

        public long Size
        {
            set
            {
                _size = value >= 0 ? value : 0;
            }

            get => _size;
        }

        public Guid GUID
        {
            get => _guid;
        }

        public bool IsLoaded
        {
            set => _isLoaded = value;
            get => _isLoaded;
        }

        public bool IsProcessed
        {
            set => _isProcessed = value;
            get => _isProcessed;
        }

        public string getContentAsString()
        {
            if (_content == null || _content.Length == 0)
                return string.Empty;

            return _content.ToString() ?? string.Empty;
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                Log.Error($"{_guid} - File must be have a name before saving");
                return;
            }


            if (!_isLoaded)
            {
                Log.Error($"{_guid} - File data must be loaded before saving for file \"{_name}\"");
                return;
            }

            try
            {
                string fullName = System.IO.Path.Combine(_savePath, _name);

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    Log.Error($"{_guid} - File \"{fullName}\" full name is not valid");
                    return;
                }

                string fileContent = Encoding.ASCII.GetString(_content);
                using (var fs = System.IO.File.Create(fullName))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fs))
                    {
                        streamWriter.Write(fileContent);
                    }
                }

                Log.Information($"{_guid} - File \"{fullName}\" saved successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"{_guid} - Something went wrong while saving \"{_name}\" - Exception: {ex.Message}");
            }
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
