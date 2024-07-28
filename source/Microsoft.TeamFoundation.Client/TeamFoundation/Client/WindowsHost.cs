// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WindowsHost
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WindowsHost : TeamFoundationRuntimeHost
  {
    protected Icon m_appIcon;
    private static UIHostColors s_uiHostColors;
    private static WinformsStyler s_winformsStyler;
    private static int s_guiModeRefCount = 0;
    private static IWin32Window m_defaultParentWindowImpl = (IWin32Window) null;
    private static Icon s_defaultIcon = new Icon(typeof (WindowsHost), "tf.ico");

    protected WindowsHost(Icon appIcon)
    {
      if (appIcon == null)
        this.m_appIcon = WindowsHost.s_defaultIcon;
      else
        this.m_appIcon = appIcon;
    }

    public override DialogResult ShowMessageBox(
      IWin32Window parent,
      string text,
      string helpTopic,
      string caption,
      MessageBoxButtons buttons,
      MessageBoxIcon icon,
      MessageBoxDefaultButton defaultButton)
    {
      return WindowsHost.ShowMessageBoxImpl(parent, text, helpTopic, caption, buttons, icon, defaultButton);
    }

    public override void Write(LogFlags flags, LogCategory category, string message) => WindowsHost.WriteImpl(flags, category, message);

    public override IWin32Window DefaultParentWindow => WindowsHost.DefaultParentWindowImpl;

    public override RuntimeEnvironmentFlags EnvironmentFlags => RuntimeEnvironmentFlags.Windows;

    public override Icon AppIcon => this.m_appIcon;

    public override bool DisplayHelp(string helpTopic) => WindowsHost.DisplayHelpImpl(helpTopic);

    public override UIHostColors HostColors => WindowsHost.HostColorsImpl;

    public override WinformsStyler WinformsStyler => WindowsHost.WinformsStylerImpl;

    public override DialogResult ShowModalDialog(
      Form form,
      IWin32Window parent,
      bool showHelpButton)
    {
      bool flag = parent == null;
      form.HelpButton = showHelpButton;
      form.ShowInTaskbar = flag;
      form.MinimizeBox = flag;
      form.ShowIcon = flag;
      Icon icon = form.Icon;
      if (flag)
      {
        form.Activated += new EventHandler(this.OnModalDialogFormActivate);
        if (this.AppIcon != null)
          form.Icon = this.AppIcon;
      }
      int num = (int) form.ShowDialog(parent);
      if (!flag)
        return (DialogResult) num;
      form.Activated -= new EventHandler(this.OnModalDialogFormActivate);
      if (this.AppIcon == null)
        return (DialogResult) num;
      form.Icon = icon;
      return (DialogResult) num;
    }

    public override Font Font => WindowsHost.FontImpl;

    public override WaitCursor GetWaitCursor() => WindowsHost.GetWaitCursorImpl();

    protected virtual void OnModalDialogFormActivate(object sender, EventArgs e)
    {
      Form form = (Form) sender;
      if (form == null)
        return;
      if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetForegroundWindow() != form.Handle)
      {
        int num = 1;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SystemParametersInfo(8196, 0, ref num, 0);
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.FlashWindowEx(ref new Microsoft.TeamFoundation.Common.Internal.NativeMethods.FLASHWINFO()
        {
          cbSize = (uint) Marshal.SizeOf(typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.FLASHWINFO)),
          dwFlags = 3U,
          dwTimeout = 0U,
          uCount = (uint) num,
          hwnd = form.Handle
        });
      }
      form.Activated -= new EventHandler(this.OnModalDialogFormActivate);
    }

    internal static DialogResult ShowModalDialogImpl(
      Form form,
      IWin32Window parent,
      bool showHelpButton)
    {
      bool flag = parent == null;
      form.HelpButton = showHelpButton;
      form.ShowInTaskbar = flag;
      form.MinimizeBox = flag;
      form.ShowIcon = flag;
      Icon icon = form.Icon;
      if (flag)
        form.Icon = WindowsHost.s_defaultIcon;
      int num = (int) form.ShowDialog(parent);
      if (!flag)
        return (DialogResult) num;
      form.Icon = icon;
      return (DialogResult) num;
    }

    internal static bool DisplayHelpImpl(string helpTopic)
    {
      try
      {
        HelpUtilities.LaunchHelpTopic(helpTopic);
        return true;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceAndDebugFailException(ex);
        return false;
      }
    }

    internal static bool IsRightToLeft(IWin32Window owner) => owner is Control control ? control.RightToLeft == RightToLeft.Yes : CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

    internal static DialogResult ShowMessageBoxImpl(
      IWin32Window parent,
      string text,
      string helpTopic,
      string caption,
      MessageBoxButtons buttons,
      MessageBoxIcon icon,
      MessageBoxDefaultButton defaultButton)
    {
      MessageBoxOptions options = !WindowsHost.IsRightToLeft(parent) ? (MessageBoxOptions) 0 : MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
      if (caption == null)
        caption = WindowsHost.GetCaptionFromMessageBoxIcon(icon);
      return MessageBox.Show(parent, text, caption, buttons, icon, defaultButton, options);
    }

    private static string GetCaptionFromMessageBoxIcon(MessageBoxIcon icon)
    {
      switch (icon)
      {
        case MessageBoxIcon.Hand:
          return ClientResources.ErrorCaption();
        case MessageBoxIcon.Exclamation:
          return ClientResources.WarningCaption();
        case MessageBoxIcon.Asterisk:
          return ClientResources.InformationCaption();
        default:
          return (string) null;
      }
    }

    internal static void WriteImpl(LogFlags flags, LogCategory category, string message)
    {
    }

    public static IWin32Window DefaultParentWindowImpl
    {
      get
      {
        if (WindowsHost.m_defaultParentWindowImpl != null)
          return WindowsHost.m_defaultParentWindowImpl;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.GUITHREADINFO lpgui = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.GUITHREADINFO();
        lpgui.cbSize = Marshal.SizeOf<Microsoft.TeamFoundation.Common.Internal.NativeMethods.GUITHREADINFO>(lpgui);
        return Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetGUIThreadInfo((uint) Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetCurrentThreadId(), ref lpgui) && lpgui.hwndActive != IntPtr.Zero && Microsoft.TeamFoundation.Common.Internal.NativeMethods.IsWindowVisible(lpgui.hwndActive) ? (IWin32Window) new Win32WindowWrapper(lpgui.hwndActive) : (IWin32Window) null;
      }
      set => WindowsHost.m_defaultParentWindowImpl = value;
    }

    internal static UIHostColors HostColorsImpl
    {
      get
      {
        if (WindowsHost.s_uiHostColors == null)
          WindowsHost.s_uiHostColors = new UIHostColors();
        return WindowsHost.s_uiHostColors;
      }
    }

    internal static WinformsStyler WinformsStylerImpl
    {
      get
      {
        if (WindowsHost.s_winformsStyler == null)
          WindowsHost.s_winformsStyler = new WinformsStyler();
        return WindowsHost.s_winformsStyler;
      }
    }

    internal static Font FontImpl => SystemFonts.IconTitleFont;

    internal static WaitCursor GetWaitCursorImpl() => (WaitCursor) new AutoWaitCursor();

    internal static string NewLineImpl => Environment.NewLine;

    internal static int OutputWidthImpl => int.MaxValue;

    internal static Encoding OutputEncodingImpl => Encoding.Default;

    internal static string VsApplicationDataPathImpl => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\VisualStudio\\17.0");

    internal static RegistryKey UserRegistryRootImpl => UIHost.UserRegistryRoot;

    internal static RegistryKey ApplicationRegistryRootImpl => UIHost.ApplicationRegistryRoot;

    internal static bool EnableModelessImpl(bool enable)
    {
      if (!enable)
        WindowsHost.EnterGuiMode();
      else
        WindowsHost.ExitGuiMode();
      return true;
    }

    internal static void ShutdownImpl() => NotificationManager.FlushNotificationQueue();

    private static void EnterGuiMode()
    {
      if (WindowsHost.s_guiModeRefCount == 0)
        UIHost.OnEnterExitModalState(true);
      ++WindowsHost.s_guiModeRefCount;
    }

    private static void ExitGuiMode()
    {
      if (WindowsHost.s_guiModeRefCount > 0)
        --WindowsHost.s_guiModeRefCount;
      if (WindowsHost.s_guiModeRefCount != 0)
        return;
      UIHost.OnEnterExitModalState(false);
    }

    public static void Initialize() => WindowsHost.Initialize((Icon) null);

    public static void Initialize(Icon appIcon)
    {
      if (UIHost.Instance != null)
        return;
      UIHost.Instance = (TeamFoundationRuntimeHost) new WindowsHost(appIcon);
    }
  }
}
