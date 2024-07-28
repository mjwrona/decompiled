// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ReleaseServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class ReleaseServiceHelper : IReleaseServiceHelper
  {
    private const int c_DaysToQueryRelease = 7;

    public virtual IReleaseHttpClientWrapper GetReleaseHttpClient(IVssRequestContext context) => (IReleaseHttpClientWrapper) new ReleaseHttpClientWrapper(context.GetClient<ReleaseHttpClient>(context.GetHttpClientOptionsForEventualReadConsistencyLevel("TestManagement.Server.GetRelease.SqlReadReplica")));

    public virtual ReleaseReference QueryReleaseReferenceById(
      IVssRequestContext context,
      GuidAndString projectId,
      int releaseId,
      int releaseEnvId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryReleaseReferenceById), "Release")))
      {
        try
        {
          Release release = this.GetReleaseHttpClient(context).GetRelease(projectId.GuidId, releaseId);
          ReleaseEnvironment env1 = release.Environments.Where<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Id == releaseEnvId)).FirstOrDefault<ReleaseEnvironment>();
          if (release != null)
          {
            if (env1 != null)
            {
              ReleaseReference releaseRef = new ReleaseReference()
              {
                ReleaseId = releaseId,
                ReleaseUri = TestManagementServiceUtility.GetArtiFactUri("Release", "ReleaseManagement", releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
                ReleaseDefId = release.ReleaseDefinitionReference.Id,
                ReleaseEnvId = releaseEnvId,
                ReleaseEnvDefId = env1.DefinitionEnvironmentId,
                ReleaseEnvUri = TestManagementServiceUtility.GetArtiFactUri("Environment", "ReleaseManagement", releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
                ReleaseName = release.Name,
                ReleaseCreationDate = release.CreatedOn,
                EnvironmentCreationDate = env1.CreatedOn.HasValue ? env1.CreatedOn.Value : DateTime.UtcNow
              };
              releaseRef.ReleaseEnvName = env1.Name;
              this.FillAttemptInReleaseRef(releaseRef, env1);
              this.FillBuildprojectId(releaseRef, release);
              return releaseRef;
            }
          }
        }
        catch (Exception ex)
        {
          context.Trace(1015125, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (ReleaseReference) null;
      }
    }

    public virtual ReleaseReference QueryReleaseReferenceByUri(
      IVssRequestContext context,
      GuidAndString projectId,
      string releaseUri,
      string releaseEnvUri)
    {
      try
      {
        int releaseArtifactId1 = this.GetReleaseArtifactId(releaseUri);
        int releaseArtifactId2 = this.GetReleaseArtifactId(releaseEnvUri);
        if (releaseArtifactId1 > 0)
        {
          if (releaseArtifactId2 > 0)
          {
            ReleaseReference releaseReference = this.QueryReleaseReferenceById(context, projectId, releaseArtifactId1, releaseArtifactId2);
            if (releaseReference != null)
            {
              releaseReference.ReleaseUri = releaseUri;
              releaseReference.ReleaseEnvUri = releaseEnvUri;
              return releaseReference;
            }
          }
        }
      }
      catch (Exception ex)
      {
        context.Trace(1015125, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (ReleaseReference) null;
    }

    public List<ReleaseDefinition> QueryReleaseDefinitionsForEnvironments(
      IVssRequestContext context,
      GuidAndString projectId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryReleaseDefinitionsForEnvironments), "Release")))
      {
        List<ReleaseDefinition> releaseDefinitionList = new List<ReleaseDefinition>();
        try
        {
          releaseDefinitionList = this.GetReleaseHttpClient(context).GetReleaseDefinitions(projectId.GuidId, ReleaseDefinitionExpands.Environments);
        }
        catch (Exception ex)
        {
          context.Trace(1015125, TraceLevel.Warning, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return releaseDefinitionList;
      }
    }

    public Dictionary<int, string> GetEnvironmentDefinitionIdToNameMap(
      IVssRequestContext context,
      GuidAndString projectId)
    {
      Dictionary<int, string> definitionIdToNameMap = new Dictionary<int, string>();
      bool flag = false;
      if (context.IsFeatureEnabled("TestManagement.Server.ReleaseEnvironmentsCache"))
      {
        List<CachedReleaseEnvironmentData> releaseEnvironments;
        if (context.GetService<ITestManagementReleaseEnvironmentCacheService>().TryGetCachedReleaseEnvironmentData(context, projectId.GuidId, out releaseEnvironments))
        {
          foreach (CachedReleaseEnvironmentData releaseEnvironmentData in releaseEnvironments)
            definitionIdToNameMap[releaseEnvironmentData.Id.ReleaseEnvDefinitionId] = releaseEnvironmentData.Name;
          return definitionIdToNameMap;
        }
        flag = true;
      }
      List<ReleaseDefinition> releaseDefinitionList = this.QueryReleaseDefinitionsForEnvironments(context, projectId);
      List<CachedReleaseEnvironmentData> releaseEnvironments1 = new List<CachedReleaseEnvironmentData>();
      foreach (ReleaseDefinition releaseDefinition in releaseDefinitionList)
      {
        foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
        {
          definitionIdToNameMap[environment.Id] = environment.Name;
          releaseEnvironments1.Add(new CachedReleaseEnvironmentData()
          {
            Id = new ReleaseEnvironmentDefinition()
            {
              ReleaseDefinitionId = releaseDefinition.Id,
              ReleaseEnvDefinitionId = environment.Id
            },
            Name = environment.Name
          });
        }
      }
      if (flag && !context.GetService<ITestManagementReleaseEnvironmentCacheService>().TryUpdateReleaseEnvironmentCache(context, projectId.GuidId, releaseEnvironments1))
        context.Trace(1015072, TraceLevel.Warning, "TestManagement", "Cache", "Release environments could not be cached for project id {0}", (object) projectId.GuidId);
      return definitionIdToNameMap;
    }

    public ReleaseReference QueryLastCompleteSuccessfulRelease(
      IVssRequestContext context,
      GuidAndString projectId,
      ReleaseReference currentRelease,
      DateTime maxCreatedTimeForRelease,
      string branchName)
    {
      return context.IsFeatureEnabled("TestManagement.Server.QueryLastCompleteSuccessfulReleaseUsingNewLogic") ? this.QueryLastCompleteSuccessfulReleaseNew(context, projectId, currentRelease, maxCreatedTimeForRelease, branchName) : this.QueryLastCompleteSuccessfulReleaseOld(context, projectId, currentRelease, maxCreatedTimeForRelease, branchName);
    }

    private ReleaseReference QueryLastCompleteSuccessfulReleaseOld(
      IVssRequestContext context,
      GuidAndString projectId,
      ReleaseReference currentRelease,
      DateTime maxCreatedTimeForRelease,
      string branchName)
    {
      context.Trace(1015125, TraceLevel.Info, "TestManagement", "BusinessLayer", "Using old logic for QueryLastCompleteSuccessfulRelease");
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryLastCompleteSuccessfulReleaseOld), "Release")))
      {
        try
        {
          List<Release> releases = this.GetReleaseHttpClient(context).GetReleases(projectId.GuidId, currentRelease.ReleaseDefId, new int?(currentRelease.ReleaseEnvDefId), ReleaseStatus.Active, maxCreatedTimeForRelease, maxCreatedTimeForRelease.AddDays(-7.0), ReleaseQueryOrder.Descending, ReleaseExpands.Environments | ReleaseExpands.Artifacts);
          Release release = (Release) null;
          if (string.IsNullOrEmpty(branchName))
          {
            if (releases != null)
            {
              for (int index = 0; index < releases.Count; ++index)
              {
                if (releases[index].Id == currentRelease.ReleaseId && index < releases.Count - 1)
                {
                  release = releases[index + 1];
                  break;
                }
              }
            }
          }
          else
            release = this.GetPreviousReleaseForBranchOld(currentRelease, releases, branchName);
          if (release != null)
          {
            ReleaseEnvironment environment = release.Environments.Where<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (e => e.DefinitionEnvironmentId == currentRelease.ReleaseEnvDefId)).FirstOrDefault<ReleaseEnvironment>();
            ReleaseReference releaseReference = (ReleaseReference) null;
            if (environment != null)
              releaseReference = this.CreateReleaseReference(release, environment);
            return releaseReference;
          }
        }
        catch (Exception ex)
        {
          context.Trace(1015126, TraceLevel.Warning, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (ReleaseReference) null;
      }
    }

    private ReleaseReference QueryLastCompleteSuccessfulReleaseNew(
      IVssRequestContext context,
      GuidAndString projectId,
      ReleaseReference currentRelease,
      DateTime maxCreatedTimeForRelease,
      string branchName)
    {
      context.Trace(1015127, TraceLevel.Info, "TestManagement", "BusinessLayer", "Using New logic for QueryLastCompleteSuccessfulRelease");
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("QueryLastCompleteSuccessfulRelease", "Release")))
      {
        try
        {
          List<Release> releases = this.GetReleaseHttpClient(context).GetReleases(projectId.GuidId, currentRelease.ReleaseDefId, new int?(currentRelease.ReleaseEnvDefId), ReleaseStatus.Active, maxCreatedTimeForRelease, maxCreatedTimeForRelease.AddDays(-7.0), ReleaseQueryOrder.Descending, ReleaseExpands.Environments | ReleaseExpands.Artifacts, new int?(148), branchName);
          if (releases == null || !releases.Any<Release>())
          {
            context.Trace(1015128, TraceLevel.Info, "TestManagement", "BusinessLayer", "No Release returned from release client");
            return (ReleaseReference) null;
          }
          foreach (Release release in releases)
          {
            if (release.Id != currentRelease.ReleaseId)
            {
              ReleaseEnvironment environment = release.Environments.Where<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (e => e.DefinitionEnvironmentId == currentRelease.ReleaseEnvDefId)).FirstOrDefault<ReleaseEnvironment>();
              ReleaseReference releaseReference = (ReleaseReference) null;
              if (environment != null)
                releaseReference = this.CreateReleaseReference(release, environment);
              return releaseReference;
            }
          }
        }
        catch (Exception ex)
        {
          context.Trace(1015126, TraceLevel.Warning, "TestManagement", "BusinessLayer", ex.ToString());
        }
        context.Trace(1015128, TraceLevel.Info, "TestManagement", "BusinessLayer", "No Release identified from the releases which could meet the criteria");
        return (ReleaseReference) null;
      }
    }

    public Dictionary<int, ReleaseReference> GetLastCompletedReleasesForEnvDefIds(
      IVssRequestContext context,
      GuidAndString projectId,
      ReleaseReference currentReleaseReference,
      DateTime releaseCreationDate)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("QueryLastCompleteSuccessfulRelease", "Release")))
      {
        try
        {
          Dictionary<int, ReleaseReference> releasesForEnvDefIds = new Dictionary<int, ReleaseReference>();
          List<Release> releases = this.GetReleaseHttpClient(context).GetReleases(projectId.GuidId, currentReleaseReference.ReleaseDefId, new int?(), ReleaseStatus.Active, releaseCreationDate, releaseCreationDate.AddDays(-7.0), ReleaseQueryOrder.Descending, ReleaseExpands.Environments | ReleaseExpands.Artifacts);
          HashSet<int> intSet = new HashSet<int>();
          if (releases != null)
          {
            for (int index = 0; index < releases.Count; ++index)
            {
              Release release = releases[index];
              foreach (ReleaseEnvironment environment in (IEnumerable<ReleaseEnvironment>) release.Environments)
              {
                if (!intSet.Contains(environment.DefinitionEnvironmentId) && currentReleaseReference.ReleaseId == release.Id)
                  intSet.Add(environment.DefinitionEnvironmentId);
                else if (intSet.Contains(environment.DefinitionEnvironmentId) && !releasesForEnvDefIds.ContainsKey(environment.DefinitionEnvironmentId))
                {
                  ReleaseReference releaseReference = this.CreateReleaseReference(release, environment);
                  releasesForEnvDefIds.Add(environment.DefinitionEnvironmentId, releaseReference);
                }
              }
            }
          }
          return releasesForEnvDefIds;
        }
        catch (Exception ex)
        {
          context.Trace(1015126, TraceLevel.Warning, "TestManagement", "BusinessLayer", ex.ToString());
        }
      }
      return (Dictionary<int, ReleaseReference>) null;
    }

    private ReleaseReference CreateReleaseReference(Release release, ReleaseEnvironment environment)
    {
      ReleaseReference releaseRef = new ReleaseReference()
      {
        ReleaseId = release.Id,
        ReleaseUri = TestManagementServiceUtility.GetArtiFactUri("Release", "ReleaseManagement", release.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
        ReleaseDefId = release.ReleaseDefinitionReference.Id,
        ReleaseEnvId = environment.Id,
        ReleaseEnvDefId = environment.DefinitionEnvironmentId,
        ReleaseEnvUri = TestManagementServiceUtility.GetArtiFactUri("Environment", "ReleaseManagement", environment.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
        ReleaseName = release.Name,
        ReleaseCreationDate = release.CreatedOn,
        EnvironmentCreationDate = environment.CreatedOn.HasValue ? environment.CreatedOn.Value : DateTime.UtcNow
      };
      this.FillAttemptInReleaseRef(releaseRef, environment);
      return releaseRef;
    }

    private void FillAttemptInReleaseRef(ReleaseReference releaseRef, ReleaseEnvironment env)
    {
      ReleaseApproval releaseApproval = env.PreDeployApprovals.LastOrDefault<ReleaseApproval>();
      if (releaseApproval == null)
        return;
      releaseRef.Attempt = releaseApproval.TrialNumber;
    }

    private void FillBuildprojectId(ReleaseReference releaseRef, Release releaseDetails)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact1;
      if (releaseDetails == null)
      {
        artifact1 = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact) null;
      }
      else
      {
        IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts = releaseDetails.Artifacts;
        artifact1 = artifacts != null ? artifacts.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, bool>) (t => t.IsPrimary)).FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>() : (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact) null;
      }
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact2 = artifact1;
      string s;
      if (artifact2 == null)
      {
        s = (string) null;
      }
      else
      {
        IDictionary<string, ArtifactSourceReference> definitionReference = artifact2.DefinitionReference;
        s = definitionReference != null ? definitionReference.GetValueOrDefault<string, ArtifactSourceReference>("version")?.Id : (string) null;
      }
      int num;
      ref int local = ref num;
      if (int.TryParse(s, out local) && num > 0)
      {
        string str1;
        if (artifact2 == null)
        {
          str1 = (string) null;
        }
        else
        {
          IDictionary<string, ArtifactSourceReference> definitionReference = artifact2.DefinitionReference;
          str1 = definitionReference != null ? definitionReference.GetValueOrDefault<string, ArtifactSourceReference>("project")?.Id : (string) null;
        }
        string str2 = str1;
        if (str2 != null)
        {
          releaseRef.PrimaryArtifactBuildId = num;
          releaseRef.PrimaryArtifactProjectId = str2;
        }
      }
      releaseRef.PrimaryArtifactType = artifact2?.Type;
    }

    private Release GetPreviousReleaseForBranchOld(
      ReleaseReference currentRelease,
      List<Release> releases,
      string branchName)
    {
      Release releaseForBranchOld = (Release) null;
      foreach (Release release in releases)
      {
        if (release.Id != currentRelease.ReleaseId && release.Artifacts != null)
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact in release.Artifacts.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, bool>) (artifact => artifact.Type.Equals("Build", StringComparison.InvariantCulture))))
          {
            ArtifactSourceReference artifactSourceReference;
            if (artifact.DefinitionReference.TryGetValue("branch", out artifactSourceReference) && artifactSourceReference.Name != null && artifactSourceReference.Name.Equals(branchName, StringComparison.InvariantCulture))
            {
              releaseForBranchOld = release;
              return releaseForBranchOld;
            }
          }
        }
      }
      return releaseForBranchOld;
    }

    public int GetReleaseArtifactId(string releaseUri)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(releaseUri);
      int result = 0;
      int.TryParse(artifactId.ToolSpecificId, out result);
      return result;
    }
  }
}
