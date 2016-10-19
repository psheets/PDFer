using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;

namespace PDFer
{
    public partial class Form1 : Form
    {
        BindingList<System.IO.FileInfo> fileList = new BindingList<System.IO.FileInfo>();
        public Form1()
        {
            InitializeComponent();

        }
        private void add_folder(object sender, EventArgs e)
        {
              UpdateFileList();
        }
        private void browse_source_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFileDialog1 = new FolderBrowserDialog();
            openFileDialog1.ShowNewFolderButton = false;
            openFileDialog1.SelectedPath = textBox1.Text;

            DialogResult result = openFileDialog1.ShowDialog();
          
            if (result == DialogResult.OK)
            {

                string source = openFileDialog1.SelectedPath;
                textBox1.Text = source;                
            }
        }

        private void browser_destination_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFileDialog1 = new FolderBrowserDialog();
            openFileDialog1.ShowNewFolderButton = true;
            openFileDialog1.SelectedPath = textBox1.Text;
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {

                string source = openFileDialog1.SelectedPath;
                textBox2.Text = source;
                

            }
        }

        private void btn_PDF_Click(object sender, EventArgs e)
        {
            PdfFiles();
        }

        // Close app when button is clicked
        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        // Use Word automation to PDF files in Directorys and out put to destination.
        private void PdfFiles()
        {

            // Create oject for not needed values
            object oMissing = System.Reflection.Missing.Value;

            var count = 0;

            // Get 
            DirectoryInfo dirInfo = new DirectoryInfo(textBox1.Text);
            var wordFiles = dirInfo.GetFiles("*.doc").Where(name => !name.Name.StartsWith("~"));


            // Create a new Word application object
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = wordFiles.Count();


            foreach (FileInfo wordFile in wordFiles)
            {
                //progress.Text = "PDFed " + count + " of " + wordFiles.Count();
                
                Object filename = (object)wordFile.FullName;
                Object outputfilename;

                if (wordFile.Name.EndsWith(".docx"))
                {
                    outputfilename = textBox2.Text + @"\" + wordFile.Name.Replace(".docx", ".pdf");
                }

                else
                {
                    
                    outputfilename = textBox2.Text + @"\" + wordFile.Name.Replace(".doc", ".pdf");

                }
                

                Document doc = word.Documents.Open(ref filename, ReadOnly:true);
                doc.Activate();
                doc.SaveAs(ref outputfilename, WdSaveFormat.wdFormatPDF);

                object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
                ((_Document)doc).Close();
                doc = null;
                count++;
                progressBar1.Value = count;
            }

            ((_Application)word).Quit(ref oMissing, ref oMissing, ref oMissing);
            word = null;

            if (count == wordFiles.Count())
            {
                MessageBox.Show("PDFing has completed!");
                count = 0;
                progressBar1.Value = count;
                listBox1.DataSource = null;
                listBox1.Items.Clear();
            }
        }

        private void UpdateFileList()
        {
            // Get Files in directory and populate listBox1
            DirectoryInfo dirInfo = new DirectoryInfo(textBox1.Text);
            var temp = dirInfo.GetFiles("*.doc", SearchOption.AllDirectories).Where(name => !name.Name.StartsWith("~")).ToList();
            foreach (var item in temp)
            {
                fileList.Add(item);
            }
            listBox1.DataSource = fileList;
            Console.WriteLine(fileList.ToString());
        }
        private void RemoveFile(object sender, EventArgs e)
        {
            var index = fileList.Select(Name => listBox1.SelectedItem).ToString();
            Console.WriteLine(index);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }
    }
}
