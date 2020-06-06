using MovieCleaner.MovieDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace MovieCleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentFile;
        List<MovieFile> movieFiles;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbSourcePath.Text = @"\\Gibbs\Multimedia\Movies";
            tbDestinationPath.Text = @"\\Gibbs\Multimedia\Movies_Clean";
            tbTitle.Text = "";

            movieFiles = new List<MovieFile>();
            AppendFiles(tbSourcePath.Text, movieFiles);

            currentFile = 0;
            ShowMovie(currentFile);
        }

        private void AppendFiles(string sourceDirectory, List<MovieFile> movieFiles)
        {
            if (movieFiles.Count >= 5)
            {
                return;
            }

            var files = Directory.GetFiles(sourceDirectory);
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file).Trim('.');
                if (MovieFile.AllowedExtensions.Contains(extension))
                {
                    movieFiles.Add(new MovieFile(file));
                }
            }

            var subDirs = Directory.GetDirectories(sourceDirectory);
            foreach (var subDir in subDirs)
            {
                var folderName = Path.GetFileName(subDir).ToLower();
                if (MovieFile.IgnoreFolders.Contains(folderName))
                {
                    continue;
                }

                AppendFiles(subDir, movieFiles);
            }
        }

        private void tbTitles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var movieSearchResult = (MovieSearchResult)((System.Windows.Controls.Primitives.Selector)e.Source).SelectedItem;
            if (movieSearchResult != null)
            {
                if (movieSearchResult.OriginalLanguage == "en" && movieFiles[currentFile].AudioLanguages.Contains("eng"))
                {
                    tbTitle.Text = movieSearchResult.OriginalTitle;
                }
                else
                {
                    tbTitle.Text = movieSearchResult.Title;
                }
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrent(currentFile);
            currentFile--;
            ShowMovie(currentFile);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrent(currentFile);
            currentFile++;
            ShowMovie(currentFile); 
        }

        private void SaveCurrent(int index)
        {
            if (index > -1)
            {
                movieFiles[index].SetTitle(tbTitle.Text);
                movieFiles[index].DoInclude(cbInclude.IsChecked.Value);
            }
        }

        private void ShowMovie(int index)
        {
            btnPrev.IsEnabled = currentFile > 0;
            btnNext.IsEnabled = currentFile < movieFiles.Count - 1;

            if (!string.IsNullOrEmpty(movieFiles[index].Title))
            {
                tbTitle.Text = movieFiles[index].Title;
            }

            tbFilename.Text = movieFiles[index].Filename;
            cbInclude.IsChecked = movieFiles[index].Include;
            tbInfo.Text = $"{index + 1} / {movieFiles.Count}" + Environment.NewLine + movieFiles[index].GetInfo();
            tbTitles.ItemsSource = movieFiles[index].MovieSearchResults;
            tbLanguages.Text ="Audio Languages: " + string.Join(", ", movieFiles[index].AudioLanguages);

            //if (cbInclude.IsChecked == true)
            //{
            //    var destinationFile = tbDestinationPath.Text + "\\" + tbTitle.Text + "\\" + tbTitle.Text + Path.GetExtension(movieFiles[currentFile].Filename);
            //    File.AppendAllLines(@"c:\temp\movies.txt", new string[] { destinationFile });
            //}
        }
    }
}
