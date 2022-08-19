using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BankLibrary;

namespace CreateFile
{
    public partial class CreateFileForm : BankUIForm
    {
        private StreamWriter fileWriter;  // Writes data to text file

        // Parameterless constructor
        public CreateFileForm()
        {
            InitializeComponent();
        }

        // Event handler for Save Button
        private void saveButton_Click(object sender, EventArgs e)
        {
            // Create and show dialog box enabling user to save file
            DialogResult result;  // Result of SaveFileDialog
            string fileName;  // Name of file containing data

            using (var fileChooser = new SaveFileDialog())
            {
                fileChooser.CheckFileExists = false;  // Let user create file
                result = fileChooser.ShowDialog();
                fileName = fileChooser.FileName; // Name of file to save data
            }

            // Ensure that user clicked "OK"
            if (result == DialogResult.OK)
            {
                // Show error if user specified invalid file type
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Invalid File Name", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Save file via FileStream
                    try
                    {
                        // Save file with write access
                        var output = new FileStream(fileName,
                            FileMode.OpenOrCreate, FileAccess.Write);

                        // Set s file to where data is written
                        fileWriter = new StreamWriter(output);

                        // Disable Save Button and enable Enter Button
                        saveButton.Enabled = false;
                        enterButton.Enabled = true;
                    }
                    catch (IOException)
                    {
                        // Notify user if file does not exist
                        MessageBox.Show("Error opening file", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void enterButton_Click(object sender, EventArgs e)
        {
            // Store TextBox values string array
            string[] values = GetTextBoxValues();

            // Determine whether TextBox account field is empty
            if (!string.IsNullOrEmpty(values[(int)TextBoxIndices.Account]))
            {
                // Store TextBox values in Record and output it
                try
                {
                    // Get account-number value from TextBox
                    int accountNumber = int.Parse(values[(int)TextBoxIndices.Account]);

                    // Determine whether accountNumber is valid
                    if (accountNumber > 0)
                    {
                        // Record containing TextBox values to output
                        var record = new Record(accountNumber,
                            values[(int)TextBoxIndices.First],
                            values[(int)TextBoxIndices.Last],
                            decimal.Parse(values[(int)TextBoxIndices.Balance]));

                        // Write Record to file, fields separted by commas
                        fileWriter.WriteLine(
                            $"{record.Account},{record.FirstName}," +
                            $"{record.LastName},{record.Balance}");
                    }
                    else
                    {
                        // Notify user if invalid account number
                        MessageBox.Show("Invalid Account Number", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("Error writing to file", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Invalid Format", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            ClearTextBoxes(); // Clear TextBox values
        }

        // Handler for exitButton Click
        private void exitButton_Click(object sender, EventArgs e)
        {
            try
            {
                fileWriter?.Close();  // Close StreamWriter and underlying file
            }
            catch (IOException)
            {
                MessageBox.Show("Cannot close file", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.Exit();
        }
    }
}
