// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestAgentHealthMonitorJobHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestAgentHealthMonitorJobHelper : ITestAgentHealthMonitorJobHelper
  {
    public void QueueHealthMonitorJobAfterDelay(TestExecutionRequestContext requestContext)
    {
      Guid healthMonitorJobId = requestContext.HealthMonitorJobId;
      ITeamFoundationJobService service = requestContext.RequestContext.GetService<ITeamFoundationJobService>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      TimeSpan valueFromTfsRegistry = Utilities.GetValueFromTfsRegistry<TimeSpan>(requestContext, DtaConstants.TfsRegistryPathForHealthJobDelay, DtaConstants.DefaultHealthJobDelay, TestAgentHealthMonitorJobHelper.\u003C\u003EO.\u003C0\u003E__TryParse ?? (TestAgentHealthMonitorJobHelper.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<TimeSpan>(TimeSpan.TryParse)));
      requestContext.RequestContext.Trace(6201705, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Queue HealthMonitorJob after {0} seconds", (object) valueFromTfsRegistry.TotalSeconds);
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      Guid[] jobIds = new Guid[1]{ healthMonitorJobId };
      int totalSeconds = (int) valueFromTfsRegistry.TotalSeconds;
      service.QueueDelayedJobs(requestContext1, (IEnumerable<Guid>) jobIds, totalSeconds);
    }

    public void QueueHealthMonitorJobNow(TestExecutionRequestContext requestContext)
    {
      Guid healthMonitorJobId = requestContext.HealthMonitorJobId;
      ITeamFoundationJobService service = requestContext.RequestContext.GetService<ITeamFoundationJobService>();
      requestContext.RequestContext.Trace(6201704, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Queue HealthMonitorJob immediately");
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      Guid[] jobIds = new Guid[1]{ healthMonitorJobId };
      service.QueueJobsNow(requestContext1, (IEnumerable<Guid>) jobIds, true);
    }
  }
}
