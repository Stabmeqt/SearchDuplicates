using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using WinForms = System.Windows.Forms;
using System.Globalization;

namespace SearchDuplicates
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        StringBuilder _builder;
        CancellationTokenSource _tokenSource = new CancellationTokenSource();
        Dupe _dupe = new Dupe();
        bool _operationPendingFlag;

        readonly WinForms.FolderBrowserDialog _folderBrowserDialog1 =
            new WinForms.FolderBrowserDialog();

        readonly WinForms.SaveFileDialog _saveFileDialog1 =
            new WinForms.SaveFileDialog();
        public MainWindow()
        {
            InitializeComponent();
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            imgLoading.Visibility = Visibility.Hidden;
        }

        private StringBuilder SearchDupes(string path)
        {
            _builder = _dupe.Search(path, _tokenSource.Token);
            _operationPendingFlag = true;
            return _builder;
        }

        private void BtnSearchClick(object sender, RoutedEventArgs e)
        {
            var tasks = new List<Task>();
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            _tokenSource = new CancellationTokenSource();
            _dupe = new Dupe();
            _folderBrowserDialog1.ShowNewFolderButton = false;
            _folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            var v = _folderBrowserDialog1.ShowDialog();
            if ((v == WinForms.DialogResult.Cancel) || (v == WinForms.DialogResult.Abort)
                || (v == WinForms.DialogResult.No))
                imgLoading.Visibility = Visibility.Hidden;
            else
            {
                txtResult.Text = "";
                imgLoading.Visibility = Visibility.Visible;
                Stopwatch.StartNew();
                var task = Task.Factory.StartNew(() =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        txtResult.Text = "Запрос обрабатывается...\r\n";
                    }));
                    return SearchDupes(_folderBrowserDialog1.SelectedPath);
                }, _tokenSource.Token);

                tasks.Add(task);


                task.ContinueWith(resultTask =>
                                      {
                                          txtResult.Text = task.Result + Environment.NewLine;
                                      },
                                  CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, ui);

                task.ContinueWith(resultTask =>
                                  txtResult.Text = "Операция отменена" +
                                                   Environment.NewLine,
                                  CancellationToken.None,
                                  TaskContinuationOptions.OnlyOnCanceled, ui);

                btnSearch.IsEnabled = false;
                btnDelete.IsEnabled = false;
                Task.Factory.ContinueWhenAll(tasks.ToArray(), result =>
                {
                    btnSearch.IsEnabled = true;
                    if (_dupe.DupesFound > 0)
                        btnDelete.IsEnabled = true;
                    imgLoading.Visibility = Visibility.Hidden;
                    _operationPendingFlag = false;
                    var elapsed = _dupe.TimeElapsed.ToString("F", CultureInfo.InvariantCulture);

                    txtResult.Text += string.Format
                        ("Время обработки запроса: {0} секунд(ы) {1}\r\n", elapsed, Environment.NewLine);
                    GC.Collect();
                }, CancellationToken.None, TaskContinuationOptions.None, ui);
            }
        }
        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            _tokenSource.Cancel(true);
            if (_operationPendingFlag)
                txtResult.Text = "Отмена" + Environment.NewLine;
        }

        private void BtnSaveLogClick(object sender, RoutedEventArgs e)
        {
            _saveFileDialog1.Filter = Properties.Resources.MainWindow_BtnSaveLogClick_Текстовые_файлы_____txt;
            _saveFileDialog1.DefaultExt = "txt";
            var result = _saveFileDialog1.ShowDialog();
            if ((result == WinForms.DialogResult.OK) || (result == WinForms.DialogResult.Yes))
                try
                {
                    _dupe.SaveLog(_saveFileDialog1.FileName);
                    txtResult.Text += "Лог успешно сохранен\r\n";
                }
                catch (Exception ex)
                {
                    txtResult.Text += "При сохранении произошли следующие ошибки:" + Environment.NewLine;
                    txtResult.Text += ex.Message;
                }
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (!txtResult.Text.Contains("Время обработки запроса")) return;
            WinForms.MessageBox.Show("Вы действительно хотите удалить все найденные дубликаты? Эта операция не может быть отменена!",
                                     "Удаление дубликатов", WinForms.MessageBoxButtons.YesNo, WinForms.MessageBoxIcon.Warning);
            var deletedEntries = _dupe.DeleteEntries();
            txtResult.Text += string.Format("Удалено {0} дубликатов.\r\n", deletedEntries);
        }
    }
}

