// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.UIHost
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class UIHost
  {
    private static ConnectedUserContext m_connectedUserContext = new ConnectedUserContext();
    private static Font s_font;
    private static UIConfig s_uiConfig;
    private static TeamFoundationRuntimeHost s_instance;
    private static string s_configurationFileName;
    private static bool s_isModal;

    public static object GetService(Type serviceType) => UIHost.Instance == null ? (object) null : UIHost.Instance.GetService(serviceType);

    public static IServiceProvider ServiceProvider => (IServiceProvider) UIHost.Instance;

    public static bool DisplayHelp(string helpTopic)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(helpTopic))
        flag = UIHost.Instance == null ? WindowsHost.DisplayHelpImpl(helpTopic) : UIHost.Instance.DisplayHelp(helpTopic);
      return flag;
    }

    public static IWin32Window GetProperOwnerWindow(IWin32Window owner)
    {
      if (owner != null && owner.Handle != IntPtr.Zero)
      {
        IntPtr num = owner.Handle;
        do
        {
          IntPtr windowLong = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetWindowLong(num, -16);
          if ((windowLong.ToInt64() & (long) int.MinValue) != (long) int.MinValue && (windowLong.ToInt64() & 1073741824L) == 1073741824L)
            num = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetParent(num);
          else
            break;
        }
        while (num != IntPtr.Zero);
        if (num != IntPtr.Zero && num != owner.Handle)
          owner = (IWin32Window) new Win32WindowWrapper(num);
      }
      return owner;
    }

    private static string GetHelpTopicFromParent(IWin32Window parent) => !(parent is BaseDialog baseDialog) || string.IsNullOrEmpty(baseDialog.HelpTopic) ? string.Empty : baseDialog.HelpTopic;

    public static DialogResult ShowModalDialog(Form form) => UIHost.ShowModalDialog(form, UIHost.DefaultParentWindow);

    public static DialogResult ShowModalDialog(Form form, IWin32Window parent)
    {
      BaseDialog baseDialog = form as BaseDialog;
      bool showHelpButton = form.HelpButton;
      IMessageFilter messageFilter;
      if (baseDialog != null)
      {
        messageFilter = baseDialog.MessageFilter;
        if (baseDialog.AlwaysShowHelpButton || !string.IsNullOrEmpty(baseDialog.HelpTopic))
          showHelpButton = true;
      }
      else
        messageFilter = (IMessageFilter) null;
      parent = UIHost.GetProperOwnerWindow(parent);
      try
      {
        if (messageFilter != null)
          Application.AddMessageFilter(messageFilter);
        UIHost.OnBeforeShowDialog(form);
        using (new AutoModal())
        {
          using (new FocusRestorer())
            return UIHost.Instance != null ? UIHost.Instance.ShowModalDialog(form, parent, showHelpButton) : WindowsHost.ShowModalDialogImpl(form, parent, showHelpButton);
        }
      }
      finally
      {
        if (messageFilter != null)
          Application.RemoveMessageFilter(messageFilter);
      }
    }

    public static void WriteError(LogCategory category, string messageFormat, params object[] args) => UIHost.WriteLine(LogFlags.Error, category, messageFormat, args);

    public static void WriteError(LogCategory category, string message) => UIHost.WriteLine(LogFlags.Error, category, message);

    public static void WriteWarning(
      LogCategory category,
      string messageFormat,
      params object[] args)
    {
      UIHost.WriteLine(LogFlags.Warning, category, messageFormat, args);
    }

    public static void WriteWarning(LogCategory category, string message) => UIHost.WriteLine(LogFlags.Warning, category, message);

    public static void WriteInfo(LogCategory category, string message) => UIHost.WriteLine(LogFlags.Information, category, message);

    public static void WriteInfo(LogCategory category, string messageFormat, params object[] args) => UIHost.WriteLine(LogFlags.Information, category, messageFormat, args);

    public static void WriteIndented(
      LogCategory category,
      string indent,
      string messageFormat,
      params object[] args)
    {
      UIHost.WriteIndented(category, indent, string.Format((IFormatProvider) CultureInfo.CurrentCulture, messageFormat, args));
    }

    public static void WriteIndented(LogCategory category, string indent, string inputString)
    {
      if (string.IsNullOrEmpty(inputString))
        return;
      int startIndex = 0;
      int index;
      for (index = 0; index < inputString.Length; ++index)
      {
        if (inputString[index] == '\n')
        {
          UIHost.Write(category, indent);
          UIHost.Write(category, inputString.Substring(startIndex, index - startIndex + 1));
          startIndex = index + 1;
        }
      }
      if (index - startIndex <= 0)
        return;
      UIHost.Write(category, indent);
      UIHost.WriteLine(category, inputString.Substring(startIndex, index - startIndex));
    }

    public static void WriteLine(LogCategory category, string messageFormat, params object[] args) => UIHost.WriteLine(category, string.Format((IFormatProvider) CultureInfo.CurrentCulture, messageFormat, args));

    public static void WriteLine(LogCategory category, string message) => UIHost.WriteLine(LogFlags.Normal, category, message);

    public static void WriteLine(LogCategory category) => UIHost.Write(LogFlags.Normal, category, UIHost.NewLine);

    public static void WriteLine(
      LogFlags flags,
      LogCategory category,
      string messageFormat,
      params object[] args)
    {
      UIHost.WriteLine(flags, category, string.Format((IFormatProvider) CultureInfo.CurrentCulture, messageFormat, args));
    }

    public static void WriteLine(LogFlags flags, LogCategory category, string message) => UIHost.Write(flags, category, message + UIHost.NewLine);

    public static void WriteLine(LogFlags flags, LogCategory category) => UIHost.Write(flags, category, UIHost.NewLine);

    public static void Write(LogCategory category, string message) => UIHost.Write(LogFlags.Normal, category, message);

    public static void Write(LogCategory category, string messageFormat, params object[] args) => UIHost.Write(LogFlags.Normal, category, string.Format((IFormatProvider) CultureInfo.CurrentCulture, messageFormat, args));

    public static void Write(
      LogFlags flags,
      LogCategory category,
      string messageFormat,
      params object[] args)
    {
      UIHost.Write(flags, category, string.Format((IFormatProvider) CultureInfo.CurrentCulture, messageFormat, args));
    }

    public static void Write(LogFlags flags, LogCategory category, string message)
    {
      if (message == null)
        message = string.Empty;
      if (UIHost.Instance != null)
        UIHost.Instance.Write(flags, category, message);
      else
        WindowsHost.WriteImpl(flags, category, message);
    }

    public static string Truncate(string text, int width) => StringUtil.Truncate(text, width, UIHost.OutputEncoding);

    public static int CalculateWidth(string text) => StringUtil.CalculateWidth(text, UIHost.OutputEncoding);

    public static string EllipsisAtFront(string fullString, int fieldWidth) => UIHost.CalculateWidth(fullString) <= fieldWidth ? fullString : "..." + StringUtil.TruncateLeft(fullString, fieldWidth - 3, UIHost.OutputEncoding);

    public static string EllipsisAtBack(string fullString, int fieldWidth) => UIHost.CalculateWidth(fullString) <= fieldWidth ? fullString : StringUtil.Truncate(fullString, fieldWidth - 3, UIHost.OutputEncoding) + "...";

    public static DialogResult ShowAndTraceException(Exception ex) => UIHost.ShowException(ex);

    public static DialogResult ShowException(Exception e) => UIHost.ShowException((IWin32Window) null, e);

    public static DialogResult ShowException(IWin32Window parent, Exception e) => UIHost.ShowException(parent, e, (string) null, ClientResources.ExceptionCaption());

    public static DialogResult ShowException(Exception e, string helpTopic, string caption) => UIHost.ShowException((IWin32Window) null, e, helpTopic, caption);

    public static DialogResult ShowException(
      IWin32Window parent,
      Exception e,
      string helpTopic,
      string caption)
    {
      string exceptionMessage = TFCommonUtil.GetExceptionMessage(e);
      if (string.IsNullOrEmpty(helpTopic))
        helpTopic = e.HelpLink;
      TeamFoundationTrace.TraceException(e);
      return UIHost.ShowError(parent, exceptionMessage, helpTopic, caption);
    }

    public static DialogResult ShowError(string text) => UIHost.ShowError((IWin32Window) null, text);

    public static DialogResult ShowError(IWin32Window parent, string text) => UIHost.ShowError(parent, text, (string) null, (string) null);

    public static DialogResult ShowError(string text, string helpTopic, string caption) => UIHost.ShowError((IWin32Window) null, text, helpTopic, caption);

    public static DialogResult ShowError(
      IWin32Window parent,
      string text,
      string helpTopic,
      string caption)
    {
      return UIHost.ShowMessageBox(parent, text, helpTopic, caption, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }

    public static DialogResult ShowWarning(string text) => UIHost.ShowWarning((IWin32Window) null, text);

    public static DialogResult ShowWarning(IWin32Window parent, string text) => UIHost.ShowWarning(parent, text, (string) null, (string) null);

    public static DialogResult ShowWarning(string text, string helpTopic, string caption) => UIHost.ShowWarning((IWin32Window) null, text, helpTopic, caption);

    public static DialogResult ShowWarning(
      IWin32Window parent,
      string text,
      string helpTopic,
      string caption)
    {
      return UIHost.ShowMessageBox(parent, text, helpTopic, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
    }

    public static DialogResult ShowInformation(string text) => UIHost.ShowInformation((IWin32Window) null, text);

    public static DialogResult ShowInformation(IWin32Window parent, string text) => UIHost.ShowInformation(parent, text, (string) null, (string) null);

    public static DialogResult ShowInformation(string text, string helpTopic, string caption) => UIHost.ShowInformation((IWin32Window) null, text, helpTopic, caption);

    public static DialogResult ShowInformation(
      IWin32Window parent,
      string text,
      string helpTopic,
      string caption)
    {
      return UIHost.ShowMessageBox(parent, text, helpTopic, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
    }

    public static DialogResult ShowMessageBox(
      string text,
      string helpTopic,
      string caption,
      MessageBoxButtons buttons,
      MessageBoxIcon icon,
      MessageBoxDefaultButton defaultButton)
    {
      return UIHost.ShowMessageBox((IWin32Window) null, text, helpTopic, caption, buttons, icon, defaultButton);
    }

    public static DialogResult ShowMessageBox(
      IWin32Window parent,
      string text,
      string helpTopic,
      string caption,
      MessageBoxButtons buttons,
      MessageBoxIcon icon,
      MessageBoxDefaultButton defaultButton)
    {
      try
      {
        parent = parent != null ? UIHost.GetProperOwnerWindow(parent) : UIHost.DefaultParentWindow;
        if (string.IsNullOrEmpty(helpTopic))
          helpTopic = UIHost.GetHelpTopicFromParent(parent);
        DialogResult dialogResult;
        using (new AutoModal())
        {
          using (new FocusRestorer())
            dialogResult = UIHost.Instance == null ? WindowsHost.ShowMessageBoxImpl(parent, text, helpTopic, caption, buttons, icon, defaultButton) : UIHost.Instance.ShowMessageBox(parent, text, helpTopic, caption, buttons, icon, defaultButton);
        }
        if (TeamFoundationTrace.IsTracingEnabled)
        {
          string format = "UIHost.ShowMessageBox: result={0} caption='{1}' text='{2}'";
          switch (icon)
          {
            case MessageBoxIcon.Hand:
              TeamFoundationTrace.Error(format, (object) dialogResult, (object) caption, (object) text);
              break;
            case MessageBoxIcon.Exclamation:
              TeamFoundationTrace.Warning(format, (object) dialogResult, (object) caption, (object) text);
              break;
            default:
              TeamFoundationTrace.Info(format, (object) dialogResult, (object) caption, (object) text);
              break;
          }
        }
        return dialogResult;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        return DialogResult.Cancel;
      }
    }

    public static bool EnableModeless(bool enable) => UIHost.Instance != null ? UIHost.Instance.EnableModeless(enable) : WindowsHost.EnableModelessImpl(enable);

    public static IWin32Window DefaultParentWindow => UIHost.Instance == null ? WindowsHost.DefaultParentWindowImpl : UIHost.Instance.DefaultParentWindow;

    public static IntPtr DefaultParentWindowHandle
    {
      get
      {
        IWin32Window defaultParentWindow = UIHost.DefaultParentWindow;
        return defaultParentWindow == null ? IntPtr.Zero : defaultParentWindow.Handle;
      }
    }

    public static void InitializeContainer(ContainerControl container) => UIHost.UpdatePrimaryFont(UIHost.Font, (Control) container, true);

    public static int OnBroadcastMessage(int msg, IntPtr wParam, IntPtr lParam)
    {
      switch (msg)
      {
        case 21:
        case 794:
          if (UIHost.Font != null && UIHost.Font != UIHost.GetInstanceFont())
          {
            UIHost.s_font = (Font) null;
            UIHost.OnFontChanged();
            goto case 785;
          }
          else
            goto case 785;
        case 785:
          UIHost.OnColorChanged();
          break;
      }
      return 0;
    }

    public static Font SystemFont => Font.FromHfont(Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetStockObject(17));

    private static Font GetInstanceFont() => UIHost.Instance != null ? UIHost.Instance.Font : WindowsHost.FontImpl;

    public static Font Font
    {
      get
      {
        if (UIHost.s_font == null)
          UIHost.s_font = UIHost.GetInstanceFont();
        return UIHost.s_font == null ? UIHost.SystemFont : UIHost.s_font;
      }
    }

    public static Icon AppIcon => UIHost.Instance != null ? UIHost.Instance.AppIcon : (Icon) null;

    public static Encoding OutputEncoding => UIHost.Instance != null ? UIHost.Instance.OutputEncoding : WindowsHost.OutputEncodingImpl;

    internal static RuntimeEnvironmentFlags EnvironmentFlags => UIHost.Instance != null ? UIHost.Instance.EnvironmentFlags : RuntimeEnvironmentFlags.None;

    public static WaitCursor GetWaitCursor() => UIHost.Instance != null ? UIHost.Instance.GetWaitCursor() : WindowsHost.GetWaitCursorImpl();

    internal static string ExtractTFSDisplayName(string url)
    {
      try
      {
        Uri uri = new Uri(url);
        int startIndex = url.IndexOf(uri.Host, StringComparison.OrdinalIgnoreCase);
        return uri.AbsolutePath.Length <= 1 || uri.AbsolutePath.Equals("/tfs", StringComparison.OrdinalIgnoreCase) || uri.AbsolutePath.Equals("/tfs/", StringComparison.OrdinalIgnoreCase) || startIndex < 0 ? uri.Host : url.Substring(startIndex);
      }
      catch
      {
        return url;
      }
    }

    internal static string ExtractTFSDisplayName(Uri url) => UIHost.ExtractTFSDisplayName(url.ToString());

    public static void Beep()
    {
      try
      {
        using (SoundPlayer soundPlayer = new SoundPlayer())
          soundPlayer.PlaySync();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceAndDebugFailException(ex);
      }
    }

    public static void UpdatePrimaryFont(Font font, Control control, bool recurse)
    {
      UIHost.UpdatePrimaryFontHelper(font, control, recurse);
      control.PerformLayout();
    }

    private static void UpdatePrimaryFontHelper(Font font, Control control, bool recurse)
    {
      control.SuspendLayout();
      if (recurse && control.HasChildren)
      {
        foreach (Control control1 in (ArrangedElementCollection) control.Controls)
          UIHost.UpdatePrimaryFontHelper(font, control1, true);
      }
      if (control.Font != font)
      {
        if (font.Style == control.Font.Style)
        {
          control.Font = font;
        }
        else
        {
          try
          {
            control.Font = new Font(font, control.Font.Style);
          }
          catch (Exception ex)
          {
            control.Font = font;
          }
        }
      }
      if (SystemInformation.HighContrast && control is Button)
        ((ButtonBase) control).UseVisualStyleBackColor = false;
      control.ResumeLayout(false);
    }

    public static event EventHandler ColorChanged;

    internal static void OnColorChanged()
    {
      try
      {
        EventHandler colorChanged = UIHost.ColorChanged;
        if (colorChanged == null)
          return;
        colorChanged((object) UIHost.Instance, EventArgs.Empty);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    public static event EventHandler FontChanged;

    internal static void OnFontChanged()
    {
      try
      {
        EventHandler fontChanged = UIHost.FontChanged;
        if (fontChanged == null)
          return;
        fontChanged((object) UIHost.Instance, EventArgs.Empty);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    public static void UpdateCommandStatus()
    {
      if (UIHost.Instance == null)
        return;
      UIHost.Instance.UpdateCommandStatus();
    }

    public static UIConfig UIConfig
    {
      get
      {
        if (UIHost.s_uiConfig == null)
          UIHost.s_uiConfig = new UIConfig(UIHost.ConfigurationFileName);
        return UIHost.s_uiConfig;
      }
    }

    private static void EnsureConfig()
    {
      string clientCacheDirectory = TfsConnection.ClientCacheDirectory;
      if (!Directory.Exists(clientCacheDirectory))
      {
        try
        {
          Directory.CreateDirectory(clientCacheDirectory);
        }
        catch (Exception ex)
        {
          UIHost.WriteError(LogCategory.General, TFCommonUtil.GetExceptionMessage(ex));
          return;
        }
      }
      UIHost.s_configurationFileName = Path.Combine(clientCacheDirectory, "UISettings.Config");
    }

    public static string NewLine => UIHost.Instance != null ? UIHost.Instance.NewLine : WindowsHost.NewLineImpl;

    public static int OutputWidth => UIHost.Instance != null ? UIHost.Instance.OutputWidth : WindowsHost.OutputWidthImpl;

    public static string VsApplicationDataPath => UIHost.Instance != null ? UIHost.Instance.VsApplicationDataPath : WindowsHost.VsApplicationDataPathImpl;

    public static RegistryKey UserRegistryRoot => TeamFoundationEnvironment.OpenOrCreateRootUserRegistryKey();

    public static RegistryKey ApplicationRegistryRoot => TeamFoundationEnvironment.OpenRootVisualStudioRegistryKey();

    public static RegistryKey TryGetUserRegistryRoot()
    {
      try
      {
        return UIHost.UserRegistryRoot;
      }
      catch
      {
      }
      return (RegistryKey) null;
    }

    public static RegistryKey TryGetApplicationRegistryRoot()
    {
      try
      {
        return UIHost.ApplicationRegistryRoot;
      }
      catch
      {
      }
      return (RegistryKey) null;
    }

    public static string ConfigurationFileName
    {
      get
      {
        UIHost.EnsureConfig();
        return UIHost.s_configurationFileName;
      }
    }

    public static void Shutdown()
    {
      if (UIHost.Instance != null)
        UIHost.Instance.Shutdown();
      else
        WindowsHost.ShutdownImpl();
    }

    public static UIHostColors Colors => UIHost.Instance != null ? UIHost.Instance.HostColors : WindowsHost.HostColorsImpl;

    public static WinformsStyler WinformsStyler => UIHost.Instance != null ? UIHost.Instance.WinformsStyler : WindowsHost.WinformsStylerImpl;

    public static Bitmap ThemeBitmap(Bitmap image, System.Drawing.Color backgroundColor) => UIHost.Instance != null ? UIHost.Instance.ThemeBitmap(image, backgroundColor) : image;

    public static ConnectedUserContext ConnectedUserContext => UIHost.m_connectedUserContext;

    public static event ModalStateEventHandler EnterModalState;

    public static event ModalStateEventHandler ExitModalState;

    public static void OnEnterExitModalState(bool enterModal)
    {
      ModalStateEventHandler stateEventHandler = enterModal ? UIHost.EnterModalState : UIHost.ExitModalState;
      UIHost.s_isModal = enterModal;
      if (stateEventHandler == null)
        return;
      foreach (ModalStateEventHandler invocation in stateEventHandler.GetInvocationList())
      {
        try
        {
          invocation();
        }
        catch (Exception ex)
        {
          UIHost.WriteError(LogCategory.General, TFCommonUtil.GetExceptionMessage(ex));
        }
      }
    }

    public static bool IsModal
    {
      get => UIHost.s_isModal;
      set => UIHost.s_isModal = value;
    }

    public static string FileTypeFilter => ClientResources.FileTypeFilter();

    public static TeamFoundationRuntimeHost Instance
    {
      get => UIHost.s_instance;
      set
      {
        UIHost.s_instance = value;
        if (value == null)
          return;
        UIHost.OnFontChanged();
        UIHost.OnColorChanged();
      }
    }

    public static void SetInstanceInternal(TeamFoundationRuntimeHost host) => UIHost.s_instance = host;

    public static bool IsVistaOrNewer => Environment.OSVersion.Version.Major >= 6;

    public static int GetComboBoxPreferredWidth(ComboBox comboBox)
    {
      int num = 0;
      foreach (object obj in comboBox.Items)
      {
        Size size = TextRenderer.MeasureText(obj.ToString(), comboBox.Font);
        if (size.Width > num)
          num = size.Width;
      }
      return num + SystemInformation.VerticalScrollBarWidth + 8;
    }

    public static event EventHandler<BeforeShowDialogEventArgs> BeforeShowDialog;

    internal static void OnBeforeShowDialog(Form dialog)
    {
      try
      {
        BeforeShowDialogEventArgs e = new BeforeShowDialogEventArgs(dialog);
        EventHandler<BeforeShowDialogEventArgs> beforeShowDialog = UIHost.BeforeShowDialog;
        if (beforeShowDialog == null)
          return;
        beforeShowDialog((object) null, e);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }
  }
}
