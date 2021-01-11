using System;
using System.Collections.Generic;
using System.IO;
using StudyProject.HelperMethods;

namespace StudyProject.DataModels
{



    public static class Subjects
    {

        #region Private Properties


        /// <summary>
        /// This is the path to the file that holds the data about 
        /// the subject that was selected by the user in choosing 
        /// a particular Subject folder in _StudyFolder
        /// </summary>
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

        /// <summary>
        /// 1. Receives  a path to the Subject of Interest Folder
        /// 2. Opens the SubjectName.txt file 
        /// 3. Loads its data into
        ///     a. AlphaBase
        ///     b. NumberOfRoots
        ///     c. ItemsDictionary
        /// 4. Add all of the root items IDs into ItemsListBoxStringsList
        /// </summary>
        public static void OpenSubjectFile()
        {
            // Create path to this subjects  data file
            var SubjectsDataFile = SubjectsFolderPath + SubjectName + ".txt";
            SubjectFilePath = SubjectsDataFile;

            #region Create a SubjectsName.txt file if it doesn't exist
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

            #endregion Create a SubjectsName.txt file if it doesn't exist

            #region Read the lines of the SubjectName.txt file into its holding properties
            // The SubjectsDataFile already exists so read in all of its lines
            string[] SubjectsDataFileStringArray = File.ReadAllLines(SubjectsDataFile);

            List<string> DelimitedDictionaryItems = new List<string>();

            #region Load AlphaBase and Number of Roots and create List of Dictionary Items

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


            #endregion Load AlphaBase and Number of Roots and create List of Dictionary Items

            #region Create ItemsDictionary and a list of RootIDs
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


            #endregion Create ItemsDictionary and a list of RootIDs


            #endregion Read the lines of the SubjectName.txt file into its holding properties

            #region Add all of the RootIDs to ItemsListBoxStringsList
            // Cycle through RootIDs creating ItemsListBoxStringsList, 
            //a list of root display strings to display upone opening
            foreach (string ID in RootIDs)
            {
                Items NewItemsObject = new Items();
                NewItemsObject = ReturnItemInDictionary(ID);
                var DisplayString = CreateDisplayString(NewItemsObject);
                ItemsListBoxStringsList.Add(DisplayString);
            }// END Cycle through RootIDs creating ItemsListBoxStringsList, a list of root display strings to display upone opening

            #endregion Add all of the RootIDs to ItemsListBoxStringsList
        }// End OpenSubjectFile()




        #endregion OpenSubjectFile

        #region SaveSubjectsFile

        /// <summary>
        /// Transfers the data in AlhaBase, NumberOfRoots and _ItemsDictionary
        /// int a List, OutputSubjectFileList, which is 
        /// written to the SubjectName.txt file
        /// </summary>
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

        /// <summary>
        /// The Key is the Item's ID 
        /// The return value is a Items object of the
        /// Item whose key was transmitted
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
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


        #region Return Display strings of Items Children

        /// <summary>
        /// Returns of List of Display String of the Item whose ID was transmitted
        /// It is called by  Click_MoveToTree  method
        /// and by  Click_ShowItemsChildren
        /// </summary>
        /// <param name="movedItemID"></param>
        /// <returns></returns>
        public static List<string> ReturnDisplaystringsOfItemsChildren(string ThisItemID)
        {
            // Create a List<string> ChidrensDisplayString
            List<string> ChidrensDisplayStringList = new List<string>();

            // Get the length of the Item associated with this Id
            var ParentsIDLength = ThisItemID.Length;

            // Calculate the length of this items childrens IS
            var LengthChildID = ParentsIDLength + AlphaBase;


            // Cycle through the ItemsDictionary looking for Items whose length is
            //   1 AlphaBase longer than ThisItemID and whose ID begins with ThisItemID
            foreach(KeyValuePair<string, Items> kvp in ItemsDictionary)
            {
                var key = kvp.Key;
                var thisItem = kvp.Value;
                if((thisItem.ItemID.Length == LengthChildID) && (thisItem.ItemID.IndexOf(ThisItemID) == 0))
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
        /// It receives an Items object and returns a display string to show in a list box
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

        internal static string ReturnDisplayStringID(string DisplayString)
        {
            return StringHelper.ReturnItemAtPos(DisplayString, '^', 1);
        }

        #endregion ReturnDisplayString

        #region UpdateDictionaryItem

        public static void UpdateItemInItemsDictionay(string ID, Items UpdatedItem)
        {
            ItemsDictionary[ID] = UpdatedItem;
        }

        #endregion UpdateDictionaryItem

        #endregion Public Methods


    }// End Subjects class
}
