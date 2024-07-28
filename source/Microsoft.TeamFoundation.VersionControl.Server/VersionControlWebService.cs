// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlWebService
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class VersionControlWebService : TeamFoundationWebService
  {
    private VersionControlRequestContext m_versionControlRequestContext;

    public VersionControlWebService()
    {
      this.RequestContext.ServiceName = "Version Control";
      this.m_versionControlRequestContext = new VersionControlRequestContext(this.RequestContext);
    }

    protected override void EnterMethod(MethodInformation methodInformation)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentServerRequests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentServerRequestsPerSec").Increment();
      base.EnterMethod(methodInformation);
    }

    protected override void LeaveMethod()
    {
      if (this.RequestContext is IWebRequestContextInternal requestContext)
      {
        string stableHashString = this.VersionControlService.GetServerSettingsStableHashString(this.RequestContext);
        requestContext.HttpContext.Response.AddHeader("X-TFS-RH", stableHashString);
      }
      base.LeaveMethod();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentServerRequests").Decrement();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.AverageResponseTime").IncrementTicks(DateTime.UtcNow - this.RequestContext.StartTime());
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.AverageResponseTimeBase").Increment();
    }

    internal VersionControlRequestContext VersionControlRequestContext => this.m_versionControlRequestContext;

    internal Validation Validation => this.m_versionControlRequestContext.Validation;

    internal TeamFoundationVersionControlService VersionControlService => this.m_versionControlRequestContext.VersionControlService;
  }
}
