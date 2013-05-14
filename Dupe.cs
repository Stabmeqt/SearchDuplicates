using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;

namespace SearchDuplicates
{
    class Dupe : IDisposable
    {
        Hashtable hashDB = new Hashtable();
        Hashtable dupeDB = new Hashtable();
        private TimeSpan ts;
        private double watch;
        private long fileSize;
        public double TimeElapsed
        {
            get { return watch; }
        }
        private System.Diagnostics.Stopwatch stopwatch;
        private void ComputeMD5Checksum(string path, System.Threading.CancellationToken ct)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            foreach (FileInfo file in d.GetFiles())
            {
                ct.ThrowIfCancellationRequested();
                using (FileStream fs = File.OpenRead(file.FullName))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, (int)fs.Length);
                    byte[] checkSum = md5.ComputeHash(fileData);
                    string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                    if (!hashDB.Contains(result))
                        hashDB.Add(result, file.FullName);
                    else
                        if (!dupeDB.Contains(file.FullName))
                        {
                            dupeDB.Add(file.FullName, result);
                            fileSize += file.Length;
                        }
                    fs.Close();
                }
            }
            ComputeMD5ChecksumFromDir(path, ct);
        }
        private void ComputeMD5ChecksumFromDir(string path, System.Threading.CancellationToken ct)
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    ct.ThrowIfCancellationRequested();
                    using (FileStream fs = File.OpenRead(file))
                    {
                        MD5 md5 = new MD5CryptoServiceProvider();
                        byte[] fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, (int)fs.Length);
                        byte[] checkSum = md5.ComputeHash(fileData);
                        string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                        if (!hashDB.Contains(result))
                            hashDB.Add(result, file);
                        else

                            if (!dupeDB.Contains(file))
                            {
                                dupeDB.Add(file, result);
                                FileInfo f = new FileInfo(file);
                                fileSize += f.Length;
                            }
                        fs.Close();
                    }
                }
                ComputeMD5ChecksumFromDir(dir, ct);
            }
        }
        public StringBuilder Search(string path, System.Threading.CancellationToken ct)
        {
            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            ComputeMD5Checksum(path, ct);
            StringBuilder builder = PrintResult();
            ts = stopwatch.Elapsed;
            watch = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Stop();
            return builder;
        }
        private DictionaryEntry SearchEntry(
            Hashtable table, object searchKey = null, object searchValue = null)
        {
            DictionaryEntry entry = new DictionaryEntry();
            foreach (DictionaryEntry d in table)
            {
                if ((d.Key.ToString() == (string)searchKey) ||
                    (d.Value.ToString() == (string)searchValue))
                    entry = d;
            }
            return entry;
        }

        private StringBuilder PrintResult()
        {
            StringBuilder builder = new StringBuilder("");
            double size = ConvertBytesToMegabytes(fileSize);
            builder.AppendFormat("Уникальных файлов: \r\n{0}\r\n", hashDB.Keys.Count);
            builder.AppendFormat("Дубликатов: \r\n{0}\r\n", dupeDB.Keys.Count);
            if (size < 1)
                builder.AppendFormat("Общий размер дубликатов: \r\nменьше 1МБ\r\n");
            else
                builder.AppendFormat("Общий размер дубликатов: \r\n{0}МБ\r\n",
                    size.ToString("0.00"));
            return builder;
        }
        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
        private void Test()
        {

        }
        public void SaveLog(string path)
        {
            string logEntry;
            using (FileStream stream = File.Create(path))
            {
                StreamWriter wr = new StreamWriter(stream, Encoding.Default);
                if (dupeDB.Keys.Count > 0)
                    foreach (DictionaryEntry file in dupeDB)
                    {
                        DictionaryEntry d = SearchEntry(hashDB, file.Value, null);
                        logEntry = string.Format("Дубликат -> [{0}]\tОригинал -> [{1}]", file.Key, d.Value);
                        wr.WriteLine(logEntry);
                        foreach (var c in logEntry)
                        {
                            wr.Write("-");
                        }
                        wr.WriteLine(Environment.NewLine);
                    }
                else
                    wr.WriteLine("Дубликаты не найдены");
                wr.WriteLine(string.Format
                    ("Время обработки запроса: {0} секунд(ы)", watch.ToString
                    ("F", CultureInfo.InvariantCulture)));
                wr.Close();
                stream.Close();
            }
        }

        #region Члены IDisposable

        public void Dispose()
        {
            GC.Collect();
        }
        #endregion
    }
}
