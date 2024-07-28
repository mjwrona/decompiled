// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PackageProviders.PackageProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PackageProviders
{
  public class PackageProvider : IPackageProvider
  {
    public void Validate(
      IVssRequestContext requestContext,
      PackageResource package,
      ServiceEndpoint endpoint)
    {
      if (string.IsNullOrEmpty(package.Type))
        throw new ResourceValidationException(TaskResources.PackageResourceTypeInputRequired(), PackagePropertyNames.Type);
      if (requestContext.GetService<IArtifactService>().GetArtifactType(requestContext, package.Type) == null)
        throw new ResourceValidationException(TaskResources.CannotFindPackageResourceExtension((object) package.Type), BuildPropertyNames.Type);
      if (package.Endpoint == null)
        throw new ResourceValidationException(TaskResources.PackageResourceConnectionInputRequired(), "Endpoint");
      if (endpoint == null)
        throw new ResourceValidationException(TaskResources.PipelineResourceInvalidConnectionInput((object) package.Endpoint.Name), "Endpoint");
      if (string.IsNullOrEmpty(package.Name))
        throw new ResourceValidationException(TaskResources.PackageResourceNameInputRequired(), PackagePropertyNames.Type);
      if (!string.Equals(endpoint.Type, "GitHub", StringComparison.OrdinalIgnoreCase) || !string.Equals(endpoint.Authorization.Scheme, "PersonalAccessToken", StringComparison.OrdinalIgnoreCase) && !string.Equals(endpoint.Authorization.Scheme, "Token", StringComparison.OrdinalIgnoreCase))
        throw new ResourceValidationException(TaskResources.GitHubPATSupportedForPackageResources(), "Endpoint");
    }
  }
}
