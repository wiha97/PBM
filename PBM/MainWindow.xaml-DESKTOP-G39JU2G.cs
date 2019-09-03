using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private string SourceURL { get { return url.Text; } set { url.Text = value; } } //Gets and sets the Source textbox
        private string TargetURL { get { return url2.Text; } set { url2.Text = value; } } //Gets and sets the Target textbox
        private string Filter { get { return filter.Text; } set { filter.Text = value; } } //Gets and sets the Filter textbox
        private string Label { get { return label.Content.ToString(); } set { label.Content = value; } } //Gets and sets the Label
        private bool? FilterCheck { get { return filterCheck.IsChecked; } set { filterCheck.IsChecked = value; } } //Gets the filtercheck checkbox
        private bool? Contains; //Checks if a string contains a word
        private string source = "source.txt"; //Savefile for Source
        private string target = "target.txt"; //Savefile for Target
        private string sPath; //Source Path
        private string tPath; //Target Path
        private string content; //Content of save files
        private long Byte; //Base size of files
        private double KB; //Size in KB
        private double MB; //Size in MB
        private double GB; //Size in GB
        private long FreeSize; //Gets the free space of a harddrive
        private double fKB; //FreeSize in KB
        private double fMB; //FreeSize in MB
        private double fGB; //FreeSize in GB
        private string size; //Byte size plus "KB, MB, etc." string
        #endregion

        #region Actions
        public MainWindow() //Runs on startup
        {
            InitializeComponent();
            ReadSaves();
            sPath = SourceURL;
            tPath = TargetURL;
            GetFiles();
            GetTargetFiles();
            GetDrives();
        }

        #region Buttons
        private void Button_Click(object sender, RoutedEventArgs e) //Get Files
        {
            sPath = SourceURL;
            tPath = TargetURL;
            GetFiles();
            GetTargetFiles();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //Move selected files to new list
        {
            SelFiles();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //Delete selected files
        {
            string caption = "Hold up!";
            string messageBoxText = "Are you sure you want to delete these files?";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    DelSourceFiles();
                    DelTargetFiles();
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
            string caption = "Copy";
            string messageBoxText = "Are you sure you want to copy these files?";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            //MessageBox.Show(messageBoxText, caption, button, icon);
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    Copy();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e) //Move
        {
            string caption = "Move";
            string messageBoxText = "Are you sure you want to move these files?";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            //MessageBox.Show(messageBoxText, caption, button, icon);
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    Move();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e) //SaveSource
        {
            SaveSource();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e) //SaveTarget
        {
            SaveTarget();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e) //Resets to Source
        {
            if (File.Exists(source))
            {
                foreach (string line in File.ReadLines(source))
                {
                    SourceURL = line;
                }
            }
        }

        private void Button_Click_10(object sender, RoutedEventArgs e) //Resets to Target
        {
            if (File.Exists(target))
            {
                foreach (string line in File.ReadLines(target))
                {
                    TargetURL = line;
                }
            }
        }

        private void filterCheck_Checked(object sender, RoutedEventArgs e) //Enables filter search
        {
            filter.IsEnabled = true;
        }
        #endregion

        #region SourceView 
        private void sourceView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Item viewItem in sourceView.SelectedItems)
            {
                Label = viewItem.Name;
            }
        }

        private void sourceView_MouseDoubleClick(object sender, MouseButtonEventArgs e) //Opens the directory in the list
        {
            GetSubFiles();
        }

        private void openSource_Click(object sender, RoutedEventArgs e) //Opens a file or folder
        {
            foreach (Item viewItem in sourceView.SelectedItems)
            {
                Process.Start(sPath + "\\" + viewItem.Name);
            }
        }

        private void openSourceExp_Click(object sender, RoutedEventArgs e) //Opens the path or folder in Windows Explorer
        {
            foreach (Item viewItem in sourceView.SelectedItems)
            {
                Process.Start(sPath);
            }
        }

        private void setSource_Click_S(object sender, RoutedEventArgs e) //Set the Source textbox 
        {
            foreach (Item viewItem in sourceView.SelectedItems)
            {
                SourceURL = sPath + "\\" + viewItem.Name;
            }
        }

        private void setTarget_Click_S(object sender, RoutedEventArgs e) //Sets the Target textbox
        {
            foreach (Item viewItem in sourceView.SelectedItems)
            {
                TargetURL = Path.GetFullPath(viewItem.Name) + "\\" + viewItem.Name;
            }
        }

        #endregion
        #region TargetView
        private void targetView_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            foreach (Item viewItem in targetView.SelectedItems)
            {
                Label = viewItem.Name;
            }
        }

        private void targetView_MouseDoubleClick(object sender, MouseButtonEventArgs e) //Gets the subfiles of a folder
        {
            GetSubTargetFiles();
        }

        private void openTarget_Click(object sender, RoutedEventArgs e)
        {
            foreach (Item viewItem in targetView.SelectedItems)
            {
                Process.Start(TargetURL + "\\" + viewItem.Name.FirstOrDefault());
            }
        }

        private void setSource_Click_T(object sender, RoutedEventArgs e) //Sets Source textbox
        {
            foreach (Item viewItem in targetView.SelectedItems)
            {
                SourceURL = SourceURL + "\\" + viewItem.Name;
            }
        }

        private void setTarget_Click_T(object sender, RoutedEventArgs e) //Sets Target textbox
        {
            foreach (Item viewItem in targetView.SelectedItems)
            {
                TargetURL = TargetURL + "\\" + viewItem.Name;
            }
        }
        #endregion
        #region DriveView
        private void driveView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void driveView_MouseDoubleClick(object sender, MouseButtonEventArgs e) //gets a drive's subfiles in sourceView
        {
            foreach (Item viewItem in driveView.SelectedItems)
            {
                sPath = viewItem.Name;
                GetFiles();
            }
        }

        private void setTarget_Click(object sender, RoutedEventArgs e) //Sets target textbox
        {
            foreach (Item viewItem in driveView.SelectedItems)
            {
                TargetURL = viewItem.Name;
            }
        }

        private void setSource_Click(object sender, RoutedEventArgs e) //Sets source textbox
        {
            foreach (Item viewItem in driveView.SelectedItems)
            {
                SourceURL = viewItem.Name;
            }
        }

        private void open_Click(object sender, RoutedEventArgs e) //Opens a drive in Windows Explorer
        {
            foreach (Item viewItem in driveView.SelectedItems)
            {
                Process.Start(viewItem.Name);
            }
        }

        private void OpenInTarget_Click(object sender, RoutedEventArgs e)
        {
            foreach(Item viewItem in driveView.SelectedItems)
            {
                tPath = viewItem.Name;
                GetTargetFiles();
            }
        }
        #endregion
        #endregion

        #region Methods
        public void GetFiles() //Gets all files in Source directory
        {
            //SelItem selItem = new SelItem();
            string type;
            int num = 0;
            string name;
            sourceView.Items.Clear();
            int fNum = 0;
            if (!Directory.Exists(sPath)) //Checks if the directory doesn't exist
            {
                string caption = "Directory not found!";
                string messageBoxText = "The directory cannot be found, would you want to create it?";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Error;
                //MessageBox.Show(messageBoxText, caption, button, icon);
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        CreateSourceFolder();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                //files = Directory.GetFiles(Source); //Makes an array of all files in the directory 
                //folders = Directory.GetDirectories(Source); //Makes an array of all folders in the directory
                ////Array.Sort(files, new AlphanumComparatorFast());

                string[] arrFiles = Directory.GetFiles(sPath);
                string[] arrFolders = Directory.GetDirectories(sPath);

                var fileList = arrFiles.ToList();
                var folderList = arrFolders.ToList();
                fileList.Sort();
                folderList.Sort();

                if (FilterCheck == true)
                {
                    type = " of type " + Filter;

                }
                else
                {
                    type = "";
                }
                if (folderList.Count == 0 && fileList.Count == 0)
                {
                    Label = "No files" + type + " were found";
                }
                else if (filterCheck.IsChecked == true)
                {
                    //Box = "These files were found:\n";
                    if (folderList.Count > 0)
                    {
                        //Box += "-------------\nFolders:\n-------------\n";

                        foreach (string folder in folderList)
                        {
                            Contains = folder.Contains(Filter);
                            if (Contains == true)
                            {
                                name = Path.GetFileName(folder);
                                DirectoryInfo dirInfo = new DirectoryInfo(name);
                                Byte = dirInfo.GetFiles().Sum(file => file.Length);
                                KB = Byte / 1024;
                                MB = KB / 1024;
                                GB = MB / 1024;
                                fNum++;
                                sourceView.Items.Add(new Item() { Name = name, ID = fNum, Size = MB.ToString() });
                                //Box += fNum + " " + name + "\n";
                            }
                        }
                    }

                    if (fileList.Count > 0)
                    {
                        //Box += "-------------\nFiles:\n-------------\n";

                        foreach (string file in fileList)
                        {
                            Contains = file.Contains(Filter);
                            if (Contains == true)
                            {
                                name = Path.GetFileName(file);
                                FileInfo fileInfo = new FileInfo(SourceURL + "\\" + name);
                                Byte = fileInfo.Length;
                                KB = Byte / 1024;
                                MB = KB / 1024;
                                GB = MB / 1024;
                                num++;
                                sourceView.Items.Add(new Item() { Name = name, ID = num, Size = MB.ToString() });
                                //Box += num + " " + name + "\n";
                            }
                        }
                    }
                }
                else
                {
                    if (folderList.Count > 0)
                    {
                        //Box += "-------------\nFolders:\n-------------\n";

                        foreach (string folder in folderList)
                        {
                            name = Path.GetFileName(folder);
                            //DirectoryInfo dirInfo = new DirectoryInfo(folder);
                            //Size = dirInfo.GetFiles().Sum(file => file.Length);
                            Byte = folder.Length;
                            KB = Byte / 1024;
                            MB = KB / 1024;
                            GB = MB / 1024;
                            fNum++;
                            if (KB >= 1)
                            {
                                if (MB >= 1)
                                {
                                    if (GB >= 1)
                                    {
                                        size = Convert.ToInt32(GB).ToString() + "GB";
                                    }
                                    else
                                    {
                                        size = Convert.ToInt32(MB).ToString() + "MB";
                                    }
                                }
                                else
                                {
                                    size = Convert.ToInt32(KB).ToString() + "KB";
                                }
                            }
                            else
                            {
                                size = Convert.ToInt32(Byte).ToString() + "B";
                            }
                            sourceView.Items.Add(new Item() { Name = name, ID = fNum, Size = size });
                            //Box += fNum + " " + name + "\n";
                        }
                    }

                    if (fileList.Count > 0)
                    {
                        //Box += "-------------\nFiles:\n-------------\n";

                        foreach (string file in fileList)
                        {
                            name = Path.GetFileName(file);
                            FileInfo fileInfo = new FileInfo(sPath + "\\" + name);
                            Byte = fileInfo.Length;
                            KB = Byte / 1024;
                            MB = KB / 1024;
                            GB = MB / 1024;
                            num++;

                            if(KB >= 1)
                            {
                                if (MB >= 1)
                                {
                                    if (GB >= 1)
                                    {
                                        size = Convert.ToInt32(GB).ToString() + "GB";
                                    }
                                    else
                                    {
                                        size = Convert.ToInt32(MB).ToString() + "MB";
                                    }
                                }
                                else
                                {
                                    size = Convert.ToInt32(KB).ToString() + "KB";
                                }
                            }
                            else
                            {
                                size = Convert.ToInt32(Byte).ToString() + "B";
                            }
                            sourceView.Items.Add(new Item() { ID = num, Name = name, Size = size });
                            //Box += num + " " + name + "\n";
                        }
                    }
                }
                Label = num + " files and " + fNum + " folders were found in " + SourceURL;
                //if(sourceView.SelectedIndex == 1)
                //{
                //    foreach (string file in files)
                //    {
                //        driveView.Items.Add(new Item() { ID = num, Name = name });
                //    }
                //}
            }
        }

        public void GetTargetFiles() //Gets all files in Target directory
        {
            targetView.Items.Clear();
            string[] arrFiles = Directory.GetFiles(tPath);
            string[] arrFolders = Directory.GetDirectories(tPath);
            int num = 0;
            int fNum = 0;
            var files = arrFiles.ToList();
            var folders = arrFolders.ToList();
            string name;

            foreach (string folder in folders)
            {
                name = Path.GetFileName(folder);
                fNum++;
                targetView.Items.Add(new Item() { ID = fNum, Name = name });
            }

            foreach (string file in files)
            {
                name = Path.GetFileName(file);
                num++;
                targetView.Items.Add(new Item() { ID = num, Name = name });
            }
        }

        public void GetSubFiles() //Gets subfiles of SourceView
        {
            foreach(Item viewItem in sourceView.SelectedItems)
            {
                sPath += "\\" + viewItem.Name;
            }
            GetFiles();
        }

        public void GetSubTargetFiles() //Gets subfiles of TargetView
        {
            foreach (Item viewItem in targetView.SelectedItems)
            {
                tPath += "\\" + viewItem.Name;
            }
            GetTargetFiles();
        }

        public void GetDrives() //Gets all drives in the system
        {
            //List<Item> dirList = new List<Item>();
            string[] arrDrives = Directory.GetLogicalDrives();
            var drives = arrDrives.ToList();
            int num = 0;

            foreach (string drive in drives)
            {
                Byte = 0;
                FreeSize = 0;
                DriveInfo dI = new DriveInfo(drive);
                Byte += dI.TotalSize;
                FreeSize += dI.TotalFreeSpace;

                KB = Byte / 1024;
                MB = KB / 1024;
                GB = MB / 1024;
                fKB = FreeSize / 1024;
                fMB = fKB / 1024;
                fGB = fMB / 1024;

                num++;
                driveView.Items.Add(new Item()
                {
                    ID = num,
                    Name = drive,
                    Size = Convert.ToInt32(GB).ToString(),
                    FreeSpace = Convert.ToInt32(fGB)
                });
            }
        }

        public void SelFiles()
        {
            //Item item = new Item();
            //string selItem = sourceView.SelectedItems[0].ToString();
            //sourceViewItem[] lVI = new sourceViewItem[sourceView.SelectedItems.Count]; //Makes an array of all selected items
            //driveView.Items.Clear();
            //foreach (Item viewItem in sourceView.SelectedItems)
            //{
            //    driveView.Items.Add(viewItem);
            //}
        }

        public void DelSourceFiles() //Deletes all selected files in SourceView
        {
            foreach (Item viewItem in sourceView.SelectedItems)
            {
                File.Delete(SourceURL + "\\" + viewItem.Name);
                Label = "Successfully removed " + viewItem.Name + " from " + SourceURL;
            }
        }

        static string NullCheck(string test)
        {
            if (test == null)
                throw new System.NullReferenceException();
            return test;
        }

        public void DelTargetFiles() //Deletes all selected files in TargetView
        {
            try
            {
                foreach (Item viewItem in targetView.SelectedItems)
                {
                    File.Delete(TargetURL + "\\" + viewItem.Name);
                    Label = "Successfully removed " + viewItem.Name + " from " + TargetURL;
                }
            }
            catch
            {
                Label = "Could not delete files";
            }
        }

        public void CreateSourceFolder() //Creates a new folder in Source
        {
            Directory.CreateDirectory(SourceURL);
        }

        public void CreateTargetFolder() //Creates a new folder in Target
        {
            Directory.CreateDirectory(TargetURL);
        }

        public void Open() // Opens the source directory in Windows Explorer
        {
            Process.Start(SourceURL);
        }

        public void Copy() //Copies all selected files from Source to Target
        {
            foreach (Item viewItem in sourceView.SelectedItems)
            {
                File.Copy(SourceURL + "\\" + viewItem.Name, TargetURL + "\\" + viewItem.Name);
                Label = driveView.Items.Count + " files has been copied to " + TargetURL;
            }
        }

        public void Move() //Moves all selected files from Source to Target
        {
            foreach (Item viewItem in sourceView.SelectedItems)
            {
                File.Move(SourceURL + "\\" + viewItem.Name, TargetURL + "\\" + viewItem.Name);
                Label = driveView.Items.Count + " files has been moved to " + TargetURL;
            }
        }

        public void SaveSource() //Saves a .txt file with the source path
        {
            //string file = "source.txt";
            content = SourceURL;
            File.WriteAllText(source, content);
        }

        public void SaveTarget() //Saves a .txt file with the target path
        {
            content = TargetURL;
            File.WriteAllText(target, content);
        }

        public void ReadSaves() //Reads both source.txt and target.txt and puts them in their respective textboxes
        {
            if (File.Exists(source))
            {
                foreach (string line in File.ReadLines(source))
                {
                    SourceURL = line;
                }
            }
            if (File.Exists(target))
            {
                foreach (string line in File.ReadLines(target))
                {
                    TargetURL = line;
                }
            }
        }
        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Item
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string Size { get; set; }
        public double FreeSpace { get; set; }
    }

    //// NOTE: This code is free to use in any program.
    //// ... It was developed by Dot Net Perls.

    //public class AlphanumComparatorFast : IComparer
    //{
    //    public int Compare(object x, object y)
    //    {
    //        string s1 = x as string;
    //        if (s1 == null)
    //        {
    //            return 0;
    //        }
    //        string s2 = y as string;
    //        if (s2 == null)
    //        {
    //            return 0;
    //        }

    //        int len1 = s1.Length;
    //        int len2 = s2.Length;
    //        int marker1 = 0;
    //        int marker2 = 0;

    //        // Walk through two the strings with two markers.
    //        while (marker1 < len1 && marker2 < len2)
    //        {
    //            char ch1 = s1[marker1];
    //            char ch2 = s2[marker2];

    //            // Some buffers we can build up characters in for each chunk.
    //            char[] space1 = new char[len1];
    //            int loc1 = 0;
    //            char[] space2 = new char[len2];
    //            int loc2 = 0;

    //            // Walk through all following characters that are digits or
    //            // characters in BOTH strings starting at the appropriate marker.
    //            // Collect char arrays.
    //            do
    //            {
    //                space1[loc1++] = ch1;
    //                marker1++;

    //                if (marker1 < len1)
    //                {
    //                    ch1 = s1[marker1];
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

    //            do
    //            {
    //                space2[loc2++] = ch2;
    //                marker2++;

    //                if (marker2 < len2)
    //                {
    //                    ch2 = s2[marker2];
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

    //            // If we have collected numbers, compare them numerically.
    //            // Otherwise, if we have strings, compare them alphabetically.
    //            string str1 = new string(space1);
    //            string str2 = new string(space2);

    //            int result;

    //            if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
    //            {
    //                int thisNumericChunk = int.Parse(str1);
    //                int thatNumericChunk = int.Parse(str2);
    //                result = thisNumericChunk.CompareTo(thatNumericChunk);
    //            }
    //            else
    //            {
    //                result = str1.CompareTo(str2);
    //            }

    //            if (result != 0)
    //            {
    //                return result;
    //            }
    //        }
    //        return len1 - len2;
    //    }
    ////}
}
