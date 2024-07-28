// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ConsoleHost
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ConsoleHost : WindowsHost
  {
    public static readonly string DisplayFallbackWidthSetting = "Display.FallbackWidth";
    public static readonly string DisplayDisableColorsSetting = "Display.DisableColors";
    public static readonly string DisplayErrorBackgroundSetting = "Display.ErrorBackground";
    public static readonly string DisplayErrorForegroundSetting = "Display.ErrorForeground";
    public static readonly string DisplayWarnBackgroundSetting = "Display.WarnBackground";
    public static readonly string DisplayWarnForegroundSetting = "Display.WarnForeground";
    public static readonly string DisplayInfoBackgroundSetting = "Display.InfoBackground";
    public static readonly string DisplayInfoForegroundSetting = "Display.InfoForeground";
    private static int s_outputWidth = 0;
    private static bool s_disableColors;
    private static ConsoleColor s_errorBackground;
    private static ConsoleColor s_errorForeground;
    private static ConsoleColor s_warnBackground;
    private static ConsoleColor s_warnForeground;
    private static ConsoleColor s_infoBackground;
    private static ConsoleColor s_infoForeground;
    private static bool s_suppressErrors = false;
    private static bool s_displayErrorsInMessageBoxWhenModal = true;
    private static bool s_isCanceled;

    protected ConsoleHost(Icon appIcon)
      : base(appIcon)
    {
      if (!ConsoleUtils.IsStdOutRedirected)
      {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture.GetConsoleFallbackUICulture();
        if (Console.OutputEncoding.CodePage != 65001 && Console.OutputEncoding.CodePage != Thread.CurrentThread.CurrentUICulture.TextInfo.OEMCodePage && Console.OutputEncoding.CodePage != Thread.CurrentThread.CurrentUICulture.TextInfo.ANSICodePage)
          Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
      }
      if (ConsoleUtils.IsStdOutRedirected)
        return;
      if (string.Equals(TFCommonUtil.GetAppSetting(ConsoleHost.DisplayDisableColorsSetting, bool.FalseString), bool.TrueString, StringComparison.OrdinalIgnoreCase))
        ConsoleHost.s_disableColors = true;
      ConsoleHost.s_errorForeground = this.GetColor(ConsoleHost.DisplayErrorForegroundSetting, ConsoleColor.Red);
      ConsoleHost.s_errorBackground = this.GetColor(ConsoleHost.DisplayErrorBackgroundSetting, Console.BackgroundColor != ConsoleColor.Red ? Console.BackgroundColor : ConsoleColor.Black);
      ConsoleHost.s_warnForeground = this.GetColor(ConsoleHost.DisplayWarnForegroundSetting, ConsoleColor.Yellow);
      ConsoleHost.s_warnBackground = this.GetColor(ConsoleHost.DisplayWarnBackgroundSetting, Console.BackgroundColor != ConsoleColor.Yellow ? Console.BackgroundColor : ConsoleColor.Black);
      ConsoleHost.s_infoForeground = this.GetColor(ConsoleHost.DisplayInfoForegroundSetting, ConsoleColor.Cyan);
      ConsoleHost.s_infoBackground = this.GetColor(ConsoleHost.DisplayInfoBackgroundSetting, Console.BackgroundColor != ConsoleColor.Cyan ? Console.BackgroundColor : ConsoleColor.Black);
    }

    public override RuntimeEnvironmentFlags EnvironmentFlags => RuntimeEnvironmentFlags.Windows | RuntimeEnvironmentFlags.Console;

    public override void Write(LogFlags flags, LogCategory category, string message)
    {
      LogFlags logFlags = flags & (LogFlags.Information | LogFlags.Warning | LogFlags.Error);
      bool toStandardOutput = true;
      try
      {
        switch (logFlags)
        {
          case LogFlags.Information:
            this.SetInfoColors();
            break;
          case LogFlags.Warning:
            toStandardOutput = false;
            this.SetWarningColors();
            break;
          case LogFlags.Error:
            toStandardOutput = false;
            this.SetErrorColors();
            break;
        }
        if (LogFlags.Error == logFlags)
        {
          if (ConsoleHost.s_suppressErrors)
            goto label_12;
        }
        if (LogFlags.Error == logFlags && UIHost.IsModal && ConsoleHost.s_displayErrorsInMessageBoxWhenModal)
          this.ShowModalErrorMessageBox(message);
        else if ((flags & LogFlags.Persist) == LogFlags.Normal)
          ConsoleUtils.Write(message, toStandardOutput);
      }
      finally
      {
        this.ResetColors();
      }
label_12:
      ConsoleHost.ThrowIfCanceled();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ShowModalErrorMessageBox(string message)
    {
      int num = (int) this.ShowMessageBox(this.DefaultParentWindow, message, (string) null, ClientResources.ErrorTitle(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }

    public override string NewLine => Console.Out.NewLine;

    public override int OutputWidth
    {
      get
      {
        if (ConsoleHost.s_outputWidth == 0)
        {
          try
          {
            ConsoleHost.s_outputWidth = Console.BufferWidth;
          }
          catch (IOException ex)
          {
            ConsoleHost.s_outputWidth = TFCommonUtil.GetAppSettingAsInt(ConsoleHost.DisplayFallbackWidthSetting, 80);
          }
        }
        return ConsoleHost.s_outputWidth;
      }
    }

    public override Encoding OutputEncoding => !ConsoleUtils.IsStdOutRedirected ? Console.Out.Encoding : ConsoleUtils.RedirectedOutputEncoding;

    protected override void OnModalDialogFormActivate(object sender, EventArgs e)
    {
      Form form = (Form) sender;
      if (form == null)
        return;
      ConsoleHost.FocusOrBlink(form.Handle);
      form.Activated -= new EventHandler(((WindowsHost) this).OnModalDialogFormActivate);
    }

    public static void FocusOrBlink(IntPtr hwnd)
    {
      IntPtr foregroundWindow = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetForegroundWindow();
      if (!(foregroundWindow != hwnd))
        return;
      IntPtr consoleWindow = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetConsoleWindow();
      if (foregroundWindow != consoleWindow)
      {
        int num = 1;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SystemParametersInfo(8196, 0, ref num, 0);
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.FlashWindowEx(ref new Microsoft.TeamFoundation.Common.Internal.NativeMethods.FLASHWINFO()
        {
          cbSize = (uint) Marshal.SizeOf(typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.FLASHWINFO)),
          dwFlags = 15U,
          dwTimeout = 0U,
          uCount = (uint) num,
          hwnd = hwnd
        });
      }
      else
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SetForegroundWindow(hwnd);
    }

    public static string ReadLine()
    {
      string str = Console.ReadLine();
      if (str == null)
        Thread.Sleep(500);
      ConsoleHost.ThrowIfCanceled();
      return str;
    }

    private ConsoleColor GetColor(string name, ConsoleColor fallback)
    {
      string appSetting = TFCommonUtil.GetAppSetting(name, (string) null);
      if (appSetting != null)
      {
        try
        {
          return (ConsoleColor) Enum.Parse(typeof (ConsoleColor), appSetting, true);
        }
        catch (ArgumentException ex)
        {
        }
      }
      return fallback;
    }

    private void SetErrorColors()
    {
      if (ConsoleUtils.IsStdErrRedirected || ConsoleHost.s_disableColors)
        return;
      Console.BackgroundColor = ConsoleHost.s_errorBackground;
      Console.ForegroundColor = ConsoleHost.s_errorForeground;
    }

    private void SetWarningColors()
    {
      if (ConsoleUtils.IsStdErrRedirected || ConsoleHost.s_disableColors)
        return;
      Console.BackgroundColor = ConsoleHost.s_warnBackground;
      Console.ForegroundColor = ConsoleHost.s_warnForeground;
    }

    private void SetInfoColors()
    {
      if (ConsoleUtils.IsStdOutRedirected || ConsoleHost.s_disableColors)
        return;
      Console.BackgroundColor = ConsoleHost.s_infoBackground;
      Console.ForegroundColor = ConsoleHost.s_infoForeground;
    }

    private void ResetColors()
    {
      if (ConsoleHost.s_disableColors)
        return;
      Console.ResetColor();
    }

    public void ShowProgress(string message)
    {
    }

    public new static void Initialize() => ConsoleHost.Initialize((Icon) null);

    public new static void Initialize(Icon appIcon)
    {
      if (UIHost.Instance != null)
        return;
      UIHost.Instance = (TeamFoundationRuntimeHost) new ConsoleHost(appIcon);
    }

    public static void WriteBuffer(byte[] rawBytes, int numBytesToWrite) => ConsoleHost.WriteBuffer(rawBytes, numBytesToWrite, false);

    public static void WriteBuffer(byte[] rawBytes, int numBytesToWrite, bool stdErr)
    {
      ConsoleUtils.Write(rawBytes, numBytesToWrite, !stdErr);
      ConsoleHost.ThrowIfCanceled();
    }

    public static bool IsStdOutRedirected => ConsoleUtils.IsStdOutRedirected;

    public static bool IsStdErrRedirected => ConsoleUtils.IsStdErrRedirected;

    public static bool IsStdInRedirected => ConsoleUtils.IsStdInRedirected;

    public static Stream GetOutputStream()
    {
      ConsoleHost.ThrowIfCanceled();
      return Console.OpenStandardOutput();
    }

    public static Stream GetErrorStream()
    {
      ConsoleHost.ThrowIfCanceled();
      return Console.OpenStandardError();
    }

    public static bool SuppressErrors
    {
      get => ConsoleHost.s_suppressErrors;
      set => ConsoleHost.s_suppressErrors = value;
    }

    public static bool DisplayErrorsInMessageBoxWhenModal
    {
      get => ConsoleHost.s_displayErrorsInMessageBoxWhenModal;
      set => ConsoleHost.s_displayErrorsInMessageBoxWhenModal = value;
    }

    public static bool Canceled
    {
      get => ConsoleHost.s_isCanceled;
      set => ConsoleHost.s_isCanceled = value;
    }

    public static void ThrowIfCanceled()
    {
      if (ConsoleHost.s_isCanceled)
        throw new System.OperationCanceledException();
    }
  }
}
