// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebHostLogger
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class WebHostLogger : VssBaseService, IVssFrameworkService, IDisposable
  {
    private const string c_loggingRootPath = "/Configuration/Logging";
    private const string c_loggingRecordAgeRegistryKey = "/Configuration/Logging/MaxHostRecordAge";
    private const string c_loggingRecordIntervalRegistryKey = "/Configuration/Logging/MinHostRecordInterval";
    private const string c_loggingLevelRegistryKey = "/Configuration/Logging/Level";
    private const int DefaultRecordAgeThreshold = 43200;
    private const int MinAgeThreshold = 21600;
    private const int MaxAgeThreshold = 86400;
    private const int DefaultMinRecordInterval = 3600;
    private const string c_area = "ActivityLog";
    private const string c_layer = "WebHostLogger";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(7060, "ActivityLog", nameof (WebHostLogger), "ServiceStart");
      try
      {
        systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, "/Configuration/Logging/MaxHostRecordAge", "/Configuration/Logging/Level");
        this.LoadSettings(systemRequestContext);
        systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(systemRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.ClearRecords), (object) null, this.MaxRecordAge));
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(7061, "ActivityLog", nameof (WebHostLogger), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceEnter(7062, "ActivityLog", nameof (WebHostLogger), "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(7065, "ActivityLog", nameof (WebHostLogger), "ServiceEnd");
      try
      {
        systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
        systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.ClearRecords));
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(7066, "ActivityLog", nameof (WebHostLogger), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(7067, "ActivityLog", nameof (WebHostLogger), "ServiceEnd");
      }
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(7070, "ActivityLog", nameof (WebHostLogger), nameof (LoadSettings));
      try
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<CachedRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Configuration/Logging/*");
        this.LogLevel = registryEntryCollection.GetValueFromPath<TeamFoundationLoggingLevel>("/Configuration/Logging/Level", TeamFoundationLoggingLevel.Normal);
        this.MaxRecordAge = registryEntryCollection.GetValueFromPath<int>("/Configuration/Logging/MaxHostRecordAge", 43200);
        this.MinRecordInterval = registryEntryCollection.GetValueFromPath<int>("/Configuration/Logging/MinHostRecordInterval", 3600);
        if (this.MaxRecordAge < 21600 || this.MaxRecordAge > 86400)
        {
          TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.AgeThresholdException((object) 21600, (object) 86400), (Exception) new ArgumentOutOfRangeException(FrameworkResources.AgeThreshold(), FrameworkResources.AgeThresholdException((object) 21600, (object) 86400)));
          if (this.MaxRecordAge < 21600)
            this.MaxRecordAge = 21600;
          else if (this.MaxRecordAge > 86400)
            this.MaxRecordAge = 86400;
        }
        if (this.MinRecordInterval > this.MaxRecordAge)
        {
          TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.AgeThresholdException((object) 0, (object) this.MaxRecordAge), (Exception) new ArgumentOutOfRangeException(FrameworkResources.AgeThreshold(), FrameworkResources.AgeThresholdException((object) 0, (object) this.MaxRecordAge)));
          this.MinRecordInterval = this.MaxRecordAge;
        }
        this.MaxRecordAge *= 1000;
        this.MinRecordInterval *= 1000;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(7071, "ActivityLog", nameof (WebHostLogger), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(7072, "ActivityLog", nameof (WebHostLogger), nameof (LoadSettings));
      }
    }

    public void LogRequest(IVssRequestContext requestContextToLog)
    {
      requestContextToLog.TraceEnter(7075, "ActivityLog", nameof (WebHostLogger), nameof (LogRequest));
      if (this.LogLevel == TeamFoundationLoggingLevel.None || this.LogLevel == TeamFoundationLoggingLevel.OnError && requestContextToLog.Status == null)
        return;
      try
      {
        RequestDetails sh = requestContextToLog.GetRequestDetails(this.LogLevel);
        if (this.Disposed)
          throw new ObjectDisposedException(nameof (WebHostLogger));
        DateTime dateTime;
        if (!this.RequestServiceHostHistory.TryGetValue(sh.InstanceId, out dateTime))
        {
          if (!this.RequestServiceHostHistory.TryAdd(sh.InstanceId, sh.StartTime))
            return;
          this.LoggingListener.SubmitRequest(requestContextToLog, sh);
        }
        else
        {
          if (sh.StartTime.Subtract(dateTime).TotalMilliseconds <= (double) this.MinRecordInterval)
            return;
          dateTime = this.RequestServiceHostHistory.AddOrUpdate(sh.InstanceId, sh.StartTime, (Func<Guid, DateTime, DateTime>) ((k, v) => !(v > sh.StartTime) ? sh.StartTime : v));
          if (!(dateTime == sh.StartTime))
            return;
          this.LoggingListener.SubmitRequest(requestContextToLog, sh);
        }
      }
      catch (Exception ex)
      {
        requestContextToLog.TraceException(7076, "ActivityLog", nameof (WebHostLogger), ex);
      }
      finally
      {
        requestContextToLog.TraceLeave(7077, "ActivityLog", nameof (WebHostLogger), nameof (LogRequest));
      }
    }

    private void ClearRecords(IVssRequestContext requestContext, object taskArg) => this.ClearRecords(requestContext);

    private void ClearRecords(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.TraceEnter(7080, "ActivityLog", nameof (WebHostLogger), nameof (ClearRecords));
        if (this.Disposed || requestContext.IsCanceled)
          return;
        this.RequestServiceHostHistory = new ConcurrentDictionary<Guid, DateTime>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(7081, "ActivityLog", nameof (WebHostLogger), ex);
      }
      finally
      {
        requestContext.TraceLeave(7082, "ActivityLog", nameof (WebHostLogger), nameof (ClearRecords));
      }
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (this.Disposed)
        return;
      this.LoadSettings(requestContext);
    }

    public void Dispose() => this.Disposed = true;

    private ConcurrentDictionary<Guid, DateTime> RequestServiceHostHistory { get; set; } = new ConcurrentDictionary<Guid, DateTime>();

    public TeamFoundationLoggingLevel LogLevel { get; set; } = TeamFoundationLoggingLevel.Normal;

    public ITeamFoundationHostLoggingListener LoggingListener { get; set; } = (ITeamFoundationHostLoggingListener) new TeamFoundationServiceHostExtendedListener();

    private bool Disposed { get; set; }

    private int MaxRecordAge { get; set; }

    private int MinRecordInterval { get; set; }
  }
}
