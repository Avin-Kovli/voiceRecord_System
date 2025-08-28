using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;


namespace voiceRecord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            userTb.Focus();
            string folderPath1 = @"AvinData";
            // Basic folder existence check
            if (!Directory.Exists(folderPath1))
            {
                try
                {
                    DirectoryInfo directory = Directory.CreateDirectory(folderPath1);// Create folder - returns DirectoryInfo object
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show( $"Error creating folder: {ex.Message}");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"AvinData\user.csv";
            bool chUser = false;
            if (userTb.Text == "") System.Windows.MessageBox.Show("Please Enter UserName.");
            else if (passTb.Password == "") System.Windows.MessageBox.Show("Please Enter Passwrod.");
            else if (!System.IO.File.Exists(filePath)) System.Windows.MessageBox.Show("Please Register First.");           
            else {
                AvinCsvClass ob = new AvinCsvClass();
                // Method 1: Read CSV without headers and Process specific columns
                var data = ob.ReadFromCsv(filePath, hasHeaders: true);
                foreach (var row in data)
                    if (userTb.Text == row[0] && passTb.Password == row[1])
                    {
                        chUser = true;
                        ob.ModifyCsvValueWhenLogIn(filePath, "1", userTb.Text, passTb.Password);
                        //--------------------------------------------------
                        string audioPath="";
                        // Basic file existence check
                         if (!System.IO.File.Exists(@"AvinData\" + userTb.Text + ".csv"))
                            {
                            filedd:
                                using (var folderDialog = new FolderBrowserDialog())
                                {
                                    folderDialog.Description = "Select a folder";
                                    folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                                    folderDialog.ShowNewFolderButton = true;

                                    DialogResult result = folderDialog.ShowDialog();

                                    if (result.ToString() == "OK" && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                                        audioPath = folderDialog.SelectedPath;
                                    else goto filedd;
                                    
                                }
                            }
                        //--------------------------------------------------
                        RecordingWindow recordingWindow = new RecordingWindow(userTb.Text, audioPath, passTb.Password);
                        recordingWindow.Show();
                        this.Close();
                        break;
                    }
                if (!chUser) System.Windows.MessageBox.Show("Please Enter CORRECT UserName and Passwrod.");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RegisterWindow window1 = new RegisterWindow();
            window1.Show(); 
            this.Close();  
        }

       
    }
}