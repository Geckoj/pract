namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _directoryPath;

        public DirectorySizeCommand(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            _directoryPath = directoryPath;
        }

        public void Execute()
        {
            long totalSize = 0;
            var files = Directory.GetFiles(_directoryPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                totalSize += fileInfo.Length;
            }

            Console.WriteLine($"Directory size: {totalSize} bytes");
        }
    }
    public class FindFilesCommand : ICommand
    {
        private readonly string _directoryPath;
        private readonly string _searchPattern;

        public FindFilesCommand(string directoryPath, string searchPattern)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            _directoryPath = directoryPath;
            _searchPattern = searchPattern;
        }

        public void Execute()
        {
            var files = Directory.GetFiles(_directoryPath, _searchPattern, SearchOption.TopDirectoryOnly);

            Console.WriteLine($"Found {files.Length} files:");
            foreach (var file in files)
            {
                Console.WriteLine(Path.GetFileName(file));
            }
        }
    }
}