// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebRequestLogger
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class WebRequestLogger : VssBaseService, IVssFrameworkService, IDisposable
  {
    private bool m_disposed;
    private TeamFoundationLoggingLevel m_loggingLevel;
    private int m_maxCompressedThresholdTime;
    private int m_maxQueuedRecords;
    private bool m_queuedTask;
    private int m_maxRecordAge;
    private ITeamFoundationLoggingListener m_loggingListener;
    private WebRequestLogger.RequestDetailsDictionary m_queuedRequests;
    private ILockName m_queuedRequestsLockName;
    private RequestDetailComparer m_requestDetailComparer;
    private const string c_loggingRootPath = "/Configuration/Logging";
    private const string c_loggingCompressionRegistryKey = "/Configuration/Logging/MaxCompressionThreshold";
    private const string c_loggingRecordAgeRegistryKey = "/Configuration/Logging/MaxRecordAge";
    private const string c_loggingRecordQueueLengthRegistryKey = "/Configuration/Logging/QueueLength";
    private const string c_loggingLevelRegistryKey = "/Configuration/Logging/Level";
    private const string c_loggingToDatabase = "/Configuration/Logging/EnableDatabaseLogging";
    private const int MaxCompressedThresholdDefault = 30000000;
    private const int DefaultAgeThreshold = 15;
    private const int MinAgeThreshold = 5;
    private const int MaxAgeThreshold = 900;
    private const int DefaultQueueLengthThreshold = 100;
    private const string c_Area = "ActivityLog";
    private const string c_Layer = "WebRequestLogger";
    private static readonly HashSet<Type> s_ExpectedExceptionTypes = new HashSet<Type>()
    {
      typeof (UnauthorizedRequestException)
    };

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(7000, "ActivityLog", nameof (WebRequestLogger), "ServiceStart");
      try
      {
        this.m_queuedRequestsLockName = this.CreateLockName(systemRequestContext, "queuedRequests");
        systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, "/Configuration/Logging/*");
        this.LoadSettings(systemRequestContext);
        systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(systemRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessRecords), (object) null, this.m_maxRecordAge));
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(7008, "ActivityLog", nameof (WebRequestLogger), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceEnter(7009, "ActivityLog", nameof (WebRequestLogger), "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(7010, "ActivityLog", nameof (WebRequestLogger), "ServiceEnd");
      try
      {
        systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
        systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.ProcessRecords));
        this.ProcessRecords(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(7018, "ActivityLog", nameof (WebRequestLogger), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(7019, "ActivityLog", nameof (WebRequestLogger), "ServiceEnd");
      }
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(7020, "ActivityLog", nameof (WebRequestLogger), nameof (LoadSettings));
      try
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<CachedRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Configuration/Logging/*");
        this.m_loggingLevel = registryEntryCollection.GetValueFromPath<TeamFoundationLoggingLevel>("/Configuration/Logging/Level", TeamFoundationLoggingLevel.Normal);
        this.m_maxCompressedThresholdTime = registryEntryCollection.GetValueFromPath<int>("/Configuration/Logging/MaxCompressionThreshold", 30000000);
        this.m_maxRecordAge = registryEntryCollection.GetValueFromPath<int>("/Configuration/Logging/MaxRecordAge", 15);
        this.m_maxQueuedRecords = registryEntryCollection.GetValueFromPath<int>("/Configuration/Logging/QueueLength", 100);
        if (this.m_maxRecordAge < 5 || this.m_maxRecordAge > 900)
        {
          TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.AgeThresholdException((object) 5, (object) 900), (Exception) new ArgumentOutOfRangeException(FrameworkResources.AgeThreshold(), FrameworkResources.AgeThresholdException((object) 5, (object) 900)));
          if (this.m_maxRecordAge < 5)
            this.m_maxRecordAge = 5;
          else if (this.m_maxRecordAge > 900)
            this.m_maxRecordAge = 900;
        }
        this.m_maxRecordAge *= 1000;
        bool defaultValue = !requestContext.ExecutionEnvironment.IsCloudDeployment && requestContext.ServiceHost.HasDatabaseAccess;
        this.m_loggingListener = (ITeamFoundationLoggingListener) new TeamFoundationActivityLogListener(registryEntryCollection.GetValueFromPath<bool>("/Configuration/Logging/EnableDatabaseLogging", defaultValue));
        using (requestContext.AcquireWriterLock(this.m_queuedRequestsLockName))
        {
          if (this.m_queuedRequests == null)
          {
            this.m_requestDetailComparer = new RequestDetailComparer(this.m_maxCompressedThresholdTime, this.m_loggingLevel);
            this.m_queuedRequests = new WebRequestLogger.RequestDetailsDictionary(this.m_maxQueuedRecords, this.m_requestDetailComparer);
          }
          else
            this.m_requestDetailComparer.MaxCompressedThresholdTime = this.m_maxCompressedThresholdTime;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(7028, "ActivityLog", nameof (WebRequestLogger), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(7029, "ActivityLog", nameof (WebRequestLogger), nameof (LoadSettings));
      }
    }

    public void Dispose() => this.m_disposed = true;

    public void LogRequest(IVssRequestContext requestContextToLog)
    {
      int num = 0;
      try
      {
        requestContextToLog.TraceEnter(7030, "ActivityLog", nameof (WebRequestLogger), nameof (LogRequest));
        if (this.m_loggingLevel == TeamFoundationLoggingLevel.None || this.m_loggingLevel == TeamFoundationLoggingLevel.OnError && requestContextToLog.Status == null)
          return;
        long executionTimeThreshold = 10000000;
        bool isExceptionExpected = false;
        bool canAggregate = true;
        try
        {
          if (requestContextToLog.ServiceHost.HasDatabaseAccess)
          {
            WhitelistingService service = requestContextToLog.To(TeamFoundationHostType.Deployment).GetService<WhitelistingService>();
            string command = requestContextToLog.Title();
            executionTimeThreshold = service.GetCommandExecutionTimeThreshold(requestContextToLog, requestContextToLog.ServiceName, command, requestContextToLog.UserAgent);
            if (requestContextToLog.IsFeatureEnabled(FrameworkServerConstants.DisableWebRequestAggregation))
              canAggregate = !service.IsCommandWhitelisted(requestContextToLog.ServiceName, command);
            if (requestContextToLog.Status != null)
              isExceptionExpected = service.IsExceptionExpected(requestContextToLog.Status, requestContextToLog.ServiceName);
          }
        }
        catch (Exception ex)
        {
          requestContextToLog.TraceException(7032, "ActivityLog", nameof (WebRequestLogger), ex);
        }
        RequestDetails requestDetails = requestContextToLog.GetRequestDetails(this.m_loggingLevel, executionTimeThreshold, isExceptionExpected, canAggregate);
        if (this.m_loggingLevel != TeamFoundationLoggingLevel.All && (requestContextToLog.Status == null || WebRequestLogger.s_ExpectedExceptionTypes.Contains(requestContextToLog.Status.GetType())) && (requestContextToLog.Method == null || this.m_loggingLevel > (TeamFoundationLoggingLevel) requestContextToLog.Method.MethodType && requestDetails.ActivityStatus == ActivityStatus.Success))
          return;
        using (requestContextToLog.AcquireReaderLock(this.m_queuedRequestsLockName))
        {
          if (this.m_disposed)
            throw new ObjectDisposedException(nameof (WebRequestLogger));
          num = this.m_queuedRequests.AddRequest(requestDetails);
        }
        if (num >= 5 * this.m_maxQueuedRecords && this.m_queuedTask)
        {
          TeamFoundationTracingService.TraceRaw(7034, TraceLevel.Error, "ActivityLog", nameof (WebRequestLogger), "ProcessRecords Task is not running, {0} records ({1} threshold) - hijacking request thread", (object) num, (object) this.m_maxQueuedRecords);
          try
          {
            this.ProcessRecords(requestContextToLog);
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(7036, "ActivityLog", nameof (WebRequestLogger), ex);
          }
        }
        if (num < this.m_maxQueuedRecords)
          return;
        if (this.m_queuedTask)
          return;
        try
        {
          TeamFoundationTracingService.TraceRaw(7038, TraceLevel.Info, "ActivityLog", nameof (WebRequestLogger), "Queueing ProcessRecords Task {0} records ({1} threshold)", (object) num, (object) this.m_maxQueuedRecords);
          requestContextToLog.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContextToLog, new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessRecords), (object) 0, 0));
          this.m_queuedTask = true;
        }
        catch (RequestCanceledException ex)
        {
        }
      }
      catch (Exception ex)
      {
        requestContextToLog.TraceException(7038, "ActivityLog", nameof (WebRequestLogger), ex);
      }
      finally
      {
        requestContextToLog.TraceLeave(7039, "ActivityLog", nameof (WebRequestLogger), nameof (LogRequest));
      }
    }

    private void ProcessRecords(IVssRequestContext requestContext, object taskArg) => this.ProcessRecords(requestContext);

    public TeamFoundationLoggingLevel LogLevel
    {
      get => this.m_loggingLevel;
      set => this.m_loggingLevel = value;
    }

    private void ProcessRecords(IVssRequestContext requestContext, bool inline = false)
    {
      try
      {
        requestContext.TraceEnter(7050, "ActivityLog", nameof (WebRequestLogger), nameof (ProcessRecords));
        if (this.m_queuedRequests.QueueRecords <= 0)
          return;
        WebRequestLogger.RequestDetailsDictionary detailsDictionary = (WebRequestLogger.RequestDetailsDictionary) null;
        using (requestContext.AcquireWriterLock(this.m_queuedRequestsLockName))
        {
          if (this.m_disposed || requestContext.IsCanceled)
          {
            if (this.m_queuedRequests.QueueRecords <= 0)
              return;
            requestContext.Trace(7051, TraceLevel.Error, "ActivityLog", nameof (WebRequestLogger), "ProcessRecords called but WebRequestLogger was disposed - there are currently {0} unprocessed records", (object) this.m_queuedRequests.QueueRecords);
            return;
          }
          if (this.m_queuedRequests.QueueRecords <= 0)
            return;
          if (!inline)
            this.m_queuedTask = false;
          detailsDictionary = this.m_queuedRequests;
          this.m_queuedRequests = new WebRequestLogger.RequestDetailsDictionary(this.m_maxQueuedRecords, this.m_requestDetailComparer);
        }
        if (this.m_loggingListener == null)
          return;
        try
        {
          requestContext.TraceEnter(7055, "ActivityLog", nameof (WebRequestLogger), "SubmitRequestDetails");
          this.m_loggingListener.SubmitRequestDetails(requestContext, detailsDictionary.Values, this.m_maxCompressedThresholdTime, this.m_loggingLevel);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(7056, "ActivityLog", nameof (WebRequestLogger), ex);
        }
        finally
        {
          requestContext.TraceLeave(7057, "ActivityLog", nameof (WebRequestLogger), "SubmitRequestDetails");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(7058, "ActivityLog", nameof (WebRequestLogger), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(7059, "ActivityLog", nameof (WebRequestLogger), nameof (ProcessRecords));
      }
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (this.m_disposed)
        return;
      this.LoadSettings(requestContext);
    }

    internal class RequestDetailsDictionary
    {
      private ConcurrentDictionary<RequestDetails, RequestDetails> m_queuedRequests;
      private int m_queuedRecords;

      public RequestDetailsDictionary(int maxRecords, RequestDetailComparer comparer)
      {
        this.m_queuedRequests = new ConcurrentDictionary<RequestDetails, RequestDetails>(Environment.ProcessorCount * 3, maxRecords, (IEqualityComparer<RequestDetails>) comparer);
        this.m_queuedRecords = 0;
      }

      public int QueueRecords => this.m_queuedRecords;

      public int AddRequest(RequestDetails rd) => this.m_queuedRequests.AddOrUpdate(rd, rd, (Func<RequestDetails, RequestDetails, RequestDetails>) ((k, v) => v.AddRef(rd))) == rd ? Interlocked.Increment(ref this.m_queuedRecords) : 0;

      public IEnumerable<RequestDetails> Values => this.m_queuedRequests.Select<KeyValuePair<RequestDetails, RequestDetails>, RequestDetails>((Func<KeyValuePair<RequestDetails, RequestDetails>, RequestDetails>) (x => x.Value));
    }
  }
}
