// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.ArtifactMetadataHelper
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Settings;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public static class ArtifactMetadataHelper
  {
    private const string Project = "Project";
    private const string PipelinesGeneral = "Pipelines/General/Settings";

    public static bool IsPublishPipelineMetadataEnabled(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.GetService<ISettingsService>().GetValue<ArtifactMetadataHelper.PipelineGeneralSettings>(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "Pipelines/General/Settings", ArtifactMetadataHelper.PipelineGeneralSettings.Default).PublishPipelineMetadata;
    }

    public struct PipelineGeneralSettings
    {
      public bool StatusBadgesArePublic;
      public bool EnforceSettableVar;
      public bool EnforceJobAuthScope;
      public bool PublishPipelineMetadata;
      public static readonly ArtifactMetadataHelper.PipelineGeneralSettings Default = new ArtifactMetadataHelper.PipelineGeneralSettings()
      {
        StatusBadgesArePublic = true,
        EnforceSettableVar = false,
        EnforceJobAuthScope = false,
        PublishPipelineMetadata = false
      };
    }
  }
}
