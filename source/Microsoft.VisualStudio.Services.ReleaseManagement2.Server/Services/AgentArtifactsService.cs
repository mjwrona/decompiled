// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.AgentArtifactsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class AgentArtifactsService : ArtifactTypeServiceBase
  {
    public const string StartCommitArtifactVersionKey = "StartCommitArtifactVersion";
    public const string EndCommitArtifactVersionKey = "EndCommitArtifactVersion";

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This is intended here")]
    public IEnumerable<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition> GetAgentArtifacts(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      bool includeCommitDetails = false)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      List<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition> agentArtifacts = new List<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition>();
      Func<AgentArtifactSqlComponent, IEnumerable<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition>> action = (Func<AgentArtifactSqlComponent, IEnumerable<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition>>) (component => component.ListAgentArtifacts(projectId, releaseId));
      foreach (Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition serverArtifact in context.ExecuteWithinUsingWithComponent<AgentArtifactSqlComponent, IEnumerable<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition>>(action))
      {
        ArtifactTypeBase artifactType = this.GetArtifactType(context, serverArtifact.ArtifactTypeId);
        if (!AgentArtifactsService.ShouldSkipDownloadUsingAgent(context, projectId, releaseId, artifactType, serverArtifact))
        {
          Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition agentArtifact1 = artifactType.ToAgentArtifact(context, projectId, serverArtifact);
          if (includeCommitDetails && ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsCommitsTraceability(context, serverArtifact.ArtifactTypeId) && !serverArtifact.SourceData.ContainsKey("ArtifactDetailsReference"))
          {
            IVssRequestContext requestContext = context;
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Id = projectId;
            int releaseId1 = releaseId;
            string artifactTypeId = serverArtifact.ArtifactTypeId;
            Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition agentArtifact2 = agentArtifact1;
            AgentArtifactsService.AddArtifactCommitDetails(requestContext, projectInfo, releaseId1, artifactTypeId, agentArtifact2);
          }
          agentArtifacts.Add(agentArtifact1);
        }
      }
      return (IEnumerable<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition>) agentArtifacts;
    }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)", Justification = "By design.")]
    private static bool ShouldSkipDownloadUsingAgent(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      ArtifactTypeBase artifactResolver,
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition serverArtifact)
    {
      if (artifactResolver is CustomArtifact customArtifact && (!string.Equals(customArtifact.ArtifactType, "Build", StringComparison.OrdinalIgnoreCase) || context.IsFeatureEnabled("VisualStudio.ReleaseManagement.CustomBuildArtifactsTasks")))
        return true;
      PropertyValue propertyValue = ReleasePropertyExtensions.GetReleasePropertyValues(context, releaseId, projectId, (IEnumerable<string>) new List<string>()
      {
        "DownloadBuildArtifactsUsingTask"
      }).FirstOrDefault<PropertyValue>();
      bool result = false;
      if (propertyValue != null)
        bool.TryParse((string) propertyValue.Value, out result);
      return result && (customArtifact != null && artifactResolver.Name.Equals("Jenkins") && context.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks") || serverArtifact.ArtifactTypeId == "Jenkins" || serverArtifact.ArtifactTypeId == "Build" && !serverArtifact.ToArtifactSource().IsXamlBuildArtifact(context));
    }

    private static void AddArtifactCommitDetails(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int releaseId,
      string artifactTypeId,
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition agentArtifact)
    {
      Release release = ReleaseWorkItemsCommitsHelper.GetRelease(requestContext, projectInfo, releaseId);
      if (release != null)
      {
        PipelineArtifactSource releaseArtifactSource1 = ReleaseWorkItemsCommitsHelper.GetReleaseArtifactSource(release, artifactTypeId, string.Empty, true);
        if (releaseArtifactSource1 != null)
        {
          Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(agentArtifact.Details);
          dictionary["EndCommitArtifactVersion"] = releaseArtifactSource1.VersionId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          Release previousRelease = ReleaseWorkItemsCommitsHelper.GetPreviousRelease(requestContext, projectInfo, release, artifactTypeId, releaseArtifactSource1.SourceId);
          if (previousRelease != null)
          {
            PipelineArtifactSource releaseArtifactSource2 = ReleaseWorkItemsCommitsHelper.GetReleaseArtifactSource(previousRelease, artifactTypeId, releaseArtifactSource1.SourceId, true);
            if (releaseArtifactSource2 != null)
              dictionary["StartCommitArtifactVersion"] = releaseArtifactSource2.VersionId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            else
              AgentArtifactsService.TraceInfo(requestContext, 1976404, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Artifact source {0} is not primary in release {1}. Commits will be fetched only for current release artifact", (object) releaseArtifactSource1.SourceId, (object) releaseId));
          }
          else
            AgentArtifactsService.TraceInfo(requestContext, 1976403, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Previous release is not found for sourceId {0}, commits will be downloaded for only target release", (object) releaseArtifactSource1.SourceId));
          agentArtifact.Details = JsonConvert.SerializeObject((object) dictionary);
        }
        else
          AgentArtifactsService.TraceInfo(requestContext, 1976402, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No primary artifact with the type {0} found in end release {1}. Not downloading the commits", (object) artifactTypeId, (object) releaseId));
      }
      else
        AgentArtifactsService.TraceError(requestContext, 1976401, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Target release {0} not found. Not sending commit version option to download commits", (object) releaseId));
    }

    private static void TraceError(
      IVssRequestContext requestContext,
      int tracePoint,
      string message)
    {
      requestContext.Trace(tracePoint, TraceLevel.Error, "ReleaseManagementService", "Service", message);
    }

    private static void TraceInfo(
      IVssRequestContext requestContext,
      int tracePoint,
      string message)
    {
      requestContext.Trace(tracePoint, TraceLevel.Info, "ReleaseManagementService", "Service", message);
    }
  }
}
