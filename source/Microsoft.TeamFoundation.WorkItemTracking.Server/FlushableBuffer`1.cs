// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FlushableBuffer`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class FlushableBuffer<T> : IDisposable where T : BufferableItem
  {
    private string m_area = "Services";
    private ConcurrentBufferedItemSet<T> m_bufferSet;
    private FlushableBufferSettings<T> m_settings = new FlushableBufferSettings<T>();
    private readonly Action<IVssRequestContext, ICollection<T>> m_flushDelegate;
    private readonly Func<IVssRequestContext, FlushableBufferSettings<T>> m_getSettingsDelegate;
    private readonly ILockName m_lockName;
    private readonly IEqualityComparer<T> m_comparer;
    private bool m_disposed;
    private readonly string m_registryPath;
    private bool m_isTaskQueued;

    protected virtual void RegisterNotification(
      IVssRequestContext systemRequestContext,
      string registryPath)
    {
      systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, registryPath + "/*");
    }

    protected virtual void UnregisterNotification(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));

    protected virtual void CreateFlushTask(
      IVssRequestContext systemRequestContext,
      int interval,
      object taskArgs = null)
    {
      TeamFoundationTaskService service1 = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
      DateTime startTime = DateTime.UtcNow.AddSeconds((double) ((this.m_settings.FlushBucketsCount + systemRequestContext.ServiceHost.PartitionId % this.m_settings.FlushBucketsCount - (int) ((double) DateTime.UtcNow.Millisecond / 1000.0 * (double) this.m_settings.FlushBucketsCount / 60.0)) % this.m_settings.FlushBucketsCount));
      IVssRequestContext requestContext1 = systemRequestContext;
      TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.Flush), taskArgs, startTime, interval);
      service1.AddTask(requestContext1, task);
      CustomerIntelligenceService service2 = systemRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("Interval", (double) interval);
      intelligenceData.Add("MaxQueueLength", (object) this.m_settings?.MaxQueuedLength);
      intelligenceData.Add("MaxItemAge", (object) this.m_settings?.MaxRecordAgeMs);
      IVssRequestContext requestContext2 = systemRequestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service2.Publish(requestContext2, nameof (FlushableBuffer<T>), nameof (CreateFlushTask), properties);
    }

    protected virtual void RemoveFlushTask(IVssRequestContext systemRequestContext) => systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.Flush));

    public string Area
    {
      set => this.m_area = value;
      get => this.m_area;
    }

    public FlushableBuffer(
      IVssRequestContext systemRequestContext,
      string registryPath,
      string lockName,
      IEqualityComparer<T> comparer,
      Action<IVssRequestContext, ICollection<T>> flushDelegate,
      Func<IVssRequestContext, FlushableBufferSettings<T>> getSettingsDelegate = null)
    {
      systemRequestContext.TraceEnter(914001, this.m_area, nameof (FlushableBuffer<T>), "FlushableBuffer constructor");
      this.m_flushDelegate = flushDelegate;
      this.m_getSettingsDelegate = getSettingsDelegate ?? new Func<IVssRequestContext, FlushableBufferSettings<T>>(this.GetSettingsFromRegistry);
      this.m_comparer = comparer;
      this.m_registryPath = registryPath;
      try
      {
        this.m_lockName = systemRequestContext.ServiceHost.CreateUniqueLockName(lockName);
        this.RegisterNotification(systemRequestContext, registryPath);
        this.ResetFromSettings(systemRequestContext);
        int interval = this.m_settings.MaxRecordAgeMs + (int) (DateTime.UtcNow.Ticks % 30000L - 15000L);
        this.CreateFlushTask(systemRequestContext, interval);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(914003, this.m_area, nameof (FlushableBuffer<T>), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(914002, this.m_area, nameof (FlushableBuffer<T>), "FlushableBuffer constructor");
      }
    }

    public void Dispose(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(914004, this.m_area, nameof (FlushableBuffer<T>), "Dispose FlushableBuffer");
      try
      {
        this.Dispose();
        this.UnregisterNotification(systemRequestContext);
        this.RemoveFlushTask(systemRequestContext);
        this.Flush(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(914005, this.m_area, nameof (FlushableBuffer<T>), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(914005, this.m_area, nameof (FlushableBuffer<T>), "Dispose FlushableBuffer");
      }
    }

    public void Dispose() => this.m_disposed = true;

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (this.m_disposed)
        return;
      this.ResetFromSettings(requestContext);
    }

    private FlushableBufferSettings<T> GetSettingsFromRegistry(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<CachedRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (this.m_registryPath + "/*"));
      return new FlushableBufferSettings<T>(registryEntryCollection.GetValueFromPath<int>(this.m_registryPath + "/MaxItemAge", 60000), registryEntryCollection.GetValueFromPath<int>(this.m_registryPath + "/MaxQueueLength", 1000));
    }

    public void ResetFromSettings(IVssRequestContext requestContext)
    {
      if (this.m_disposed)
        return;
      requestContext.TraceEnter(914006, this.m_area, nameof (FlushableBuffer<T>), "LoadSettings");
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      try
      {
        properties.Add("SettingsRegistryPath", this.m_registryPath);
        properties.Add("OldMaxQueueLength", (object) this.m_settings?.MaxQueuedLength);
        properties.Add("OldMaxItemAge", (object) this.m_settings?.MaxRecordAgeMs);
        this.m_settings = this.m_getSettingsDelegate(requestContext);
        using (requestContext.AcquireWriterLock(this.m_lockName))
        {
          if (this.m_bufferSet != null)
            return;
          this.m_bufferSet = new ConcurrentBufferedItemSet<T>(this.m_settings.MaxQueuedLength, this.m_comparer);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(914008, this.m_area, nameof (FlushableBuffer<T>), ex);
        throw;
      }
      finally
      {
        properties.Add("NewMaxQueueLength", (object) this.m_settings?.MaxQueuedLength);
        properties.Add("NewMaxItemAge", (object) this.m_settings?.MaxRecordAgeMs);
        service.Publish(requestContext, nameof (FlushableBuffer<T>), nameof (ResetFromSettings), properties);
        requestContext.TraceLeave(914007, this.m_area, nameof (FlushableBuffer<T>), "LoadSettings");
      }
    }

    public void Add(IVssRequestContext requestContext, ICollection<T> items)
    {
      try
      {
        requestContext.TraceEnter(914009, this.m_area, nameof (FlushableBuffer<T>), "AddToBuffer");
        using (requestContext.AcquireReaderLock(this.m_lockName))
        {
          if (this.m_disposed)
            throw new ObjectDisposedException(nameof (FlushableBuffer<T>));
          items.ForEach<T>((Action<T>) (item => this.m_bufferSet.Add(item)));
        }
        int count = this.m_bufferSet.Count;
        if (count >= 2 * this.m_settings.MaxQueuedLength && this.m_isTaskQueued)
        {
          TeamFoundationTracingService.TraceRaw(914011, TraceLevel.Error, this.m_area, nameof (FlushableBuffer<T>), "Flush Task is not running, {0} records ({1} threshold) - hijacking request thread", (object) count, (object) this.m_settings.MaxQueuedLength);
          try
          {
            this.Flush(requestContext, true);
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(914011, this.m_area, nameof (FlushableBuffer<T>), ex);
          }
        }
        if (count < this.m_settings.MaxQueuedLength)
          return;
        if (this.m_isTaskQueued)
          return;
        try
        {
          TeamFoundationTracingService.TraceRaw(914012, TraceLevel.Info, this.m_area, nameof (FlushableBuffer<T>), "Queueing Flush Task {0} records ({1} threshold)", (object) count, (object) this.m_settings.MaxQueuedLength);
          this.CreateFlushTask(requestContext, 0, (object) 0);
          this.m_isTaskQueued = true;
        }
        catch (RequestCanceledException ex)
        {
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(914003, this.m_area, nameof (FlushableBuffer<T>), ex);
      }
      finally
      {
        requestContext.TraceLeave(914010, this.m_area, nameof (FlushableBuffer<T>), "AddToBuffer");
      }
    }

    private void Flush(IVssRequestContext requestContext, object taskArg) => this.Flush(requestContext);

    private void Flush(IVssRequestContext requestContext, bool inline = false)
    {
      try
      {
        requestContext.TraceEnter(914009, this.m_area, nameof (FlushableBuffer<T>), nameof (Flush));
        ICollection<T> objs = (ICollection<T>) null;
        if (this.m_bufferSet.Count <= 0)
          return;
        using (requestContext.AcquireWriterLock(this.m_lockName))
        {
          if (this.m_disposed || requestContext.IsCanceled)
          {
            if (this.m_bufferSet.Count <= 0)
              return;
            requestContext.Trace(914011, TraceLevel.Error, this.m_area, nameof (FlushableBuffer<T>), "Flush called but FlushableBuffer was disposed - there are currently {0} items records", (object) this.m_bufferSet.Count);
            return;
          }
          if (this.m_bufferSet.Count <= 0)
            return;
          if (!inline)
            this.m_isTaskQueued = false;
          objs = this.m_bufferSet.Values;
          this.m_bufferSet = new ConcurrentBufferedItemSet<T>(this.m_settings.MaxQueuedLength, this.m_comparer);
        }
        if (objs == null || objs.Count <= 0)
          return;
        if (this.m_flushDelegate == null)
          return;
        try
        {
          requestContext.TraceEnter(914012, this.m_area, nameof (FlushableBuffer<T>), nameof (Flush));
          this.m_flushDelegate(requestContext, objs);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(914011, this.m_area, nameof (FlushableBuffer<T>), ex);
        }
        finally
        {
          requestContext.TraceLeave(914010, this.m_area, nameof (FlushableBuffer<T>), nameof (Flush));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(914011, this.m_area, nameof (FlushableBuffer<T>), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(914011, this.m_area, nameof (FlushableBuffer<T>), nameof (Flush));
      }
    }
  }
}
