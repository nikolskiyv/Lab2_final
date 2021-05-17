using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ClassLibrary;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private V5MainCollection MainCollection = new V5MainCollection();
        BindDataOnGrid bind;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainCollection;
            bind = new BindDataOnGrid(ref MainCollection);
            AddCustomGrid.DataContext = bind;
        }

        public static RoutedCommand AddCustomDoG = new RoutedCommand("Add", typeof(WpfApp1.MainWindow));

        private void Add_FromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog Dialog = new Microsoft.Win32.OpenFileDialog();
                if ((bool)Dialog.ShowDialog())
                    MainCollection.AddFromFile(Dialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add From File error: " + ex.Message);
            }
            finally
            {
                ErrorMsg();
            }
        }

        private void AddDefault_V5DataCollection_Click(object sender, RoutedEventArgs e)
        {
            MainCollection.AddDefaultDataCollection();
            ErrorMsg();
        }

        private void AddDefaults_Click(object sender, RoutedEventArgs e)
        {
            MainCollection.AddDefaults();
            DataContext = MainCollection;
            ErrorMsg();
        }

        private void AddDefault_V5DataOnGrid_Click(object sender, RoutedEventArgs e)
        {
            MainCollection.AddDefaultDataOnGrid();
            ErrorMsg();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (MainCollection.IsChanged)
            {
                UnsavedChanges();
            } 
            MainCollection = new V5MainCollection();
            DataContext = MainCollection;
            bind = new BindDataOnGrid(ref MainCollection);
            AddCustomGrid.DataContext = bind;
            ErrorMsg();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainCollection.IsChanged)
                {
                    UnsavedChanges();
                }
                Microsoft.Win32.OpenFileDialog fd = new Microsoft.Win32.OpenFileDialog();
                if ((bool)fd.ShowDialog())
                {
                    MainCollection = new V5MainCollection();
                    MainCollection.Load(fd.FileName);
                    DataContext = MainCollection;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loading Error: " + ex.Message);
            }
            finally
            {
                ErrorMsg();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                if ((bool)dialog.ShowDialog())
                    MainCollection.Save(dialog.FileName);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Saving Error: " + ex.Message);
            }
            finally
            {
                ErrorMsg();
            }
        }

        private bool UnsavedChanges()
        {
            MessageBoxResult Message = MessageBox.Show("Save Changes?", "Save", MessageBoxButton.YesNoCancel);
            if (Message == MessageBoxResult.Yes)
            {
                Microsoft.Win32.SaveFileDialog Dialog = new Microsoft.Win32.SaveFileDialog();
                if ((bool)Dialog.ShowDialog())
                    MainCollection.Save(Dialog.FileName);
            }
            else if (Message == MessageBoxResult.Cancel)
            {
                return true;
            }
            return false;
        }

        private void AppClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainCollection.IsChanged)
            {
                e.Cancel = UnsavedChanges();
            }
            ErrorMsg();
        }

        public void ErrorMsg()
        {
            if (MainCollection.ErrorMessage != null)
            {
                MessageBox.Show(MainCollection.ErrorMessage, "Error");
                MainCollection.ErrorMessage = null;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var selectedLB = listBox_Main.SelectedItems;
            List<V5Data> Items = new List<V5Data>();
            Items.AddRange(selectedLB.Cast<V5Data>());
            foreach (V5Data item in Items)
            {
                MainCollection.Remove(item.Info, item.Time);
            }
            ErrorMsg();
        }

        private void FilterDataCollection(object sender, FilterEventArgs args)
        {
            var item = args.Item;
            if (item != null)
            {
                if (item.GetType() == typeof(V5DataCollection)) args.Accepted = true;
                else args.Accepted = false;
            }
        }

        private void FilterDataOnGrid(object sender, FilterEventArgs args)
        {
            var item = args.Item;
            if (item != null)
            {
                if (item.GetType() == typeof(V5DataOnGrid)) args.Accepted = true;
                else args.Accepted = false;
            }
        }

        private void LB_DoG_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void OpenHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (MainCollection.IsChanged)
            {
                UnsavedChanges();
            }
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            if ((bool)dialog.ShowDialog())
            {
                MainCollection = new V5MainCollection();
                MainCollection.Load(dialog.FileName);
                DataContext = MainCollection;
            }
            ErrorMsg();
        }

        private void SaveHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            if ((bool)dialog.ShowDialog())
                MainCollection.Save(dialog.FileName);
            ErrorMsg();
        }

        private void CanSaveHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainCollection.IsChanged;
        }

        private void DeleteHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var Selection = listBox_Main.SelectedItems;
            List<V5Data> selectedItems = new List<V5Data>();
            selectedItems.AddRange(Selection.Cast<V5Data>());
            foreach (V5Data item in selectedItems)
            {
                MainCollection.Remove(item.Info, item.Time);
            }
        }

        private void CanDeleteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (listBox_Main != null)
            {
                var Selection = listBox_Main.SelectedItems;
                List<V5Data> selectedItems = new List<V5Data>();
                selectedItems.AddRange(Selection.Cast<V5Data>());
                if (selectedItems.Count != 0)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
        }

        private void AddDataOnGridHandler(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                bind.Add();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void CanAddDataOnGridHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TextBox_Size == null || TextBox_Xnum == null ||
                TextBox_Ynum == null || TextBox_DGstr == null)
            {
                e.CanExecute = false;
            }
            else if (Validation.GetHasError(TextBox_Size) || Validation.GetHasError(TextBox_Xnum) ||
                     Validation.GetHasError(TextBox_Ynum) || Validation.GetHasError(TextBox_DGstr))
            {
                e.CanExecute = false;
            }
            else
                e.CanExecute = true;
        }

        private void listBox_Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
