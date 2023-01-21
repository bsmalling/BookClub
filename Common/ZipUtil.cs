using System;
using System.IO.Compression;
using System.Text;

namespace BookClub.Common
{

    /// <summary>
    /// Zip and Unzip in memory using System.IO.Compression.
    /// </summary>
    /// <remarks>
    /// Include System.IO.Compression in your project.
    /// </remarks>
    public static class ZipUtil
    {

        /// <summary>
        /// Zips a string into a zipped byte array.
        /// </summary>
        /// <param name="text">The text to be zipped.</param>
        /// <returns>byte[] representing a zipped stream</returns>
        public static byte[] Zip(string text)
        {
            using (var memoryStream = new MemoryStream())
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var demoFile = zipArchive.CreateEntry("zipped.txt");
                using (var entryStream = demoFile.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(text);
                }
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Unzips a zipped byte array into a string.
        /// </summary>
        /// <param name="buffer">The byte array to be unzipped</param>
        /// <returns>string representing the original stream</returns>
        public static string? Unzip(byte[] buffer)
        {
            using (var zippedStream = new MemoryStream(buffer))
            using (var archive = new ZipArchive(zippedStream))
            {
                var entry = archive.Entries.FirstOrDefault();
                if (entry != null)
                {
                    using (var unzippedEntryStream = entry.Open())
                    using (var ms = new MemoryStream())
                    {
                        unzippedEntryStream.CopyTo(ms);
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                return null;
            }
        }

    }

}
