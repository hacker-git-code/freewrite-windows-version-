using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using FreewriteWindows.Models;
using FreewriteWindows.Services;

namespace FreewriteWindows
{
    public partial class MainWindow : Window
    {
        private bool isDarkMode = false;
        private int currentFontSizeIndex = 1;
        private readonly double[] fontSizes = { 16, 18, 20, 22, 24, 26 };
        private string currentFontFamily = "Segoe UI";
        private DispatcherTimer saveTimer;
        private DispatcherTimer countdownTimer;
        private int timeRemaining = 900; // 15 minutes in seconds
        private bool timerRunning = false;
        private FileService fileService;
        private ObservableCollection<JournalEntry> entries;
        private JournalEntry currentEntry;
        private readonly string[] placeholders = {
            "Begin writing",
            "Pick a thought and go",
            "Start typing",
            "What's on your mind",
            "Just start",
            "Type your first thought",
            "Start with one sentence",
            "Just say it"
        };

        public double CurrentFontSize => fontSizes[currentFontSizeIndex];
        public string CurrentFontFamily => currentFontFamily;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            fileService = new FileService();
            entries = new ObservableCollection<JournalEntry>();
            
            InitializeTheme();
            InitializeTimers();
            LoadEntries();
            SetRandomPlaceholder();
            
            // Auto-save every second
            saveTimer = new DispatcherTimer();
            saveTimer.Interval = TimeSpan.FromSeconds(1);
            saveTimer.Tick += SaveTimer_Tick;
            saveTimer.Start();
        }

        private void InitializeTheme()
        {
            UpdateTheme();
        }

        private void InitializeTimers()
        {
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void SetRandomPlaceholder()
        {
            var random = new Random();
            PlaceholderText.Text = placeholders[random.Next(placeholders.Length)];
        }

        private void LoadEntries()
        {
            var loadedEntries = fileService.LoadAllEntries();
            entries.Clear();
            foreach (var entry in loadedEntries.OrderByDescending(e => e.CreatedDate))
            {
                entries.Add(entry);
            }
            
            EntriesListBox.ItemsSource = entries;

            // Check if we need to create a new entry for today
            var today = DateTime.Today;
            var todayEntry = entries.FirstOrDefault(e => 
                e.CreatedDate.Date == today && string.IsNullOrWhiteSpace(e.PreviewText));

            if (todayEntry != null)
            {
                currentEntry = todayEntry;
                MainTextBox.Text = fileService.LoadEntry(currentEntry);
            }
            else if (entries.Count == 0)
            {
                CreateNewEntry();
            }
            else
            {
                CreateNewEntry();
            }
        }

        private void CreateNewEntry()
        {
            currentEntry = new JournalEntry
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Date = DateTime.Now.ToString("MMM d"),
                PreviewText = ""
            };
            
            entries.Insert(0, currentEntry);
            MainTextBox.Text = "";
            UpdatePlaceholderVisibility();
        }

        private void SaveCurrentEntry()
        {
            if (currentEntry != null && !string.IsNullOrWhiteSpace(MainTextBox.Text))
            {
                fileService.SaveEntry(currentEntry, MainTextBox.Text);
                
                // Update preview
                var preview = MainTextBox.Text
                    .Replace("\n", " ")
                    .Replace("\r", "")
                    .Trim();
                currentEntry.PreviewText = preview.Length > 30 
                    ? preview.Substring(0, 30) + "..." 
                    : preview;
                
                EntriesListBox.Items.Refresh();
            }
        }

        private void UpdateTheme()
        {
            var windowBg = isDarkMode ? new SolidColorBrush(Color.FromRgb(30, 30, 30)) : Brushes.White;
            var textColor = isDarkMode ? new SolidColorBrush(Color.FromRgb(229, 229, 229)) : new SolidColorBrush(Color.FromRgb(51, 51, 51));
            var placeholderColor = isDarkMode ? new SolidColorBrush(Color.FromRgb(128, 128, 128)) : new SolidColorBrush(Color.FromRgb(180, 180, 180));
            var sidebarBg = isDarkMode ? new SolidColorBrush(Color.FromRgb(25, 25, 25)) : new SolidColorBrush(Color.FromRgb(250, 250, 250));

            Resources["WindowBackground"] = windowBg;
            Resources["TextColor"] = textColor;
            Resources["PlaceholderColor"] = placeholderColor;
            Resources["SidebarBackground"] = sidebarBg;
            Resources["GrayText"] = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        }

        private void UpdatePlaceholderVisibility()
        {
            PlaceholderText.Visibility = string.IsNullOrWhiteSpace(MainTextBox.Text) 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        private void UpdateTimerDisplay()
        {
            int minutes = timeRemaining / 60;
            int seconds = timeRemaining % 60;
            TimerButton.Content = $"{minutes}:{seconds:D2}";
        }

        // Event Handlers
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized 
                    ? WindowState.Normal 
                    : WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentEntry();
            Close();
        }

        private void MainTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void MainTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void FontSizeButton_Click(object sender, RoutedEventArgs e)
        {
            currentFontSizeIndex = (currentFontSizeIndex + 1) % fontSizes.Length;
            MainTextBox.FontSize = CurrentFontSize;
            PlaceholderText.FontSize = CurrentFontSize;
            FontSizeButton.Content = $"{(int)CurrentFontSize}px";
        }

        private void SegoeFontButton_Click(object sender, RoutedEventArgs e)
        {
            currentFontFamily = "Segoe UI";
            MainTextBox.FontFamily = new FontFamily(currentFontFamily);
            PlaceholderText.FontFamily = new FontFamily(currentFontFamily);
        }

        private void ArialFontButton_Click(object sender, RoutedEventArgs e)
        {
            currentFontFamily = "Arial";
            MainTextBox.FontFamily = new FontFamily(currentFontFamily);
            PlaceholderText.FontFamily = new FontFamily(currentFontFamily);
        }

        private void ConsolasFontButton_Click(object sender, RoutedEventArgs e)
        {
            currentFontFamily = "Consolas";
            MainTextBox.FontFamily = new FontFamily(currentFontFamily);
            PlaceholderText.FontFamily = new FontFamily(currentFontFamily);
        }

        private void TimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!timerRunning)
            {
                timerRunning = true;
                countdownTimer.Start();
            }
            else
            {
                timerRunning = false;
                countdownTimer.Stop();
                timeRemaining = 900;
                UpdateTimerDisplay();
            }
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (timeRemaining > 0)
            {
                timeRemaining--;
                UpdateTimerDisplay();
            }
            else
            {
                countdownTimer.Stop();
                timerRunning = false;
                MessageBox.Show("Time's up!", "Freewrite", MessageBoxButton.OK, MessageBoxImage.Information);
                timeRemaining = 900;
                UpdateTimerDisplay();
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            Sidebar.Visibility = Sidebar.Visibility == Visibility.Visible 
                ? Visibility.Collapsed 
                : Visibility.Visible;
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;
            UpdateTheme();
        }

        private void NewEntryButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentEntry();
            CreateNewEntry();
        }

        private void EntriesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (EntriesListBox.SelectedItem is JournalEntry selectedEntry)
            {
                SaveCurrentEntry();
                currentEntry = selectedEntry;
                MainTextBox.Text = fileService.LoadEntry(currentEntry);
                UpdatePlaceholderVisibility();
            }
        }

        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            SaveCurrentEntry();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveCurrentEntry();
            base.OnClosing(e);
        }
    }
}