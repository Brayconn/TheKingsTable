using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Windows.Input;

namespace TheKingsTable.Controls
{
    public class TileEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }
        public PointerUpdateKind? Pressed { get; }

        public TileEventArgs(int x, int y, PointerUpdateKind? pressed = null)
        {
            X = x;
            Y = y;
            Pressed = pressed;
        }
    }
    public abstract class TileMouseHelper : Control
    {
        static protected readonly AvaloniaProperty[] Properties;
        static TileMouseHelper()
        {
            //needed to use the static constructor
            //to avoid complaints about null
            Properties = new AvaloniaProperty[]
            {
                TileSizeProperty,
                SelectionStartXProperty,
                SelectionStartYProperty,
                SelectionEndXProperty,
                SelectionEndYProperty,
                ScaleProperty
            };
        }

        #region TileSize Styled Property
        public static readonly StyledProperty<int> TileSizeProperty =
            AvaloniaProperty.Register<TileMouseHelper, int>(nameof(TileSize), 16);

        public int TileSize
        {
            get => GetValue(TileSizeProperty);
            set => SetValue(TileSizeProperty, value);
        }
        #endregion

        #region SelectionStartX Styled Property
        public static readonly StyledProperty<int> SelectionStartXProperty =
            AvaloniaProperty.Register<TileMouseHelper, int>(nameof(SelectionStartX), -1);

        public int SelectionStartX
        {
            get => GetValue(SelectionStartXProperty);
            set => SetValue(SelectionStartXProperty, value);
        }
        #endregion

        #region SelectionStartY Styled Property
        public static readonly StyledProperty<int> SelectionStartYProperty =
            AvaloniaProperty.Register<TileMouseHelper, int>(nameof(SelectionStartY), -1);

        public int SelectionStartY
        {
            get => GetValue(SelectionStartYProperty);
            set => SetValue(SelectionStartYProperty, value);
        }
        #endregion

        #region SelectionEndX Styled Property
        public static readonly StyledProperty<int> SelectionEndXProperty =
            AvaloniaProperty.Register<TileMouseHelper, int>(nameof(SelectionEndX), -1);

        public int SelectionEndX
        {
            get => GetValue(SelectionEndXProperty);
            set => SetValue(SelectionEndXProperty, value);
        }
        #endregion

        #region SelectionEndY Styled Property
        public static readonly StyledProperty<int> SelectionEndYProperty =
            AvaloniaProperty.Register<TileMouseHelper, int>(nameof(SelectionEndY), -1);

        public int SelectionEndY
        {
            get => GetValue(SelectionEndYProperty);
            set => SetValue(SelectionEndYProperty, value);
        }
        #endregion

        #region Scale Styled Property
        public static readonly StyledProperty<double> ScaleProperty =
            AvaloniaProperty.Register<TileMouseHelper, double>(nameof(Scale), 1);

        public double Scale
        {
            get => GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }
        #endregion

        protected double ZoomMin = 0.25, ZoomStep = 0.25, ZoomMax = 10;

        int TileX = -1, TileY = -1;
        TileEventArgs CurrentPos => new TileEventArgs(TileX, TileY);
        protected Rect GetTileRect(byte tile, int width)
        {
            return new Rect((tile % width) * TileSize, (tile / width) * TileSize, TileSize, TileSize);
        }
        protected (int, int) PointToTile(Point p)
        {
            return ((int)(p.X / (TileSize * Scale)),
                    (int)(p.Y / (TileSize * Scale)));
        }

        public TileMouseHelper()
        {
            PointerEnter += OnPointerEnter;
            PointerMoved += OnPointerMoved;
            PointerWheelChanged += OnPointerWheelChanged;

            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;

            PointerLeave += OnPointerLeave;
            PointerCaptureLost += OnPointerCaptureLost;

            this.AddHandler(Control.KeyDownEvent, OnKeyDown);
            //KeyDown += OnKeyDown;
            Focusable = true;
        }
        ~TileMouseHelper()
        {
            PointerEnter -= OnPointerEnter;
            PointerMoved -= OnPointerMoved;
            PointerWheelChanged -= OnPointerWheelChanged;

            PointerPressed -= OnPointerPressed;
            PointerReleased -= OnPointerReleased;

            PointerLeave -= OnPointerLeave;
            PointerCaptureLost -= OnPointerCaptureLost;

            this.RemoveHandler(Control.KeyUpEvent, OnKeyDown);
            //KeyDown -= OnKeyDown;
        }
        public event EventHandler<TileEventArgs>? PointerEnterTile, PointerWheelChangedTile, PointerMovedTile, PointerPressedTile, PointerReleasedTile;
        private void OnPointerEnter(object? sender, PointerEventArgs e)
        {
            (TileX, TileY) = PointToTile(e.GetPosition(this));
            PointerEnterTile?.Invoke(sender, CurrentPos);
            pointerEnterCommand?.Execute(CurrentPos);
        }
        protected bool StepZoom(double step)
        {
            if (step != 0)
            {
                if (step > 0)
                    Scale += ZoomStep;
                else if (step < 0)
                    Scale -= ZoomStep;

                if (Scale < ZoomMin)
                    Scale = ZoomMin;
                else if (Scale > ZoomMax)
                    Scale = ZoomMax;

                return true;
            }
            return false;
        }
        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (e.KeyModifiers == KeyModifiers.Control)
            {
                e.Handled = StepZoom(e.Delta.Y);
            }
        }
        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            (var x, var y) = PointToTile(e.GetPosition(this));
            if (x != TileX || y != TileY)
            {
                TileX = x;
                TileY = y;
                PointerMovedTile?.Invoke(sender, CurrentPos);
                pointerMovedCommand?.Execute(CurrentPos);
            }
        }
        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            //certain circumstances (such as opening the context menu, then clicking again)
            //cause the cursor position to desync at this point, so we run the move code again just in case
            OnPointerMoved(this, e);
            var args = new TileEventArgs(TileX, TileY, e.GetCurrentPoint(this).Properties.PointerUpdateKind);
            PointerPressedTile?.Invoke(sender, args);
            pointerPressedCommand?.Execute(args);
        }
        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var args = new TileEventArgs(TileX, TileY, e.GetCurrentPoint(this).Properties.PointerUpdateKind);
            PointerReleasedTile?.Invoke(sender, args);
            pointerReleasedCommand?.Execute(args);
        }
        private void OnPointerLeave(object? sender, PointerEventArgs e)
        {
            TileX = -1;
            TileY = -1;
            pointerLeaveCommand?.Execute(null);
        }
        private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
        {
            TileX = -1;
            TileY = -1;
            pointerCaptureLostCommand?.Execute(null);
        }

        #region PointerEnterCommand Direct Property
        public static readonly DirectProperty<StageRenderer, ICommand?> PointerEnterCommandProperty =
            AvaloniaProperty.RegisterDirect<StageRenderer, ICommand?>(nameof(PointerEnterCommand),
                o => o.PointerEnterCommand, (o, v) => o.PointerEnterCommand = v);

        ICommand? pointerEnterCommand;
        public ICommand? PointerEnterCommand
        {
            get => pointerEnterCommand;
            private set => SetAndRaise(PointerEnterCommandProperty, ref pointerEnterCommand, value);
        }
        #endregion

        #region PointerMovedCommand Direct Property
        public static readonly DirectProperty<StageRenderer, ICommand?> PointerMovedCommandProperty =
            AvaloniaProperty.RegisterDirect<StageRenderer, ICommand?>(nameof(PointerMovedCommand),
                o => o.PointerMovedCommand, (o, v) => o.PointerMovedCommand = v);

        ICommand? pointerMovedCommand;
        public ICommand? PointerMovedCommand
        {
            get => pointerMovedCommand;
            private set => SetAndRaise(PointerMovedCommandProperty, ref pointerMovedCommand, value);
        }
        #endregion

        #region PointerWheelChangedCommand Direct Property
        public static readonly DirectProperty<StageRenderer, ICommand?> PointerWheelChangedCommandProperty =
            AvaloniaProperty.RegisterDirect<StageRenderer, ICommand?>(nameof(PointerWheelChangedCommand),
                o => o.PointerWheelChangedCommand, (o, v) => o.PointerWheelChangedCommand = v);

        ICommand? pointerWheelChangedCommand;
        public ICommand? PointerWheelChangedCommand
        {
            get => pointerWheelChangedCommand;
            private set => SetAndRaise(PointerWheelChangedCommandProperty, ref pointerWheelChangedCommand, value);
        }
        #endregion

        #region PointerPressedCommand Direct Property
        public static readonly DirectProperty<StageRenderer, ICommand?> PointerPressedCommandProperty =
            AvaloniaProperty.RegisterDirect<StageRenderer, ICommand?>(nameof(PointerPressedCommand),
                o => o.PointerPressedCommand, (o, v) => o.PointerPressedCommand = v);

        ICommand? pointerPressedCommand;
        public ICommand? PointerPressedCommand
        {
            get => pointerPressedCommand;
            private set => SetAndRaise(PointerPressedCommandProperty, ref pointerPressedCommand, value);
        }
        #endregion

        #region PointerReleasedCommand Direct Property
        public static readonly DirectProperty<StageRenderer, ICommand?> PointerReleasedCommandProperty =
            AvaloniaProperty.RegisterDirect<StageRenderer, ICommand?>(nameof(PointerReleasedCommand),
                o => o.PointerReleasedCommand, (o, v) => o.PointerReleasedCommand = v);

        ICommand? pointerReleasedCommand;
        public ICommand? PointerReleasedCommand
        {
            get => pointerReleasedCommand;
            private set => SetAndRaise(PointerReleasedCommandProperty, ref pointerReleasedCommand, value);
        }
        #endregion

        #region PointerLeaveCommand Direct Property
        public static readonly DirectProperty<StageRenderer, ICommand?> PointerLeaveCommandProperty =
            AvaloniaProperty.RegisterDirect<StageRenderer, ICommand?>(nameof(PointerLeaveCommand),
                o => o.PointerLeaveCommand, (o, v) => o.PointerLeaveCommand = v);

        ICommand? pointerLeaveCommand;
        public ICommand? PointerLeaveCommand
        {
            get => pointerLeaveCommand;
            private set => SetAndRaise(PointerLeaveCommandProperty, ref pointerLeaveCommand, value);
        }
        #endregion

        #region PointerCaptureLostCommand Direct Property
        public static readonly DirectProperty<StageRenderer, ICommand?> PointerCaptureLostCommandProperty =
            AvaloniaProperty.RegisterDirect<StageRenderer, ICommand?>(nameof(PointerCaptureLostCommand),
                o => o.PointerCaptureLostCommand, (o, v) => o.PointerCaptureLostCommand = v);

        ICommand? pointerCaptureLostCommand;
        public ICommand? PointerCaptureLostCommand
        {
            get => pointerCaptureLostCommand;
            private set => SetAndRaise(PointerCaptureLostCommandProperty, ref pointerCaptureLostCommand, value);
        }
        #endregion

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if(e.KeyModifiers == KeyModifiers.Control)
            {
                switch (e.Key)
                {
                    case Key.OemPlus:
                    case Key.Add:
                        e.Handled = StepZoom(1);
                        break;
                    case Key.OemMinus:
                    case Key.Subtract:
                        e.Handled = StepZoom(-1);
                        break;
                }
            }
            keyDownCommand?.Execute(e);
        }

        #region KeyDownCommand Direct Property
        public static readonly DirectProperty<TileMouseHelper, ICommand?> KeyDownCommandProperty =
            AvaloniaProperty.RegisterDirect<TileMouseHelper, ICommand?>(nameof(KeyDownCommand),
                o => o.KeyDownCommand, (o,v) => o.KeyDownCommand = v);
        
        ICommand? keyDownCommand;
        public ICommand? KeyDownCommand
        {
            get => keyDownCommand;
            private set => SetAndRaise(KeyDownCommandProperty, ref keyDownCommand, value);
        }
        #endregion
    }
}
