// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CollectionPackageUpstreamRefreshJobQueuer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class CollectionPackageUpstreamRefreshJobQueuer : ICollectionPackageUpstreamRefreshJobQueuer
  {
    private readonly IJobQueuer jobService;
    private readonly ITracerService tracer;
    private readonly string packageJobExtensionFullName;
    private readonly string packageJobName;
    private const int MaxJobNameLength = 128;

    public CollectionPackageUpstreamRefreshJobQueuer(
      IJobQueuer jobServiceFacade,
      ITracerService tracer,
      string packageJobExtensionFullName,
      string packageJobName)
    {
      this.jobService = jobServiceFacade;
      this.tracer = tracer;
      this.packageJobExtensionFullName = packageJobExtensionFullName;
      this.packageJobName = packageJobName;
    }

    public void QueuePackageJob(
      string normalizedPackageName,
      IEnumerable<Guid> feedsToRefresh,
      PushDrivenUpstreamsNotificationTelemetry pushDrivenUpstreamsTelemetry)
    {
      using (ITracerBlock tracerBlock = this.tracer.Enter((object) this, nameof (QueuePackageJob)))
      {
        UpstreamMetadataCachePackageJobData objectToSerialize = new UpstreamMetadataCachePackageJobData(feedsToRefresh, normalizedPackageName, pushDrivenUpstreamsTelemetry);
        string jobName = this.packageJobName + "_multifeed_" + normalizedPackageName;
        if (jobName.Length > 128)
          jobName = jobName.Substring(0, 128);
        tracerBlock.TraceInfo("QueuePackageJob " + jobName);
        this.jobService.QueueOneTimeJob(jobName, this.packageJobExtensionFullName, TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize));
      }
    }
  }
}
