// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.GdprDataCleanup.GdprDataCleanupJobHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.GdprDataCleanup
{
  public class GdprDataCleanupJobHandlerBootstrapper : 
    IBootstrapper<IHandler<TeamFoundationJobDefinition, JobResult>>
  {
    private readonly IVssRequestContext deploymentContext;
    private readonly Guid jobId;

    public GdprDataCleanupJobHandlerBootstrapper(IVssRequestContext deploymentContext, Guid jobId)
    {
      this.deploymentContext = deploymentContext;
      this.jobId = jobId;
    }

    public IHandler<TeamFoundationJobDefinition, JobResult> Bootstrap() => (IHandler<TeamFoundationJobDefinition, JobResult>) new GdprDataCleanupJobHandler(new PackageGDPRDataStoreBootstrapper(this.deploymentContext).Bootstrap(), this.deploymentContext.GetFeatureFlagFacade(), this.deploymentContext.GetTracerFacade());
  }
}
