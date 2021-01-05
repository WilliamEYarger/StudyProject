using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyProject.DataModels
{

    /// <summary>
    /// Constructs an item object which will contain all of the properties of 
    /// a subject
    /// </summary>
    public class Items
    {

        #region Properties


        #region LeedingChar
        private char _LeedingChar = '-';
        /// <summary>
        /// The leading char indicates whether this subject has children ('+')
        /// or not ('-')
        /// </summary>
        public char LeedingChar
        {
            get { return _LeedingChar; }
            set { _LeedingChar = value; }
        }
        #endregion

        #region ItemText
        private string _ItemText;
        /// <summary>
        /// Contains the text that the user types into the tbxItemText TextBox
        /// </summary>
        public string ItemText
        {
            get { return _ItemText; }
            set { _ItemText = value; }
        }
        #endregion ItemText

        #region ItemID
        private string _ItemID;
        /// <summary>
        /// The ItemID is an Alpha nuber which is composed of
        /// the Item's Parents ID but the Alpha number reflecting
        /// this items position if its parents children
        /// </summary>
        public string ItemID
        {
            get { return _ItemID; }
            set { _ItemID = value; }
        }

        #endregion ItemID

        #region ItemsNumberOfChildren

        private int _ItemsNumberOfChildren;
        public int ItemsNumberOfChildren
        {
            get { return _ItemsNumberOfChildren; }
            set { _ItemsNumberOfChildren = value; }
        }

        #endregion ItemsNumberOfChildren

        #region ParentsID

        /// <summary>
        private string _ParentsID;
        ///  The ParentsID is an alpha number reflecting the parents 
        ///  parent and parents number of children
        /// </summary>
        public string ParentsID
        {
            get { return _ParentsID; }
            set { _ParentsID = value; }
        }

        #endregion ParentsID

        #region ParentsNumberOfChildren
        private int _ParentsNumberOfChildren;
        public int ParentsNumberOfChildren
        {
            get { return _ParentsNumberOfChildren; }
            set { _ParentsNumberOfChildren = value; }
        }


        #endregion ParentsNumberOfChildren

        #region TerminalNode
        private bool _TerminalNode;
        /// <summary>
        /// A boolean value reflecting wheter the node is a terminal node or not
        /// </summary>
        public bool TerminalNode
        {
            get { return _TerminalNode; }
            set { _TerminalNode = value; }
        }

        #endregion TerminalNode

        #endregion Properties

       
    }// End Items
}
