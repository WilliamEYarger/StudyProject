using System;
using System.Collections.Generic;
using System.IO;
using StudyProject.HelperMethods;

namespace StudyProject.DataModels
{



    public static class Subjects
    {
        // TODO- There is a problem in saving the files and in reconstructing lbxTree after adding a new child

        #region Private Properties
        private static List<string> TreeItemsList;

        private static string SubjectFilePath;

        #endregion Private Properties


        #region Properties

        #region ItemsListBoxStringsList

        private static List<string> _ItemsListBoxStringsList = new List<string>();

        public static List<string> ItemsListBoxStringsList
        {
            get { return _ItemsListBoxStringsList; }
            set { _ItemsListBoxStringsList = value; }
        }


        #endregion ItemsListBoxStringsList

        #region SubjectData
        private static string[] _SubjectData;

        public static string[] SubjectData
        {
            get { return _SubjectData; }
            set { _SubjectData = value; }
        }


        #endregion SubjectData

       

        #endregion SubjectName

        #region AlphaBase
        private static int alphaBase = 1;

        public static int AlphaBase
        {
            get { return alphaBase; }
            set { alphaBase = value; }
        }
        #endregion

        #region SubjectName

        private static string _SubjectName;

        public static string SubjectName
        {
            get { return _SubjectName; }
            set { _SubjectName = value; }
        }

        #endregion SubjectName

        #region Properties

        #region SubjectsFolderPath
        private static string subjectsFolderPath;

        public static string SubjectsFolderPath
        {
            get { return subjectsFolderPath; }
            set { subjectsFolderPath = value; }
        }
        #endregion  SubjectsFolderPath 


        #region NumberOfRoots

        private static int _NumberOfRoots;
        public static int NumberOfRoots
        {
            get { return _NumberOfRoots; }
            set { _NumberOfRoots = value; }
        }


        #endregion NumberOfRoots


        #region ItemsDictionary

        private static Dictionary<string,Items> _ItemsDictionary = new Dictionary<string, Items>();

        public static Dictionary<string,Items> ItemsDictionary
        {
            get { return _ItemsDictionary; }
            set { _ItemsDictionary = value; }
        }




        #endregion ItemsDictionary

      

        #endregion Properties

        #region Public Methods

        #region OpenSubjectFile


        public static void OpenSubjectFile()
        {
            // Create path to this subjects  data file
            var SubjectsDataFile = SubjectsFolderPath + SubjectName + ".txt";
            SubjectFilePath = SubjectsDataFile;

            // Test to see if this file exist and if not create it
            if (!File.Exists(SubjectsDataFile))
            {
                // Create a string array to hold the first two lines
                string[] InitialLinesArray = new string[2];

                // the first line is the default AlphaNumberBase string
                AlphaBase = 1;
                InitialLinesArray[0] = AlphaBase.ToString();

                // The initial number of Roots is 0
                NumberOfRoots = 0;
                InitialLinesArray[1] = NumberOfRoots.ToString();

                //Write all of these lines to the SubjectsDataFile
                File.WriteAllLines(SubjectsDataFile, InitialLinesArray);
                return;
                
            }

            // The SubjectsDataFile already exists so read in all of its lines
            string[] SubjectsDataFileStringArray = File.ReadAllLines(SubjectsDataFile);

            List<string> DelimitedDictionaryItems = new List<string>();
            
            // Cycle through SubjectsDataFileStringArray assiginig the to their values
            for (int i =0; i< SubjectsDataFileStringArray.Length; i++)
            {
                // Assign Alpha base
                if (i == 0)
                {
                    AlphaBase = Int32.Parse(SubjectsDataFileStringArray[i]);
                }
                else if (i == 1)
                {
                    NumberOfRoots = Int32.Parse(SubjectsDataFileStringArray[i]);
                }
                else
                {
                    // These are all dictionary lines
                    DelimitedDictionaryItems.Add(SubjectsDataFileStringArray[i]);
                }


            }// END  Cycle through SubjectsDataFileStringArray assiginig the to their values

            // Create a List<string> RootIDs
            List<string> RootIDs = new List<string>();

            // Cycle through DictionaryLinesArray extracting  the properties of each Item object and Add Root ID to
            for (int i = 0; i < DelimitedDictionaryItems.Count; i++)
            {
                // get the next line in DictionaryLinesArray
                var thisLine = DelimitedDictionaryItems[i];

                // create thisItemsPropertyStringsArray to hold the properties of each new item
                string[] thisItemsPropertyStringsArray = thisLine.Split('ɀ');

                // create a new Item object
                Items NewItemObj = new Items();

                // get the leeding character
                char[] leedingCharStringChars = thisItemsPropertyStringsArray[0].ToCharArray();
                char leedingChar = leedingCharStringChars[0];
                NewItemObj.LeedingChar = leedingChar;
                NewItemObj.ItemText = thisItemsPropertyStringsArray[1];
                NewItemObj.ItemID = thisItemsPropertyStringsArray[2];

                // Deteermine if this is a root ID and if so add it to RootIDs
                if(NewItemObj.ItemID.Length == 1)
                {
                    RootIDs.Add(NewItemObj.ItemID);
                }

                NewItemObj.ItemsNumberOfChildren = Int32.Parse(thisItemsPropertyStringsArray[3]);
                NewItemObj.ParentsID = thisItemsPropertyStringsArray[4];
                NewItemObj.ParentsNumberOfChildren = Int32.Parse(thisItemsPropertyStringsArray[5]);
                var TreminalCharStr = thisItemsPropertyStringsArray[6];
                if (TreminalCharStr == "False")
                {
                    NewItemObj.TerminalNode = false;
                }
                else
                {
                    NewItemObj.TerminalNode = true;
                }

                var thisItemsID = NewItemObj.ItemID;
                // Add this item to the ItemsDictionary
                ItemsDictionary.Add(thisItemsID, NewItemObj);
            }// END Cycle through DictionaryLinesArray extracting  the properties of each Item object


            // Cycle through RootIDs creating ItemsListBoxStringsList, a list of root display strings to display upone opening
            foreach (string ID in RootIDs)
            {
                Items NewItemsObject = new Items();
                NewItemsObject = ReturnItemInDictionary(ID);
                var DisplayString = CreateDisplayString(NewItemsObject);
                ItemsListBoxStringsList.Add(DisplayString);
            }// END Cycle through RootIDs creating ItemsListBoxStringsList, a list of root display strings to display upone opening

        }// End OpenSubjectFile()




        #endregion OpenSubjectFile

        #region Open Dictionary File

        public static void OpenDictionaryFile()
        {
            var DictionaryPath = SubjectsFolderPath + "temsDictionaryStrings.txt";
            string[] DicgtionaryArray = File.ReadAllLines(DictionaryPath);
        }

        #endregion Open Dictionary File


        #region SaveSubjectsFile

        public static void SaveSubjectsFile()
        {
            // Create a List<string> OutputSubjectFileList to hold the strings to be written to the Subjects file
            List<string> OutputSubjectFileList = new List<string>();

            // Enter the AlphaBase
            OutputSubjectFileList.Add(AlphaBase.ToString());

            // Enter the Number of Roots
            OutputSubjectFileList.Add(NumberOfRoots.ToString());

           
            //Get each itme in the dictionary and convert its properties to a delimited string to story
            foreach (KeyValuePair<string, Items> kvp in _ItemsDictionary)
            {
                string Key = kvp.Key;
                Items Value = kvp.Value;
                char LeedingChar = Value.LeedingChar;
                string ItemText = Value.ItemText;
                string ItemID = Value.ItemID;
                int ItemsNumberOfChildren = Value.ItemsNumberOfChildren;
                string ItemsNumberOfChildrenString = ItemsNumberOfChildren.ToString();
                string ParentID = Value.ParentsID;
                int ParentsNumberOfChildren = Value.ParentsNumberOfChildren;
                string ParentsNumberOfChildrenString = ParentsNumberOfChildren.ToString();
                bool TerminalNode = Value.TerminalNode;
                string TerminalNodeString = TerminalNode.ToString();
                char D = '\u0240';
                string OutputString = LeedingChar.ToString() + D + ItemText + D + ItemID + D + ItemsNumberOfChildrenString + D + ParentID + D
                    + ParentsNumberOfChildrenString + D + TerminalNodeString;

                OutputSubjectFileList.Add(OutputString);
            }

            var DictionaryFilePath = SubjectsFolderPath + "ItemsDictionaryStrings.txt";


            File.WriteAllLines(SubjectFilePath, OutputSubjectFileList);

           
        }


        #endregion SaveSubjectsFile

        #region ReturnItemInDictionary
        public static Items ReturnItemInDictionary(string Key)
        {
            Items NewItem = ItemsDictionary[Key];
            if (NewItem == null)
            {
                return null;

            }
            return NewItem;
        }

        #endregion ReturnItemInDictionary

        #region TreeLise


       

        
        public static void AddItemToTreeList(string NewItem)
        {
            //If TreeItemsList is null create it
            if (TreeItemsList == null)
            {
                TreeItemsList = new List<string>();
            }
            var LeadingSpacesLength = TreeItemsList.Count * (AlphaBase * 2);
            var LeadingSpacesString = new String(' ', LeadingSpacesLength);
            var StringToAdd = LeadingSpacesString + NewItem;
            TreeItemsList.Add(StringToAdd);
        }


        /// <summary>
        /// Position is Index of string int TreeList
        /// NumberOfChildren is the items new number of children
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="NumberOfChildren"></param>
        /// <returns></returns>
        public static List<string> ReturnUpdatedTreeList(List<string> OldTreeList, int Position, int NumberOfChildren)
        {
            var StringToUpdate = OldTreeList[Position];
            // Update the leading char
            //      Convert the string to an array of characters
            char[] StringToUpdateChars = StringToUpdate.ToCharArray();

            //      Using the position in the List get the position of the leeding char
            int LeedingCharPosition = Position * 2;

            //      Change the leeding char to '+'
            StringToUpdateChars[LeedingCharPosition] = '+';

            // Get Position of  the delimiter before the Number of children
            var PosLastDel = StringToUpdate.LastIndexOf('^');

            // Get the front substring up to the position of the last char
            var FrontString = StringToUpdate.Substring(0, PosLastDel);

            // Convert the number of thildren to a string
            var NumberOfChildrenStr = NumberOfChildren.ToString();

            // Add the NumberOfChildrenStr to the Front string

            var NewString = FrontString + NumberOfChildrenStr;

            // Convert TreeList to string []
            string[] TreeListArray = OldTreeList.ToArray();

            // Replace the item at Position
            TreeListArray[Position] = NewString;

            //Reconstitute the List from this array
            List<string> TreeList = new List<string>();

            foreach(string Item in TreeListArray)
            {
                TreeList.Add(Item);
            }
            return TreeList;

        }

        internal static List<string> ReturnTreeList()
        {
            return TreeItemsList;
        }

        #endregion TreeLise

        #region Return Display strings of Items Children

        internal static List<string> ReturnDisplaystringsOfItemsChildren(string movedItemID)
        {
            // Create a List<string> ChidrensDisplayString
            List<string> ChidrensDisplayStringList = new List<string>();

            // Get the length of the Item associated with this Id
            var ParentsIDLength = movedItemID.Length;

            // Calculate the length of this items childrens IS
            var LengthChildID = ParentsIDLength + AlphaBase;


            // Cycle through the ItemsDictionary looking for Items whose length is
            //   1 AlphaBase longer than movedItemID and whose ID begins with movedItemID
            foreach(KeyValuePair<string, Items> kvp in ItemsDictionary)
            {
                var key = kvp.Key;
                var thisItem = kvp.Value;
                if((thisItem.ItemID.Length == LengthChildID) && (thisItem.ItemID.IndexOf(movedItemID) == 0))
                {
                    // Get the Display string for thisItem
                    var thisDisplayString = CreateDisplayString(thisItem);
                    ChidrensDisplayStringList.Add(thisDisplayString);
                }
            }

            return ChidrensDisplayStringList;


        }



        #endregion Return Display strings of Items Children

        #region ReturnDisplayString

        /// <summary>
        /// Create a display string to show in a list box
        /// </summary>
        /// <param name="LeedingChar"> is a + or - </param>
        /// <param name="Text"></param>
        /// <param name="ID"> the Items AlphaNumber</param>
        /// <param name="NumberOfChildren"> The Items number of Children</param>
        /// <returns></returns>
        public static string CreateDisplayString(Items thisItemsObject)
        {

            //char LeedingChar, string Text, string ID, int NumberOfChildren
            char LeedingChar = thisItemsObject.LeedingChar;
            string Text = thisItemsObject.ItemText;
            string ID = thisItemsObject.ItemID;
            int NumberOfChildren = thisItemsObject.ItemsNumberOfChildren;
            string thisItemsListString;

            int LengthOFItemText = Text.Length;
            int addSpacesNumber = 100 - LengthOFItemText;
            string spacesString = new string(' ', addSpacesNumber);
            if (LeedingChar == '-')
            {
                thisItemsListString = "- " + Text + spacesString + '^' + ID + '^' + NumberOfChildren.ToString();
            }
            else
            {
                thisItemsListString = "+ " + Text + spacesString + '^' + ID + '^' + NumberOfChildren.ToString();
            }


            return thisItemsListString;
        }

        #endregion ReturnDisplayString

        #endregion Public Methods


    }// End Subjects class
}
