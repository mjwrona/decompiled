// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.NotificationManager
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Client
{
  [CLSCompliant(false)]
  public static class NotificationManager
  {
    private static Dictionary<Notification, NotificationManager.MessageInfo> s_infos = NotificationManager.GetMessageInfos();
    private static Queue<NotificationManager.NotificationInstance> s_sendQueue = new Queue<NotificationManager.NotificationInstance>();
    private static Queue<NotificationManager.NotificationInstance> s_holdQueue = new Queue<NotificationManager.NotificationInstance>();
    private static Dictionary<NotificationManager.NotificationHandler, NotificationManager.NotificationRange> s_handlers = new Dictionary<NotificationManager.NotificationHandler, NotificationManager.NotificationRange>();
    private static int s_suspended = 0;
    private static IntPtr s_hwndNotify = IntPtr.Zero;
    private static bool s_initialized = false;
    private static Microsoft.TeamFoundation.Common.Internal.NativeMethods.WndProc s_wndProc;
    private static object s_timerLock = new object();
    private static Timer s_timer;
    private static readonly TimeSpan s_timerInterval = new TimeSpan(0, 0, 5);
    private const string c_WindowClass = "TeamFoundationNotificationWindow";

    static NotificationManager()
    {
      try
      {
        TeamFoundationTrace.Info("Entering NotificationManager()");
        NotificationManager.s_wndProc = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.WndProc(NotificationManager.WndProc);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        throw;
      }
      finally
      {
        TeamFoundationTrace.Info("Leaving NotificationManager()");
      }
    }

    private static Dictionary<Notification, NotificationManager.MessageInfo> GetMessageInfos()
    {
      TeamFoundationTrace.Info("Initializing NotificationManager.GetMessageInfos");
      Dictionary<Notification, NotificationManager.MessageInfo> messageInfos = new Dictionary<Notification, NotificationManager.MessageInfo>(8);
      try
      {
        messageInfos.Add(Notification.VersionControlWorkspaceCreated, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, false));
        messageInfos.Add(Notification.VersionControlWorkspaceDeleted, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, false));
        messageInfos.Add(Notification.VersionControlWorkspaceChanged, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, false));
        messageInfos.Add(Notification.VersionControlPendingChangesChanged, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, false));
        messageInfos.Add(Notification.VersionControlGetCompleted, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, false));
        messageInfos.Add(Notification.VersionControlChangesetReconciled, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, false));
        messageInfos.Add(Notification.VersionControlFolderContentChanged, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, false));
        messageInfos.Add(Notification.VersionControlManualMergeClosed, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, true));
        messageInfos.Add(Notification.VersionControlLocalWorkspaceScan, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, false));
        messageInfos.Add(Notification.TfsConnectionUserChanged, new NotificationManager.MessageInfo(NotificationManager.ExpectedParameters.Two, NotificationManager.CollapseMode.SecondArg, false, true));
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        throw;
      }
      TeamFoundationTrace.Info("Leaving NotificationManager.GetMessageInfos");
      return messageInfos;
    }

    private static NotificationManager.MessageInfo GetMessageInfo(Notification notification)
    {
      NotificationManager.MessageInfo messageInfo = (NotificationManager.MessageInfo) null;
      NotificationManager.s_infos.TryGetValue(notification, out messageInfo);
      return messageInfo;
    }

    internal static void EnqueueNotification(Notification notification) => NotificationManager.SendNotification(false, notification, (object) null, (object) null);

    internal static void EnqueueNotification(Notification notification, object param1) => NotificationManager.SendNotification(false, notification, param1, (object) null);

    internal static void EnqueueNotification(
      Notification notification,
      object param1,
      object param2)
    {
      NotificationManager.SendNotification(false, notification, param1, param2);
    }

    internal static void EnqueueNotification(
      Notification notification,
      object param1,
      object param2,
      NotificationManager.GetNotificationParametersCallback callback,
      object userData)
    {
      NotificationManager.SendNotification(false, notification, param1, param2, callback, userData);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void EnqueueNotification(Notification notification, IntPtr wParam, IntPtr lParam) => NotificationManager.SendNotification(false, notification, wParam, lParam);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void EnqueueNotification(
      Notification notification,
      IntPtr wParam,
      IntPtr lParam,
      NotificationManager.GetNotificationParametersCallback callback,
      object userData)
    {
      NotificationManager.SendNotification(false, notification, wParam, lParam, callback, userData);
    }

    internal static void BroadcastNotification(Notification notification) => NotificationManager.SendNotification(true, notification, (object) null, (object) null);

    internal static void BroadcastNotification(Notification notification, object param1) => NotificationManager.SendNotification(true, notification, param1, (object) null);

    internal static void BroadcastNotification(
      Notification notification,
      object param1,
      object param2)
    {
      NotificationManager.SendNotification(true, notification, param1, param2);
    }

    internal static void BroadcastNotification(
      Notification notification,
      object param1,
      object param2,
      NotificationManager.GetNotificationParametersCallback callback,
      object userData)
    {
      NotificationManager.SendNotification(true, notification, param1, param2, callback, userData);
    }

    internal static void BroadcastNotification(
      Notification notification,
      IntPtr wParam,
      IntPtr lParam)
    {
      NotificationManager.SendNotification(true, notification, wParam, lParam);
    }

    internal static void BroadcastNotification(
      Notification notification,
      IntPtr wParam,
      IntPtr lParam,
      NotificationManager.GetNotificationParametersCallback callback,
      object userData)
    {
      NotificationManager.SendNotification(true, notification, wParam, lParam, callback, userData);
    }

    private static void SendNotification(
      bool deliverNow,
      Notification notification,
      object param1,
      object param2)
    {
      NotificationManager.SendNotification(deliverNow, notification, param1, param2, (NotificationManager.GetNotificationParametersCallback) null, (object) null);
    }

    private static void SendNotification(
      bool deliverNow,
      Notification notification,
      object param1,
      object param2,
      NotificationManager.GetNotificationParametersCallback callback,
      object userData)
    {
      NotificationManager.MessageInfo messageInfo = NotificationManager.GetMessageInfo(notification);
      IntPtr wParam = param1 == null ? IntPtr.Zero : new IntPtr(param1.GetHashCode());
      IntPtr lParam = param2 == null ? IntPtr.Zero : new IntPtr(param2.GetHashCode());
      NotificationManager.SendNotification(deliverNow, notification, wParam, lParam, callback, userData, messageInfo);
    }

    internal static void SendNotification(
      bool deliverNow,
      Notification notification,
      IntPtr wParam,
      IntPtr lParam)
    {
      NotificationManager.SendNotification(deliverNow, notification, wParam, lParam, (NotificationManager.GetNotificationParametersCallback) null, (object) null);
    }

    internal static void SendNotification(
      bool deliverNow,
      Notification notification,
      IntPtr wParam,
      IntPtr lParam,
      NotificationManager.GetNotificationParametersCallback callback,
      object userData)
    {
      NotificationManager.MessageInfo messageInfo = NotificationManager.GetMessageInfo(notification);
      NotificationManager.SendNotification(deliverNow, notification, wParam, lParam, callback, userData, messageInfo);
    }

    private static void SendNotification(
      bool deliverNow,
      Notification notification,
      IntPtr wParam,
      IntPtr lParam,
      NotificationManager.GetNotificationParametersCallback callback,
      object userData,
      NotificationManager.MessageInfo info)
    {
      if (!Environment.UserInteractive || info == null)
        return;
      if (!NotificationManager.s_initialized)
        deliverNow = true;
      bool deliverInProcess = info.deliverInProcess;
      if (!deliverNow && info.collapseMode != NotificationManager.CollapseMode.Never)
      {
        bool flag = false;
        lock (NotificationManager.s_sendQueue)
        {
          foreach (NotificationManager.NotificationInstance send in NotificationManager.s_sendQueue)
          {
            if (send.notification == notification && (info.collapseMode == NotificationManager.CollapseMode.Message || send.wParam == wParam && (info.collapseMode == NotificationManager.CollapseMode.FirstArg || !(send.lParam != lParam))))
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
          return;
      }
      if (deliverNow)
      {
        NotificationManager.NotificationInstance immediateNotification = new NotificationManager.NotificationInstance(notification, wParam, lParam, callback, userData, deliverInProcess);
        StringBuilder sb = (StringBuilder) null;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.EnumWindows((Microsoft.TeamFoundation.Common.Internal.NativeMethods.EnumWindowsProc) ((hwnd, dummy) =>
        {
          sb = NotificationManager.GetClassNameHelper(hwnd, sb);
          if (NotificationManager.IsOurWindowClass(sb))
            NotificationManager.PostNotificationInstance(immediateNotification, hwnd);
          return 1;
        }), IntPtr.Zero);
      }
      else
      {
        lock (NotificationManager.s_sendQueue)
          NotificationManager.s_sendQueue.Enqueue(new NotificationManager.NotificationInstance(notification, wParam, lParam, callback, userData, deliverInProcess));
        lock (NotificationManager.s_timerLock)
        {
          if (NotificationManager.s_timer != null)
            return;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          NotificationManager.s_timer = new Timer(NotificationManager.\u003C\u003EO.\u003C0\u003E__TimerCallback ?? (NotificationManager.\u003C\u003EO.\u003C0\u003E__TimerCallback = new System.Threading.TimerCallback(NotificationManager.TimerCallback)), (object) null, NotificationManager.s_timerInterval, new TimeSpan(-1L));
        }
      }
    }

    private static void TimerCallback(object state)
    {
      try
      {
        lock (NotificationManager.s_timerLock)
        {
          if (NotificationManager.s_timer != null)
          {
            NotificationManager.s_timer.Dispose();
            NotificationManager.s_timer = (Timer) null;
          }
        }
        NotificationManager.FlushNotificationQueue();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private static void DeliverLocalNotification(NotificationManager.NotificationInstance ni)
    {
      if (!ni.deliverInProcess)
        return;
      NotificationManager.MessageInfo messageInfo = NotificationManager.GetMessageInfo(ni.notification);
      if (UIHost.IsModal && !messageInfo.deliverInModalDialogState)
        NotificationManager.s_holdQueue.Enqueue(ni);
      else
        NotificationManager.DeliverLocalNotificationNow(ni.notification, ni.wParam, ni.lParam);
    }

    private static void DeliverLocalNotificationNow(
      Notification notification,
      IntPtr wParam,
      IntPtr lParam)
    {
      KeyValuePair<NotificationManager.NotificationHandler, NotificationManager.NotificationRange>[] array;
      lock (NotificationManager.s_handlers)
      {
        array = new KeyValuePair<NotificationManager.NotificationHandler, NotificationManager.NotificationRange>[NotificationManager.s_handlers.Count];
        ((ICollection<KeyValuePair<NotificationManager.NotificationHandler, NotificationManager.NotificationRange>>) NotificationManager.s_handlers).CopyTo(array, 0);
      }
      foreach (KeyValuePair<NotificationManager.NotificationHandler, NotificationManager.NotificationRange> keyValuePair in array)
      {
        NotificationManager.NotificationRange notificationRange = keyValuePair.Value;
        if (notification >= notificationRange.begin && notification <= notificationRange.end)
        {
          NotificationManager.NotificationHandler key = keyValuePair.Key;
          try
          {
            key(notification, wParam, lParam);
          }
          catch (Exception ex)
          {
          }
        }
      }
    }

    private static void PostNotificationInstance(
      NotificationManager.NotificationInstance ni,
      IntPtr hwnd)
    {
      if (hwnd == NotificationManager.s_hwndNotify)
      {
        NotificationManager.DeliverLocalNotification(ni);
      }
      else
      {
        uint int32 = (uint) Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetWindowLong(hwnd, -21).ToInt32();
        IntPtr wParam = ni.wParam;
        IntPtr lParam = ni.lParam;
        if (ni.callback != null)
        {
          try
          {
            if (!ni.callback(ni.notification, int32, ni.userData, ref wParam, ref lParam))
              return;
          }
          catch (Exception ex)
          {
          }
        }
        if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.PostMessage(new HandleRef((object) null, hwnd), (int) ni.notification, wParam, lParam) != 0)
          return;
        Marshal.GetLastWin32Error();
      }
    }

    internal static void FlushNotificationQueue()
    {
      if (NotificationManager.IsSuspended)
        return;
      NotificationManager.NotificationInstance[] sendQueue;
      lock (NotificationManager.s_sendQueue)
      {
        sendQueue = NotificationManager.s_sendQueue.ToArray();
        NotificationManager.s_sendQueue.Clear();
      }
      if (sendQueue.Length == 0)
        return;
      StringBuilder sb = (StringBuilder) null;
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.EnumWindows((Microsoft.TeamFoundation.Common.Internal.NativeMethods.EnumWindowsProc) ((hwnd, lParam) =>
      {
        sb = NotificationManager.GetClassNameHelper(hwnd, sb);
        if (NotificationManager.IsOurWindowClass(sb))
        {
          foreach (NotificationManager.NotificationInstance ni in sendQueue)
            NotificationManager.PostNotificationInstance(ni, hwnd);
        }
        return 1;
      }), IntPtr.Zero);
    }

    private static void UIHost_ExitModalState()
    {
      while (NotificationManager.s_holdQueue.Count > 0)
      {
        NotificationManager.NotificationInstance notificationInstance = NotificationManager.s_holdQueue.Dequeue();
        NotificationManager.DeliverLocalNotificationNow(notificationInstance.notification, notificationInstance.wParam, notificationInstance.lParam);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Suspend() => Interlocked.Increment(ref NotificationManager.s_suspended);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Resume()
    {
      Interlocked.Decrement(ref NotificationManager.s_suspended);
      NotificationManager.FlushNotificationQueue();
    }

    internal static bool IsSuspended => NotificationManager.s_suspended > 0;

    private static bool IsOurWindowClass(StringBuilder sb)
    {
      if (sb.Length != "TeamFoundationNotificationWindow".Length)
        return false;
      for (int index = 0; index < "TeamFoundationNotificationWindow".Length; ++index)
      {
        if ((int) sb[index] != (int) "TeamFoundationNotificationWindow"[index])
          return false;
      }
      return true;
    }

    private static void RegisterClass()
    {
      if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.RegisterClass(new Microsoft.TeamFoundation.Common.Internal.NativeMethods.WNDCLASS()
      {
        hInstance = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetModuleHandle((string) null),
        lpfnWndProc = NotificationManager.s_wndProc,
        lpszClassName = "TeamFoundationNotificationWindow"
      }) != (short) 0)
        return;
      Marshal.GetLastWin32Error();
    }

    private static void CreateWindow()
    {
      StringBuilder lpFilename = new StringBuilder(256);
      if (NotificationManager.GetModuleFileName(IntPtr.Zero, lpFilename, lpFilename.Capacity) == 0)
        throw new Win32Exception();
      for (int index = lpFilename.Length - 1; index >= 0; --index)
      {
        if (lpFilename[index] == '\\')
        {
          lpFilename.Remove(0, index + 1);
          break;
        }
      }
      lpFilename.Append(':');
      lpFilename.Append(NotificationManager.GetCurrentProcessId());
      lpFilename.Append(':');
      lpFilename.Append(AppDomain.CurrentDomain.Id);
      NotificationManager.s_hwndNotify = Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateWindowEx(0, "TeamFoundationNotificationWindow", lpFilename.ToString(), 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetModuleHandle((string) null), (object) null);
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.SetWindowLong(NotificationManager.s_hwndNotify, -21, new IntPtr(5));
    }

    private static void DestroyWindow()
    {
      try
      {
        if (!(NotificationManager.s_hwndNotify != IntPtr.Zero) || Microsoft.TeamFoundation.Common.Internal.NativeMethods.DestroyWindow(NotificationManager.s_hwndNotify))
          return;
        Marshal.GetLastWin32Error();
      }
      finally
      {
        NotificationManager.s_hwndNotify = IntPtr.Zero;
      }
    }

    public static void Initialize()
    {
      NotificationManager.EnsureReady();
      NotificationManager.s_initialized = true;
    }

    public static void Shutdown()
    {
      if (!NotificationManager.s_initialized)
        return;
      NotificationManager.s_initialized = false;
      if (Environment.UserInteractive)
        NotificationManager.FlushNotificationQueue();
      NotificationManager.DestroyWindow();
      UIHost.ExitModalState -= new ModalStateEventHandler(NotificationManager.UIHost_ExitModalState);
    }

    private static bool EnsureReady()
    {
      if (Environment.UserInteractive)
      {
        if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.IsWindow(NotificationManager.s_hwndNotify))
        {
          NotificationManager.RegisterClass();
          NotificationManager.CreateWindow();
        }
        UIHost.ExitModalState += new ModalStateEventHandler(NotificationManager.UIHost_ExitModalState);
      }
      return NotificationManager.s_hwndNotify != IntPtr.Zero;
    }

    [DllImport("user32", CharSet = CharSet.Auto)]
    internal static extern IntPtr DefWindowProc(
      IntPtr hWnd,
      int msg,
      IntPtr wParam,
      IntPtr lParam);

    private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
    {
      bool flag = true;
      try
      {
        if (msg >= 1024 && msg <= 1724)
          NotificationManager.DeliverLocalNotification(new NotificationManager.NotificationInstance((Notification) msg, wParam, lParam, (NotificationManager.GetNotificationParametersCallback) null, (object) null, true));
        else if (msg == 2)
          NotificationManager.s_hwndNotify = IntPtr.Zero;
        else
          flag = false;
      }
      catch (Exception ex)
      {
        flag = false;
      }
      return flag ? IntPtr.Zero : NotificationManager.DefWindowProc(hwnd, msg, wParam, lParam);
    }

    internal static void AddNotificationHandler(NotificationManager.NotificationHandler handler) => NotificationManager.AddNotificationHandler(handler, Notification.TeamFoundationNotificationBegin, Notification.TeamFoundationNotificationEnd);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddNotificationHandler(
      NotificationManager.NotificationHandler handler,
      Notification begin,
      Notification end)
    {
      lock (NotificationManager.s_handlers)
        NotificationManager.s_handlers[handler] = new NotificationManager.NotificationRange(begin, end);
    }

    internal static void RemoveNotificationHandler(NotificationManager.NotificationHandler handler)
    {
      lock (NotificationManager.s_handlers)
        NotificationManager.s_handlers.Remove(handler);
    }

    private static StringBuilder GetClassNameHelper(IntPtr hWnd, StringBuilder sb)
    {
      if (sb == null)
        sb = new StringBuilder(256);
      if (NotificationManager.GetClassName(hWnd, sb, sb.Capacity) == 0)
        sb.Length = 0;
      return sb;
    }

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetModuleFileName(
      IntPtr hModule,
      StringBuilder lpFilename,
      int nSize);

    [DllImport("kernel32")]
    private static extern int GetCurrentProcessId();

    [DllImport("user32", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    private enum ExpectedParameters
    {
      Zero,
      One,
      Two,
      Any,
    }

    private enum CollapseMode
    {
      Never,
      Message,
      FirstArg,
      SecondArg,
    }

    private class MessageInfo
    {
      public NotificationManager.ExpectedParameters expectedParamters;
      public NotificationManager.CollapseMode collapseMode;
      public bool deliverInProcess;
      public bool deliverInModalDialogState;

      public MessageInfo(
        NotificationManager.ExpectedParameters expectedParamters,
        NotificationManager.CollapseMode collapseMode,
        bool deliverInProcess,
        bool deliverInModalDialogState)
      {
        this.expectedParamters = expectedParamters;
        this.collapseMode = collapseMode;
        this.deliverInProcess = deliverInProcess;
        this.deliverInModalDialogState = deliverInModalDialogState;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void NotificationHandler(
      Notification notification,
      IntPtr param1,
      IntPtr param2);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate bool GetNotificationParametersCallback(
      Notification notification,
      uint windowFlags,
      object userData,
      ref IntPtr wParam,
      ref IntPtr lParam);

    internal class NotificationWindowVersion
    {
      internal const int OrcasRTM = 0;
      internal const int OrcasSP1 = 1;
      internal const int Dev10RTM = 3;
      internal const int Dev12RTM = 5;
      internal const int Latest = 5;
    }

    private class NotificationRange
    {
      public Notification begin;
      public Notification end;

      public NotificationRange(Notification begin, Notification end)
      {
        this.begin = begin;
        this.end = end;
      }
    }

    private struct NotificationInstance
    {
      public Notification notification;
      public IntPtr wParam;
      public IntPtr lParam;
      public bool deliverInProcess;
      public NotificationManager.GetNotificationParametersCallback callback;
      public object userData;

      public NotificationInstance(
        Notification notification,
        IntPtr wParam,
        IntPtr lParam,
        NotificationManager.GetNotificationParametersCallback callback,
        object userData,
        bool deliverInProcess)
      {
        this.notification = notification;
        this.wParam = wParam;
        this.lParam = lParam;
        this.callback = callback;
        this.userData = userData;
        this.deliverInProcess = deliverInProcess;
      }
    }
  }
}
