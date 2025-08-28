using System.IO;
using System.Windows;

namespace voiceRecord
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            usernameTxb.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AvinCsvClass ob = new AvinCsvClass();
            string filePath = @"AvinData\user.csv";
            bool nameCheck=true;
            try
            {
                string a = usernameTxb.Text.ToLower();
                if (usernameTxb.Text == "") MessageBox.Show("Please Enter UserName.");
                else if (passTxb.Password=="") MessageBox.Show("Please Enter Passwrod.");
                else if (passTxb.Password != repasTxb.Password) MessageBox.Show("Please Enter The Same Passwrod.");
                else
                {
                    foreach (char i in a)
                    {
                        if (i >= 'a' && i <= 'z' || i >= '0' && i <= '9')continue;                             
                        else
                        {
                            nameCheck = false;
                            MessageBox.Show("Please Enter Only Letters and Numbers.");
                            break;
                        }
                    }
                    var singleRow = new string[] { };
                    if (!File.Exists(filePath))
                    {
                        // Example 1: Append single row
                        singleRow = new string[] { "UserName", "Password", "state" };
                        ob.AppendToCsvAvin(filePath, singleRow);
                    }
                    else
                    {
                        // Method 1: Read CSV without headers and Process specific columns
                        var data = ob.ReadFromCsv(@"AvinData\user.csv", hasHeaders: true);
                        foreach (var row in data)
                        {
                            if (usernameTxb.Text == row[0])
                            {
                                MessageBox.Show("This UserName is already available;\nPlease Enter another UserName.");
                                nameCheck = false;
                                break;
                            }
                        }
                    }
                    if (nameCheck == true) {
                        singleRow = new string[] { usernameTxb.Text, passTxb.Password, "0" };
                        ob.AppendToCsvAvin(filePath, singleRow);
                        MessageBox.Show("Successfully Registered");
                        MainWindow window1 = new MainWindow();
                        window1.Show(); 
                        this.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
