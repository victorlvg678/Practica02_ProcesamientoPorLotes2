using Serilog;
using System.Collections.Concurrent;
using System.Text;

namespace Practica02_ProcesamientoPorLotes2.Classes
{
    public class FileDataTransformer
    {
        public static File Process(File file)
        {
            if (file == null)
            {
                Log.Error($"{Guid.NewGuid()} - Error while transforming file data - Exception: File is null");
                return new File();
            }

            if (!file.IsLoaded)
            {
                Log.Error($"{Guid.NewGuid()} - Error while transforming file data - Exception: File content must be loaded");
                return file;
            }

            try
            {
                Log.Information($"{file.GUID} - Processing file \"{file.Name}\"");
                file.Content = Transform(file.Content);
                file.IsProcessed = true;
                Log.Information($"{file.GUID} - File \"{file.Name}\" processed successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"{file.GUID} - Something went wrong while processing file \"{file.Name}\" - Exception: {ex.Message}");
            }

            return file;
        }

        private static byte[] Transform(byte[] content)
        {
            string stringContent = content == null ? "Not data" : Encoding.ASCII.GetString(content);
            stringContent = TransformContent(stringContent);
            return Encoding.ASCII.GetBytes(stringContent);
        }


        private static string TransformContent(string content)
        {
            var lines = content.Split("\n");

            var resultLines = new ConcurrentBag<string>();

            Parallel.ForEach(lines, (line) =>
            {
                if(!string.IsNullOrWhiteSpace(line))
                    resultLines.Add(TransformLine(line));
            });

            return string.Join("\n", resultLines);
        }

        private static string TransformLine(string line)
        {
            var asciiLine = Encoding.ASCII.GetBytes(line);

            for(int i = 0; i < asciiLine.Length; i++)
            {
                if (IsASCIICharALetter(asciiLine[i]))
                {
                    asciiLine[i] = GetRandomDigit();
                    continue;
                }

                if (IsASCIICharANumber(asciiLine[i]))
                    asciiLine[i] = GetRandomCapitalLetter();
            }

            return Encoding.ASCII.GetString(asciiLine);
        }

        private static bool IsASCIICharALetter(byte character)
        {
            return (character >= 65 && character <= 90) || (character >= 97 && character <= 122);
        }

        private static byte GetRandomNumber(int lowerbound, int upperbound) 
        {
            var random = new Random();

            return Convert.ToByte(random.Next(lowerbound, upperbound));
        }

        private static byte GetRandomDigit()
        {
            return GetRandomNumber(48, 57);
        }

        private static bool IsASCIICharANumber(byte character)
        {
            return character >= 48 && character <= 57;
        }

        private static byte GetRandomCapitalLetter() 
        {
            return GetRandomNumber(65, 90);
        }

    }
}
