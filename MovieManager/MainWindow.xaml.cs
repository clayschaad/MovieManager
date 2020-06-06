using MovieCleaner.MovieDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace MovieCleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentFile;
        List<MovieFile> movieFiles;

        private const string MovieXml = @"c:\temp\movies.xml";
        private const string MovieLog = @"c:\temp\movies.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbSourcePath.Text = @"\\Gibbs\Multimedia\Movies";
            tbDestinationPath.Text = @"\\Gibbs\Multimedia\Movies_Clean";
            tbTitle.Text = "";

            if (File.Exists(MovieXml))
            {
                movieFiles = DeserializeMovies(MovieXml);
            }
            else
            {
                movieFiles = new List<MovieFile>();
                AppendFiles(tbSourcePath.Text, movieFiles);
            }

            currentFile = 0;
            ShowMovie();
        }

        private void AppendFiles(string sourceDirectory, List<MovieFile> movieFiles)
        {
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
                if (movieSearchResult.OriginalLanguage == "en" && (movieFiles[currentFile].AudioLanguages.Contains("eng") || movieFiles[currentFile].AudioLanguages.Count == 0))
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
            SaveCurrent();
            currentFile--;
            ShowMovie();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrent();
            currentFile++;
            ShowMovie(); 
        }

        private void SaveCurrent()
        {
            if (currentFile > -1)
            {
                movieFiles[currentFile].SetTitle(tbTitle.Text);
                movieFiles[currentFile].DoInclude(cbInclude.IsChecked.Value);

                SerializeMovies(MovieXml);
            }
        }

        private void ShowMovie()
        {
            btnPrev.IsEnabled = currentFile > 0;
            btnNext.IsEnabled = currentFile < movieFiles.Count - 1;

            if (!string.IsNullOrEmpty(movieFiles[currentFile].Title))
            {
                tbTitle.Text = movieFiles[currentFile].Title;
            }

            tbFilename.Text = movieFiles[currentFile].Filename;
            cbInclude.IsChecked = movieFiles[currentFile].Include;
            tbInfo.Text = $"{currentFile + 1} / {movieFiles.Count}" + Environment.NewLine + movieFiles[currentFile].GetInfo();
            tbTitles.ItemsSource = movieFiles[currentFile].MovieSearchResults;
            tbLanguages.Text ="Audio Languages: " + string.Join(", ", movieFiles[currentFile].AudioLanguages);
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrent();

            if (MessageBox.Show("Move all files to target folder?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var filesToMove = movieFiles.Where(m => m.Include).ToList();
                File.WriteAllLines(MovieLog, new string[] { $"Move {filesToMove.Count} movie files" });

                foreach (var movie in filesToMove)
                {
                    var name = GetValidName(movie.Title);
                    var destinationFile = tbDestinationPath.Text + "\\" + name + Path.GetExtension(movie.Filename);

                    if (!File.Exists(movie.FullPathToFile))
                    {
                        File.AppendAllLines(MovieLog, new string[] { $"File {movie.FullPathToFile} already moved. skipping." });
                        continue;
                    }

                    if (File.Exists(destinationFile))
                    {
                        File.AppendAllLines(MovieLog, new string[] { $"File {destinationFile} already exists. skipping." });
                        continue;
                    }
                    
                    File.AppendAllLines(MovieLog, new string[] { $"Move {movie.FullPathToFile} to {destinationFile}" });
                    File.Move(movie.FullPathToFile, destinationFile, false);
                }

                MessageBox.Show($"{filesToMove.Count} file moved.");

                foreach (var movie in movieFiles.Where(m => !m.Include))
                {
                    File.AppendAllLines(MovieLog, new string[] { $"File {movie.FullPathToFile} ignored" });
                }
            }
        }

        private string GetValidName(string originalName)
        {
            var invalids = Path.GetInvalidFileNameChars();
            var newName = String.Join(" ", originalName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            return newName;
        }

        private void SerializeMovies(string filename)
        {
            var ser = new XmlSerializer(typeof(List<MovieFile>));
            var writer = new StreamWriter(filename);
            ser.Serialize(writer, movieFiles);
            writer.Close();
        }

        private List<MovieFile> DeserializeMovies(string filename)
        {
            var ser = new XmlSerializer(typeof(List<MovieFile>));
            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                return (List<MovieFile>)ser.Deserialize(reader);
            }
        }
    }
}
