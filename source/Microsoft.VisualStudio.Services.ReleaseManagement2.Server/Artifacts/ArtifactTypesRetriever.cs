// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Artifacts.ArtifactTypesRetriever
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Build;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Jenkins;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Tfvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Artifacts
{
  public static class ArtifactTypesRetriever
  {
    public static IReadOnlyList<ArtifactTypeBase> GetArtifactTypes(IVssRequestContext requestContext)
    {
      List<ArtifactTypeBase> internalArtifactTypes = ArtifactTypesRetriever.GetInternalArtifactTypes(requestContext);
      internalArtifactTypes.AddRange(ExtensionArtifactsRetriever.GetExtensionArtifacts(requestContext));
      return (IReadOnlyList<ArtifactTypeBase>) internalArtifactTypes;
    }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Reviewed")]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It makes more sense to keep this as method")]
    public static List<ArtifactTypeBase> GetInternalArtifactTypes(IVssRequestContext requestContext)
    {
      List<ArtifactTypeBase> internalArtifactTypes = new List<ArtifactTypeBase>();
      bool isUseNewBrandingEnabled = requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UseNewBranding");
      bool isCustomArtifactTypeSupported = requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.ShowServiceConnectionsUsedInLinkedArtifacts");
      internalArtifactTypes.AddRange((IEnumerable<ArtifactTypeBase>) new List<ArtifactTypeBase>()
      {
        (ArtifactTypeBase) new BuildArtifact(isCustomArtifactTypeSupported),
        (ArtifactTypeBase) new GitArtifact(isUseNewBrandingEnabled),
        (ArtifactTypeBase) new GitHubArtifact(),
        (ArtifactTypeBase) new TfvcArtifact(),
        (ArtifactTypeBase) new NullArtifact()
      });
      if (!requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks"))
        internalArtifactTypes.Add((ArtifactTypeBase) new JenkinsArtifact());
      return internalArtifactTypes;
    }
  }
}
