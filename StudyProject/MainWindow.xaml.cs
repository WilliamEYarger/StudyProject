using Microsoft.Win32;
using System.Windows;
using System.IO;
using System;
using StudyProject.HelperMethods;
using StudyProject.DataModels;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAPICodePack.Dialogs;


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
        /// This is a Major Revisioin beginning in the dev brance of 20210105
        /// The User will only create the folder to hold the subject 
        /// (whose name is that of the Subject) and the program will create all needed files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_FileOpen(object sender, RoutedEventArgs e)
        {
            // Get the Name of the Subject folder
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            string FolderPath = "";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FolderPath = dialog.FileName+'\\';
            }

            // Send this to the Subjects SubjectsFolderPath
            Subjects.SubjectsFolderPath = FolderPath;

            // Get the number of '\\'s in FolderPath
            var NumberOfSlashes = StringHelper.ReturnNumberOfDeliniters(FolderPath, '\\');


            // Get the Subjects Name from the item a position NumberOfSlashes -1
            var FolderName = StringHelper.ReturnItemAtPos(FolderPath, '\\', NumberOfSlashes - 1);

            // Set Subjects SubjectName
            Subjects.SubjectName = FolderName;

            // Send FolderName to tbkMessage
            tbkMessage.Text = FolderName;

            Subjects.OpenSubjectFile();

            // Get List of Root Items, if any, to display in the lbxItems on startup

            List<String> ListOfRootStrings = Subjects.ItemsListBoxStringsList;

            if(ListOfRootStrings.Count > 0)
            {
                foreach(string RootString in ListOfRootStrings)
                {
                    lbxItems.Items.Add(RootString);                   
                }
            }

           
        }// End FileOpen


        #endregion FileOpen 

        #region File Save
        private void Click_FileSave(object sender, RoutedEventArgs e)
        {
            Subjects.SaveSubjectsFile();
            // Clear any data in the list boxes
            lbxItems.Items.Clear();
            lbxTree.Items.Clear();
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
            var DisplayString = Subjects.CreateDisplayString(NewRootItem);


            // add this string to the items listbox
            lbxItems.Items.Add(DisplayString);
        }

        #endregion Add Root Item

        #region Add Child Item
        private void Click_AddChildItem(object sender, RoutedEventArgs e)
        {
            /*
             20210108 the problem with the lbxTree display is in this method 
             */


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
            var ChildsDisplayString = Subjects.CreateDisplayString(NewChildItem);

            // Display the Childs display string in the items listbox
            lbxItems.Items.Add(ChildsDisplayString);
            /*
             * At this point I need to get all the items in lbxTree, 
             * convert them to a List<string>
             * update the Parent's string
             * clear lbxTree
             * reload the updated List
             */

            // Convert all of the items in the tree list box to an List<string>
            List<string> CurrnetTreeItems = new List<string>();
            
            // Load all the items in lbxTree into this list
            foreach(string TreeItem in lbxTree.Items)
            {
                CurrnetTreeItems.Add(TreeItem);
            }

            // Convert this list to a string []
            string[] TreeItemsArray = CurrnetTreeItems.ToArray();

            // Create a new Display string for the updated Parent
            var NewParentsDisplayString = StringHelper.CreateDisplayString(ParentItem.LeedingChar, ParentItem.ItemText,
                ParentItem.ItemID, ParentItem.ItemsNumberOfChildren);

            // Cycle through the list until you fing the Parent's ID
            foreach(string TreeItem in lbxTree.Items)
            {
                var ID = StringHelper.ReturnItemAtPos(TreeItem, '^', 1);
                if(ID == ParentsID)
                {
                    // Get the position of the Parent display string in TreeItemsArray from the length of its ID
                    var PosOfParent = ID.Length / Subjects.AlphaBase - 1;
                    //Before Adding NewParentsDisplayString back to the tree update its leading spaces
                    int addSpacesNumber = (ID.Length - 1) * 2;
                    string spacesString = new string(' ', addSpacesNumber);
                    NewParentsDisplayString = spacesString + NewParentsDisplayString;
                    TreeItemsArray[PosOfParent] = NewParentsDisplayString;
                    break;
                }
            }

            // Clear the TreeList
            lbxTree.Items.Clear();

            // Cycle through TreeItemsArray addint them to lbxTree

            foreach(string TreeItemDisplayString in TreeItemsArray)
            {
                lbxTree.Items.Add(TreeItemDisplayString);
            }

            //// creat a counter for the array of strings holding the items in the tree list box
            //var CurrentTreeItemsCount = lbxTree.Items.Count;

            //// Create a string array of this size
            //string[] TreeArray = new string[CurrentTreeItemsCount];

            //// Create a new string [] to hold the revised  tree listbox
            //string [] NewTreeArray = new string[CurrentTreeItemsCount];

            //// Create a counter
            //int Cntr = 0;

            //// Process each item in this array, updating it as necessary
            //for(int i =0; i<lbxTree.Items.Count; i++)
            //{
            //    var displayLine = lbxTree.Items[i].ToString();
            //    // determine if this line is the parents line
            //    int IndexOfParentsID = displayLine.IndexOf(ParentsID);
            //    if(IndexOfParentsID != -1)
            //    {
            //        // Repalce leeding char with a '+'
            //        string tempString = displayLine.Trim();
            //        //tring somestring = "abcdefg";
            //        StringBuilder sb = new StringBuilder(tempString);
            //        sb[0] = '+'; // index starts at 0!
            //        tempString = sb.ToString();

            //        // convert tempSring to an array of strigs
            //        string[] arrayOfSections = tempString.Split('^');

            //        // Replaced the current number of children with  CurrentNumberOfChildren
            //        arrayOfSections[2] = CurrentNumberOfChildren.ToString();

            //        // Add back the leading spaces, if any

            //        int addSpacesNumber = (ParentsID.Length - 1) * 2;
            //        string spacesString = new string(' ', addSpacesNumber);

            //        //Reassemble the string
            //        var newDisplayString = spacesString + arrayOfSections[0] +'^'+  arrayOfSections[1] + '^' + arrayOfSections[2];
            //        NewTreeArray[Cntr] = newDisplayString;
            //    }
            //    else
            //    {
            //        NewTreeArray[Cntr] = displayLine;
            //    }
            //}// End foreach 

            //// destroy the old tree listbox
            //lbxTree.Items.Clear();

            //// add the items in NewTreeArray to the tree ListBox
            //foreach(string DisplayLine in NewTreeArray)
            //{
            //    lbxTree.Items.Add(DisplayLine);
            //}


        }// End CreateChild Node


        #endregion Add Child Item

        #endregion Items Menu

        #region Selected Items Menu

        #region Click_MoveToTree
        private void Click_MoveToTree(object sender, RoutedEventArgs e)
        {
            // Get the selected item string
            var MovedItemsDisplayString = lbxItems.SelectedItem.ToString();

            // Create a List<string> of the current tree items
            List<string> ListOfTreeItems = new List<string>();

            // Add all items currently in lbxTree to ListOfTreeItems

            // It there are items in lbxTree add them to TreeDisplayString
            if (lbxTree.Items.Count > 0)
            {
                foreach (string TreeDisplayString in lbxTree.Items)
                {
                    ListOfTreeItems.Add(TreeDisplayString);
                }
            }
            

            // Adjust the header string of MovedItemsDisplayString
            string MovedItemsID = StringHelper.ReturnItemAtPos(MovedItemsDisplayString, '^', 1);
            var LengthOfHeaderString = (MovedItemsID.Length - Subjects.AlphaBase) * 2;
            var LeadingSpacesString = new String(' ', LengthOfHeaderString);
            MovedItemsDisplayString = LeadingSpacesString + MovedItemsDisplayString;

            // Add the adjusted selected item string to ListOfTreeItems
            ListOfTreeItems.Add(MovedItemsDisplayString);

            // Clear the old values in lbxTree
            lbxTree.Items.Clear();

            // Recreate lbxTree with ListOfTreeItems
            foreach(string DisplayString in ListOfTreeItems)
            {
                lbxTree.Items.Add(DisplayString);
            }

            // Create an Item object of the moved item to see if it has children
            Items MovedItem = Subjects.ReturnItemInDictionary(MovedItemsID);


            // Clear any items currently in the lbxItem
            lbxItems.Items.Clear();

            // If MovedItem has children, get their display strings
            if (MovedItem.ItemsNumberOfChildren > 0)
            {
                List<string> ListOfItemsChildrensDisplayStrings = new List<string>();
                ListOfItemsChildrensDisplayStrings = Subjects.ReturnDisplaystringsOfItemsChildren(MovedItemsID);


                // Add Display strings in ListOfItemsChildrensDisplayStrings to lbxItems
                foreach (string DisplayLine in ListOfItemsChildrensDisplayStrings)
                {
                    lbxItems.Items.Add(DisplayLine);
                }
            }

            





            





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
