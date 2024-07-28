// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeploymentPackageUpstreamRefreshJobQueuer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class DeploymentPackageUpstreamRefreshJobQueuer : IDeploymentPackageUpstreamRefreshJobQueuer
  {
    private readonly IJobQueuer jobService;
    private readonly ITracerService tracer;
    private readonly string packageJobExtensionFullName;
    private readonly string packageJobName;
    private ITimeProvider timeProvider;
    private IExecutionEnvironment executionEnvironment;
    private const int MaxJobNameLength = 128;

    public DeploymentPackageUpstreamRefreshJobQueuer(
      IJobQueuer jobServiceFacade,
      ITracerService tracer,
      ITimeProvider timeProvider,
      IExecutionEnvironment executionEnvironment,
      string packageJobExtensionFullName,
      string packageJobName)
    {
      this.jobService = jobServiceFacade;
      this.tracer = tracer;
      this.packageJobExtensionFullName = packageJobExtensionFullName;
      this.packageJobName = packageJobName;
      this.timeProvider = timeProvider;
      this.executionEnvironment = executionEnvironment;
    }

    public void QueuePackageJob(
      IPackageName packageName,
      IPackageVersion packageVersion,
      WellKnownUpstreamSource source,
      DateTime ingestionTimestamp)
    {
      using (ITracerBlock tracerBlock = this.tracer.Enter((object) this, nameof (QueuePackageJob)))
      {
        DeploymentPackageUpstreamRefreshJobData objectToSerialize = new DeploymentPackageUpstreamRefreshJobData(packageName.DisplayName, new PushDrivenUpstreamsNotificationTelemetry()
        {
          IngestionTimestamp = ingestionTimestamp,
          TriggerCommitType = TriggerCommitType.GenericChange,
          UpstreamToFeedNotificationSendTimestamp = this.timeProvider.Now,
          UpstreamToFeedNotificationSendActivityId = this.executionEnvironment.ActivityId,
          PackageVersion = packageVersion.DisplayVersion,
          ExternalUpstreamLocation = source.LocationUriString
        });
        string jobName = this.packageJobName + "_deployment_" + packageName.NormalizedName;
        if (jobName.Length > 128)
          jobName = jobName.Substring(0, 128);
        tracerBlock.TraceInfo("QueuePackageJob " + jobName);
        this.jobService.QueueOneTimeJob(jobName, this.packageJobExtensionFullName, TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize));
      }
    }
  }
}
