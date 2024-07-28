// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlHttpHandler
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public abstract class VersionControlHttpHandler : TeamFoundationHttpHandler
  {
    private VersionControlRequestContext m_versionControlRequestContext;
    private bool m_methodEntered;

    protected VersionControlHttpHandler()
    {
      this.RequestContext.ServiceName = "Version Control";
      this.m_versionControlRequestContext = new VersionControlRequestContext(this.RequestContext);
    }

    protected abstract string Layer { get; }

    protected abstract MethodType MethodType { get; }

    protected abstract EstimatedMethodCost EstimatedMethodCost { get; }

    protected override void EnterMethod(MethodInformation methodInformation)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentServerRequests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentServerRequestsPerSec").Increment();
      base.EnterMethod(methodInformation);
      this.m_methodEntered = true;
    }

    protected override void LeaveMethod()
    {
      if (!this.m_methodEntered)
      {
        try
        {
          this.EnterMethod(new MethodInformation(this.Layer + "_NeverEntered", this.MethodType, this.EstimatedMethodCost));
        }
        catch (Exception ex)
        {
          this.RequestContext.TraceCatch(700318, TraceArea.General, this.Layer, ex);
        }
      }
      base.LeaveMethod();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentServerRequests").Decrement();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.AverageResponseTime").IncrementTicks(DateTime.UtcNow - this.RequestContext.StartTime());
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.AverageResponseTimeBase").Increment();
    }

    protected MethodInformation GetMethodInformation() => new MethodInformation(this.Layer, this.MethodType, this.EstimatedMethodCost, true, true);

    protected MethodInformation GetMethodInformation(TimeSpan overrideTimeOut) => new MethodInformation(this.Layer, this.MethodType, this.EstimatedMethodCost, true, true, overrideTimeOut);

    protected abstract void Execute();

    protected override sealed void ProcessRequestImpl(HttpContext context)
    {
      try
      {
        this.Execute();
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    internal VersionControlRequestContext VersionControlRequestContext => this.m_versionControlRequestContext;

    internal TeamFoundationVersionControlService VersionControlService => this.m_versionControlRequestContext.VersionControlService;
  }
}
