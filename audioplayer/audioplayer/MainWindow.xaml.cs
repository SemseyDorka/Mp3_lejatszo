using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using System.IO;



namespace audioplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        DispatcherTimer timer = new DispatcherTimer();
        List<Songs> items = new List<Songs>();
        public MainWindow()
        {
            InitializeComponent();
            listbox.ItemsSource = paths;


        }
        string[] paths, files;
        int rows=0;
        private void file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
           

            if (openFileDialog.ShowDialog() == true)
            {
                files = openFileDialog.SafeFileNames;
                paths = openFileDialog.FileNames;
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    listbox.Items.Add(files[i]);
                    mediaPlayer.Open(new Uri(openFileDialog.FileName));
                    mediaPlayer.Play();
                }

            }

           
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
       
        void timer_Tick(object sender, EventArgs e)
        {
            
            if ((mediaPlayer.Source != null) && (mediaPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                Status.Content=String.Format("{0}/{1}",mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                Slider.Minimum = 0;
                Slider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                Slider.Value = mediaPlayer.Position.TotalSeconds;
            }
            else
            {
                Status.Content = "0:00/0:00";
            }
        }
        private void mediaplayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                Slider.Minimum = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                timer.Interval = TimeSpan.FromMilliseconds(200);
                timer.Tick += timer_Tick;
                timer.Start();
               
            }
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                mediaPlayer.Open(new Uri(path));
                mediaPlayer.Play();
            }

        }

        private void _mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
           
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Szöveges állományok (*.txt)|*.txt|Minden állomány (*.*)|*.*";
            save.ShowDialog();
            StreamWriter sw = new StreamWriter(save.FileName);
            for (int i = 0; i < paths.Length; i++)
            {
                sw.WriteLine(files[i] + "@" + paths[i]);
            }
            sw.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            try
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Szöveges állományok (*.txt)|*.txt|Minden állomány (*.*)|*.*";
                open.ShowDialog();
                StreamReader sr = new StreamReader(open.FileName);
                while (!sr.EndOfStream)
                {
                    sr.ReadLine();
                    rows++;
                }
                sr.Close();
                StreamReader read = new StreamReader(open.FileName);
                files = new string[rows];
                paths = new string[rows];
                while (!read.EndOfStream)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        string s = read.ReadLine();
                        string[] resz = s.Split('@');
                        files[i] = resz[0];
                        paths[i] = resz[1];
                        listbox.Items.Add(files[i]);
                    }
                }
                read.Close();


            }
            catch (Exception)
            {

                MessageBox.Show("Nem megfelelő  a formátum vagy az elérési út!");
            }
        }


        private void Play_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
           

        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
           
        }

        private void listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mediaPlayer.Stop();
            string path = paths[listbox.SelectedIndex];
            mediaPlayer.Open(new Uri(path));
            mediaPlayer.Play();

        }
        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }
        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(Slider.Value);
        }


        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (listbox.Items.Count>listbox.SelectedIndex)
            {
                mediaPlayer.Stop();
                listbox.SelectedIndex = listbox.SelectedIndex + 1;
                string path = paths[listbox.SelectedIndex];
                mediaPlayer.Open(new Uri(path));
                mediaPlayer.Play();
            }
            else
            {
                mediaPlayer.Stop();
                string path = paths[-1];
                mediaPlayer.Open(new Uri(path));
                mediaPlayer.Play();
            }

        }

        private void volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = e.NewValue;
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (listbox.SelectedIndex!=-1)
            {
                mediaPlayer.Stop();
                listbox.SelectedIndex = listbox.SelectedIndex - 1;
                string path = paths[listbox.SelectedIndex];
                mediaPlayer.Open(new Uri(path));
                mediaPlayer.Play();
            }

        }
    }
}
