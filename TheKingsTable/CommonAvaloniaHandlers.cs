using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;

namespace TheKingsTable
{
    public static class CommonAvaloniaHandlers
    {
        static List<FileDialogFilter> PrepareFilters(FileSelection input)
        {
            var filters = new List<FileDialogFilter>(input.Filters.Count / 2);
            foreach (var filter in input.Filters)
            {
                filters.Add(new FileDialogFilter()
                {
                    Name = filter.Item1,
                    Extensions = new List<string>() { filter.Item2 }
                });
            }
            return filters;
        }
        public static async Task ShowOpenFileBrowser(InteractionContext<FileSelection, string?> context, Window parent)
        {
            var o = new OpenFileDialog()
            {
                Title = context.Input.Title,
                AllowMultiple = false,
                Directory = context.Input.Start,
            };
            o.Filters = PrepareFilters(context.Input);
            
            var result = await o.ShowAsync(parent);
            //cancelling the dialog returns null on windows
            //but I think it returns empty array on mac...?
            context.SetOutput(result?.Length > 0 ? result[0] : null);
        }

        public static async Task ShowSaveFileBrowser(InteractionContext<FileSelection, string?> context, Window parent)
        {
            var s = new SaveFileDialog()
            {
                Title = context.Input.Title,
                Directory = context.Input.Start
            };
            s.Filters = PrepareFilters(context.Input);

            var result = await s.ShowAsync(parent);
            context.SetOutput(result);
        }

        public static async Task ShowFolderBrowser(InteractionContext<FolderSelection, string?> context, Window parent)
        {
            var o = new OpenFolderDialog()
            {
                Title = context.Input.Title,
                Directory = context.Input.Start
            };
            var result = await o.ShowAsync(parent);
            context.SetOutput(result);
        }

        public static async Task ShowYesNoMessage(InteractionContext<Words, bool> context, Window parent)
        {
            var m = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ButtonDefinitions = MessageBox.Avalonia.Enums.ButtonEnum.YesNo,
                ContentTitle = context.Input.Title,
                ContentMessage = context.Input.Question
            });
            var res = await m.ShowDialog(parent);
            context.SetOutput(res == MessageBox.Avalonia.Enums.ButtonResult.Yes);
        }
    }
}
