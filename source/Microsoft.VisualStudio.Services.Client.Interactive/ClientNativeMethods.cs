// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.ClientNativeMethods
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Microsoft.VisualStudio.Services.Client
{
  internal class ClientNativeMethods
  {
    public const int ERROR_SUCCESS = 0;
    public const int ERROR_INSUFFICIENT_BUFFER = 122;
    public const int ERROR_NOT_CAPABLE = 775;
    public const int SPI_GETFOREGROUNDFLASHCOUNT = 8196;
    public const int FLASHW_CAPTION = 1;
    public const int FLASHW_TRAY = 2;
    public const int FLASHW_ALL = 3;
    public const int LOGPIXELSX = 88;
    public const int CRED_PACK_PROTECTED_CREDENTIALS = 1;
    public const int CREDUI_MAX_USERNAME_LENGTH = 513;
    public const int CREDUI_MAX_PASSWORD_LENGTH = 256;
    public const int CREDUI_MAX_CAPTION_LENGTH = 128;
    public const int CREDUI_MAX_MESSAGE_LENGTH = 32767;
    public const int CREDUIWIN_CHECKBOX = 2;
    public const int CREDUIWIN_AUTHPACKAGE_ONLY = 16;

    public static IntPtr GetDefaultParentWindow()
    {
      Application current = Application.Current;
      if (current != null && current.CheckAccess())
      {
        Window mainWindow = current.MainWindow;
        if (mainWindow != null)
        {
          IntPtr handle = new WindowInteropHelper(mainWindow).Handle;
          if (handle != IntPtr.Zero)
            return handle;
        }
      }
      ClientNativeMethods.GUITHREADINFO lpgui = new ClientNativeMethods.GUITHREADINFO();
      lpgui.cbSize = Marshal.SizeOf<ClientNativeMethods.GUITHREADINFO>(lpgui);
      return ClientNativeMethods.GetGUIThreadInfo((uint) ClientNativeMethods.GetCurrentThreadId(), ref lpgui) && lpgui.hwndActive != IntPtr.Zero && ClientNativeMethods.IsWindowVisible(lpgui.hwndActive) ? lpgui.hwndActive : IntPtr.Zero;
    }

    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern int FlashWindowEx(ref ClientNativeMethods.FLASHWINFO fwi);

    [DllImport("user32")]
    public static extern bool SystemParametersInfo(
      int nAction,
      int nParam,
      ref int value,
      int ignore);

    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("kernel32.dll")]
    public static extern int GetCurrentThreadId();

    [DllImport("user32")]
    public static extern bool IsWindowVisible(IntPtr hwnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool GetGUIThreadInfo(
      uint idThread,
      ref ClientNativeMethods.GUITHREADINFO lpgui);

    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    [DllImport("IEFRAME.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int SetQueryNetSessionCount(SessionOp sessionOp);

    [DllImport("credui.dll", CharSet = CharSet.Unicode)]
    public static extern int CredUIPromptForWindowsCredentials(
      ref ClientNativeMethods.CREDUI_INFO pUiInfo,
      int dwAuthError,
      ref uint pulAuthPackage,
      byte[] pvInAuthBuffer,
      uint ulInAuthBufferSize,
      out IntPtr ppvOutAuthBuffer,
      out uint pulOutAuthBufferSize,
      ref bool pfSave,
      int dwFlags);

    [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredPackAuthenticationBuffer(
      int dwFlags,
      string pszUserName,
      string pszPassword,
      byte[] pPackedCredentials,
      ref uint pcbPackedCredentials);

    [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredUnPackAuthenticationBuffer(
      int dwFlags,
      IntPtr pAuthBuffer,
      uint cbAuthBuffer,
      StringBuilder pszUserName,
      ref uint pcchMaxUserName,
      StringBuilder pszDomainName,
      ref uint pcchMaxDomainName,
      StringBuilder pszPassword,
      ref uint pcchMaxPassword);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void ZeroMemory(IntPtr address, uint byteCount);

    public struct FLASHWINFO
    {
      public uint cbSize;
      public IntPtr hwnd;
      public uint dwFlags;
      public uint uCount;
      public uint dwTimeout;
    }

    public struct GUITHREADINFO
    {
      public int cbSize;
      public int flags;
      public IntPtr hwndActive;
      public IntPtr hwndFocus;
      public IntPtr hwndCapture;
      public IntPtr hwndMenuOwner;
      public IntPtr hwndMoveSize;
      public IntPtr hwndCaret;
      public Rectangle rcCaret;
    }

    public struct CREDUI_INFO
    {
      public int cbSize;
      public IntPtr hwndParent;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszMessageText;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszCaptionText;
      public IntPtr hbmBanner;
    }
  }
}
