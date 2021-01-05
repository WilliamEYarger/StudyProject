using Microsoft.Win32;
using System.Windows;
using System.IO;
using System;
using StudyProject.HelperMethods;
using StudyProject.DataModels;
using System.Collections.Generic;
using System.Text;

namespace StudyProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        


        #region MenuItems

        #region File Menu

        #region FileOpen
        /// <summary>
        ///  Select the folder which holds the data you want to work on
        ///  using the file dialogue. It will get the Alpha base
        ///  and the Subject's Name and send them and the Path to the
        ///  folder to the Subjects class it will then call on the Subjects
        ///  folder OpenDataFiles metod to load all of the data about this
        ///  subject
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_FileOpen(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "Text  Document (*.txt)";
            if (ofd.ShowDialog() == true)
            {
                var FilePath = ofd.FileName;
                var RevisedFilePath = FilePath.Replace('\\','/');
                var PosLastSlash = RevisedFilePath.LastIndexOf('/');
                var FolderPath = RevisedFilePath.Substring(0, PosLastSlash+1);
                Subjects.DataFolderPath = FolderPath;

                //Send data file path to Subjects class
                Subjects.OpenDataFiles(FilePath);

                // Open the dictgionary file
                Subjects.OpenDictionaryFile();

                // Get the Subject Name and  publish it in tbxMessage
                tbkMessage.Text = Subjects.SubjectName;

               
            }
            
        }
        #endregion FileOpen 

        #region File Save
        private void Click_FileSave(object sender, RoutedEventArgs e)
        {
            // Get the New Number of Roots
            var NewNumberOfRoots = Subjects.NumberOfRoots;

            // Get the  SubjectData array
            string[] SubjectDataArray = Subjects.SubjectData;

            // Update the number of roots
            string NumberOfRootsString = SubjectDataArray[2];

            // Get the Number of children line
            var NumberOfChildrenStr = SubjectDataArray[2];

            // Update the number of children in this line
            StringHelper.ReplaceItemAtPosition(ref NumberOfChildrenStr, '^', 1, NewNumberOfRoots.ToString());

            // Replace this updated number of children line
            SubjectDataArray[2] = NumberOfChildrenStr;

            // Update the SubjectData array
            Subjects.SubjectData = SubjectDataArray;

            Subjects.SaveSubjectDataFile();

            Subjects.SaveDataDictionary();

        }
        #endregion File Save

        #region Exit
        private void Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion Exit

        #endregion File Menu

        #region Items Menu

        #region Add Root Item
        private void Click_AddRootItem(object sender, RoutedEventArgs e)
        {

            
            if(tbxItemText.Text == "")
            {
                MessageBox.Show("You Must enter a value in the Enter Item Text TextBox");
                return;
            }

            // Create a new Items object
            Items NewRootItem = new Items();

            // Get its name
            NewRootItem.ItemText = tbxItemText.Text;

            // Clear the text box
            tbxItemText.Text = "";

            // Get its parents number of Children
            var ThisRootsChildNumber = Subjects.NumberOfRoots;
            NewRootItem.ParentsNumberOfChildren = ThisRootsChildNumber;

            // Increment the number of roots
            Subjects.NumberOfRoots += 1;

            // Gets its ID
            NewRootItem.ItemID = StringHelper.ReturnItemAlphaNumber(ThisRootsChildNumber);

            NewRootItem.ItemsNumberOfChildren = 0;

            NewRootItem.ParentsID = "";

            // Get terminal node
            if ((bool)ckbxTerminal.IsChecked)
            {
                NewRootItem.TerminalNode = true;
            }
            else
            {
                NewRootItem.TerminalNode = false;
            }

            // Add this Item to the Items Dictionary

            Dictionary<string, Items> ItemsDictionary = Subjects.ItemsDictionary;
            ItemsDictionary.Add(NewRootItem.ItemID, NewRootItem);
            Subjects.ItemsDictionary = ItemsDictionary;

            //Get the Display string for this item
            var DisplayString = StringHelper.CreateDisplayString(NewRootItem.LeedingChar, NewRootItem.ItemText, NewRootItem.ItemID, NewRootItem.ItemsNumberOfChildren);


            // add this string to the items listbox
            lbxItems.Items.Add(DisplayString);
        }

        #endregion Add Root Item

        #region Add Child Item
        private void Click_AddChildItem(object sender, RoutedEventArgs e)
        {
            // Make sure that you have selected an item in the tree listbox
            if(lbxTree.SelectedItem == null)
            {
                MessageBox.Show("You must select an item in the Tree as a parent before adding a child element!");
                return;
            }

            // Create a new Child Items object
            Items NewChildItem = new Items();

            // Get the Parent Item
            var ParentDisplayLine = lbxTree.SelectedItem.ToString();

            // Get the Parents ID
            var ParentsID = StringHelper.ReturnItemAtPos(ParentDisplayLine, '^', 1);

            // Get the Parents Items object
            Items ParentItem = Subjects.ReturnItemInDictionary(ParentsID);

            //Get the number of children in the parent
           var CurrentNumberOfChildren = ParentItem.ItemsNumberOfChildren;

            // Get the Alpha Number corresponding to this child number
            var ChildsTempID = StringHelper.ReturnItemAlphaNumber(CurrentNumberOfChildren);

            // Create the Childs ID
            NewChildItem.ItemID = ParentsID + ChildsTempID;

            // Set the Childs Text
            NewChildItem.ItemText = tbxItemText.Text;

            // Set the Childs leeding char
            NewChildItem.LeedingChar = '-';

            // Set the Childes number of Children
            NewChildItem.ItemsNumberOfChildren = 0;

            // Set the Childs ParentsID
            NewChildItem.ParentsID = ParentsID;

            // Increment the Parents CurrentNumberOfChildren
            CurrentNumberOfChildren += 1;

            // Set the Parents Number of Children to this incremented value
            ParentItem.ItemsNumberOfChildren = CurrentNumberOfChildren;

            //Update Parents leeding char
            ParentItem.LeedingChar = '+';

            //Set the CHilds ParentsNumberOfChildren
            NewChildItem.ParentsNumberOfChildren = ParentItem.ItemsNumberOfChildren;

            // Set the Childs Terminal value
            if ((bool)ckbxTerminal.IsChecked)
            {
                NewChildItem.TerminalNode = true;
            }
            else
            {
                NewChildItem.TerminalNode = false;
            }

            // Send the NewChildItem to the items dictionary
            Dictionary<string, Items> ItemsDictionary = Subjects.ItemsDictionary;
            ItemsDictionary.Add(NewChildItem.ItemID, NewChildItem);
            Subjects.ItemsDictionary = ItemsDictionary;

            //Create the childs display string
            var ChildsDisplayString = StringHelper.CreateDisplayString(NewChildItem.LeedingChar, NewChildItem.ItemText, 
                NewChildItem.ItemID, NewChildItem.ItemsNumberOfChildren);

            // Display the Childs display string in the items listbox
            lbxItems.Items.Add(ChildsDisplayString);

            // Convert all of the items in the tree list box to an array

            // creat a counter for the array of strings holding the items in the tree list box
            var CurrentTreeItemsCount = lbxTree.Items.Count;

            // Create a string array of this size
            string[] TreeArray = new string[CurrentTreeItemsCount];

            // Create a new string [] to hold the revised  tree listbox
            string [] NewTreeArray = new string[CurrentTreeItemsCount];

            // Create a counter
            int Cntr = 0;

            // Process each item in this array, updating it as necessary
            for(int i =0; i<lbxTree.Items.Count; i++)
            {
                var displayLine = lbxTree.Items[i].ToString();
                // determine if this line is the parents line
                int IndexOfParentsID = displayLine.IndexOf(ParentsID);
                if(IndexOfParentsID != -1)
                {
                    // Repalce leeding char with a '+'
                    string tempString = displayLine.Trim();
                    //tring somestring = "abcdefg";
                    StringBuilder sb = new StringBuilder(tempString);
                    sb[0] = '+'; // index starts at 0!
                    tempString = sb.ToString();

                    // convert tempSring to an array of strigs
                    string[] arrayOfSections = tempString.Split('^');

                    // Replaced the current number of children with  CurrentNumberOfChildren
                    arrayOfSections[2] = CurrentNumberOfChildren.ToString();

                    // Add back the leading spaces, if any

                    int addSpacesNumber = (ParentsID.Length - 1) * 2;
                    string spacesString = new string(' ', addSpacesNumber);

                    //Reassemble the string
                    var newDisplayString = spacesString + arrayOfSections[0] +'^'+  arrayOfSections[1] + '^' + arrayOfSections[2];
                    NewTreeArray[Cntr] = newDisplayString;
                }
                else
                {
                    NewTreeArray[Cntr] = displayLine;
                }
            }// End foreach 

            // destroy the old tree listbox
            lbxTree.Items.Clear();

            // add the items in NewTreeArray to the tree ListBox
            foreach(string DisplayLine in NewTreeArray)
            {
                lbxTree.Items.Add(DisplayLine);
            }



        }// End CreateChild Node


        #endregion Add Child Item

        #endregion Items Menu

        #region Selected Items Menu

        #region Click_MoveToTree
        private void Click_MoveToTree(object sender, RoutedEventArgs e)
        {
            // Get the selected item string
            var SelectedItem = lbxItems.SelectedItem.ToString();


            //Add this Item to the TreeList
            Subjects.AddItemToTreeList(SelectedItem);

            // Clear the lbxTree
            lbxTree.Items.Clear();

            //Get the Update treelist
            List<string> NewTreeList = Subjects.ReturnTreeList();

            //Fill lbxTree with this list
            foreach(string Item in NewTreeList)
            {
                lbxTree.Items.Add(Item);
            }

            // Clear the items List
            lbxItems.Items.Clear();

            //// Get the Position of the Item in the Tree that was selected as the Parent
            //var SelectedItemIndex = lbxItems.Items.IndexOf(SelectedItem);

            //// Get the string that represents the current number of children of this item
            //var NumberOfChildrenString = StringHelper.ReturnItemAtPos(SelectedItem, '^', 2);

            //// Convert this to an int
            //var CurrentNumberOfChildren = Int32.Parse(NumberOfChildrenString);

            ////Increment it
            //CurrentNumberOfChildren += 1;

            //// Reconvert it to a String
            //NumberOfChildrenString = CurrentNumberOfChildren.ToString();

            //// Add This Item to the TreeList
            //Subjects.AddItemToTreeList(SelectedItem);

            //// Get the current TreeList
            //List<string> CurrentTreeList = Subjects.ReturnTreeList();

            //// Get a new TreeList with the Parents String updated
            //List<string> UpdatedTreeList = Subjects.ReturnUpdatedTreeList(CurrentTreeList, SelectedItemIndex, CurrentNumberOfChildren);

            //// Clear all of the items in the tree list
            //lbxItems.Items.Clear();

            //// reconstitute the tree swith the UpdatedTreeList

            //foreach(string Item in UpdatedTreeList)
            //{
            //    lbxItems.Items.Add(Item);
            //}
        }
        #endregion Click_MoveToTree

        private void Click_OpenWorkPage(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("OpenWorkPage Clicked");
        }


        #endregion Selected Items Menu

        #region Instrunctions Menu
        private void Click_CreateNewSubject(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("To Create a new subject:\r\n" +
                "In the OneDrive's Documents Folder's  _StudyFolder sub-folder create a new folder. The Name " +
                "of this folder should be the name of the subject (ie Subject1). In this folder" +
                " you shold also create a text file whose name is also that of the subject (ie Subject1.txt)" +
                "This text file should have two lines.  The  first specifying the AlphaBase (ie AlphaBase^1) " +
                "and the second specifying the subjects name (ie Name^Subject) For Example: \r\n" +
                "Documents\r\n" +
                "   _StudyFolder\r\n" +
                "       Subject1 \r\n" +
                "           Subject1 Data.txt\r\n" +
                "               AlphaBase^1\r\n" +
                "               Name^Subject1");
        }
        #endregion Instrunctions Menu

        #endregion MenuItems

        #region Private Methods

        
        #endregion Private Methods

    }
}
