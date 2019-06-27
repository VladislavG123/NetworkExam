using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HtmlAgilityPack;

namespace NetworkExam
{
    public partial class MainWindow : Window
    {
        private List<File> _files;

        public MainWindow()
        {
            InitializeComponent();
            _files = new List<File>();
            dataGrid.ItemsSource = _files;
        }

        private async void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            string html = await GetHtml(urlTextBox.Text);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var node = doc.DocumentNode.SelectSingleNode("//body");

            foreach (var nNode in node.Descendants())
            {
                if (nNode.NodeType == HtmlNodeType.Element)
                {
                    if (nNode.Name == "a")
                    {
                        foreach (var attribute in nNode.Attributes)
                        {
                            if (attribute.Name == "href" && attribute.Value.Contains(".exe"))
                            {
                                filesComboBox.Items.Add(attribute.Value);
                            }
                        }
                    }
                }
            }
        }

        private Task<string> GetHtml(string gettingUrl)
        {
            return Task.Run(() =>
            {
                string url = gettingUrl;
                return new WebClient().DownloadString(url);
            });
        }

        private async void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            if (filesComboBox.SelectedItem is null)
            {
                System.Windows.MessageBox.Show("Select file!");
                return;
            }


            var url = urlTextBox.Text + filesComboBox.SelectedItem as string;

            File newFile = new File
            {
                LocalPath = pathTextBox.Text,
                Name = url.Remove(0, url.LastIndexOf("/") + 1),
                Icon = MaterialDesignThemes.Wpf.PackIconKind.ProgressDownload,
                Url = url
            };

            await AddFile(newFile);

            if (await DownloadFile(url, pathTextBox.Text))
            {
                System.Windows.MessageBox.Show("Successful download!");
                _files.Where(file => file.Id == newFile.Id).ToList()[0].Icon = MaterialDesignThemes.Wpf.PackIconKind.Downloads;
                await UpdateDataGrid();
            }
            else
            {
                System.Windows.MessageBox.Show("Loading interrupted!");
                _files.Where(file => file.Id == newFile.Id).ToList()[0].Icon = MaterialDesignThemes.Wpf.PackIconKind.Cross;
                await UpdateDataGrid();
            }


        }

        private Task<bool> DownloadFile(string url, string to)
        {
            return Task.Run(() =>
            {
                try
                {
                    WebClient client = new WebClient();

                    client.DownloadFile(url, to);
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            });
        }

        private Task AddFile(File newFile)
        {
            return Task.Run(() =>
            {
                _files.Add(newFile);
                UpdateDataGrid();
            });
        }

        private Task UpdateDataGrid()
        {
            return Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = _files;
                });
            });
        }
    }
}
