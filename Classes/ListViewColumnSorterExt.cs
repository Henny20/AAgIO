using System;
using System.Collections;
using Avalonia.Controls;


namespace AAgIO.Classes
{

       public class ListViewColumnSorterExt
    {
    
    
    
                 /**/ /// <summary> ///  Negeer de IComparer-interface. /// </summary> /// <param name="x"> Het eerste object dat moet worden vergeleken </param> /// < param name="y"> Het tweede te vergelijken object </param> /// <returns> Het resultaat van de vergelijking. Retourneert 0 indien gelijk, retourneert 1 als x groter is dan y, en retourneert -1 als x is kleiner dan y </ retourneert>
                 
                 /***********
        public int Compare(object x, object y)
        {
            int compareResult;
            ListBoxItem listviewX, listviewY;

              //  Converteer het vergelijkingsobject naar een ListViewItem-object      
              listviewX = (ListBoxItem)x;
              listviewY = (ListBoxItem)y;

              int a=0, b=0;
              if (int.TryParse(listviewX.SubItems[ColumnToSort].Text, out a) && int.TryParse(listviewY.SubItems[ColumnToSort].Text, out b))
              {
                  compareResult = ObjectCompare.Compare(a, b);
              }
              else
              {
                  compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text.Trim(), listviewY.SubItems[ColumnToSort].Text.Trim());
              }



             //  Retourneert het juiste vergelijkingsresultaat volgens het bovenstaande vergelijkingsresultaa
            if (OrderOfSort == SortOrder.Ascending)
            {
                //  Omdat het in positieve volgorde is gesorteerd, geeft u het resultaat direct terug
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                //  Als het in omgekeerde volgorde is gesorteerd, neem dan een negatieve waarde en retourneer 
                return (-compareResult);
            }
            else
            {
                //  Als ze gelijk zijn, retourneer dan 0
                return 0;
            }
        }
        ***************/
   /************

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        private ListView listView;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorterExt(ListView lv)
        {
            listView = lv;
            listView.ListViewItemSorter = this;
            listView.ColumnClick += new ColumnClickEventHandler(listView_ColumnClick);

            // Initialize the column to '0'
            SortColumn = 0;

            // Initialize the sort order to 'none'
            Order = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        private int SortColumn { set; get; }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        private SortOrder Order { set; get; }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ReverseSortOrderAndSort(e.Column, (ListView)sender);
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            if (decimal.TryParse(listviewX.SubItems[SortColumn].Text, out decimal dx) && decimal.TryParse(listviewY.SubItems[SortColumn].Text, out decimal dy))
            {
                //compare the 2 items as doubles
                compareResult = decimal.Compare(dx, dy);
            }
            else if (DateTime.TryParse(listviewX.SubItems[SortColumn].Text, out DateTime dtx) && DateTime.TryParse(listviewY.SubItems[SortColumn].Text, out DateTime dty))
            {
                //compare the 2 items as doubles
                compareResult = DateTime.Compare(dtx, dty);
            }
            // When one is a number and the other not, return -1 to have the numbers on top (or bottom)
            else if (decimal.TryParse(listviewX.SubItems[SortColumn].Text, out dx))
            {
                compareResult = -1;
            }
            // When one is a number and the other not, return 1 to have the numbers on top (or bottom)
            else if (decimal.TryParse(listviewY.SubItems[SortColumn].Text, out dy))
            {
                compareResult = 1;
            }
            else
            {
                // Compare the two items
                compareResult = ObjectCompare.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);
            }

            // Calculate correct return value based on object comparison
            if (Order == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (Order == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        private void ReverseSortOrderAndSort(int column, ListView lv)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (column == SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (Order == SortOrder.Ascending)
                {
                    Order = SortOrder.Descending;
                }
                else
                {
                    Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                SortColumn = column;
                Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lv.Sort();
        }
        ***********/
    }
}
