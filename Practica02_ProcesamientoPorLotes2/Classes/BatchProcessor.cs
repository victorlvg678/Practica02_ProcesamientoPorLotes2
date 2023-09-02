using Serilog;
using System.IO;
using System.Text;

namespace Practica02_ProcesamientoPorLotes2.Classes
{
    public class BatchFileProcessor
    {
        private readonly Guid _guid;
        private List<File> _files = new List<File>();
        private string _extension;
        private Folder _folder;
        private Action<File> _process;

        public BatchFileProcessor(string path, string extension, string savePath, Action<File> process)
        {
            _guid = Guid.NewGuid();
            _files = new List<File>();
            _folder = new Folder(path, savePath);
            _extension = !string.IsNullOrWhiteSpace(extension) ? extension : ".txt";
            _process = process;
        }

        public BatchFileProcessor(string path, string savePath, Action<File> process)
        {
            _guid = Guid.NewGuid();
            _files = new List<File>();
            _folder = new Folder(path, savePath);
            _extension = ".txt";
            _process = process;
        }

        public List<File> Files
        {
            get => _files;
        }

        public string Extension
        {
            set
            {
                _extension = !string.IsNullOrWhiteSpace(value) ? value : ".txt";
            }

            get => _extension;
        }

        public Guid Guid
        {
            get => _guid;
        }

        public Folder Folder
        {
            get => _folder;
        }

        public Action<File> Process
        {
            set
            {
                _process = value;
            }

            get => _process;
        }

        private string CreateCopySaveFolder(string directoryName) 
        {
            var folders = directoryName.Split("\\");

            if (folders.Length <= 1)
                return directoryName;

            for (int i = 2; i < folders.Length; i++)
            {
                folders[i] = $"{folders[i]}Processed";
            }

            return string.Join("\\", folders);
        }

        public void GetFiles()
        {
            try
            {
                var directoryInfo = new DirectoryInfo(_folder.Path);

                Log.Information($"{_guid} - Getting file information from all files within folder \"{_folder.Path}\"");
                _files = directoryInfo.GetFiles($"*{_extension}", SearchOption.AllDirectories)
                    .Select(file => new File()
                    {
                        Path = file.DirectoryName,
                        SavePath = CreateCopySaveFolder(file.DirectoryName),
                        Name = file.Name,
                        Size = file.Length,
                        Content = new byte[0]
                    })
                    .ToList();

                Log.Information($"{_guid} - {_files.Count} files loaded from \"{_folder.Path}\"");
            }
            catch (Exception ex)
            {
                Log.Error($"{_guid} - Something went wrong while getting files from path \"{_folder.Path}\" - Exception: {ex.Message}");
            }
        }

        public void LoadFiles()
        {
            if (_files.Count == 0)
            {
                Log.Information($"{_guid} - Not files found within folder \"{_folder.Path}\"");
                return;
            }

            Log.Information($"{_guid} - Processing {_files.Count} files from folder \"{_folder.Path}\"");
            Parallel.ForEach(_files, (file) =>
            {
                try
                {
                    string fullName = System.IO.Path.Combine(file.Path, file.Name);

                    if (string.IsNullOrWhiteSpace(fullName))
                    {
                        Log.Error($"{_guid} - File \"{fullName}\" full name is not valid");
                        return;
                    }

                    string stringContent = string.Empty;
                    using (StreamReader streamReader = new StreamReader(fullName, Encoding.UTF8))
                    {
                        stringContent = streamReader.ReadToEnd();
                    }

                    file.Content = Encoding.ASCII.GetBytes(stringContent);
                    file.IsLoaded = true;
                }
                catch (Exception ex)
                {
                    Log.Information($"{file.GUID} - Something went wrong while loading file \"{file.Name}\" - Exception: {ex.Message}");
                }
            });

            Log.Information($"{_guid} - {_files.Count(file => file.IsLoaded)} out of {_files.Count} files loaded successfully");
        }

        public void ProcessFiles()
        {
            if (_files.Count == 0)
            {
                Log.Information($"{_guid} - Not files found within folder \"{_folder.Path}\"");
                return;
            }

            Log.Information($"{_guid} - Processing {_files.Count} files from folder \"{_folder.Path}\"");
            Parallel.ForEach(_files, _process);
            Log.Information($"{_guid} - {_files.Count(file => file.IsProcessed)} out of {_files.Count} files processed successfully");
        }
    }
}
