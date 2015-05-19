using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using BS_PokedexManager;
using DAL_JSON;

namespace PL_WPF
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow
    {
        private Business.Generation generation;
        private ListClass _list;

        public LoadingWindow()
        {
            InitializeComponent();

            if (!File.Exists(Path.Combine(System.Windows.Forms.Application.LocalUserAppDataPath, "PokemonCries","001.wav")))
            {
                DirectoryCopy("PokemonCries", Path.Combine(System.Windows.Forms.Application.LocalUserAppDataPath, "PokemonCries"), true);
            }

            _list = BS_PokedexManager.Business.CheckSetting();
            if (_list != null)
            {
                CloseWindow();
            }

        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            var v = new MainWindow(_list);
            v.Show();
            this.Close();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _list = BS_PokedexManager.Business.GeneratePokeList(generation, sender as BackgroundWorker);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
            DescriptionTextBlock.DataContext = BS_PokedexManager.Business.DescriptionProgress;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            //Need to close MainWindow!!!

            RadioButton rb = (RadioButton)sender;
            generation = (Business.Generation)Enum.Parse(typeof(Business.Generation), rb.Content.ToString());

            RadioButtonStackPanel.IsEnabled = false;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        //"Lend" this nice method from MSDN!!!
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

    }
}
