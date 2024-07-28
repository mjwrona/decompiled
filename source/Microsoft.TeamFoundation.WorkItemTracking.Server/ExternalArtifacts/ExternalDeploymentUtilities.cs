// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ExternalDeploymentUtilities
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts
{
  internal static class ExternalDeploymentUtilities
  {
    internal static ExternalDeploymentDataset ConvertToExternalDeploymentDataset(
      Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalDeployment deployment)
    {
      return new ExternalDeploymentDataset()
      {
        ArtifactId = deployment.ArtifactId,
        PipelineId = deployment.Pipeline.Id,
        PipelineDisplayName = deployment.Pipeline.DisplayName,
        PipelineUrl = deployment.Pipeline.Url.AbsoluteUri,
        EnvironmentId = deployment.Environment.Id,
        EnvironmentDisplayName = deployment.Environment.DisplayName,
        EnvironmentType = deployment.Environment.Type,
        RunId = deployment.RunId,
        SequenceNumber = deployment.SequenceNumber,
        DisplayName = deployment.DisplayName,
        Description = deployment.Description,
        Status = deployment.Status,
        Group = deployment.Group,
        Url = deployment.Url.AbsoluteUri,
        StatusDate = deployment.StatusDate,
        CreatedBy = deployment.CreatedBy
      };
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalDeployment ConvertToServerExternalDeployment(
      ExternalDeploymentDataset dataset)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalDeployment()
      {
        ArtifactId = dataset.ArtifactId,
        Pipeline = new Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalPipeline()
        {
          Id = dataset.PipelineId,
          DisplayName = dataset.PipelineDisplayName,
          Url = new Uri(dataset.PipelineUrl)
        },
        Environment = new Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalEnvironment()
        {
          Id = dataset.EnvironmentId,
          DisplayName = dataset.EnvironmentDisplayName,
          Type = dataset.EnvironmentType
        },
        RunId = dataset.RunId,
        SequenceNumber = dataset.SequenceNumber,
        DisplayName = dataset.DisplayName,
        Description = dataset.Description,
        Status = dataset.Status,
        Group = dataset.Group,
        Url = new Uri(dataset.Url),
        StatusDate = dataset.StatusDate,
        CreatedBy = dataset.CreatedBy
      };
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ExternalDeployment ConvertToClientExternalDeployment(
      Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalDeployment externalDeployment)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ExternalDeployment()
      {
        ArtifactId = externalDeployment.ArtifactId,
        Pipeline = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ExternalPipeline()
        {
          Id = externalDeployment.Pipeline.Id,
          DisplayName = externalDeployment.Pipeline.DisplayName,
          Url = externalDeployment.Pipeline.Url
        },
        Environment = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ExternalEnvironment()
        {
          Id = externalDeployment.Environment.Id,
          DisplayName = externalDeployment.Environment.DisplayName,
          Type = externalDeployment.Environment.Type
        },
        RunId = externalDeployment.RunId,
        SequenceNumber = externalDeployment.SequenceNumber,
        DisplayName = externalDeployment.DisplayName,
        Description = externalDeployment.Description,
        Status = externalDeployment.Status,
        Group = externalDeployment.Group,
        Url = externalDeployment.Url,
        StatusDate = externalDeployment.StatusDate,
        CreatedBy = externalDeployment.CreatedBy
      };
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalDeployment ConvertToServerExternalDeployment(
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ExternalDeployment externalDeployment)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalDeployment()
      {
        ArtifactId = externalDeployment.ArtifactId,
        Pipeline = new Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalPipeline()
        {
          Id = externalDeployment.Pipeline.Id,
          DisplayName = externalDeployment.Pipeline.DisplayName,
          Url = externalDeployment.Pipeline.Url
        },
        Environment = new Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalEnvironment()
        {
          Id = externalDeployment.Environment.Id,
          DisplayName = externalDeployment.Environment.DisplayName,
          Type = externalDeployment.Environment.Type
        },
        RunId = externalDeployment.RunId,
        SequenceNumber = externalDeployment.SequenceNumber,
        DisplayName = externalDeployment.DisplayName,
        Description = externalDeployment.Description,
        Status = externalDeployment.Status,
        Group = externalDeployment.Group,
        Url = externalDeployment.Url,
        StatusDate = externalDeployment.StatusDate,
        CreatedBy = externalDeployment.CreatedBy
      };
    }
  }
}
