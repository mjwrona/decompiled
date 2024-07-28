// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.RegistryWatcher
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RegistryWatcher : IDisposable
  {
    private Thread m_monitorThread;
    private SafeHandle m_registryHandle;
    private AutoResetEvent m_shutdownEvent;
    private AutoResetEvent m_registryChangedEvent;

    public RegistryWatcher(RegistryHive hive, string keyPath)
      : this(hive, keyPath, false)
    {
    }

    public RegistryWatcher(RegistryHive hive, string keyPath, bool watchSubtree)
      : this(hive, keyPath, watchSubtree, RegistryChangeNotificationFilter.NameChange | RegistryChangeNotificationFilter.ValueChange)
    {
    }

    [CLSCompliant(false)]
    public RegistryWatcher(
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

    [CLSCompliant(false)]
    public RegistryChangeNotificationFilter Filter { get; private set; }

    public Action<Exception> Error { get; set; }

    public Action KeyChanged { get; set; }

    public void Dispose() => this.Stop();

    public void Stop()
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

    public void Start()
    {
      this.m_registryHandle = this.m_registryHandle == null ? RegistryHelper.OpenSubKey(this.Hive, this.KeyPath, RegistryAccessMask.Execute) : throw new InvalidOperationException();
      this.m_shutdownEvent = new AutoResetEvent(false);
      this.m_registryChangedEvent = new AutoResetEvent(false);
      this.m_monitorThread = new Thread(new ThreadStart(this.MonitorRegistryKey));
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
        RegistryHelper.NotifyChangeKeyValue(this.m_registryHandle, this.WatchSubtree, this.Filter, (SafeHandle) this.m_registryChangedEvent.SafeWaitHandle, true);
        return true;
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
        while (true)
        {
          if (WaitHandle.WaitAny(new WaitHandle[2]
          {
            (WaitHandle) this.m_shutdownEvent,
            (WaitHandle) this.m_registryChangedEvent
          }) != 0)
          {
            if (this.StartKeyMonitor())
              this.OnKeyChanged();
            else
              goto label_1;
          }
          else
            break;
        }
        this.StopInternal();
        return;
label_1:;
      }
      catch (Exception ex)
      {
        this.StopInternal();
        this.OnError(ex);
      }
    }
  }
}
