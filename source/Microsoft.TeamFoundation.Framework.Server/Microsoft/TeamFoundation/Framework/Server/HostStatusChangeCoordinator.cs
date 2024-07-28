// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostStatusChangeCoordinator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostStatusChangeCoordinator : IDisposable
  {
    private SemaphoreSlim m_semaphore;
    private readonly Guid m_hostId;
    private readonly object m_processLock = new object();
    private readonly HashSet<Guid> m_processesToWaitFor;
    private readonly Func<IVssRequestContext, List<TeamFoundationServiceHostProcess>> m_refreshHosts;
    private static readonly string s_Area = "HostManagement";
    private static readonly string s_Layer = nameof (HostStatusChangeCoordinator);

    public HostStatusChangeCoordinator(
      IVssRequestContext requestContext,
      Guid hostId,
      Func<IVssRequestContext, List<TeamFoundationServiceHostProcess>> refreshHosts)
    {
      this.m_hostId = hostId;
      this.m_refreshHosts = refreshHosts;
      this.m_processesToWaitFor = new HashSet<Guid>(refreshHosts(requestContext).Select<TeamFoundationServiceHostProcess, Guid>((Func<TeamFoundationServiceHostProcess, Guid>) (p => p.ProcessId)));
      this.m_semaphore = new SemaphoreSlim(0);
    }

    public void ProcessMessage(
      IVssRequestContext requestContext,
      ServiceHostStatusChangedResponse response)
    {
      if (!(response.HostId == this.m_hostId))
        return;
      if (response.Status == TeamFoundationServiceHostStatus.Stopped)
      {
        lock (this.m_processLock)
        {
          requestContext.Trace(57125, TraceLevel.Info, HostStatusChangeCoordinator.s_Area, HostStatusChangeCoordinator.s_Layer, "Process {0} acknowledged Host {1} transitioning to {2} response processed at {3}", (object) response.ProcessId, (object) response.HostId, (object) response.Status, (object) response.TimeProcessed);
          if (!this.m_processesToWaitFor.Remove(response.ProcessId) || this.m_semaphore == null)
            return;
          this.m_semaphore.Release();
        }
      }
      else
        requestContext.Trace(57125, TraceLevel.Error, HostStatusChangeCoordinator.s_Area, HostStatusChangeCoordinator.s_Layer, "Process {0} acknowledged Host {1} could not be stoppped - response processed at {2}", (object) response.ProcessId, (object) response.HostId, (object) response.TimeProcessed);
    }

    private bool HasTransitioned()
    {
      lock (this.m_processesToWaitFor)
        return this.m_processesToWaitFor.Count == 0;
    }

    public bool Wait(IVssRequestContext requestContext, TimeSpan timeout)
    {
      TimeSpan timeSpan1 = new TimeSpan(24, 0, 0);
      DateTime utcNow1 = DateTime.UtcNow;
      requestContext.Trace(57122, TraceLevel.Verbose, HostStatusChangeCoordinator.s_Area, HostStatusChangeCoordinator.s_Layer, "Waiting for notification");
      TimeSpan timeSpan2 = new TimeSpan(0, 1, 0);
      TimeSpan timeSpan3 = new TimeSpan(Math.Min(timeout.Ticks, timeSpan2.Ticks));
      if (timeout.TotalMilliseconds >= (double) int.MaxValue)
        timeout = timeSpan1;
      do
      {
        DateTime utcNow2 = DateTime.UtcNow;
        do
        {
          TimeSpan timeout1 = utcNow2 + timeSpan3 - DateTime.UtcNow;
          if (timeout1 <= TimeSpan.Zero || !this.m_semaphore.Wait(timeout1))
            requestContext.TraceAlways(57130, TraceLevel.Info, HostStatusChangeCoordinator.s_Area, HostStatusChangeCoordinator.s_Layer, "No responses within {0}", (object) timeSpan3);
        }
        while (!this.HasTransitioned() && utcNow2.Add(timeSpan3) > DateTime.UtcNow);
        if (!this.HasTransitioned())
        {
          List<TeamFoundationServiceHostProcess> source = this.m_refreshHosts(requestContext);
          lock (this.m_processLock)
          {
            foreach (Guid guid in (IEnumerable<Guid>) this.m_processesToWaitFor.Except<Guid>(source.Select<TeamFoundationServiceHostProcess, Guid>((Func<TeamFoundationServiceHostProcess, Guid>) (p => p.ProcessId))).ToList<Guid>())
            {
              if (this.m_processesToWaitFor.Remove(guid))
              {
                requestContext.Trace(57127, TraceLevel.Error, HostStatusChangeCoordinator.s_Area, HostStatusChangeCoordinator.s_Layer, "Detected that Process ID {0} has gone", (object) guid);
                this.m_semaphore.Release();
              }
            }
            foreach (TeamFoundationServiceHostProcess serviceHostProcess in source)
            {
              if (this.m_processesToWaitFor.Contains(serviceHostProcess.ProcessId))
                requestContext.TraceAlways(57131, TraceLevel.Warning, HostStatusChangeCoordinator.s_Area, HostStatusChangeCoordinator.s_Layer, "Still waiting on {0} ({1}, pid {2}). {3} elapsed.", (object) serviceHostProcess.ProcessName, (object) serviceHostProcess.MachineName, (object) serviceHostProcess.OSProcessId, (object) (DateTime.UtcNow - utcNow1));
            }
          }
        }
      }
      while (!this.HasTransitioned() && utcNow1.Add(timeout) > DateTime.UtcNow);
      if (this.HasTransitioned())
      {
        requestContext.TraceAlways(57123, TraceLevel.Info, HostStatusChangeCoordinator.s_Area, HostStatusChangeCoordinator.s_Layer, "Host {0} transitioned successfully", (object) this.m_hostId);
      }
      else
      {
        requestContext.TraceAlways(57124, TraceLevel.Error, HostStatusChangeCoordinator.s_Area, HostStatusChangeCoordinator.s_Layer, "Host {0} did not transition in the time allowed ({1})", (object) this.m_hostId, (object) timeout);
        lock (this.m_processLock)
        {
          List<TeamFoundationServiceHostProcess> serviceHostProcessList = this.m_refreshHosts(requestContext);
          if (serviceHostProcessList != null)
          {
            foreach (TeamFoundationServiceHostProcess serviceHostProcess in serviceHostProcessList)
            {
              if (this.m_processesToWaitFor.Contains(serviceHostProcess.ProcessId))
                requestContext.Trace(57132, TraceLevel.Error, HostStatusChangeCoordinator.s_Area, HostStatusChangeCoordinator.s_Layer, "Host {0} did not transition; timed out waiting on {1} ({2}, pid {3}). Timeout: {4}.", (object) this.m_hostId, (object) serviceHostProcess.ProcessName, (object) serviceHostProcess.MachineName, (object) serviceHostProcess.OSProcessId, (object) timeout);
            }
          }
        }
      }
      return this.HasTransitioned();
    }

    public void Dispose()
    {
      this.m_semaphore?.Dispose();
      this.m_semaphore = (SemaphoreSlim) null;
    }
  }
}
