using System;
using System.Collections.Generic;
using System.IO;
using StudyProject.HelperMethods;

namespace StudyProject.DataModels
{



    public static class Subjects
    {



        #region Properties

        #region SubjectData
        private static string[] _SubjectData;

        public static string[] SubjectData
        {
            get { return _SubjectData; }
            set { _SubjectData = value; }
        }


        #endregion SubjectData

        #region SubjectName

        private static string _SubjectName;

        public static string SubjectName
        {
            get { return _SubjectName; }
            set { _SubjectName = value; }
        }

        #endregion SubjectName

        #region AlphaBase
        private static int alphaBase = 1;

        public static int AlphaBase
        {
            get { return alphaBase; }
            set { alphaBase = value; }
        }
        #endregion


        #region DataFolderPath
        private static string dataFolderPath;

        public static string DataFolderPath
        {
            get { return dataFolderPath; }
            set { dataFolderPath = value; }
        }
        #endregion  DataFolderPath 


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

        #region TreeItemsList

        //private static List<string> _TreeItemsList;

        //public static List<string> TreeItemsList
        //{
        //    get { return _TreeItemsList; }
        //    set { _TreeItemsList = value; }
        //}



        #endregion TreeItemsList

        #endregion Properties

        #region Public Methods

        #region Open Data Files

        public static void OpenDataFiles(string FilePath)
        {
            // Read the file as one string.
            string text = File.ReadAllText(FilePath);

            // Read each line of the file into a string array. Each element
            // of the array is one line of the file.
            string[] lines = System.IO.File.ReadAllLines(FilePath);

            string[] SubjectDataStringArray = new string[3];
            // Place these lines in the SubjectData string array
            SubjectDataStringArray[0] = lines[0];
            SubjectDataStringArray[1] = lines[1];
            SubjectDataStringArray[2] = lines[2];

            // Set SubjectData to this array
            SubjectData = SubjectDataStringArray;

            //Get the alpha base and send it to the Subject class
            var AlphaBaseString = StringHelper.ReturnItemAtPos(lines[0], '^', 1);

            var AlphaBase = Int32.Parse(AlphaBaseString);
            Subjects.AlphaBase = AlphaBase;

            SubjectName = StringHelper.ReturnItemAtPos(lines[1], '^', 1);
           //tbkMessage.Text = SubjectName;

            // Get the number of root items and send it to the subjects folder
            var NumberOfRootsStr = StringHelper.ReturnItemAtPos(lines[2], '^', 1);

            // Get the Item ID
            var NumberOfRoots = Int32.Parse(NumberOfRootsStr);
            Subjects.NumberOfRoots = NumberOfRoots;

            // Create a TreeList in the Subjects object to manage additions to lbxTree
            Subjects.CreateTreeList();
        }

        internal static void SaveSubjectDataFile()
        {
            var DataFilePath = DataFolderPath + SubjectName + " Data" + ".txt";
            File.WriteAllLines(DataFilePath, SubjectData);
        }
        #endregion Open Data Files

        #region Open Dictionary File

        public static void OpenDictionaryFile()
        {
            var DictionaryPath = DataFolderPath + "temsDictionaryStrings.txt";
            string[] DicgtionaryArray = File.ReadAllLines(DictionaryPath);
        }

        #endregion Open Dictionary File


        #region SaveDataDictionary

        public static void SaveDataDictionary()
        {
            // Create a list of strings to hold the data in the Items Dictionary
            List<string> ItemsDistionaryStringsList = new List<string>();


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

                ItemsDistionaryStringsList.Add(OutputString);
            }

            var DictionaryFilePath = DataFolderPath + "ItemsDictionaryStrings.txt";


            File.WriteAllLines(DictionaryFilePath, ItemsDistionaryStringsList);
        }


        #endregion SaveDataDictionary

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


        // Construct a new TreeList

       public static List<string> TreeList;

        /// <summary>
        /// construct a new TreeList
        /// </summary>
       public static void CreateTreeList()
        {
            TreeList = new List<string>();
        }

        public static List<string> ReturnTreeList()
        {
            return TreeList;
        }

        // Add and Item to the TreeList
        public static void AddItemToTreeList(string NewItem)
        {
            var LeadingSpacesLength = TreeList.Count *2;
            var LeadingSpacesString = new String(' ', LeadingSpacesLength);
            var StringToAdd = LeadingSpacesString + NewItem;
            TreeList.Add(StringToAdd);
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

        #endregion TreeLise

        #endregion Public Methods


    }// End Subjects class
}
