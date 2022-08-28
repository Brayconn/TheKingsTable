using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;

namespace TheKingsTable.Controls
{
    public partial class FileSystemSelector : UserControl
    {
        #region Text Styled Property
        public static readonly StyledProperty<string> TextProperty =
            //TextBox.TextProperty.AddOwner<FileSystemSelector>();
            AvaloniaProperty.Register<FileSystemSelector, string>(nameof(Text));
        public string Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
        #endregion

        #region StartDirectory Styled Property
        public static readonly StyledProperty<string> StartDirectoryProperty =
            AvaloniaProperty.Register<FileSystemSelector, string>(nameof(StartDirectory));
        public string StartDirectory { get => GetValue(StartDirectoryProperty); set => SetValue(StartDirectoryProperty, value); }
        #endregion

        #region Watermark Styled Property
        public static readonly StyledProperty<string> WatermarkProperty =
            TextBox.WatermarkProperty.AddOwner<FileSystemSelector>();
            //AvaloniaProperty.Register<FileSystemSelector, string>(nameof(Watermark));
        public string Watermark { get => GetValue(WatermarkProperty); set => SetValue(WatermarkProperty, value); }
        #endregion

        #region Description Styled Property
        public static readonly StyledProperty<string> DescriptionProperty =
            AvaloniaProperty.Register<FileSystemSelector, string>(nameof(Description));
        public string Description { get => GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }
        #endregion

        #region WindowTitle Styled Property
        public static readonly StyledProperty<string> WindowTitleProperty =
            AvaloniaProperty.Register<FileSystemSelector, string>(nameof(WindowTitle));
        public string WindowTitle { get => GetValue(WindowTitleProperty); set => SetValue(WindowTitleProperty, value); }
        #endregion

        #region Filters Styled Properties
        public static readonly StyledProperty<string> FiltersProperty =
            AvaloniaProperty.Register<FileSystemSelector, string>(nameof(Filters));
        public string Filters { get => GetValue(FiltersProperty); set => SetValue(FiltersProperty, value); }
        #endregion

        #region IsSave Styled Property
        public static readonly StyledProperty<bool> IsSaveProperty =
            AvaloniaProperty.Register<FileSystemSelector, bool>(nameof(IsSave), false);

        public bool IsSave
        {
            get => GetValue(IsSaveProperty);
            set => SetValue(IsSaveProperty, value);
        }
        #endregion

        public FileSystemSelector()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void OnBrowse(object sender, RoutedEventArgs e)
        {
            string? result = null;

            string start;
            if (!string.IsNullOrWhiteSpace(Text))
                start = Path.GetDirectoryName(Text) ?? "";
            else
                start = StartDirectory;

            //filters means we're finding files
            if (!string.IsNullOrEmpty(Filters))
            {
                var splitFilters = Filters.Split("|");
                var formattedFilters = new List<Tuple<string, string>>(splitFilters.Length / 2);
                for (int i = 0; i < splitFilters.Length; i += 2)
                    formattedFilters.Add(new Tuple<string, string>(splitFilters[i], splitFilters[i + 1]));

                var interaction = IsSave ? CommonInteractions.BrowseToSaveFile : CommonInteractions.BrowseToOpenFile;

                result = await interaction.Handle(new FileSelection(WindowTitle, formattedFilters, start));
            }
            //none means we're finding directories
            else
            {
                result = await CommonInteractions.BrowseForFolder
                        .Handle(new FolderSelection(WindowTitle, start));
            }
            if (result != null)
            {
                Text = result;
            }
        }
    }
}
