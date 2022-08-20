using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace TheKingsTable
{
    public class FileSelection : FolderSelection
    {
       public List<Tuple<string,string>> Filters { get; }
        public FileSelection(string title, List<Tuple<string,string>> filters, string start = "") : base(title, start)
        {
            Filters = filters;
        }
    }
    public class FolderSelection
    {
        public string Title { get; }
        public string Start { get; set; }
        public FolderSelection(string title, string start = "")
        {
            Title = title;
            Start = start;
        }
    }
    public class Words
    {
        public string Title { get; }
        public string Question { get; }

        public Words(string title, string question)
        {
            Title = title;
            Question = question;
        }
    }
    public static class CommonInteractions
    {
        public static readonly Interaction<FileSelection, string?> BrowseToOpenFile = new Interaction<FileSelection, string?>();
        public static readonly Interaction<FileSelection, string?> BrowseToSaveFile = new Interaction<FileSelection, string?>();
        public static readonly Interaction<FolderSelection, string?> BrowseForFolder = new Interaction<FolderSelection, string?>();
        public static readonly Interaction<Words, bool> IsOk = new Interaction<Words, bool>();
    }
}
