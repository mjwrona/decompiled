// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.DeploymentStatusConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class DeploymentStatusConverter
  {
    private static readonly IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus> ServerToWebApiStatusMap = (IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus>) new Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus>()
    {
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Undefined,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.Undefined
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.NotDeployed,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.NotDeployed
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.InProgress,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.InProgress
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Succeeded,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.Succeeded
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.PartiallySucceeded,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.PartiallySucceeded
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Failed,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.Failed
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.All,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.All
      }
    };

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus status)
    {
      return DeploymentStatusConverter.ServerToWebApiStatusMap[status];
    }
  }
}
