
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Helper
{
    public class FileSystemHelpers
    {
        private static Logger log = Logger.GetLoggerInstance();
        public static void DeleteAllFilesInFolder(string folderName)
        {
            var dir = new DirectoryInfo(folderName);
            foreach (var file in dir.EnumerateFiles()) {
                file.Delete();
            }
            log.Debug($"Deleting all files in folder '{folderName}'");
        }

        public static void ExtractZipFile(string zipFileName, string folderName)
        {
            try 
            {
                bool overwriteFiles = true;
                ZipFile.ExtractToDirectory(zipFileName, folderName, overwriteFiles);
            }
            catch(Exception ex)
            {
                log.Error($"Error while unziping file {zipFileName}. {ex.Message}");
                throw new Exception("Error while extracting Zip file");
            }
        }
    }
    public class FileDownloader
    {
        private static Logger log = Logger.GetLoggerInstance();
        public async Task DownloadAsFileAsync(string url, string filename, List<KeyValuePair<string, string>> httpHeaders)
        {
            Stopwatch sw = new Stopwatch();
            log.Debug($"Downloading(async) <{url}> => <{filename}>");
            sw.Start();
            using (HttpClient client = new HttpClient())
            {
                foreach (var item in httpHeaders)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
                // Set the HTTP headers
                using (HttpResponseMessage response = await client.GetAsync(url))                           
                using (HttpContent content = response.Content)
                {
                    // Read the data
                    var result = await content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(filename, result);
                }
            }
            sw.Stop();
            log.Debug($"{url} (async) took {sw.Elapsed} seconds");
            return;
        }
    }
}