using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Editors;
using CaveStoryModdingFramework.Entities;
using CaveStoryModdingFramework.Stages;
using CaveStoryModdingFramework.Utilities;
using NP.Avalonia.UniDockService;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using TheKingsTable.Controls;
using TheKingsTable.Models;

namespace TheKingsTable.ViewModels.Editors
{
    internal class StageEditorDockItemViewModel : DockItemViewModel<StageEditorViewModel> { }
    public class StageEditorViewModel : ViewModelBase
    {
        public string StageName
        {
            get
            {
                var name = Entry.MapName;
                if (!string.IsNullOrWhiteSpace(Entry.JapaneseName))
                {
                    name += " | " + Entry.JapaneseName;
                }
                name += " (" + Entry.Filename + ")";
                return name;
            }
        }
        public ProjectFile Project { get; }
        public StageTableEntry Entry { get; private set; }

        public Bitmap Background { get; private set; }
        public Bitmap TilesetImage { get; private set; }
        public Bitmap TileTypesImage { get; private set; }

        CaveStoryModdingFramework.Maps.Attribute attributes;
        public CaveStoryModdingFramework.Maps.Attribute Attributes { get => attributes; private set => this.RaiseAndSetIfChanged(ref attributes, value); }


        public ChangeTracker<object> ChangeTracker { get; }

        StageEditorClipboard Clipboard { get; } = new StageEditorClipboard();

        #region Active editor
        public enum Editors
        {
            Tile,
            Entity,
            MapState
        }
        Editors activeEditor = Editors.Tile;
        public Editors ActiveEditor
        {
            get => activeEditor;
            set => this.RaiseAndSetIfChanged(ref activeEditor, value);
        }
        #endregion

        #region Tile mode radio buttons
        string lastSetTile = nameof(TileDraw);
        void SetRadioEnumFromBool<T>(ref T field, bool value, T radio,
            ref string lastSet,
            [System.Runtime.CompilerServices.CallerMemberName]
            string propertyName = "") where T : Enum
        {
            if (value && !field.Equals(radio))
            {
                this.RaisePropertyChanging(propertyName);
                this.RaisePropertyChanging(lastSet);
                field = radio;
                this.RaisePropertyChanged(propertyName);
                this.RaisePropertyChanged(lastSet);
                lastSet = propertyName;
            }
        }
        bool TileDraw
        {
            get => TileAction == TileEditorActions.Draw;
            set => SetRadioEnumFromBool(ref TileAction, value, TileEditorActions.Draw, ref lastSetTile);
        }
        bool TileRectangle
        {
            get => TileAction == TileEditorActions.Rectangle;
            set => SetRadioEnumFromBool(ref TileAction, value, TileEditorActions.Rectangle, ref lastSetTile);
        }
        bool TileFill
        {
            get => TileAction == TileEditorActions.Fill;
            set => SetRadioEnumFromBool(ref TileAction, value, TileEditorActions.Fill, ref lastSetTile);
        }
        bool TileReplace
        {
            get => TileAction == TileEditorActions.Replace;
            set => SetRadioEnumFromBool(ref TileAction, value, TileEditorActions.Replace, ref lastSetTile);
        }
        bool TileSelect
        {
            get => TileAction == TileEditorActions.Select;
            set => SetRadioEnumFromBool(ref TileAction, value, TileEditorActions.Select, ref lastSetTile);
        }
        #endregion

        public TileEditor TileEditor { get; }
        TileEditorActions TileAction = TileEditorActions.Draw;
        
        public CaveStoryModdingFramework.Editors.EntityEditor EntityEditor { get; }
        bool selectInProgress = false, moveInProgress = false;
        short entityMoveOffsetX = 0, entityMoveOffsetY = 0;
        public short EntityMoveOffsetX { get => entityMoveOffsetX; set => this.RaiseAndSetIfChanged(ref entityMoveOffsetX, value); }
        public short EntityMoveOffsetY { get => entityMoveOffsetY; set => this.RaiseAndSetIfChanged(ref entityMoveOffsetY, value); }
        short selectedEntityType = 0;
        public short SelectedEntityType { get => selectedEntityType; set => this.RaiseAndSetIfChanged(ref selectedEntityType, value); }


        bool showCursor = true;
        public bool ShowCursor { get => showCursor; private set => this.RaiseAndSetIfChanged(ref showCursor, value); }

        int selectionStartX = -1, selectionStartY = -1, selectionEndX = -1, selectionEndY = -1;
        public int SelectionStartX { get => selectionStartX; private set => this.RaiseAndSetIfChanged(ref selectionStartX, value); }
        public int SelectionStartY { get => selectionStartY; private set => this.RaiseAndSetIfChanged(ref selectionStartY, value); }
        public int SelectionEndX { get => selectionEndX; private set => this.RaiseAndSetIfChanged(ref selectionEndX, value); }
        public int SelectionEndY { get => selectionEndY; private set => this.RaiseAndSetIfChanged(ref selectionEndY, value); }

        string PXMPath, PXEPath;
        public StageEditorViewModel(
            ProjectFile project, StageTableEntry entry,
            Bitmap background,
            Bitmap tileset, CaveStoryModdingFramework.Maps.Attribute attributes,
            string pxmPath, string pxePath)
        {
            Project = project;
            Entry = entry;

            //TODO LOAD FROM PROJECT URL AND MAYBE CACHE???
            TileTypesImage = new Bitmap(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"tiletypes.png"));

            Background = background;

            TilesetImage = tileset;
            Attributes = attributes;

            PXMPath = pxmPath;
            PXEPath = pxePath;

            ChangeTracker = new ChangeTracker<object>();
            TileEditor = new TileEditor(PXMPath, ChangeTracker);
            EntityEditor = new CaveStoryModdingFramework.Editors.EntityEditor(PXEPath, ChangeTracker);

            CopyTRACommand = ReactiveCommand.Create<RoutedEventArgs>(async e => {
                await Avalonia.Application.Current.Clipboard.SetTextAsync($"<TRA{SelectionStartX:D4}:{SelectionStartY:D4}:");
            });
            CopyCMPCommand = ReactiveCommand.Create<RoutedEventArgs>(async e => {
                await Avalonia.Application.Current.Clipboard.SetTextAsync($"<CMP{SelectionStartX:D4}:{SelectionStartY:D4}:");
            });
            CopySMPCommand = ReactiveCommand.Create<RoutedEventArgs>(async e => {
                await Avalonia.Application.Current.Clipboard.SetTextAsync($"<SMP{SelectionStartX:D4}:{SelectionStartY:D4}");
            });

            //TODO create save command somehow

            UndoCommand = ReactiveCommand.Create<RoutedEventArgs>(e => DoAndTriggerRedraw(ChangeTracker.Undo));
            RedoCommand = ReactiveCommand.Create<RoutedEventArgs>(e => DoAndTriggerRedraw(ChangeTracker.Redo));

            PointerEnterCommand = ReactiveCommand.Create<TileEventArgs>(PointerEnter);
            PointerMovedCommand = ReactiveCommand.Create<TileEventArgs>(PointerMoved);
            PointerPressedCommand = ReactiveCommand.Create<TileEventArgs>(PointerPressed);
            PointerReleasedCommand = ReactiveCommand.Create<TileEventArgs>(PointerReleased);
            PointerLeaveCommand = ReactiveCommand.Create(PointerLeave);
            PointerCaptureLostCommand = ReactiveCommand.Create(PointerCaptureLost);

            KeyDownCommand = ReactiveCommand.Create<KeyEventArgs>(KeyDown);

            InsertEntityCommand = ReactiveCommand.Create<Tuple<short,short>>(InsertEntity);
            SelectEntityCommand = ReactiveCommand.Create<Entity>(SelectEntity);

            SaveCommand = ReactiveCommand.Create<RoutedEventArgs>(Save);
        }
        public ReactiveCommand<RoutedEventArgs, Unit> CopyTRACommand { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> CopyCMPCommand { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> CopySMPCommand { get; }

        public ReactiveCommand<RoutedEventArgs, Unit> SaveCommand { get; }

        public ReactiveCommand<RoutedEventArgs, Unit> UndoCommand { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> RedoCommand { get; }

        public ReactiveCommand<TileEventArgs, Unit> PointerEnterCommand { get; }
        public ReactiveCommand<TileEventArgs, Unit> PointerMovedCommand { get; }
        public ReactiveCommand<TileEventArgs, Unit> PointerPressedCommand { get; }
        public ReactiveCommand<TileEventArgs, Unit> PointerReleasedCommand { get; }
        public ReactiveCommand<Unit, Unit> PointerLeaveCommand { get; }
        public ReactiveCommand<Unit, Unit> PointerCaptureLostCommand { get; }

        public ReactiveCommand<Entity, Unit> SelectEntityCommand { get; }
        public ReactiveCommand<Tuple<short,short>, Unit> InsertEntityCommand { get; }
        public ReactiveCommand<KeyEventArgs, Unit> KeyDownCommand { get; }

        void SelectEntity(Entity e)
        {
            EntityEditor.Selection.Clear();
            EntityEditor.Selection.Add(e);
        }
        //HACK using a Tuple is kinda hacky, but it's the only way to have a single parameter for the command, so...
        void InsertEntity(Tuple<short,short> coords)
        {
            if(coords.Item1 > -1 && coords.Item2 > -1)
                EntityEditor.CreateEntity(coords.Item1, coords.Item2, SelectedEntityType);
        }

        async void KeyDown(KeyEventArgs e)
        {
            switch (ActiveEditor)
            {
                case Editors.Tile:
                    switch (e.Key)
                    {
                        case Key.C when e.KeyModifiers == KeyModifiers.Control:
                            await Clipboard.SetTiles(TileEditor.Selection);
                            e.Handled = true;
                            break;
                        case Key.V when e.KeyModifiers == KeyModifiers.Control:
                        case Key.Insert when e.KeyModifiers == KeyModifiers.Shift:
                            var ts = await Clipboard.GetTiles();
                            if (ts != null)
                            {
                                TileEditor.Selection.Contents = ts.Contents;
                                TileEditor.Selection.CursorX = ts.CursorX;
                                TileEditor.Selection.CursorY = ts.CursorY;
                            }
                            e.Handled = true;
                            break;
                    }
                    break;
                case Editors.Entity:    
                    switch (e.Key)
                    {
                        case Key.Insert when e.KeyModifiers == KeyModifiers.None:
                        case Key.I when e.KeyModifiers == KeyModifiers.None:
                            InsertEntity(Tuple.Create((short)SelectionEndX, (short)SelectionEndY));
                            e.Handled = true;
                            break;
                        case Key.Delete when e.KeyModifiers == KeyModifiers.None && EntityEditor.Selection.Count > 0:
                            EntityEditor.DeleteSelection();
                            e.Handled = true;
                            break;

                        case Key.C when e.KeyModifiers == KeyModifiers.Control:
                            await Clipboard.SetEntities(EntityEditor.Selection);
                            e.Handled = true;
                            break;
                        case Key.V when e.KeyModifiers == KeyModifiers.Control:
                        case Key.Insert when e.KeyModifiers == KeyModifiers.Shift:
                            var es = await Clipboard.GetEntities();
                            if(es != null)
                            {
                                EntityEditor.PasteEntities((short)SelectionEndX, (short)SelectionEndY, es);
                            }
                            e.Handled = true;
                            break;
                    }
                    break;
                        
            }
        }

        //HACK this is 100% not the correct way to create a context menu in this situation
        public Avalonia.Controls.ContextMenu ContextMenu
        {
            get
            {
                var cm = new Avalonia.Controls.ContextMenu();
                var items = new List<Avalonia.Controls.Control>();
                switch (ActiveEditor)
                {
                    case Editors.Tile:
                        items.Add(new Avalonia.Controls.MenuItem()
                        {
                            Header = "Copy <TRA",
                            Command = CopyTRACommand
                        });
                        items.Add(new Avalonia.Controls.MenuItem()
                        {
                            Header = "Copy <CMP",
                            Command = CopyCMPCommand
                        });
                        items.Add(new Avalonia.Controls.MenuItem()
                        {
                            Header = "Copy <SMP",
                            Command = CopySMPCommand
                        });
                        break;
                    case Editors.Entity:
                        items.Add(new Avalonia.Controls.MenuItem()
                        {
                            Header = "Insert Entity",
                            Command = InsertEntityCommand,
                            CommandParameter = Tuple.Create((short)SelectionEndX, (short)SelectionEndY)
                        });
                        var ents = EntityEditor.GetEntitiesAt((short)SelectionEndX, (short)SelectionEndY);
                        if (ents.Count > 0)
                        {
                            items.Add(new Avalonia.Controls.Separator());
                            foreach (var e in ents)
                                items.Add(new Avalonia.Controls.MenuItem()
                                {
                                    Header = $"{EntityEditor.Entities.IndexOf(e)} - {e.Type}",
                                    Command = SelectEntityCommand,
                                    CommandParameter = e
                                });
                        }
                        break;
                    case Editors.MapState:
                        break;
                    default:
                        throw new ArgumentException("Invalid Active Editor: " + ActiveEditor, nameof(ActiveEditor));
                }
                cm.Items = items;
                return cm;
            }
        }



        bool redrawTilesNeeded = true;
        public bool RedrawTilesNeeded { get => redrawTilesNeeded; set => this.RaiseAndSetIfChanged(ref redrawTilesNeeded, value); }
        bool redrawTileTypesNeeded = true;
        public bool RedrawTileTypesNeeded { get => redrawTileTypesNeeded; set => this.RaiseAndSetIfChanged(ref redrawTileTypesNeeded, value); }

        void DoAndTriggerRedraw(Action a)
        {
            a();
            RedrawTilesNeeded = RedrawTileTypesNeeded = true;
        }
        bool showTileTypes = false;
        public bool ShowTileTypes { get => showTileTypes; set => this.RaiseAndSetIfChanged(ref showTileTypes, value); }

        #region Pointer stuff
        void HidePointer()
        {
            SelectionStartX = SelectionEndX = -1;
            SelectionStartY = SelectionEndY = -1;
        }
        void ResetPointer(TileEventArgs e)
        {
            SelectionStartX = SelectionEndX = e.X;
            SelectionStartY = SelectionEndY = e.Y;
        }
        void PointerPressed(TileEventArgs e)
        {
            if (e.Pressed == PointerUpdateKind.LeftButtonPressed)
            {
                switch (ActiveEditor)
                {
                    case Editors.Tile:
                        TileEditor.BeginSelection(e.X, e.Y, TileAction);
                        break;
                    case Editors.Entity:
                        if (EntityEditor.AnySelectedEntitiesAt((short)e.X, (short)e.Y))
                        {
                            moveInProgress = true;
                            ShowCursor = false;
                        }
                        else
                        {
                            selectInProgress = true;
                        }
                        break;
                }
            }
            //HACK the context menu does indeed change every time you right click... potentionally
            else if(e.Pressed == PointerUpdateKind.RightButtonPressed)
            {
                ResetPointer(e);
                this.RaisePropertyChanged(nameof(ContextMenu));
            }
        }
        void PointerEnter(TileEventArgs e)
        {
            SharedMove(e);
        }
        void PointerMoved(TileEventArgs e)
        {
            SharedMove(e);
        }
        void SharedMove(TileEventArgs e)
        {
            switch (ActiveEditor)
            {
                case Editors.Tile:
                    switch (TileEditor.CurrentAction)
                    {
                        //these use a rectangular cursor
                        case TileEditorActions.Rectangle:
                        case TileEditorActions.Select:
                            TileEditor.MoveSelection(SelectionEndX = e.X, SelectionEndY = e.Y);
                            break;

                        //these all use a size 1 cursor...
                        case TileEditorActions.Fill:
                        case TileEditorActions.Replace:
                        //...except for draw, which has a special case when you've selected it too
                        case TileEditorActions.Draw:
                            TileEditor.MoveSelection(e.X, e.Y);
                            goto default;
                        default:
                            //draw mode ALWAYS has a cursor as big as the selection
                            if (TileAction == TileEditorActions.Draw)
                            {
                                SelectionStartX = e.X - TileEditor.Selection.CursorX;
                                SelectionStartY = e.Y - TileEditor.Selection.CursorY;
                                SelectionEndX = e.X + (TileEditor.Selection.Contents.Width - TileEditor.Selection.CursorX - 1);
                                SelectionEndY = e.Y + (TileEditor.Selection.Contents.Height - TileEditor.Selection.CursorY - 1);
                            }
                            else
                            {
                                ResetPointer(e);
                            }
                            break;
                    }
                    break;
                case Editors.Entity:
                    if (selectInProgress)
                    {
                        SelectionEndX = e.X;
                        SelectionEndY = e.Y;
                    }
                    else if (moveInProgress)
                    {
                        EntityMoveOffsetX = (short)((SelectionEndX = e.X) - SelectionStartX);
                        EntityMoveOffsetY = (short)((SelectionEndY = e.Y) - SelectionStartY);
                    }
                    else
                    {
                        ResetPointer(e);
                    }
                    break;
            }
        }
        void PointerReleased(TileEventArgs e)
        {
            if(e.Pressed == PointerUpdateKind.LeftButtonReleased)
            {
                switch (ActiveEditor)
                {
                    case Editors.Tile:
                        TileEditor.CommitSelection();
                        break;
                    case Editors.Entity:
                        if (moveInProgress)
                        {
                            EntityEditor.MoveSelection(EntityMoveOffsetX, EntityMoveOffsetY);
                            ShowCursor = true;
                            EntityMoveOffsetX = 0;
                            EntityMoveOffsetY = 0;
                            moveInProgress = false;
                        }
                        else
                        {
                            EntityEditor.SelectEntitiesInRange((short)SelectionStartX, (short)SelectionStartY, (short)SelectionEndX, (short)SelectionEndY);
                            selectInProgress = false;
                        }
                        break;
                }
                ResetPointer(e);
            }
        }
        private void PointerLeave()
        {
            switch (ActiveEditor)
            {
                case Editors.Tile when TileEditor.CurrentAction == TileEditorActions.None:
                case Editors.Entity:
                    HidePointer();
                    break;
            }
        }
        private void PointerCaptureLost()
        {
            switch (ActiveEditor)
            {
                case Editors.Tile:
                    TileEditor.BeginSelection(-1, -1, TileEditorActions.None);
                    break;
                case Editors.Entity:
                    selectInProgress = false;
                    moveInProgress = false;
                    EntityMoveOffsetX = 0;
                    EntityMoveOffsetY = 0;
                    ShowCursor = true;
                    break;
            }
            HidePointer();
        }
        #endregion

        void Save(RoutedEventArgs e)
        {
            EntityEditor.Save(PXEPath);
            TileEditor.Save(PXMPath);
        }
        /*
        private void LinkedEntry_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is StageTableEntry ste)
            {
                switch (e.PropertyName)
                {
                    case nameof(StageTableEntry.Filename):
                        //newFilename = ste.Filename;
                        break;
                    case nameof(StageTableEntry.TilesetName):
                        //newTileset = ste.TilesetName;
                        break;
                    case nameof(StageTableEntry.BackgroundName):
                        //newBackground = ste.BackgroundName;
                        break;
                    case nameof(StageTableEntry.Spritesheet1):
                        //newSpritesheet1 = ste.Spritesheet1;
                        break;
                    case nameof(StageTableEntry.Spritesheet2):
                        //newSpritesheet2 = ste.Spritesheet2;
                        break;

                    case nameof(StageTableEntry.MapName):
                    case nameof(StageTableEntry.JapaneseName):
                    case nameof(StageTableEntry.BossNumber):
                    case nameof(StageTableEntry.BackgroundType):
                        //auto update
                        break;
                }
            }
        }
        */
    }
}
