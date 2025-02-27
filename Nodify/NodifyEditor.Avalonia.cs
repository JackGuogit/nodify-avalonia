namespace Nodify;

public partial class NodifyEditor
{
    private Point viewportLocation;
    private Guid Id = Guid.NewGuid();
    private CancellationTokenSource? bringToViewToken;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DisplayConnectionsOnTopProperty)
            PseudoClasses.Set(":connections-on-top", DisplayConnectionsOnTop);
    }

    private bool inOnSelectedItemsChanged;

    /// <summary>
    /// On WPF single mouse wheel delta is 120, on Avalonia it is 0.6 (at least on macOS)
    /// </summary>
    private static double MouseWheelAvaloniaToWpfScale => 120 / 0.6;
    
    private double PointerTouchGestureMagnifyScale => 4;

    private void OnPointerTouchPadGestureMagnify(object? sender, PointerDeltaEventArgs e)
    {
        State.HandleMouseWheel(new MouseWheelEventArgs());

        if (!e.Handled && EditorGestures.Mappings.Editor.ZoomModifierKey == e.KeyModifiers)
        {
            double zoom = Math.Pow(2.0, e.Delta.Y / 3.0 / MouseWheelDeltaForOneLine * MouseWheelAvaloniaToWpfScale * PointerTouchGestureMagnifyScale);
            ZoomAtPosition(zoom, e.GetPosition(ItemsHost));

            // Handle it for nested editors
            if (e.Source is NodifyEditor)
            {
                e.Handled = true;
            }
        }
    }

    private void OnSourceReset(object sender, EventArgs e)
    {
        SelectedItems?.Clear();
    }

    private void OnPreviewPointerPressed(object sender, PointerPressedEventArgs e)
    {
        // Avalonia, contrary to WPF, automatically captures the pointer when pressed
        // this interferes with Nodify behaviour, so here's the workaround:
        // OnPreviewPointerPressed is a tunneled handler, meaning it is called BEFORE any other mouse handler
        // basically we release the pointer capture as soon as it is captured, so that later it can be manually captured
        // in correct places

        // However, the above interferes with Thumb behaviour, which does not capture the pointer in OnPointerPressed,
        // because it assumes the pointer is captured automatically. RIP
        if (e.Source is Thumb || e.Source is Control sourceControl && sourceControl.FindAncestorOfType<Thumb>() != null)
            return;

        // this check is required for StickyConnections to work
        // when IsMouseCaptureWithin is true, then the pointer is actually captured by an explicit call to Capture
        // so we don't wanna interrupt it
        if (!IsMouseCaptureWithin)
            e.Pointer.Capture(null);
    }

    protected override void OnDataContextEndUpdate()
    {
        // Required to synchronize SelectedItems with the DataContext
        base.OnDataContextEndUpdate();
        Selection.Clear();
        if (SelectedItems != null && SelectedItems.Count > 0)
        {
            for (var i = 0; i < SelectedItems.Count; i++)
            {
                Selection.Select(Items.IndexOf(SelectedItems[i]));
            }
        }
    }

    #region Scrollable

    public Size Extent => new Size(_extentWidth, _extentHeight);

    public Size Viewport => ViewportSize;

    public Vector Offset
    {
        get => new Vector(_horizontalOffset, _verticalOffset);
        set
        {
            _horizontalOffset = value.X;
            _verticalOffset = value.Y;
            UpdateViewportLocationOnScroll();
            // merged SetHorizontalOffset and SetVerticalOffset
        }
    }

    #endregion

    protected override Type StyleKeyOverride => typeof(NodifyEditor);

    public bool BringIntoView(Control target, Rect targetRect) => false;

    public Control? GetControlInDirection(NavigationDirection direction, Control? from) => null;

    public void RaiseScrollInvalidated(EventArgs e) => ScrollInvalidated?.Invoke(this, e);

    public bool IsLogicalScrollEnabled => true;
    public Size ScrollSize => new Size(ScrollIncrement / ViewportZoom, ScrollIncrement / ViewportZoom);
    public Size PageScrollSize => ViewportSize;
    public event EventHandler? ScrollInvalidated;
}