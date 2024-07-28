// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.BrowserWindow
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  public partial class BrowserWindow : Window, IComponentConnector
  {
    public static Size CompactSize = new Size(464.0, 570.0);
    public static Size FullSize = new Size(1000.0, 615.0);
    public static Size MaxResize = new Size(1025.0, 750.0);
    public static Size MaxResizeCheckSize = new Size(1025.0, 768.0);
    private static readonly Key[] BlockedKeys = new Key[8]
    {
      Key.F5,
      Key.BrowserBack,
      Key.BrowserFavorites,
      Key.BrowserForward,
      Key.BrowserHome,
      Key.BrowserRefresh,
      Key.BrowserSearch,
      Key.BrowserStop
    };
    private static readonly Key[] BlockedAltKeys = new Key[2]
    {
      Key.Left,
      Key.Right
    };
    private static readonly Key[] BlockedControlKeys = new Key[6]
    {
      Key.L,
      Key.N,
      Key.O,
      Key.P,
      Key.R,
      Key.S
    };
    internal WebBrowser WebBrowser;
    private bool _contentLoaded;

    public BrowserWindow(object external, Size desiredSize = default (Size))
    {
      this.InitializeComponent();
      this.BrowserHandler = new ComBrowserHandler(this.WebBrowser, true, true, external);
      if (!(desiredSize != Size.Empty))
        return;
      this.Width = desiredSize.Width;
      this.Height = desiredSize.Height;
    }

    internal ComBrowserHandler BrowserHandler { get; private set; }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      if (this.BrowserHandler != null)
      {
        this.BrowserHandler.Dispose();
        this.BrowserHandler = (ComBrowserHandler) null;
      }
      if (this.WebBrowser == null)
        return;
      this.WebBrowser.Dispose();
      this.WebBrowser = (WebBrowser) null;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      this.DragMove();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Back && !this.BrowserHandler.IsActiveElementTextInput())
      {
        e.Handled = true;
      }
      else
      {
        bool flag1 = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
        bool flag2 = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        if (((IEnumerable<Key>) BrowserWindow.BlockedKeys).Contains<Key>(e.Key) || flag1 && ((IEnumerable<Key>) BrowserWindow.BlockedAltKeys).Contains<Key>(e.Key) || flag2 && ((IEnumerable<Key>) BrowserWindow.BlockedControlKeys).Contains<Key>(e.Key))
          e.Handled = true;
        else
          base.OnPreviewKeyDown(e);
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.VisualStudio.Services.Client.Interactive;V19.0.0.0;component/controls/browserwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.WebBrowser = (WebBrowser) target;
      else
        this._contentLoaded = true;
    }

    private class KeyDownHandler : IDisposable
    {
      private List<IInputElement> elements = new List<IInputElement>();
      private List<KeyBinding> handleBindings = new List<KeyBinding>();
      private List<KeyGesture> ignoreKeyGestures = new List<KeyGesture>();
      private List<ModifierKeys> ignoreModifiers = new List<ModifierKeys>();

      public IEnumerable<RoutedCommand> Commands { get; set; }

      public void Attach(IInputElement element)
      {
        Keyboard.AddPreviewKeyDownHandler((DependencyObject) element, new KeyEventHandler(this.element_PreviewKeyDown));
        this.elements.Add(element);
      }

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      public void Handle(ICommand command, Key key, ModifierKeys modifiers = ModifierKeys.None) => this.handleBindings.Add(new KeyBinding(command, key, modifiers));

      public void Ignore(ModifierKeys modifiers, Key key = Key.None)
      {
        if (key == Key.None)
          this.ignoreModifiers.Add(modifiers);
        else
          this.ignoreKeyGestures.Add(new KeyGesture(key, modifiers));
      }

      protected virtual void Dispose(bool disposing)
      {
        if (disposing)
        {
          if (this.elements != null)
          {
            foreach (DependencyObject element in this.elements)
              Keyboard.RemovePreviewKeyDownHandler(element, new KeyEventHandler(this.element_PreviewKeyDown));
            this.elements.Clear();
          }
          if (this.handleBindings != null)
            this.handleBindings.Clear();
          if (this.ignoreKeyGestures != null)
            this.ignoreKeyGestures.Clear();
          if (this.ignoreModifiers != null)
            this.ignoreModifiers.Clear();
        }
        this.elements = (List<IInputElement>) null;
        this.handleBindings = (List<KeyBinding>) null;
        this.ignoreKeyGestures = (List<KeyGesture>) null;
        this.ignoreModifiers = (List<ModifierKeys>) null;
      }

      private void element_PreviewKeyDown(object sender, KeyEventArgs e)
      {
        if (this.ignoreModifiers.Contains(Keyboard.Modifiers) || this.ignoreKeyGestures.Any<KeyGesture>((Func<KeyGesture, bool>) (x => x.Matches(sender, (InputEventArgs) e))))
          return;
        KeyBinding keyBinding = this.handleBindings.Where<KeyBinding>((Func<KeyBinding, bool>) (x => x.Gesture.Matches(sender, (InputEventArgs) e))).FirstOrDefault<KeyBinding>();
        if (keyBinding != null)
          keyBinding.Command.Execute((object) null);
        else if (this.Commands != null)
        {
          foreach (RoutedCommand command in this.Commands)
          {
            if (command.InputGestures.Cast<InputGesture>().Any<InputGesture>((Func<InputGesture, bool>) (x => x.Matches(sender, (InputEventArgs) e))))
            {
              command.Execute((object) null, sender as IInputElement);
              break;
            }
          }
        }
        e.Handled = true;
      }
    }
  }
}
