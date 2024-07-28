// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.GdprDataCleanup.GdprDataCleanupJobHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.GdprDataCleanupJob;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.GdprDataCleanup
{
  public class GdprDataCleanupJobHandler : IHandler<TeamFoundationJobDefinition, JobResult>
  {
    private IPackageGdprDataStore gdprDataStore;
    private ITracerService tracerService;
    private IFeatureFlagService featureFlagService;

    public GdprDataCleanupJobHandler(
      IPackageGdprDataStore gdprDataStore,
      IFeatureFlagService featureFlagService,
      ITracerService tracerService)
    {
      this.gdprDataStore = gdprDataStore;
      this.tracerService = tracerService;
      this.featureFlagService = featureFlagService;
    }

    public JobResult Handle(TeamFoundationJobDefinition request)
    {
      using (this.tracerService.Enter((object) this, nameof (Handle)))
      {
        if (!this.featureFlagService.IsEnabled("Packaging.GdprCleanupJobEnabled"))
          return JobResult.Succeeded(new JobTelemetry()
          {
            Message = "Packaging.GdprCleanupJobEnabled feature flag is disabled."
          });
        GdprCleanupJobTelemetry telemetry1 = new GdprCleanupJobTelemetry();
        try
        {
          IEnumerable<string> source = this.gdprDataStore.DeleteExpiredData();
          telemetry1.ContainersDeleted = (IList<string>) source.ToList<string>();
          telemetry1.DeletedContainerCount = source.Count<string>();
        }
        catch (Exception ex)
        {
          GdprCleanupJobTelemetry telemetry2 = telemetry1;
          throw new JobFailedException(ex, (JobTelemetry) telemetry2);
        }
        return JobResult.Succeeded((JobTelemetry) telemetry1);
      }
    }
  }
}
