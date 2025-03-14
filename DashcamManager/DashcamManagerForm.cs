// Form1.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
//using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DashcamManager
{
    public partial class DashcamManagerForm : Form
    {
        private readonly string DashcamIPAddress;
        private readonly string DashcamDownloadPath;

        private string BaseUrl => "http://" + DashcamIPAddress; // Property for dynamic updates
        private readonly HttpClient httpClient = new HttpClient();
        private ProgressBar fileProgressBar;
        private ProgressBar totalProgressBar;
        private Label fileNameLabel;
        private Label downloadCountLabel;
        private ListView listView; // Store as field for access in Load event

        public DashcamManagerForm(string dashcamdashcamIPAddress, string dashcamDownloadFolderIP)
        {
            DashcamIPAddress = dashcamdashcamIPAddress;
            DashcamDownloadPath = dashcamDownloadFolderIP;

            InitializeComponent();
            //DashcamIP = dashcamIP; // Use the provided IP
            //DownloadPath = downloadPath; 
            SetupForm();
            Load += async (s, e) => await RefreshFileList(listView);
        }

        private void SetupForm()
        {
            Text = "Viofo A229 Pro Dashcam File Manager";
            Width = 800;
            Height = 600;

            listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(760, 400)
            };
            listView.Columns.Add("File Name", 300);
            listView.Columns.Add("Date", 150);
            listView.Columns.Add("Size (MB)", 100);
            listView.ColumnClick += ListView_ColumnClick;
            Controls.Add(listView);

            var refreshButton = new Button
            {
                Text = "Refresh List",
                Location = new System.Drawing.Point(10, 420),
                Size = new System.Drawing.Size(100, 30)
            };
            refreshButton.Click += async (s, e) => await RefreshFileList(listView);
            Controls.Add(refreshButton);

            var downloadButton = new Button
            {
                Text = "Download",
                Location = new System.Drawing.Point(120, 420),
                Size = new System.Drawing.Size(100, 30)
            };
            downloadButton.Click += async (s, e) => await DownloadSelectedFiles(listView);
            Controls.Add(downloadButton);

            var deleteButton = new Button
            {
                Text = "Delete Selected",
                Location = new System.Drawing.Point(230, 420),
                Size = new System.Drawing.Size(100, 30)
            };
            deleteButton.Click += async (s, e) => await DeleteSelectedFiles(listView);
            Controls.Add(deleteButton);

            var exitButton = new Button
            {
                Text = "Exit",
                Location = new System.Drawing.Point(340, 420),
                Size = new System.Drawing.Size(100, 30)
            };
            exitButton.Click += (s, e) => Application.Exit();
            Controls.Add(exitButton);

            fileNameLabel = new Label
            {
                Text = "XXX",
                Location = new System.Drawing.Point(10, 460),
                AutoSize = true,
                Visible = false
            };
            Controls.Add(fileNameLabel);

            int height = 20;
            using (Graphics g = this.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(fileNameLabel.Text, fileNameLabel.Font);
                height = (int)Math.Round(textSize.Height);
            }

            downloadCountLabel = new Label
            {
                Text = "foo",
                Location = new System.Drawing.Point(10, 490),
                AutoSize = true,
                Visible = false
            };
            Controls.Add(downloadCountLabel);

            fileProgressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(210, 460),
                Size = new System.Drawing.Size(500, height),
                Visible = false
            };
            Controls.Add(fileProgressBar);

            totalProgressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(210, 490),
                Size = new System.Drawing.Size(500, height),
                Visible = false
            };
            Controls.Add(totalProgressBar);

            ShowInTaskbar = true;
        }


        private async Task RefreshFileList(ListView listView)
        {
            try
            {
                if (listView.InvokeRequired)
                {
                    listView.Invoke(new Action(() => listView.Items.Clear()));
                }
                else
                {
                    listView.Items.Clear();
                }

                var response = await httpClient.GetStringAsync($"{BaseUrl}/?custom=1&cmd=3015");
                var serializer = new XmlSerializer(typeof(FileList));
                using (var reader = new StringReader(response))
                {
                    var fileList = (FileList)serializer.Deserialize(reader);
                    if (fileList?.AllFiles != null)
                    {
                        foreach (var allFile in fileList.AllFiles)
                        {
                            var file = allFile.File;
                            var date = DateTime.Parse(file.Time);
                            var item = new ListViewItem(new[] {
                                file.Name,
                                date.ToString("yyyy-MM-dd HH:mm:ss"),
                                (file.Size / 1048576.0).ToString("F2")
                            });
                            item.Tag = file;
                            if (listView.InvokeRequired)
                            {
                                listView.Invoke(new Action(() => listView.Items.Add(item)));
                            }
                            else
                            {
                                listView.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => MessageBox.Show($"Error refreshing file list: {ex.Message}")));
                }
                else
                {
                    MessageBox.Show($"Error refreshing file list: {ex.Message}");
                }
            }
        }

        private async Task DownloadSelectedFiles(ListView listView)
        {
            if (listView.SelectedItems.Count == 0) return;

            if (fileProgressBar.InvokeRequired)
            {
                fileProgressBar.Invoke(new Action(() =>
                {
                    fileProgressBar.Visible = true;
                    fileProgressBar.Maximum = 100;
                    fileProgressBar.Value = 0;
                }));
            }
            else
            {
                fileProgressBar.Visible = true;
                fileProgressBar.Maximum = 100;
                fileProgressBar.Value = 0;
            }
            String s = this.fileNameLabel.Text;

            fileNameLabel.Visible = true;
            downloadCountLabel.Visible = true;

            if (totalProgressBar.InvokeRequired)
            {
                totalProgressBar.Invoke(new Action(() =>
                {
                    totalProgressBar.Visible = true;
                    totalProgressBar.Maximum = listView.SelectedItems.Count;
                    totalProgressBar.Value = 0;
                }));
            }
            else
            {
                totalProgressBar.Visible = true;
                totalProgressBar.Maximum = listView.SelectedItems.Count;
                totalProgressBar.Value = 0;
            }

            int fileIndex = 0;
            foreach (ListViewItem item in listView.SelectedItems)
            {
                var file = (FileInfo)item.Tag;
                try
                {
                    fileIndex++;
                    string filePath = file.FPath.Replace(@"A:\DCIM\", "/DCIM/").Replace("\\", "/");
                    var fileUrl = $"{BaseUrl}{filePath}";

                    if (fileNameLabel.InvokeRequired)
                    {
                        fileNameLabel.Invoke(new Action(() =>
                            fileNameLabel.Text = file.Name));
                    }
                    else
                    {
                        fileNameLabel.Text = file.Name;
                    }
                    if (downloadCountLabel.InvokeRequired)
                    {
                        downloadCountLabel.Invoke(new Action(() =>
                            downloadCountLabel.Text = String.Format("Downloading file {0} / {1}", fileIndex, listView.SelectedItems.Count)));
                    }
                    else
                    {
                        downloadCountLabel.Text = String.Format("Downloading file {0} / {1}", fileIndex, listView.SelectedItems.Count);
                    }

                    using (var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        long totalBytes = response.Content.Headers.ContentLength ?? -1L;
                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(
                            Path.Combine(DashcamDownloadPath, Path.GetFileName(file.Name)),
                            FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead;
                            long totalRead = 0;
                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalRead += bytesRead;

                                if (totalBytes > 0)
                                {
                                    int fileProgress = (int)(totalRead * 100 / totalBytes);
                                    if (fileProgressBar.InvokeRequired)
                                    {
                                        fileProgressBar.Invoke(new Action(() =>
                                            fileProgressBar.Value = Math.Min(fileProgress, fileProgressBar.Maximum)));
                                    }
                                    else
                                    {
                                        fileProgressBar.Value = Math.Min(fileProgress, fileProgressBar.Maximum);
                                    }
                                }
                            }
                        }
                    }

                    if (totalProgressBar.InvokeRequired)
                    {
                        totalProgressBar.Invoke(new Action(() => totalProgressBar.Value = fileIndex));
                    }
                    else
                    {
                        totalProgressBar.Value = fileIndex;
                    }
                }
                catch (Exception ex)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => MessageBox.Show($"Error downloading {file.Name}: {ex.Message}")));
                    }
                    else
                    {
                        MessageBox.Show($"Error downloading {file.Name}: {ex.Message}");
                    }
                }
            }

            if (fileProgressBar.InvokeRequired)
            {
                fileProgressBar.Invoke(new Action(() => fileProgressBar.Visible = false));
            }
            else
            {
                fileProgressBar.Visible = false;
            }
            if (fileNameLabel.InvokeRequired)
            {
                fileNameLabel.Invoke(new Action(() => fileNameLabel.Visible = false));
            }
            else
            {
                fileNameLabel.Visible = false;
            }
            if (totalProgressBar.InvokeRequired)
            {
                totalProgressBar.Invoke(new Action(() => totalProgressBar.Visible = false));
            }
            else
            {
                totalProgressBar.Visible = false;
            }
            if (downloadCountLabel.InvokeRequired)
            {
                downloadCountLabel.Invoke(new Action(() => downloadCountLabel.Visible = false));
            }
            else
            {
                downloadCountLabel.Visible = false;
            }
            if (InvokeRequired)
            {
                Invoke(new Action(() => MessageBox.Show("Download completed!")));
            }
            else
            {
                MessageBox.Show("Download completed!");
            }
        }

        private async Task DeleteSelectedFiles(ListView listView)
        {
            if (listView.SelectedItems.Count == 0) return;

            if (MessageBox.Show(String.Format("Are you sure you want to delete the selected {0} {1}?", listView.SelectedItems.Count, listView.SelectedItems.Count==1? "file": "files"), 
                "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                fileNameLabel.Visible = true;
                int count = 0;
                int totfilecount = listView.SelectedItems.Count;
                foreach (ListViewItem item in listView.SelectedItems)
                {
                    var file = (FileInfo)item.Tag;

                    count++;
                    if (fileNameLabel.InvokeRequired)
                    {
                        fileNameLabel.Invoke(new Action(() =>
                            fileNameLabel.Text = String.Format("Deleting file {0} / {1}", count, totfilecount)));
                    }
                    else
                    {
                        fileNameLabel.Text = String.Format("Deleting file {0} / {1}", count, totfilecount);
                    }

                    try
                    {
                        await httpClient.GetAsync($"{BaseUrl}/?custom=1&cmd=4003&str={file.FPath}");
                        if (listView.InvokeRequired)
                        {
                            listView.Invoke(new Action(() => listView.Items.Remove(item)));
                        }
                        else
                        {
                            listView.Items.Remove(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => MessageBox.Show($"Error deleting {file.Name}: {ex.Message}")));
                        }
                        else
                        {
                            MessageBox.Show($"Error deleting {file.Name}: {ex.Message}");
                        }
                    }
                }

                fileNameLabel.Visible = false;
                if (InvokeRequired)
                {
                    Invoke(new Action(() => MessageBox.Show("Deletion completed!")));
                }
                else
                {
                    MessageBox.Show("Deletion completed!");
                }
            }
        }

        private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var listView = (ListView)sender;
            listView.ListViewItemSorter = new ListViewItemComparer(e.Column, listView.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending);
            listView.Sort();
            listView.Sorting = listView.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }
    }

    // Custom comparer for sorting ListView columns (non-generic IComparer)
    public class ListViewItemComparer : IComparer
    {
        private readonly int column;
        private readonly SortOrder sortOrder;

        public ListViewItemComparer(int column, SortOrder sortOrder)
        {
            this.column = column;
            this.sortOrder = sortOrder;
        }

        public int Compare(object x, object y)
        {
            var itemX = (ListViewItem)x;
            var itemY = (ListViewItem)y;
            int result;

            switch (column)
            {
                case 0: // File Name
                    result = string.Compare(itemX.SubItems[column].Text, itemY.SubItems[column].Text);
                    break;
                case 1: // Date
                    result = DateTime.Parse(itemX.SubItems[column].Text).CompareTo(DateTime.Parse(itemY.SubItems[column].Text));
                    break;
                case 2: // Size (MB)
                    result = double.Parse(itemX.SubItems[column].Text).CompareTo(double.Parse(itemY.SubItems[column].Text));
                    break;
                default:
                    result = 0;
                    break;
            }

            return sortOrder == SortOrder.Ascending ? result : -result;
        }
    }

    // XML data structures (unchanged)
    [XmlRoot("LIST")]
    public class FileList
    {
        [XmlElement("ALLFile")]
        public List<AllFile> AllFiles { get; set; }
    }

    public class AllFile
    {
        [XmlElement("File")]
        public FileInfo File { get; set; }
    }

    public class FileInfo
    {
        [XmlElement("NAME")] public string Name { get; set; }
        [XmlElement("FPATH")] public string FPath { get; set; }
        [XmlElement("SIZE")] public long Size { get; set; }
        [XmlElement("TIMECODE")] public long TimeCode { get; set; }
        [XmlElement("TIME")] public string Time { get; set; }
        [XmlElement("ATTR")] public int Attr { get; set; }
    }
}