using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FishReader.Json;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace FishReader
{
    public partial class ReaderWindow : Window
    {
        private List<string> _pages = new List<string>(); // 存储分页后的文件内容
        private int _currentPageIndex = -1; // 当前页的索引
        private string _filePath;
        
        private NotifyIcon _notifyIcon;

        public ReaderWindow(string filePath)
        {

            _filePath = filePath;
            var config = Config.GetConfig();
            InitializeComponent();
            InitializeTrayIcon();

            this.ContentTextBox.FontSize = config.Setting.FontSize;
            // 读取文件内容并按字符数分页
            LoadFileAndSplitIntoPages(filePath);
            _currentPageIndex = config.Path[filePath];
            // 显示第一页的内容
            ShowCurrentPage();
        }

        protected override void OnClosed(EventArgs e)
        {
            _notifyIcon.Dispose(); 

            var config = Config.GetConfig();
            config.Path[_filePath] = this._currentPageIndex;
            config.Save();
            base.OnClosed(e);
        }

        private void ContentTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_isMouseOverTextBox)
            {
                if (e.Delta > 0) // 向上滚动
                {
                    PreviousPage_Click(sender, e);
                }
                else if (e.Delta < 0) // 向下滚动
                {
                    NextPage_Click(sender, e);
                }

                // 阻止事件冒泡，防止滚动影响其他控件
                e.Handled = true;
            }
        }

        private void ContentTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 阻止文本选择
            ContentTextBox.Focus(); // 保持焦点
            e.Handled = true;       // 阻止事件冒泡
        }

        private void ContentTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            var config = Config.GetConfig();
            config.Path[_filePath] = _currentPageIndex;
            config.Save();
        }

        private bool _isMouseOverTextBox = false;

        private void ContentTextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOverTextBox = true;

            // 设置文本为不透明
            ContentTextBox.Foreground = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(255, 0, 0, 0)); // 完全不透明的黑色
        }

        private void ContentTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOverTextBox = false;

            // 设置文本为透明
            ContentTextBox.Foreground = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(0, 0, 0, 0)); // 完全透明的黑色
        }



        // 拖动窗口的事件处理
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove(); // 使用 DragMove 方法来移动窗口
            }
        }

        // 关闭按钮点击事件
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // 关闭窗口
        }

        // 读取文件内容并按字符数分页
        private void LoadFileAndSplitIntoPages(string filePath)
        {
            try
            {
                // 读取整个文件内容
                string fileContent = File.ReadAllText(filePath);

                var setting = Setting.GetSetting();
                // 将文件内容按字符数分割
                for (int i = 0; i < fileContent.Length; i += setting.FontNumber)
                {
                    int endIndex = Math.Min(i + setting.FontNumber, fileContent.Length);
                    string pageContent = fileContent.Substring(i, endIndex - i);
                    _pages.Add(pageContent);
                }

                // 设置当前页索引为第一页
                _currentPageIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取文件时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 上一页按钮点击事件
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex > 0)
            {
                _currentPageIndex--; // 移动到上一页
                ShowCurrentPage();
            }
            else
            {
                MessageBox.Show("这已经是第一页了。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        // 初始化托盘图标
        private void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("favicon.ico"), // 替换为您的图标文件路径
                Visible = true,
                Text = "ReaderWindow"
            };

            // 添加右键菜单
            var contextMenu = new ContextMenuStrip();
            var showMenuItem = new ToolStripMenuItem("显示窗口");
            var exitMenuItem = new ToolStripMenuItem("退出");

            showMenuItem.Click += (s, e) => RestoreWindow();
            exitMenuItem.Click += (s, e) => ExitApplication();

            contextMenu.Items.Add(showMenuItem);
            contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenu;

            // 双击托盘图标恢复窗口
            _notifyIcon.DoubleClick += (s, e) => RestoreWindow();
        }

        // 恢复窗口
        private void RestoreWindow()
        {
            ContentTextBox.Foreground = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(255, 0, 0, 0)); // 完全不透明的黑色
            this.Show();
            this.WindowState = WindowState.Normal;
            _notifyIcon.Visible = false; // 隐藏托盘图标
        }

        // 退出应用程序
        private void ExitApplication()
        {
            _notifyIcon.Visible = false; // 隐藏托盘图标
            this.Close();
            System.Windows.Application.Current.Shutdown(); // 关闭应用程序
        }

        // 处理窗口状态变化
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide(); // 隐藏窗口
                _notifyIcon.Visible = true; // 显示托盘图标
            }

            base.OnStateChanged(e);
        }
        

        // 下一页按钮点击事件
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex < _pages.Count - 1)
            {
                _currentPageIndex++; // 移动到下一页
                ShowCurrentPage();
            }
            else
            {
                MessageBox.Show("这已经是最末页了。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // 显示当前页的内容
        private void ShowCurrentPage()
        {
            if (_currentPageIndex >= 0 && _currentPageIndex < _pages.Count)
            {
                ContentTextBox.Text = _pages[_currentPageIndex];
            }
        }
    }
}