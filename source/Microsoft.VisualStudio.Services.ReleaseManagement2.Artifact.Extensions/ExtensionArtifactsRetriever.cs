// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.ExtensionArtifactsRetriever
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  public static class ExtensionArtifactsRetriever
  {
    public static readonly string ArtifactsContributionId = "ms.vss-releaseartifact.artifact-types";

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception if we aren't able to load some artifact, want to set the log the error.")]
    public static IEnumerable<ArtifactTypeBase> GetExtensionArtifacts(
      IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      List<ArtifactTypeBase> source = new List<ArtifactTypeBase>();
      IContributionService service = requestContext.GetService<IContributionService>();
      IEnumerable<Contribution> contributions;
      if (requestContext.IsUserContext)
        contributions = service.QueryContributions(requestContext, (IEnumerable<string>) new List<string>()
        {
          ExtensionArtifactsRetriever.ArtifactsContributionId
        }, queryOptions: ContributionQueryOptions.IncludeChildren);
      else
        contributions = service.QueryContributionsForTarget(requestContext, ExtensionArtifactsRetriever.ArtifactsContributionId);
      foreach (Contribution artifactContribution in contributions)
      {
        try
        {
          CustomArtifact customArtifact = artifactContribution.ToCustomArtifact();
          source.Add((ArtifactTypeBase) customArtifact);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1973202, TraceLevel.Info, "ReleaseManagementService", "Service", ex);
        }
      }
      return !requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks") ? source.Where<ArtifactTypeBase>((Func<ArtifactTypeBase, bool>) (a => !string.Equals(a.Name, "Jenkins", StringComparison.OrdinalIgnoreCase))) : (IEnumerable<ArtifactTypeBase>) source;
    }

    private static CustomArtifact ToCustomArtifact(this Contribution artifactContribution)
    {
      JObject properties = artifactContribution.Properties;
      return new CustomArtifact(artifactContribution.Id, properties.ToObject<ArtifactContributionDefinition>());
    }
  }
}
