// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountRegistryWatcher
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  internal class AccountRegistryWatcher : IDisposable
  {
    private Thread m_monitorThread;
    private SafeHandle m_registryHandle;
    private AutoResetEvent m_shutdownEvent;
    private AutoResetEvent m_registryChangedEvent;
    private static int ERROR_FILE_NOT_FOUND = 2;
    private static int ERROR_SUCCESS = 0;
    private static IntPtr HKEY_CLASSES_ROOT = new IntPtr(int.MinValue);
    private static IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
    private static IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);
    private static IntPtr HKEY_USERS = new IntPtr(-2147483645);
    private static IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);

    public AccountRegistryWatcher(RegistryHive hive, string keyPath)
      : this(hive, keyPath, false)
    {
    }

    public AccountRegistryWatcher(RegistryHive hive, string keyPath, bool watchSubtree)
      : this(hive, keyPath, watchSubtree, RegistryChangeNotificationFilter.NameChange | RegistryChangeNotificationFilter.ValueChange)
    {
    }

    public AccountRegistryWatcher(
      RegistryHive hive,
      string keyPath,
      bool watchSubtree,
      RegistryChangeNotificationFilter filter)
    {
      this.Hive = hive;
      this.KeyPath = keyPath;
      this.WatchSubtree = watchSubtree;
      this.Filter = filter;
    }

    public RegistryHive Hive { get; private set; }

    public string KeyPath { get; private set; }

    public bool WatchSubtree { get; private set; }

    public RegistryChangeNotificationFilter Filter { get; private set; }

    public Action<Exception> Error { get; set; }

    public Action KeyChanged { get; set; }

    public void Dispose() => this.Stop();

    public void Stop()
    {
      try
      {
        if (this.m_monitorThread != null && this.m_shutdownEvent != null)
        {
          this.m_registryChangedEvent.SafeWaitHandle.SetHandleAsInvalid();
          this.m_shutdownEvent.Set();
          if (this.m_monitorThread.ManagedThreadId != Environment.CurrentManagedThreadId)
            this.m_monitorThread.Join();
          this.m_monitorThread = (Thread) null;
        }
        else
          this.StopInternal();
      }
      catch (Exception ex)
      {
        this.OnError(ex);
      }
    }

    public void Start()
    {
      this.m_registryHandle = this.m_registryHandle == null ? AccountRegistryWatcher.OpenSubKey(this.Hive, this.KeyPath, RegistryAccessMask.Execute) : throw new InvalidOperationException();
      this.m_shutdownEvent = new AutoResetEvent(false);
      this.m_registryChangedEvent = new AutoResetEvent(false);
      this.m_monitorThread = new Thread(new ThreadStart(this.MonitorRegistryKey), 262144);
      this.m_monitorThread.IsBackground = true;
      this.m_monitorThread.Start();
    }

    private void OnError(Exception ex)
    {
      if (this.Error == null)
        return;
      try
      {
        this.Error(ex);
      }
      catch (Exception ex1)
      {
      }
    }

    private void OnKeyChanged()
    {
      if (this.KeyChanged == null)
        return;
      try
      {
        this.KeyChanged();
      }
      catch (Exception ex)
      {
      }
    }

    private void StopInternal()
    {
      if (this.m_registryHandle != null)
      {
        this.m_registryHandle.Dispose();
        this.m_registryHandle = (SafeHandle) null;
      }
      if (this.m_shutdownEvent != null)
      {
        this.m_shutdownEvent.Dispose();
        this.m_shutdownEvent = (AutoResetEvent) null;
      }
      if (this.m_registryChangedEvent == null)
        return;
      this.m_registryChangedEvent.Dispose();
      this.m_registryChangedEvent = (AutoResetEvent) null;
    }

    private bool StartKeyMonitor()
    {
      try
      {
        int error = AccountRegistryWatcher.RegNotifyChangeKeyValue(this.m_registryHandle, this.WatchSubtree, this.Filter, (SafeHandle) this.m_registryChangedEvent.SafeWaitHandle, true);
        if (error == 0)
          return true;
        this.StopInternal();
        this.OnError((Exception) new Win32Exception(error));
        return false;
      }
      catch (Exception ex)
      {
        this.StopInternal();
        this.OnError(ex);
        return false;
      }
    }

    private void MonitorRegistryKey()
    {
      try
      {
        if (!this.StartKeyMonitor())
          return;
        int num;
        do
        {
          if (WaitHandle.WaitAny(new WaitHandle[2]
          {
            (WaitHandle) this.m_shutdownEvent,
            (WaitHandle) this.m_registryChangedEvent
          }) == 0)
          {
            this.StopInternal();
            break;
          }
          num = this.StartKeyMonitor() ? 1 : 0;
          this.OnKeyChanged();
        }
        while (num != 0);
      }
      catch (Exception ex)
      {
        this.StopInternal();
        this.OnError(ex);
      }
    }

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern int RegNotifyChangeKeyValue(
      SafeHandle hKey,
      bool watchSubtree,
      [MarshalAs(UnmanagedType.U4)] RegistryChangeNotificationFilter dwNotifyFilter,
      SafeHandle hEvent,
      bool fAsynchronous);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    private static extern int RegOpenKeyEx(
      IntPtr hKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey,
      int ulOptions,
      RegistryAccessMask samDesired,
      out SafeRegistryHandle hkResult);

    public static SafeHandle OpenSubKey(
      RegistryHive hive,
      string subKey,
      RegistryAccessMask accessMask)
    {
      SafeRegistryHandle hkResult;
      int error = AccountRegistryWatcher.RegOpenKeyEx(AccountRegistryWatcher.Convert(hive), subKey, 0, accessMask, out hkResult);
      if (error == AccountRegistryWatcher.ERROR_SUCCESS && !hkResult.IsInvalid)
        return (SafeHandle) hkResult;
      if (error == AccountRegistryWatcher.ERROR_FILE_NOT_FOUND)
        return (SafeHandle) null;
      throw new Win32Exception(error);
    }

    private static IntPtr Convert(RegistryHive hive)
    {
      IntPtr num = IntPtr.Zero;
      switch (hive)
      {
        case RegistryHive.ClassesRoot:
          num = AccountRegistryWatcher.HKEY_CLASSES_ROOT;
          break;
        case RegistryHive.CurrentUser:
          num = AccountRegistryWatcher.HKEY_CURRENT_USER;
          break;
        case RegistryHive.LocalMachine:
          num = AccountRegistryWatcher.HKEY_LOCAL_MACHINE;
          break;
        case RegistryHive.Users:
          num = AccountRegistryWatcher.HKEY_USERS;
          break;
        case RegistryHive.CurrentConfig:
          num = AccountRegistryWatcher.HKEY_CURRENT_CONFIG;
          break;
      }
      return num;
    }
  }
}
