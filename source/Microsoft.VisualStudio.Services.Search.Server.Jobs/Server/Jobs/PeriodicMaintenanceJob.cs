// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.PeriodicMaintenanceJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class PeriodicMaintenanceJob : AbstractPeriodicMaintenanceJob
  {
    public PeriodicMaintenanceJob()
      : base(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), new ThresholdViolationIdentifier())
    {
    }

    internal PeriodicMaintenanceJob(
      IDataAccessFactory dataAccessFactory,
      ThresholdViolationIdentifier thresholdViolationIdentifier)
      : base(dataAccessFactory, thresholdViolationIdentifier)
    {
    }

    public override void PostRun(
      IVssRequestContext requestContext,
      StringBuilder resultMessageBuilder)
    {
    }

    public override void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
    }

    public override void ReQueueJobIfNeeded(ExecutionContext executionContext)
    {
      if (!executionContext.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      int maintenanceJobDelayInSec = executionContext.ServiceSettings.JobSettings.PeriodicMaintenanceJobDelayInSec;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(AbstractPeriodicMaintenanceJob.s_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Queuing Periodic Maintenance Job with a delay of {0} seconds", (object) maintenanceJobDelayInSec)));
      executionContext.RequestContext.QueueDelayedNamedJob(JobConstants.PeriodicMaintenanceJobId, maintenanceJobDelayInSec, JobPriorityLevel.BelowNormal);
    }
  }
}
