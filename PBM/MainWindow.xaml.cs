using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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

namespace PBM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        //private string Box { get { return box.Text; } set { box.Text = value; } } //Declares variable "Box"
        private string Source { get { return url.Text; } set { url.Text = value; } } //Declares variable "URL"
        private string Target { get { return url2.Text; } set { url2.Text = value; } }
        private string Filter { get { return filter.Text; } set { filter.Text = value; } }
        private string ListV { get { return listView.ItemStringFormat; } set { listView.ItemStringFormat = value; } }
        private string Label { get { return label.Content.ToString(); } set { label.Content = value; } }
        private string[] files;
        private string[] folders;
        private int num = 0; //Declares integer "num" and gives it the value "0"
        private int fNum = 0; //Declares integer "fNum" and gives it the value "0"
        private string name; //Declares string "name"
        private string messageBoxText;
        private string caption;
        private List<Item> items;
        private bool _showButton;

        public bool ShowButton { get; set; }
        #endregion

        #region Actions
        public MainWindow() //Runs on startup
        {
            InitializeComponent();
            GetFiles();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //Get Files
        {
            GetFiles();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //Move selected files to new list
        {
            SelFiles();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //Delete selected files
        {
            caption = "Hold up!";
            messageBoxText = "Are you sure you want to delete these files?";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    DelFiles();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) //Open folder
        {
            Open();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) //Creates a .txt file
        {
        }

        private void Button_Click_5(object sender, RoutedEventArgs e) //Copy
        {
            Copy();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e) //Move
        {
            Move();
        }


        private void Button_Click_7(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        #region Methods
        public void GetFiles() //Gets all files in the given directory
        {
            //SelItem selItem = new SelItem();
            //listView.Items.Clear();
            //num = 0;
            listView.Items.Clear();
            num = 0;
            fNum = 0;
            Item item = new Item();
            if (!Directory.Exists(Source)) //Checks if the directory doesn't exist
            {
                caption = "Directory not found!";
                messageBoxText = "The directory cannot be found, would you want to create it?";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Error;
                //MessageBox.Show(messageBoxText, caption, button, icon);
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        CreateFolder();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                files = Directory.GetFiles(Source); //Makes an array of all files in the directory 
                folders = Directory.GetDirectories(Source); //Makes an array of all folders in the directory
                if (folders.Length < 1 && files.Length < 1)
                {
                    Label = "No files were found";
                }
                else
                {
                    //Box = "These files were found:\n";
                    if (folders.Length > 0)
                    {
                        //Box += "-------------\nFolders:\n-------------\n";

                        foreach (string folder in folders)
                        {
                            name = Path.GetFileName(folder);
                            fNum++;
                            listView.Items.Add(new Item() { ID = fNum, Name = name });
                            //Box += fNum + " " + name + "\n";
                        }
                    }

                    if (files.Length > 0)
                    {
                        //Box += "-------------\nFiles:\n-------------\n";

                        foreach (string file in files)
                        {
                            name = Path.GetFileName(file);
                            num++;
                            listView.Items.Add(new Item() { ID = num, Name = name });
                            //Box += num + " " + name + "\n";
                        }
                    }
                }
                Label = num + " files and " + fNum + " folders were found in " + Source;

                //if(listView.SelectedIndex == 1)
                //{
                //    foreach (string file in files)
                //    {
                //        listView_Copy.Items.Add(new Item() { ID = num, Name = name });
                //    }
                //}
            }
        }

        public void SelFiles()
        {
            //Item item = new Item();
            //string selItem = listView.SelectedItems[0].ToString();
            //ListViewItem[] lVI = new ListViewItem[listView.SelectedItems.Count]; //Makes an array of all selected items
            listView_Copy.Items.Clear();
            foreach (Item viewItem in listView.SelectedItems)
            {
                listView_Copy.Items.Add(viewItem);
            }
        }

        public void DelFiles()
        {
            foreach (Item viewItem in listView_Copy.Items)
            {
                File.Delete(Source + "\\" + viewItem.Name);
                Label = "Successfully removed " + viewItem.Name + " from " + Source;
            }
        }

        public void CreateFolder()
        {
            Directory.CreateDirectory(Source);
        }

        public void Open()
        {
            Process.Start(Source);
        }

        public void Copy()
        {
            foreach(Item viewItem in listView_Copy.Items)
            {
                File.Copy(Source + "\\" + viewItem.Name, Target + "\\" + viewItem.Name);
                Label = listView_Copy.Items.Count + " files has been copied to " + Target;
            }
        }

        public void Move()
        {
            foreach(Item viewItem in listView_Copy.Items)
            {
                File.Move(Source + "\\" + viewItem.Name, Target + "\\" + viewItem.Name);
                Label = listView_Copy.Items.Count + " files has been moved to " + Target;
            }
        }
        #endregion
    }

    public class Item
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }
}
