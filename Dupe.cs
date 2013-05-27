using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SearchDuplicates
{
    internal class Dupe : IDisposable
    {
        private int _unaccessibleFiles;
        private readonly Hashtable _dupeDb = new Hashtable();
        private readonly Hashtable _hashDb = new Hashtable();
        private int _count;
        private long _fileSize;
        private bool _isDirectoryErrorSuppressionEnabled;
        private bool _isFileErrorSuppressionEnabled;
        private string _tmpFileName = "";
        private string _tmpDirName = "";

        private Stopwatch _stopwatch;
        public int DupesFound { get; private set; }
        public double TimeElapsed { get; private set; }

        public void Dispose()
        {
            GC.Collect();
        }

        private void ComputeMd5Checksum(string path, CancellationToken ct)
        {
            if (path == null) throw new ArgumentNullException("path");
            var d = new DirectoryInfo(path);
            foreach (FileInfo file in d.GetFiles())
            {
                _tmpFileName = file.FullName;
                ct.ThrowIfCancellationRequested();
                using (FileStream fs = File.OpenRead(file.FullName))
                {
                    string result = Util.GetHash(fs);
                    if (!_hashDb.Contains(result))
                        _hashDb.Add(result, file.FullName);
                    else if (!_dupeDb.Contains(file.FullName))
                    {
                        _dupeDb.Add(file.FullName, result);
                        _fileSize += file.Length;
                    }
                    fs.Close();
                }
            }
            ComputeMd5ChecksumFromDir(path, ct);
        }
        private void ComputeMd5ChecksumFromDir(string path, CancellationToken ct)
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    ct.ThrowIfCancellationRequested();
                    _tmpFileName = file;
                    _tmpDirName = dir;
                    using (FileStream fs = File.OpenRead(file))
                    {
                        string result = Util.GetHash(fs);
                        if (!_hashDb.Contains(result))
                            _hashDb.Add(result, file);
                        else if (!_dupeDb.Contains(file))
                        {
                            _dupeDb.Add(file, result);
                            var f = new FileInfo(file);
                            _fileSize += f.Length;
                        }
                        fs.Close();
                    }
                }
                ComputeMd5ChecksumFromDir(dir, ct);
            }
        }
        private void HandleMd5(string path, CancellationToken ct)
        {
            try
            {
                ComputeMd5Checksum(path, ct);
            }
            catch (UnauthorizedAccessException)
            {
                DialogResult dialogResult;
                if (!_isDirectoryErrorSuppressionEnabled)
                {
                    dialogResult = MessageBox.Show("У запускающего пользователя не хватает "
                                                   + "прав для обращения к объекту " + _tmpDirName +
                                                   ". Продолжить поиск с повышенными привилегиями?\r\n(При нажатии на 'Да' это окно больше не появится.)",
                                                   "Нехватка прав",
                                                   MessageBoxButtons.YesNoCancel);
                }
                else
                {
                    dialogResult = new DialogResult();
                }
                if (dialogResult == DialogResult.Yes)
                {
                    _isDirectoryErrorSuppressionEnabled = true;
                    Util.SetAccessRights(_tmpDirName);
                }
                else if (dialogResult == DialogResult.No)
                {
                    Util.SetAccessRights(_tmpDirName);
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    throw new ApplicationException("Операция прервана пользователем");
                }
            }
            catch (IOException)
            {
                ++_unaccessibleFiles;
                DialogResult dialogResult;
                if (!_isFileErrorSuppressionEnabled)
                {
                    dialogResult =
                        MessageBox.Show(
                            "Ошибка доступа к объекту " + _tmpFileName +
                            ". Продолжить поиск?\r\n(При нажатии на 'Да' это окно больше не появится.)",
                            "Нет доступа",
                            MessageBoxButtons.YesNoCancel);
                }
                else
                {
                    dialogResult = new DialogResult();
                }

                if (dialogResult == DialogResult.Yes)
                {
                    _isFileErrorSuppressionEnabled = true;
                }
                else if (dialogResult == DialogResult.No)
                {
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    throw new ApplicationException("Операция прервана пользователем");
                }
            }
        }

        public StringBuilder Search(string path, CancellationToken ct)
        {
            _stopwatch = Stopwatch.StartNew();
            //ComputeMd5Checksum(path, ct);
            try
            {
                HandleMd5(path, ct);
            }
            catch (ApplicationException)
            {
                return null;
            }
            DupesFound = _dupeDb.Keys.Count;
            StringBuilder builder = PrintResult();
            TimeElapsed = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Stop();
            return builder;
        }

        private static DictionaryEntry SearchEntry(
            IEnumerable table, object searchKey = null, object searchValue = null)
        {
            var entry = new DictionaryEntry();
            foreach (DictionaryEntry d in from DictionaryEntry d in table
                                          where (d.Key.ToString() == (string)searchKey) ||
                                                (d.Value.ToString() == (string)searchValue)
                                          select d)
            {
                entry = d;
            }
            return entry;
        }

        private StringBuilder PrintResult()
        {
            var builder = new StringBuilder("");
            double size = Util.ConvertBytesToMegabytes(_fileSize);
            builder.AppendFormat("Уникальных файлов: {0}\r\n", _hashDb.Keys.Count);
            builder.AppendFormat("\r\nДубликатов: {0}\r\n", _dupeDb.Keys.Count);
            if (size < 1)
                builder.AppendFormat("\r\nОбщий размер дубликатов: меньше 1МБ\r\n");
            else
                builder.AppendFormat("\r\nОбщий размер дубликатов: {0}МБ\r\n",
                                     size.ToString("0.00"));
            if (_unaccessibleFiles > 0)
                builder.AppendFormat("\r\nПоиск не затронул: {0} файлов.\r\nНа момент поиска они были недоступны.\r\n",
                                     _unaccessibleFiles);
            return builder;
        }

        public int DeleteEntries()
        {
            if (_dupeDb != null)
            {
                foreach (DictionaryEntry entry in _dupeDb)
                {
                    var file = new FileInfo(entry.Key.ToString());
                    if (!file.Exists) continue;
                    try
                    {
                        file.Delete();
                        _count++;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(
                            "Невозможно удалить дубликат. Вероятно в данный момент он используется системой.",
                            "Ошибка удаления", MessageBoxButtons.OK,
                            MessageBoxIcon.Stop);
                    }
                }
            }
            return _count;
        }

        public void SaveLog(string path)
        {
            using (FileStream stream = File.Create(path))
            {
                var wr = new StreamWriter(stream, Encoding.Default);
                if (_dupeDb.Keys.Count > 0)
                    foreach (string logEntry in from DictionaryEntry file in _dupeDb
                                                let d = SearchEntry(_hashDb, file.Value)
                                                select
                                                    string.Format("Дубликат -> [{0}]\tОригинал -> [{1}]", file.Key,
                                                                  d.Value))
                    {
                        wr.WriteLine(logEntry);

#pragma warning disable 168
                        foreach (char c in logEntry)
#pragma warning restore 168
                            wr.Write("-");
                        wr.WriteLine(Environment.NewLine);
                    }
                else
                    wr.WriteLine("Дубликаты не найдены");
                wr.WriteLine(string.Format
                                 ("Время обработки запроса: {0} секунд(ы)", TimeElapsed.ToString
                                                                                ("F", CultureInfo.InvariantCulture)));
                if (_count > 0)
                {
                    wr.WriteLine();
                    wr.WriteLine(string.Format("Удалено {0} дубликатов.", _count));
                }
                wr.Close();
                stream.Close();
            }
        }
    }
}