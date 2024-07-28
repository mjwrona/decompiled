// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Converters.ReleaseDefinitionSummaryConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Converters
{
  public class ReleaseDefinitionSummaryConverter
  {
    private readonly Func<IVssRequestContext, int, string, string, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference> getReleaseDefinitionShallowReference;
    private readonly Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IVssRequestContext, Guid, bool, bool, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> getReleaseContract;

    public ReleaseDefinitionSummaryConverter()
      : this(ReleaseDefinitionSummaryConverter.\u003C\u003EO.\u003C0\u003E__GetReleaseDefinitionShallowReference ?? (ReleaseDefinitionSummaryConverter.\u003C\u003EO.\u003C0\u003E__GetReleaseDefinitionShallowReference = new Func<IVssRequestContext, int, string, string, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference>(ReleaseIdentityHandler.GetReleaseDefinitionShallowReference)), ReleaseDefinitionSummaryConverter.\u003C\u003EO.\u003C1\u003E__ToContract ?? (ReleaseDefinitionSummaryConverter.\u003C\u003EO.\u003C1\u003E__ToContract = new Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IVssRequestContext, Guid, bool, bool, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>(ReleaseExtensions.ToContract)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected ReleaseDefinitionSummaryConverter(
      Func<IVssRequestContext, int, string, string, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference> getReleaseDefinitionShallowReference,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IVssRequestContext, Guid, bool, bool, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> getReleaseContract1)
    {
      this.getReleaseDefinitionShallowReference = getReleaseDefinitionShallowReference;
      this.getReleaseContract = getReleaseContract1;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionSummary ConvertToWebApi(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionSummary releaseDefinitionSummary,
      IVssRequestContext context,
      Guid projectId)
    {
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.GetReleaseDefinitionSummary", 1971011))
        return new ReleaseDefinitionSummaryConverter().ToWebApi(releaseDefinitionSummary, context, projectId);
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionSummary ToWebApi(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionSummary releaseDefinitionSummary,
      IVssRequestContext context,
      Guid projectId)
    {
      if (releaseDefinitionSummary == null)
        throw new ArgumentNullException(nameof (releaseDefinitionSummary));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionSummary webApiSummary = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionSummary();
      if (releaseDefinitionSummary.Releases.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>())
        webApiSummary.Releases = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>();
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference shallowReference = this.getReleaseDefinitionShallowReference(context, releaseDefinitionSummary.ReleaseDefinition.Id, releaseDefinitionSummary.ReleaseDefinition.Name, releaseDefinitionSummary.ReleaseDefinition.Path, projectId);
      webApiSummary.ReleaseDefinitionReference = shallowReference;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releaseDefinitionSummary.Releases)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release releaseContract = this.getReleaseContract(release, context, projectId, false, false);
        ReleaseDefinitionSummaryConverter.CleanupReleaseContract(releaseContract);
        webApiSummary.Releases.Add(releaseContract);
      }
      ReleaseDefinitionSummaryConverter.PopulateEnvironmentsData(webApiSummary, (IEnumerable<ReleaseToEnvironmentMap>) releaseDefinitionSummary.ReleaseToEnvironmentMap, context, projectId);
      return webApiSummary;
    }

    private static void CleanupReleaseContract(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release releaseContract)
    {
      releaseContract.ReleaseDefinitionReference = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference) null;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) releaseContract.Environments)
      {
        environment.DeployPhasesSnapshot.Clear();
        foreach (DeploymentAttempt deployStep in environment.DeploySteps)
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase releaseDeployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase>) deployStep.ReleaseDeployPhases)
            releaseDeployPhase.DeploymentJobs = (IList<DeploymentJob>) new List<DeploymentJob>();
        }
      }
    }

    private static void PopulateEnvironmentsData(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionSummary webApiSummary,
      IEnumerable<ReleaseToEnvironmentMap> releaseToEnvironmentMap,
      IVssRequestContext context,
      Guid projectId)
    {
      foreach (ReleaseDefinitionEnvironmentSummary environmentSummary in releaseToEnvironmentMap.GroupBy<ReleaseToEnvironmentMap, int>((Func<ReleaseToEnvironmentMap, int>) (map => map.DefinitionEnvironmentId)).Select<IGrouping<int, ReleaseToEnvironmentMap>, ReleaseDefinitionEnvironmentSummary>((Func<IGrouping<int, ReleaseToEnvironmentMap>, ReleaseDefinitionEnvironmentSummary>) (group => ReleaseDefinitionSummaryConverter.GetEnvironmentReference(group.ToList<ReleaseToEnvironmentMap>(), context, projectId))).ToList<ReleaseDefinitionEnvironmentSummary>())
        webApiSummary.Environments.Add(environmentSummary);
    }

    private static ReleaseDefinitionEnvironmentSummary GetEnvironmentReference(
      List<ReleaseToEnvironmentMap> releaseToEnvironmentMap,
      IVssRequestContext context,
      Guid projectId)
    {
      int definitionEnvironmentId = releaseToEnvironmentMap.First<ReleaseToEnvironmentMap>().DefinitionEnvironmentId;
      ReleaseDefinitionEnvironmentSummary environmentReference = new ReleaseDefinitionEnvironmentSummary()
      {
        Id = definitionEnvironmentId
      };
      foreach (ReleaseToEnvironmentMap toEnvironmentMap in releaseToEnvironmentMap.Where<ReleaseToEnvironmentMap>((Func<ReleaseToEnvironmentMap, bool>) (map => map.ReleaseId != 0)))
        environmentReference.LastReleases.Add(ShallowReferencesHelper.CreateReleaseShallowReference(context, projectId, toEnvironmentMap.ReleaseId, (string) null));
      return environmentReference;
    }
  }
}
