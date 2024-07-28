// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.DeploymentResourceConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.DeploymentTracking.WebApi;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class DeploymentResourceConverter
  {
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Cannot avoid this as this is a deployment resource object which is a core contract")]
    public static Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource ConvertToWebApiContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentResource resource,
      Guid projectId)
    {
      if (resource == null)
        throw new ArgumentNullException(nameof (resource));
      return new Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource()
      {
        Id = resource.Id,
        ResourceIdentifier = resource.ResourceIdentifier,
        ReleaseDefinitionId = resource.ReleaseDefinitionId,
        DefinitionEnvironmentId = resource.DefinitionEnvironmentId,
        ProjectReference = new ProjectReference()
        {
          Id = projectId
        }
      };
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be reviewed")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentResource ConvertToModel(
      this Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource resourceContract)
    {
      if (resourceContract == null)
        throw new ArgumentNullException(nameof (resourceContract));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentResource()
      {
        Id = resourceContract.Id,
        ResourceIdentifier = resourceContract.ResourceIdentifier,
        ReleaseDefinitionId = resourceContract.ReleaseDefinitionId,
        DefinitionEnvironmentId = resourceContract.DefinitionEnvironmentId
      };
    }
  }
}
