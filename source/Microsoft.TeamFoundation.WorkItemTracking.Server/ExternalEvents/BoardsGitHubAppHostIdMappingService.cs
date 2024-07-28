// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents.BoardsGitHubAppHostIdMappingService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents
{
  public class BoardsGitHubAppHostIdMappingService : 
    HostIdMappingService,
    IBoardsGitHubAppHostIdMappingService,
    IHostIdMappingService,
    IVssFrameworkService
  {
    private static readonly BoardsGitHubAppHostIdMappingProviderData providerData = BoardsGitHubAppHostIdMappingProviderData.Instance;

    protected override string Layer => nameof (BoardsGitHubAppHostIdMappingService);

    public IDictionary<string, Guid?> GetHostMappingsForRepositories(
      IVssRequestContext requestContext,
      string installationId,
      List<string> repoNodeIds)
    {
      Dictionary<string, Guid?> mappingsForRepositories = new Dictionary<string, Guid?>();
      if (string.IsNullOrEmpty(installationId) || repoNodeIds == null)
      {
        requestContext.Trace(918113, TraceLevel.Info, this.Area, this.Layer, "Parameters null or empty. installationId, repoNodeIds");
        return (IDictionary<string, Guid?>) mappingsForRepositories;
      }
      foreach (string repoNodeId in repoNodeIds)
      {
        HostIdMappingData mappingData = new HostIdMappingData()
        {
          Id = installationId,
          PropertyName = GitHubConstants.InstallationId,
          Qualifier = repoNodeId
        };
        mappingsForRepositories.Add(repoNodeId, this.GetHostId(requestContext, BoardsGitHubAppHostIdMappingService.providerData.ProviderId, mappingData, true));
      }
      return (IDictionary<string, Guid?>) mappingsForRepositories;
    }

    public IList<Guid> GetActiveHostIds(
      IVssRequestContext requestContext,
      IList<Guid> collectionIds)
    {
      return (IList<Guid>) collectionIds.Where<Guid>((Func<Guid, bool>) (id => this.HostHasActiveEntries(requestContext, id))).ToList<Guid>();
    }

    public bool AddHostMappingForRepository(
      IVssRequestContext requestContext,
      string installationId,
      string repoNodeId)
    {
      ArgumentUtility.CheckForNull<string>(installationId, nameof (installationId));
      ArgumentUtility.CheckForNull<string>(repoNodeId, nameof (repoNodeId));
      if (string.IsNullOrEmpty(installationId) || string.IsNullOrEmpty(repoNodeId) || string.IsNullOrEmpty(BoardsGitHubAppHostIdMappingService.providerData.ProviderId) || BoardsGitHubAppHostIdMappingService.providerData.Routers == null)
      {
        requestContext.Trace(918100, TraceLevel.Info, this.Area, this.Layer, "Not adding mapping for collection {0}. the parameters are not valid", (object) requestContext.ServiceHost.InstanceId);
        return false;
      }
      foreach (IHostIdMappingRouter router in (IEnumerable<IHostIdMappingRouter>) BoardsGitHubAppHostIdMappingService.providerData.Routers)
      {
        Guid conflictingHostId;
        if (!this.TryAddHostIdMapping(requestContext, router, BoardsGitHubAppHostIdMappingService.providerData.ProviderId, installationId, repoNodeId, out conflictingHostId))
        {
          if (conflictingHostId == requestContext.ServiceHost.InstanceId)
            return true;
          requestContext.Trace(918101, TraceLevel.Info, this.Area, this.Layer, "HostIdMapping already exists. This host id: {0}; conflicting host id: {1}.", (object) requestContext.ServiceHost.InstanceId, (object) conflictingHostId);
          return false;
        }
      }
      return true;
    }

    public void RemoveHostMappingForRepositories(
      IVssRequestContext requestContext,
      string installationId,
      IReadOnlyList<string> repoNodeIds,
      Guid currentProjectId,
      string providerKey)
    {
      ArgumentUtility.CheckForNull<string>(installationId, nameof (installationId));
      ArgumentUtility.CheckForNull<IReadOnlyList<string>>(repoNodeIds, nameof (repoNodeIds));
      ArgumentUtility.CheckForNull<string>(providerKey, nameof (providerKey));
      IEnumerable<ExternalConnectionWithFilteredRepos> byRepoExternalIds = requestContext.GetService<IExternalConnectionService>().GetExternalConnectionsByRepoExternalIds(requestContext.Elevate(), new Guid?(), providerKey, (IEnumerable<string>) repoNodeIds);
      foreach (IHostIdMappingRouter router in (IEnumerable<IHostIdMappingRouter>) BoardsGitHubAppHostIdMappingService.providerData.Routers)
      {
        foreach (string repoNodeId1 in (IEnumerable<string>) repoNodeIds)
        {
          string repoNodeId = repoNodeId1;
          if (byRepoExternalIds.Any<ExternalConnectionWithFilteredRepos>((Func<ExternalConnectionWithFilteredRepos, bool>) (c => c.ProjectId != currentProjectId && c.ExternalGitRepos.Any<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => r.NodeId() == repoNodeId)))))
          {
            requestContext.Trace(918111, TraceLevel.Info, this.Area, this.Layer, "Not removing mapping for collection {0}. Other project usage found for repo: {1}", (object) requestContext.ServiceHost.InstanceId, (object) repoNodeId);
          }
          else
          {
            HostIdMappingData mappingData = new HostIdMappingData()
            {
              Id = installationId,
              PropertyName = GitHubConstants.InstallationId,
              Qualifier = repoNodeId
            };
            requestContext.Trace(918112, TraceLevel.Info, this.Area, this.Layer, "Removing mapping for collection {0}. No other project need the mapping for repo: {1}", (object) requestContext.ServiceHost.InstanceId, (object) mappingData?.Qualifier);
            this.TryRemoveHostIdMapping(requestContext, router, BoardsGitHubAppHostIdMappingService.providerData.ProviderId, mappingData, requestContext.ServiceHost.InstanceId);
          }
        }
      }
    }

    public IEnumerable<string> AddHostMappingForRepositories(
      IVssRequestContext requestContext,
      string installationId,
      IReadOnlyList<string> repoNodeIds)
    {
      ArgumentUtility.CheckForNull<string>(installationId, nameof (installationId));
      ArgumentUtility.CheckForNull<IReadOnlyList<string>>(repoNodeIds, nameof (repoNodeIds));
      List<string> stringList = new List<string>();
      foreach (string repoNodeId in (IEnumerable<string>) repoNodeIds)
      {
        if (!this.AddHostMappingForRepository(requestContext, installationId, repoNodeId))
          stringList.Add(repoNodeId);
      }
      return (IEnumerable<string>) stringList;
    }

    public void RemoveHostMappingForRepositoriesWithoutChecking(
      IVssRequestContext requestContext,
      string installationId,
      IReadOnlyList<string> repoNodeIds)
    {
      ArgumentUtility.CheckForNull<string>(installationId, nameof (installationId));
      ArgumentUtility.CheckForNull<IReadOnlyList<string>>(repoNodeIds, nameof (repoNodeIds));
      foreach (string repoNodeId in (IEnumerable<string>) repoNodeIds)
      {
        HostIdMappingData mappingData = new HostIdMappingData()
        {
          Id = installationId,
          PropertyName = GitHubConstants.InstallationId,
          Qualifier = repoNodeId
        };
        requestContext.Trace(918114, TraceLevel.Info, this.Area, this.Layer, "Removing mapping for collection: {0}, InstallationId: {1}, NodeIds: {2}", (object) requestContext.ServiceHost.InstanceId, (object) installationId, (object) string.Join(",", (IEnumerable<string>) repoNodeIds));
        this.RemoveHostIdMapping(requestContext, BoardsGitHubAppHostIdMappingService.providerData.ProviderId, mappingData);
      }
    }

    public void RemoveHostMappingForInstallation(
      IVssRequestContext requestContext,
      string installationId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(installationId, nameof (installationId));
      this.RemoveRoute(requestContext, (IHostIdMappingProviderData) BoardsGitHubAppHostIdMappingService.providerData, installationId);
    }

    public void AddHostMappingForInstallation(
      IVssRequestContext requestContext,
      string installationId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(installationId, nameof (installationId));
      this.AddRoute(requestContext, (IHostIdMappingProviderData) BoardsGitHubAppHostIdMappingService.providerData, installationId);
    }

    public Guid? GetHostMappingForInstallation(
      IVssRequestContext requestContext,
      string installationId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(installationId, nameof (installationId));
      HostIdMappingData mappingData = new HostIdMappingData()
      {
        Id = installationId,
        PropertyName = GitHubConstants.InstallationId
      };
      return this.GetHostId(requestContext, BoardsGitHubAppHostIdMappingService.providerData.ProviderId, mappingData, true);
    }
  }
}
