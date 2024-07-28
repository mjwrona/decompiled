// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.IImageDetailsRepositoryService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  [DefaultServiceImplementation(typeof (ImageDetailsService))]
  public interface IImageDetailsRepositoryService : IVssFrameworkService
  {
    ArtifactTraceabilityResourceInfo GetImageRepositoryDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      string imageName,
      string hostName);
  }
}
