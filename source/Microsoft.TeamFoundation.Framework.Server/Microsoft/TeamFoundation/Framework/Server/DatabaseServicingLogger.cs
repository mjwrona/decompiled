// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseServicingLogger
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class DatabaseServicingLogger : 
    IServicingStepDetailLogger,
    ISupportDurationLogging,
    IDisposable
  {
    private TeamFoundationTask m_task;
    private short m_completedStepCount;
    private bool m_needToWriteLog;
    private bool m_needToUpdateCompletedStepsCount;
    private readonly ILockName m_dataLockName;
    private readonly ILockName m_flushLockName;
    private readonly List<ServicingStepDetail> m_buffer = new List<ServicingStepDetail>(500);
    private readonly DateTime m_queueTime;
    private readonly Guid m_servicingHostId;
    private readonly Guid m_jobId;
    private readonly DeploymentServiceHost m_deploymentHost;
    private const int c_bufferSize = 500;
    private static readonly TimeSpan s_hostedFlushInterval = TimeSpan.FromSeconds(8.0);
    private static readonly TimeSpan s_onPremisesFlushInterval = TimeSpan.FromSeconds(2.0);
    private static readonly TimeSpan s_hostedUpgradeDatabasesFlushInterval = TimeSpan.FromSeconds(15.0);
    private static readonly TimeSpan s_hostedUpgradeHostFlushInterval = TimeSpan.FromSeconds(15.0);
    private static readonly TimeSpan s_hostMoveFlushInterval = TimeSpan.FromSeconds(20.0);
    private static readonly TimeSpan s_hostedCreateProjectFlushInterval = TimeSpan.FromSeconds(3.0);
    private static readonly TimeSpan s_sleepIncreaseTime = TimeSpan.FromSeconds(5.0);
    private static readonly string s_area = "Servicing";
    private static readonly string s_layer = nameof (DatabaseServicingLogger);
    private static readonly char[] s_semicolonArray = new char[1]
    {
      ';'
    };
    private static readonly ServicingStepDetail[] s_emptyStepDetailsArray = Array.Empty<ServicingStepDetail>();

    public DatabaseServicingLogger(
      DeploymentServiceHost deploymentHost,
      Guid servicingHostId,
      Guid jobId,
      DateTime queueTime,
      string operationClass)
    {
      DatabaseServicingLogger databaseServicingLogger = this;
      ArgumentUtility.CheckForNull<DeploymentServiceHost>(deploymentHost, nameof (deploymentHost));
      this.m_dataLockName = deploymentHost.CreateUniqueLockName("DatabaseServicingLogger.DataLock");
      this.m_flushLockName = deploymentHost.CreateUniqueLockName("DatabaseServicingLogger.FlushLock");
      this.m_queueTime = queueTime;
      this.m_servicingHostId = servicingHostId;
      this.m_jobId = jobId;
      this.m_deploymentHost = deploymentHost;
      using (IVssRequestContext servicingContext = this.m_deploymentHost.CreateServicingContext())
      {
        TeamFoundationExecutionEnvironment executionEnvironment = servicingContext.ExecutionEnvironment;
        TimeSpan timeSpan;
        if (executionEnvironment.IsOnPremisesDeployment)
        {
          timeSpan = DatabaseServicingLogger.s_onPremisesFlushInterval;
        }
        else
        {
          switch (operationClass)
          {
            case "UpgradeDatabase":
              timeSpan = DatabaseServicingLogger.s_hostedUpgradeDatabasesFlushInterval;
              break;
            case "CreateProject":
              timeSpan = DatabaseServicingLogger.s_hostedCreateProjectFlushInterval;
              break;
            case "UpgradeHost":
              timeSpan = DatabaseServicingLogger.s_hostedUpgradeHostFlushInterval;
              break;
            case "MoveHost":
              timeSpan = DatabaseServicingLogger.s_hostMoveFlushInterval;
              break;
            default:
              timeSpan = DatabaseServicingLogger.s_hostedFlushInterval;
              break;
          }
        }
        executionEnvironment = servicingContext.ExecutionEnvironment;
        if (executionEnvironment.IsHostedDeployment)
        {
          string str = servicingContext.GetService<CachedRegistryService>().GetValue(servicingContext, (RegistryQuery) FrameworkServerConstants.ServicingGenerateLogs, false, (string) null);
          if (!string.IsNullOrEmpty(str))
          {
            this.m_needToWriteLog = ((IEnumerable<string>) str.Split(DatabaseServicingLogger.s_semicolonArray, StringSplitOptions.RemoveEmptyEntries)).FirstOrDefault<string>((Func<string, bool>) (op => op.Equals(operationClass, StringComparison.OrdinalIgnoreCase) || op.Equals("*", StringComparison.Ordinal))) != null;
          }
          else
          {
            executionEnvironment = servicingContext.ExecutionEnvironment;
            this.m_needToWriteLog = executionEnvironment.IsDevFabricDeployment;
          }
          if (string.Equals(operationClass, "CreateProject", StringComparison.OrdinalIgnoreCase))
            this.m_needToUpdateCompletedStepsCount = true;
        }
        else
          this.m_needToWriteLog = true;
        this.m_task = new TeamFoundationTask((TeamFoundationTaskCallback) ((requestContext, args) => databaseServicingLogger.TryFlush(requestContext)), (object) null, (int) timeSpan.TotalMilliseconds);
        servicingContext.GetService<TeamFoundationTaskService>().AddTask(servicingContext, this.m_task);
      }
    }

    public void Dispose()
    {
      using (IVssRequestContext servicingContext = this.m_deploymentHost.CreateServicingContext())
      {
        if (this.m_task != null)
        {
          servicingContext.GetService<TeamFoundationTaskService>().RemoveTask(servicingContext, this.m_task);
          this.m_task = (TeamFoundationTask) null;
        }
        this.Flush(servicingContext);
      }
    }

    void IServicingStepDetailLogger.AddServicingStepDetail(ServicingStepDetail stepDetail)
    {
      ServicingStepStateChange servicingStepStateChange = stepDetail as ServicingStepStateChange;
      bool flag = false;
      using (this.m_deploymentHost.ServiceHostInternal().LockManager.Lock(LockManager.GetExemptionLockName(), LockManager.LockType.ResourceExemption, (long) -Environment.CurrentManagedThreadId))
      {
        using (this.m_deploymentHost.ServiceHostInternal().LockManager.Lock(this.m_dataLockName, LockManager.LockType.ResourceExclusive, (long) -Environment.CurrentManagedThreadId))
        {
          this.CheckDisposed();
          if (servicingStepStateChange != null && (servicingStepStateChange.StepState == ServicingStepState.Passed || servicingStepStateChange.StepState == ServicingStepState.PassedWithWarnings || servicingStepStateChange.StepState == ServicingStepState.PassedWithSkipChildren || servicingStepStateChange.StepState == ServicingStepState.Skipped))
            ++this.m_completedStepCount;
          this.m_buffer.Add(stepDetail);
          if (this.m_buffer.Count >= 500)
          {
            if (!this.m_needToWriteLog)
            {
              int count = 250;
              using (IVssRequestContext systemContext = this.m_deploymentHost.CreateSystemContext(true))
              {
                TeamFoundationTracingService service = systemContext.GetService<TeamFoundationTracingService>();
                for (int index = 0; index < count; ++index)
                  service.TraceServicingStepDetail(this.m_jobId, this.m_queueTime, this.m_buffer[index]);
              }
              this.m_buffer.RemoveRange(0, count);
            }
          }
          else if (stepDetail is ServicingStepLogEntry servicingStepLogEntry)
          {
            if (servicingStepLogEntry.EntryKind != ServicingStepLogEntryKind.Warning)
            {
              if (servicingStepLogEntry.EntryKind != ServicingStepLogEntryKind.Error)
                goto label_20;
            }
            flag = true;
            this.m_needToWriteLog = true;
          }
        }
label_20:
        if (!flag)
          return;
        using (IVssRequestContext servicingContext = this.m_deploymentHost.CreateServicingContext())
          this.TryFlush(servicingContext);
      }
    }

    private bool TryFlush(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(94000, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, nameof (TryFlush));
      try
      {
        int num = 0;
        if (!requestContext.LockManagerInternal().TryGetLock(this.m_flushLockName, 50))
          return false;
        Stopwatch stopwatch;
        try
        {
          using (requestContext.AcquireExemptionLock())
          {
            stopwatch = new Stopwatch();
            List<ServicingStepDetail> details = (List<ServicingStepDetail>) null;
            int count = 0;
            using (requestContext.AcquireWriterLock(this.m_dataLockName))
            {
              if (this.m_buffer.Count > 0)
              {
                details = this.m_buffer.GetRange(0, this.m_buffer.Count);
                count = details.Count;
              }
            }
            if (details != null)
            {
              try
              {
                if (!this.m_needToWriteLog)
                {
                  if (!this.m_needToUpdateCompletedStepsCount)
                    goto label_20;
                }
                stopwatch.Start();
                num = details.Count;
                using (ServicingComponent2 servicingComponent = (ServicingComponent2) this.CreateServicingComponent(requestContext))
                {
                  if (this.m_needToUpdateCompletedStepsCount && !this.m_needToWriteLog)
                  {
                    num = 0;
                    servicingComponent.AddServicingStepDetails(this.m_jobId, this.m_queueTime, (ICollection<ServicingStepDetail>) DatabaseServicingLogger.s_emptyStepDetailsArray, this.m_servicingHostId, this.m_completedStepCount);
                  }
                  else
                    servicingComponent.AddServicingStepDetails(this.m_jobId, this.m_queueTime, (ICollection<ServicingStepDetail>) details, this.m_servicingHostId, this.m_completedStepCount);
                }
                stopwatch.Stop();
              }
              catch (Exception ex)
              {
                requestContext.TraceException(94001, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, ex);
                return false;
              }
label_20:
              try
              {
                TeamFoundationTracingService service = requestContext.GetService<TeamFoundationTracingService>();
                foreach (ServicingStepDetail stepDetail in details)
                  service.TraceServicingStepDetail(this.m_jobId, this.m_queueTime, stepDetail);
              }
              catch (Exception ex)
              {
                requestContext.TraceException(94002, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, ex);
              }
              try
              {
                using (requestContext.AcquireWriterLock(this.m_dataLockName))
                  this.m_buffer.RemoveRange(0, count);
              }
              catch (ArgumentException ex)
              {
                this.m_buffer.Clear();
                string message = string.Format("RemoveRange (0, {0}) for buffer (length {1}) saw following exeception: {2}", (object) count, (object) this.m_buffer.Count, (object) ex.Message);
                requestContext.Trace(94003, TraceLevel.Error, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, message);
              }
            }
          }
        }
        finally
        {
          requestContext.LockManagerInternal().ReleaseLock(this.m_flushLockName);
        }
        if (num > 0)
        {
          if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          {
            string str = string.Format("It took {0} ms to write {1} log entries.", (object) stopwatch.ElapsedMilliseconds, (object) num);
            if (stopwatch.ElapsedMilliseconds > 10000L)
              requestContext.TraceAlways(94009, TraceLevel.Warning, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, str);
            else if (stopwatch.ElapsedMilliseconds > 1000L)
              requestContext.Trace(94004, TraceLevel.Warning, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, str);
            else
              requestContext.Trace(94005, TraceLevel.Info, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, str);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(94000, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, nameof (TryFlush));
      }
      return true;
    }

    public void Flush(IVssRequestContext requestContext)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      TimeSpan timeSpan = TimeSpan.FromMinutes(requestContext.ExecutionEnvironment.IsHostedDeployment ? 1.0 : 5.0);
      while (!this.TryFlush(requestContext))
      {
        if (stopwatch.Elapsed < timeSpan)
          Thread.Sleep(new Random().Next(100, 500));
        else
          requestContext.TraceAlways(94006, TraceLevel.Error, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, "DatabaseServicingLogger.Flush hit timeout.");
      }
      if (stopwatch.ElapsedMilliseconds >= 15000L)
        requestContext.TraceAlways(94007, TraceLevel.Warning, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, "DatabaseServicingLogger.Flush took {0} ms.", (object) stopwatch.ElapsedMilliseconds);
      else
        requestContext.Trace(94008, TraceLevel.Info, DatabaseServicingLogger.s_area, DatabaseServicingLogger.s_layer, "DatabaseServicingLogger.Flush took {0} ms.", (object) stopwatch.ElapsedMilliseconds);
    }

    private ServicingComponent CreateServicingComponent(IVssRequestContext requestContext) => ServicingComponent.Create(requestContext.To(TeamFoundationHostType.Deployment));

    private void CheckDisposed()
    {
      if (this.m_task == null)
        throw new ObjectDisposedException(nameof (DatabaseServicingLogger));
    }
  }
}
