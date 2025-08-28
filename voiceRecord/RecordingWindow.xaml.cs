using System.Windows;
using NAudio.Wave;
using System.Windows.Threading;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
namespace voiceRecord
{
    /// <summary>
    /// Interaction logic for RecordingWindow.xaml
    /// </summary>
    public partial class RecordingWindow : Window
    {
        private WaveInEvent waveIn;
        private WaveFileWriter writer;

        // Timers
        private DispatcherTimer stopwatchTimer;

        // Time variables
        private TimeSpan stopwatchTime;


        // State variables
        private bool isStopwatchRunning = false;

        string[] lines;
        int lineIndx = 0;
        string filePath,pa;
        int rbValue = 48000;
        AvinCsvClass ob = new AvinCsvClass();
        
        public RecordingWindow(string username,string audioPath,string pas)
        {
            pa = pas;
            InitializeComponent();
            InitializeTimers();
            stopBtn.IsEnabled = isStopwatchRunning;
            userLbl.Content=username;
            
            string filePath1 = AppDomain.CurrentDomain.BaseDirectory;
             filePath1 = Path.Combine(filePath1.Remove(filePath1.Length - 25), "20000_sent_2025_08_11.txt");
            try
            {
                filePath = @"AvinData\" + username + ".csv";
                // Basic file existence check
                if (File.Exists(filePath))
                {
                    string[] userlines = File.ReadAllLines(filePath);
                    lineIndx = Convert.ToInt32(userlines[0]);
                    audioPath= userlines[1];
                }
                else
                {
                    var singleRow = new string[] { lineIndx.ToString() };
                    //AvinCsvClass ob= new AvinCsvClass();
                    ob.AppendToCsvAvin(filePath, singleRow);
                     singleRow = new string[] {  audioPath };
                    ob.AppendToCsvAvin(filePath, singleRow);
                }

                txtFilePath.Text = audioPath;
                // Read all lines into an array
                lines = File.ReadAllLines(filePath1);
                txtTblok.Text = $"({lineIndx + 1})\n"+ lines[lineIndx] ;
               
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error reading file: {ex.Message}");
            }
        }

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
            short max = 0;
            for(int i = 0; i < e.BytesRecorded; i += 2)
            {
                short sample=BitConverter.ToInt16(e.Buffer, i);
                if (sample < 0) sample = (short)-sample;
                if (sample > max) max = sample;
            }
            
        }

        private void WaveIn_RcordingStopped(object? sender, StoppedEventArgs e)
        {
            waveIn.Dispose();
            writer.Close();
            writer = null;
            btnRec.Content = "Record";
           
        }

        private void InitializeTimers()
        {
            stopwatchTimer = new DispatcherTimer();
            stopwatchTimer.Interval = TimeSpan.FromSeconds(1);
            stopwatchTimer.Tick += StopwatchTimer_Tick;
        }

        private void UpdateStopwatchUI()
        {
            btnRec.IsEnabled = !isStopwatchRunning;
            stopBtn.IsEnabled = isStopwatchRunning;
            prevBtn.IsEnabled = !isStopwatchRunning;
            nextBtn.IsEnabled = !isStopwatchRunning;
        }

        private void UpdateStopwatchDisplay()
        {
            txtStopwatch.Text = stopwatchTime.ToString(@"hh\:mm\:ss");
        }
        private void StopwatchTimer_Tick(object? sender, EventArgs e)
        {
            stopwatchTime = stopwatchTime.Add(TimeSpan.FromSeconds(1));
            UpdateStopwatchDisplay();
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            waveIn?.StopRecording(); btnRec.Content = "Record";

            stopwatchTimer.Stop();
            isStopwatchRunning = false;
            UpdateStopwatchDisplay();
            UpdateStopwatchUI();

            
            ob.ModifyUserFileCsv(filePath, (lineIndx+1).ToString(), txtFilePath.Text);
            var singleRow = new string[] { };
            string pa = $@"AvinData\KurdisData_{userLbl.Content}_{rbValue}.csv";
            if (!File.Exists(pa))
            {
                 singleRow = new string[] { "Sentences_id", "Sentence","Audio_file" };
                ob.AppendToCsvAvin(pa, singleRow);
            }
            singleRow = new string[] { (lineIndx+1).ToString(), lines[lineIndx],$"{userLbl.Content}_{rbValue}_{lineIndx + 1}.wav" };
            ob.AppendToCsvAvin(pa, singleRow);
            
        }

        private void btnRec_Click(object sender, RoutedEventArgs e)
        {
            
            if (rb1.IsChecked == true) rbValue = 24000;
            else rbValue = 48000;
            if (!File.Exists($@"{txtFilePath.Text}\{userLbl.Content}_{rbValue}_{lineIndx + 1}.wav"))
            {
                if (!isStopwatchRunning)
                {
                    stopwatchTime = TimeSpan.Zero;
                    stopwatchTimer.Start();
                    isStopwatchRunning = true;
                    UpdateStopwatchUI();
                }
                btnRec.Content = "Recording";

                waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(rbValue, 1)
                };
                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.RecordingStopped += WaveIn_RcordingStopped;
                writer = new WaveFileWriter($@"{txtFilePath.Text}\{userLbl.Content}_{rbValue}_{lineIndx + 1}.wav", waveIn.WaveFormat);
                waveIn.StartRecording();
            }
            else System.Windows.MessageBox.Show("You already recorded this one before.");
        }

        private void prevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (lineIndx > 0) {lineIndx--;
            txtTblok.Text = $"({lineIndx+1})\n" + lines[lineIndx];
                txtStopwatch.Text = "00:00:00";
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (lineIndx <lines.Length-1)
            {
                lineIndx++;
                txtTblok.Text = $"({lineIndx+1})\n" + lines[lineIndx];
                txtStopwatch.Text = "00:00:00";
            }
        }

        private void brouzBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder";
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                folderDialog.ShowNewFolderButton = true;

                DialogResult result = folderDialog.ShowDialog();

                if (result.ToString() == "OK" && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    txtFilePath.Text = folderDialog.SelectedPath;
                    btnRec.IsEnabled = true;
                }
            }
         
        }

        private void LogoutBt_Click(object sender, RoutedEventArgs e)
        {
            ob.ModifyCsvValueWhenLogIn(@"AvinData\user.csv", "0", userLbl.Content.ToString(), pa);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
