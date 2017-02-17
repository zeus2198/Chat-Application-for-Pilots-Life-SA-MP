using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Media.Effects;
using Xceed.Wpf.Toolkit;
using System.Collections.ObjectModel;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for settings_window.xaml
    /// </summary>
    public partial class settings_window : Window
    {
        private ObservableCollection<nickalert> items = new ObservableCollection<nickalert>();
        public settings_window()
        {
            InitializeComponent();
            logCheckBox.IsChecked = ((App)Application.Current).chatlogging;
            fetchBox.Text = ((App)Application.Current).fetchsec.ToString();            
            nickListGrid.ItemsSource = items;
            foreach (var x in ((App)Application.Current).nicks)         
            {             
                items.Add(new nickalert { name = x.name, color = x.color, activated = x.activated });
            }
            
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //draggin window movement
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
            Keyboard.ClearFocus();            
        }
        private void clo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private static bool IsTextAllowed(string txt)
        {
            int n;
            return int.TryParse(txt, out n);
        }

        private void fetchBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (IsTextAllowed(e.Text))
            {
                e.Handled = false;
                fetchBox.Effect =
                    new DropShadowEffect
                    {
                        Direction = 270,
                        ShadowDepth = 2,
                        BlurRadius = 4,
                        Color = Colors.Yellow
                    };

            }
            else
            {
                e.Handled = true;
                fetchBox.Effect =
                   new DropShadowEffect
                   {
                       Direction = 270,
                       ShadowDepth = 2,
                       BlurRadius = 4,
                       Color = Colors.Red
                   };
            }
        }
        private void fetchSave(object sender, RoutedEventArgs e)
        {
            short n;
            if (!short.TryParse(fetchBox.Text, out n))
            {
                fetchBox.Effect =
                  new DropShadowEffect
                  {
                      Direction = 270,
                      ShadowDepth = 2,
                      BlurRadius = 4,
                      Color = Colors.Red
                  };
                return;
            }
            ((App)Application.Current).fetchsec = n;
            ((App)Application.Current).SaveSettings();
            fetchBox.ClearValue(EffectProperty);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).chatlogging = false;
            ((App)Application.Current).SaveSettings();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).chatlogging = true;
            ((App)Application.Current).SaveSettings();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (nickListGrid.SelectedItem == null) System.Windows.MessageBox.Show("No row Selected!");
            else
            {
                ((App)Application.Current).nicks.RemoveAt(nickListGrid.Items.IndexOf(nickListGrid.SelectedItem));
                ((App)Application.Current).SaveSettings();
                items.RemoveAt(nickListGrid.Items.IndexOf(nickListGrid.SelectedItem));
            }
        }
        private void AddNewNick(object sender, RoutedEventArgs e)
        {
            nickalert ob = new nickalert { name = "New NickAlert", color = "#00f0f0", activated = false };
            items.Add(ob);
            ((App)Application.Current).nicks.Add(ob);
            ((App)Application.Current).SaveSettings();
        }
        private DataGridRow GetRow(DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
        private T FindVisualChild<T>(DependencyObject item)
        where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(item);
            var result = item as T;
            //the for-loop contains a null check; we stop when we find the result. 
            //so the stop condition for this method is embedded in the initialization
            //of the result variable.
            for (int i = 0; i < childCount && result == null; i++)
            {
                result = FindVisualChild<T>(VisualTreeHelper.GetChild(item, i));
            }
            return result;
        }

        private void SaveNicks(object sender, RoutedEventArgs e)
        {
            nickalert ob;
            ((App)Application.Current).nicks.Clear();
            for (int i = 0; i < nickListGrid.Items.Count; i++)
            {
                Color c = (Color)FindVisualChild<ColorPicker>(nickListGrid.Columns[1].GetCellContent(GetRow(nickListGrid, i))).SelectedColor;
                   ob = new nickalert
                   {
                       name = FindVisualChild<TextBox>(nickListGrid.Columns[0].GetCellContent(GetRow(nickListGrid, i))).Text,
                       color = string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B),
                       activated = (bool)FindVisualChild<CheckBox>(nickListGrid.Columns[2].GetCellContent(GetRow(nickListGrid, i))).IsChecked
                   };
                  ((App)Application.Current).nicks.Add(ob);
            }            
            ((App)Application.Current).SaveSettings();
            System.Windows.MessageBox.Show("Settings Saved!");
        }
    }
}
