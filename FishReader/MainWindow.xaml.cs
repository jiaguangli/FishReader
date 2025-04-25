using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using FishReader.Json;
using Microsoft.Win32;
using Application = System.Windows.Forms.Application;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace FishReader
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Items { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // 初始化列表项
            Items = new ObservableCollection<string>
            {
               
            };

            var config = Config.GetConfig().Path.Select(x => x.Key);
            foreach (var c in config)
            {
                Items.Add(c);
            }
            // 设置数据上下文
            DataContext = Items;
            
            
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {

            var dig = new OpenFileDialog();
            dig.ShowDialog();
            Items.Add(dig.FileName);
            var config = Config.GetConfig();
            config.Path.Add(dig.FileName,0);
            config.Save();

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // var config = new Config();
            // config.Path.Add("aaa",123);
            // config.Path.Add("bbb",123);
            // config.Save();
            // return;
            // 删除选中的项

            var config = Config.GetConfig();
            
            var selectedItem = itemListBox.SelectedItem as string;
            if (selectedItem != null)
            {
                config.Path.Remove(selectedItem);
                Items.Remove(selectedItem);
                
                config.Save();
            }
        }

        // 添加双击命令支持
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (itemListBox.SelectedItem != null)
            {
                var config = Config.GetConfig();
                // var dic = new Dictionary<string, int>();
                var index = config.Path[itemListBox.SelectedItems[0].ToString()];
                
                var window = new ReaderWindow(itemListBox.SelectedItem.ToString());
                window.Show();
                // MessageBox.Show($"Double clicked on: {itemListBox.SelectedItem}");
            }
        }
    }
}