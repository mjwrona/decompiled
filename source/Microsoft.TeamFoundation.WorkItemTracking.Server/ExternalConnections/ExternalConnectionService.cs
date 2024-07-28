// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class ExternalConnectionService : IExternalConnectionService, IVssFrameworkService
  {
    public static readonly int MaxAllowedBulkRepoSize = 200;
    private const string c_BoardsExternalConnectionHasPermissionProvisionedRootKey = "/Service/BoardsExternalConnection/Settings";
    private const string c_BoardsExternalConnectionHasPermissionProvisionedKey = "HasPermissionProvisioned";
    private const string SqlSaveExternalConnectionProcedureName = "prc_SaveExternalConnection";
    private const int SqlMergeOperationCode = 8672;
    private static readonly string s_EmptyMetadata = JsonConvert.SerializeObject(new object());
    protected Dictionary<string, IExternalConnectionProvider> m_endpointTypeToProviderLookup;

    public ExternalConnectionService() => this.m_endpointTypeToProviderLookup = new Dictionary<string, IExternalConnectionProvider>((IEqualityComparer<string>) VssStringComparer.ServiceEndpointTypeCompararer);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      foreach (IExternalConnectionProvider extension in (IEnumerable<IExternalConnectionProvider>) systemRequestContext.GetExtensions<IExternalConnectionProvider>(ExtensionLifetime.Service))
      {
        foreach (string key in extension.ServiceEndpointTypesSupported)
        {
          IExternalConnectionProvider connectionProvider;
          if (this.m_endpointTypeToProviderLookup.TryGetValue(key, out connectionProvider))
            systemRequestContext.Trace(this.TracePointStart + 5, TraceLevel.Error, this.Area, this.Layer, "Already registered provider '" + connectionProvider.GetType().FullName + "' for service endpoint type '" + key + "', but another provider '" + extension.GetType().FullName + "' also claimed support the type.");
          this.m_endpointTypeToProviderLookup[key] = extension;
        }
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual IReadOnlyCollection<ExternalConnection> GetExternalConnections(
      IVssRequestContext requestContext,
      Guid? projectId,
      string providerKey,
      bool refreshMetadata = false,
      bool includeAuthorization = false,
      bool includeRepoProviderData = false,
      bool includeInvalidConnections = false)
    {
      requestContext.TraceEnter(this.TracePointStart + 1, this.Area, this.Layer, nameof (GetExternalConnections));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        this.CheckReadPermissions(requestContext, projectId);
        List<ExternalConnection> externalConnectionList = new List<ExternalConnection>();
        Dictionary<int, Guid> mapping = new Dictionary<int, Guid>();
        IEnumerable<ExternalConnectionDataset> datasets = this.ExecuteSql<IEnumerable<ExternalConnectionDataset>>(requestContext, (Func<ExternalConnectionSqlComponent, IEnumerable<ExternalConnectionDataset>>) (component =>
        {
          datasets = component.GetExternalConnectionsDataset(projectId, providerKey, includeRepoProviderData: includeRepoProviderData);
          if (projectId.HasValue && datasets.Any<ExternalConnectionDataset>())
          {
            mapping.Add(datasets.FirstOrDefault<ExternalConnectionDataset>().DataspaceId, projectId.Value);
          }
          else
          {
            foreach (ExternalConnectionDataset connectionDataset in datasets)
            {
              if (!mapping.ContainsKey(connectionDataset.DataspaceId))
              {
                try
                {
                  Guid dataspaceIdentifier = component.GetDataspaceIdentifier(connectionDataset.DataspaceId);
                  mapping.Add(connectionDataset.DataspaceId, dataspaceIdentifier);
                }
                catch (Exception ex)
                {
                }
              }
            }
          }
          return datasets;
        }));
        List<ExternalConnection> source = this.HydrateWithServiceEndpointData<ExternalConnection>(requestContext, mapping, datasets, refreshMetadata, includeAuthorization);
        if (!includeInvalidConnections)
          source = source.Where<ExternalConnection>((Func<ExternalConnection, bool>) (c => c.IsConnectionValid)).ToList<ExternalConnection>();
        return (IReadOnlyCollection<ExternalConnection>) source;
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 2, this.Area, this.Layer, nameof (GetExternalConnections));
      }
    }

    public virtual IEnumerable<ExternalConnectionWithFilteredRepos> GetExternalConnectionsByRepoExternalIds(
      IVssRequestContext requestContext,
      Guid? projectId,
      string providerKey,
      IEnumerable<string> externalIds,
      bool refreshMetadata = false,
      bool includeAuthorization = false)
    {
      requestContext.TraceEnter(this.TracePointStart + 3, this.Area, this.Layer, "GetExternalConnections");
      try
      {
        this.CheckReadPermissions(requestContext, projectId);
        Dictionary<int, Guid> mapping = new Dictionary<int, Guid>();
        IEnumerable<ExternalConnectionDataset> datasets = this.ExecuteSql<IEnumerable<ExternalConnectionDataset>>(requestContext, (Func<ExternalConnectionSqlComponent, IEnumerable<ExternalConnectionDataset>>) (component =>
        {
          if (component.Version < 4)
          {
            datasets = component.GetExternalConnectionsDataset(projectId, providerKey).Where<ExternalConnectionDataset>(closure_1 ?? (closure_1 = (Func<ExternalConnectionDataset, bool>) (ec => ec.Repos.Any<ExternalConnectionRepository>((Func<ExternalConnectionRepository, bool>) (r => externalIds.Contains<string>(r.ExternalId))))));
            foreach (ExternalConnectionDataset connectionDataset in datasets)
              connectionDataset.Repos = connectionDataset.Repos.Where<ExternalConnectionRepository>(closure_2 ?? (closure_2 = (Func<ExternalConnectionRepository, bool>) (repo => externalIds.Contains<string>(repo.ExternalId))));
          }
          else
            datasets = component.GetExternalConnectionsDatasetByRepoExternalIds(projectId, providerKey, externalIds);
          this.MapDataspaceIds(component, projectId, mapping, datasets);
          return datasets;
        }));
        return (IEnumerable<ExternalConnectionWithFilteredRepos>) this.HydrateWithServiceEndpointData<ExternalConnectionWithFilteredRepos>(requestContext, mapping, datasets, refreshMetadata, includeAuthorization);
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 4, this.Area, this.Layer, "GetExternalConnections");
      }
    }

    public virtual IEnumerable<ExternalConnectionWithFilteredRepos> GetExternalConnectionsByRepoInternalIds(
      IVssRequestContext requestContext,
      Guid? projectId,
      string providerKey,
      IEnumerable<Guid> internalIds,
      bool refreshMetadata = false,
      bool includeAuthorization = false)
    {
      requestContext.TraceEnter(this.TracePointStart + 5, this.Area, this.Layer, "GetExternalConnections");
      try
      {
        this.CheckReadPermissions(requestContext, projectId);
        Dictionary<int, Guid> mapping = new Dictionary<int, Guid>();
        IEnumerable<ExternalConnectionDataset> datasets = this.ExecuteSql<IEnumerable<ExternalConnectionDataset>>(requestContext, (Func<ExternalConnectionSqlComponent, IEnumerable<ExternalConnectionDataset>>) (component =>
        {
          if (component.Version < 4)
          {
            datasets = component.GetExternalConnectionsDataset(projectId, providerKey).Where<ExternalConnectionDataset>(closure_1 ?? (closure_1 = (Func<ExternalConnectionDataset, bool>) (ec => ec.Repos.Any<ExternalConnectionRepository>((Func<ExternalConnectionRepository, bool>) (r => internalIds.Contains<Guid>(r.RepositoryId))))));
            foreach (ExternalConnectionDataset connectionDataset in datasets)
              connectionDataset.Repos = connectionDataset.Repos.Where<ExternalConnectionRepository>(closure_2 ?? (closure_2 = (Func<ExternalConnectionRepository, bool>) (repo => internalIds.Contains<Guid>(repo.RepositoryId))));
          }
          else
            datasets = component.GetExternalConnectionsDatasetByRepoInternalIds(projectId, internalIds);
          this.MapDataspaceIds(component, projectId, mapping, datasets);
          return datasets;
        }));
        return (IEnumerable<ExternalConnectionWithFilteredRepos>) this.HydrateWithServiceEndpointData<ExternalConnectionWithFilteredRepos>(requestContext, mapping, datasets, refreshMetadata, includeAuthorization);
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 6, this.Area, this.Layer, "GetExternalConnections");
      }
    }

    public ExternalConnection GetExternalConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid connectionId,
      bool refreshMetadata = false,
      bool includeAuthorization = false,
      bool includeRepoProviderData = false,
      bool includeConnectionWithInvalidEndpoint = false)
    {
      requestContext.TraceEnter(this.TracePointStart + 7, this.Area, this.Layer, nameof (GetExternalConnection));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckForEmptyGuid(connectionId, nameof (connectionId));
        ExternalConnectionDataset connectionDataset = this.ExecuteSql<ExternalConnectionDataset>(requestContext, (Func<ExternalConnectionSqlComponent, ExternalConnectionDataset>) (component => component.GetExternalConnectionDataset(projectId, connectionId, includeRepoProviderData)));
        if (connectionDataset == null)
          return (ExternalConnection) null;
        IVssRequestContext requestContext1 = requestContext;
        Dictionary<int, Guid> dataspaceIdToProjectIdMapping = new Dictionary<int, Guid>();
        dataspaceIdToProjectIdMapping.Add(connectionDataset.DataspaceId, projectId);
        List<ExternalConnectionDataset> datasets = new List<ExternalConnectionDataset>();
        datasets.Add(connectionDataset);
        int num1 = refreshMetadata ? 1 : 0;
        int num2 = includeAuthorization ? 1 : 0;
        ExternalConnection externalConnection = this.HydrateWithServiceEndpointData<ExternalConnection>(requestContext1, dataspaceIdToProjectIdMapping, (IEnumerable<ExternalConnectionDataset>) datasets, num1 != 0, num2 != 0).FirstOrDefault<ExternalConnection>();
        return includeConnectionWithInvalidEndpoint ? externalConnection : (externalConnection.ServiceEndpoint.IsEndpointValid ? externalConnection : (ExternalConnection) null);
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 8, this.Area, this.Layer, nameof (GetExternalConnection));
      }
    }

    public ExternalConnectionProvisionResult CreateExternalConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint serviceEndpoint,
      IEnumerable<string> repoExternalIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(serviceEndpoint, nameof (serviceEndpoint));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(repoExternalIds, nameof (repoExternalIds));
      this.ValidateRepoCount(requestContext, repoExternalIds);
      requestContext.TraceEnter(this.TracePointStart + 21, this.Area, this.Layer, nameof (CreateExternalConnection));
      try
      {
        IExternalConnectionProvider connectionProvider;
        if (!this.m_endpointTypeToProviderLookup.TryGetValue(serviceEndpoint.Type, out connectionProvider))
          throw new ExternalConnectionProviderNotFoundException(serviceEndpoint.Type);
        string connectionName = connectionProvider.GetConnectionNameForCreation(serviceEndpoint);
        ExternalConnectionService.ValidateConnectionName(connectionName);
        connectionName = connectionName.Trim();
        string providerKey = connectionProvider.GetProviderKey(serviceEndpoint);
        HashSet<string> repoExternalIdsToAdd = new HashSet<string>(repoExternalIds);
        IReadOnlyCollection<ExternalConnection> externalConnections = this.GetExternalConnections(requestContext, new Guid?(projectId), providerKey, false, true, false, false);
        if (!connectionProvider.CanCreateNewConnection(serviceEndpoint, (IEnumerable<ExternalConnection>) externalConnections))
          throw new ExternalConnectionCannotCreateConnectionException();
        this.ValidateReposNotUsedByOtherConnection(new Guid?(), (IEnumerable<ExternalConnection>) externalConnections, repoExternalIdsToAdd);
        if (!externalConnections.Any<ExternalConnection>() && !connectionProvider.IsBuiltInProvider)
        {
          string externalProviderType = connectionProvider.ExternalProviderType;
          this.SaveExternalProvider(requestContext, new Guid?(Guid.NewGuid()), providerKey, externalProviderType);
        }
        ExternalConnectionProvisionResult provisionResult = connectionProvider.ProvisionIntegration(requestContext, projectId, serviceEndpoint, repoExternalIds, Enumerable.Empty<string>());
        IList<ExternalGitRepoProvisionResult> repoProvisionResult = provisionResult.RepoProvisionResult;
        if ((repoProvisionResult != null ? (repoProvisionResult.Any<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => r.IsSuccess)) ? 1 : 0) : 0) != 0)
        {
          IEnumerable<ExternalGitRepo> succeededRepos = provisionResult.RepoProvisionResult.Where<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => r.IsSuccess)).Select<ExternalGitRepoProvisionResult, ExternalGitRepo>((Func<ExternalGitRepoProvisionResult, ExternalGitRepo>) (r => r.Repo));
          Guid createdBy = this.GetRequestContextUserVsid(requestContext);
          Guid guid = this.ExecuteSql<Guid>(requestContext, (Func<ExternalConnectionSqlComponent, Guid>) (component => component.SaveExternalConnection(projectId, providerKey, connectionName, serviceEndpoint.Id, succeededRepos, createdBy)));
          provisionResult.ProvisionedConnection = new ExternalConnection()
          {
            Id = guid,
            Name = connectionName,
            ExternalGitRepos = succeededRepos,
            ServiceEndpoint = new ServiceEndpointViewModel(serviceEndpoint),
            ProjectId = projectId,
            ProviderKey = providerKey
          };
        }
        connectionProvider.FinalizeProvision(requestContext, projectId, provisionResult);
        return provisionResult;
      }
      finally
      {
        requestContext.TraceEnter(this.TracePointStart + 22, this.Area, this.Layer, nameof (CreateExternalConnection));
      }
    }

    public IEnumerable<ExternalGitRepo> GetExternalConnectionRepos(
      IVssRequestContext requestContext,
      Guid projectId,
      string providerKey,
      Guid connectionId,
      IEnumerable<string> repoExternalIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(providerKey, nameof (providerKey));
      ArgumentUtility.CheckForEmptyGuid(connectionId, nameof (connectionId));
      return requestContext.TraceBlock<IEnumerable<ExternalGitRepo>>(this.TracePointStart + 25, this.TracePointStart + 26, this.Area, this.Layer, nameof (GetExternalConnectionRepos), (Func<IEnumerable<ExternalGitRepo>>) (() =>
      {
        this.CheckPermission(requestContext, 1, new Guid?(projectId));
        IReadOnlyCollection<ExternalConnection> externalConnections = this.GetExternalConnections(requestContext, new Guid?(projectId), providerKey, false, true, false, false);
        IEnumerable<ExternalGitRepo> source = ((externalConnections != null ? externalConnections.FirstOrDefault<ExternalConnection>((Func<ExternalConnection, bool>) (conn => conn.Id == connectionId)) : (ExternalConnection) null) ?? throw new GitHubBoardsConnectionDoesNotExistException(connectionId.ToString())).ExternalGitRepos;
        if (repoExternalIds != null && repoExternalIds.Any<string>())
        {
          IEnumerable<string> intersection = repoExternalIds.Intersect<string>(source.Select<ExternalGitRepo, string>((Func<ExternalGitRepo, string>) (r => r.Id)));
          source = source.Where<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => intersection.Any<string>()));
        }
        return source;
      }));
    }

    public ExternalConnectionProvisionResult AddExternalConnectionRepos(
      IVssRequestContext requestContext,
      Guid projectId,
      string providerKey,
      Guid connectionId,
      IEnumerable<string> repoExternalNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(providerKey, nameof (providerKey));
      ArgumentUtility.CheckForEmptyGuid(connectionId, nameof (connectionId));
      return requestContext.TraceBlock<ExternalConnectionProvisionResult>(this.TracePointStart + 25, this.TracePointStart + 26, this.Area, this.Layer, nameof (AddExternalConnectionRepos), (Func<ExternalConnectionProvisionResult>) (() =>
      {
        this.CheckPermission(requestContext, 2, new Guid?(projectId));
        IReadOnlyCollection<ExternalConnection> externalConnections = this.GetExternalConnections(requestContext, new Guid?(projectId), providerKey, false, true, false, false);
        ExternalConnection currentConnection = externalConnections.FirstOrDefault<ExternalConnection>((Func<ExternalConnection, bool>) (conn => conn.Id == connectionId));
        if (currentConnection == null)
          throw new GitHubBoardsConnectionDoesNotExistException(connectionId.ToString());
        ServiceEndpoint serviceEndpoint = currentConnection.ServiceEndpoint.RawServiceEndpoint;
        IExternalConnectionProvider provider;
        if (!this.m_endpointTypeToProviderLookup.TryGetValue(currentConnection.ServiceEndpoint.Type, out provider))
          throw new ExternalConnectionProviderNotFoundException(serviceEndpoint.Type);
        this.CheckCreatorPermission(requestContext, provider, currentConnection);
        Dictionary<string, string> repoExternal = new Dictionary<string, string>();
        if (repoExternalNames != null && repoExternalNames.Any<string>())
        {
          List<string> list = repoExternalNames.Select<string, string>((Func<string, string>) (r => "repo:" + r)).ToList<string>();
          repoExternal = provider.GetRepoIdsByRepoNames(requestContext, projectId, serviceEndpoint, (IList<string>) list);
        }
        IEnumerable<ExternalGitRepo> source = externalConnections.SelectMany<ExternalConnection, ExternalGitRepo>((Func<ExternalConnection, IEnumerable<ExternalGitRepo>>) (c => c.ExternalGitRepos)).Where<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => repoExternal.ContainsKey(r.Id)));
        HashSet<string> usedReposIdsSet = source.Select<ExternalGitRepo, string>((Func<ExternalGitRepo, string>) (r => r.Id)).ToHashSet<string>();
        HashSet<string> hashSet1 = repoExternal.Keys.Where<string>((Func<string, bool>) (r => !usedReposIdsSet.Contains(r))).ToHashSet<string>();
        HashSet<string> repoIdsToRemove = new HashSet<string>();
        ExternalConnectionProvisionResult provisionResult = new ExternalConnectionProvisionResult()
        {
          ProvisionedConnection = currentConnection
        };
        if (hashSet1.Any<string>())
        {
          this.ValidateRepoCount(requestContext, repoExternal.Values.Union<string>((IEnumerable<string>) hashSet1));
          provisionResult = provider.ProvisionIntegration(requestContext, projectId, serviceEndpoint, (IEnumerable<string>) hashSet1, (IEnumerable<string>) repoIdsToRemove);
          IList<ExternalGitRepoProvisionResult> repoProvisionResult1 = provisionResult.RepoProvisionResult;
          if ((repoProvisionResult1 != null ? (repoProvisionResult1.Any<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => r.IsSuccess)) ? 1 : 0) : 0) != 0)
          {
            IList<ExternalGitRepoProvisionResult> repoProvisionResult2 = provisionResult.RepoProvisionResult;
            HashSet<string> succeededAddRepoIdsSet = new HashSet<string>((repoProvisionResult2 != null ? repoProvisionResult2.Where<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => r.IsSuccess && r.IsAdd)).Select<ExternalGitRepoProvisionResult, string>((Func<ExternalGitRepoProvisionResult, string>) (r => r.Repo.Id)) : (IEnumerable<string>) null) ?? Enumerable.Empty<string>());
            IEnumerable<ExternalGitRepo> externalGitRepos = currentConnection.ExternalGitRepos;
            IList<ExternalGitRepoProvisionResult> repoProvisionResult3 = provisionResult.RepoProvisionResult;
            IEnumerable<ExternalGitRepo> second = repoProvisionResult3 != null ? repoProvisionResult3.Where<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => succeededAddRepoIdsSet.Contains(r.Repo.Id))).Select<ExternalGitRepoProvisionResult, ExternalGitRepo>((Func<ExternalGitRepoProvisionResult, ExternalGitRepo>) (r => r.Repo)) : (IEnumerable<ExternalGitRepo>) null;
            List<ExternalGitRepo> latestRepos = externalGitRepos.Concat<ExternalGitRepo>(second).ToList<ExternalGitRepo>();
            Guid createdBy = this.GetRequestContextUserVsid(requestContext);
            try
            {
              this.ExecuteSql<Guid>(requestContext, (Func<ExternalConnectionSqlComponent, Guid>) (component => component.SaveExternalConnection(projectId, providerKey, currentConnection.Name, serviceEndpoint.Id, (IEnumerable<ExternalGitRepo>) latestRepos, createdBy)));
            }
            catch (SqlException ex)
            {
              if (ex.Procedure == "prc_SaveExternalConnection" && ex.Number == 8672)
              {
                HashSet<string> hashSet2 = latestRepos.GroupBy<ExternalGitRepo, string>((Func<ExternalGitRepo, string>) (repo => repo.Id.ToUpperInvariant())).Where<IGrouping<string, ExternalGitRepo>>((Func<IGrouping<string, ExternalGitRepo>, bool>) (group => group.Count<ExternalGitRepo>() > 1)).Select<IGrouping<string, ExternalGitRepo>, string>((Func<IGrouping<string, ExternalGitRepo>, string>) (group => group.Key)).ToHashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                foreach (ExternalGitRepoProvisionResult repoProvisionResult4 in (IEnumerable<ExternalGitRepoProvisionResult>) provisionResult.RepoProvisionResult)
                  repoProvisionResult4.Message = !hashSet2.Contains(repoProvisionResult4.Repo.Id) ? Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ExternalConnectionFailedToAddRepository() : Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ExternalConnectionRepositoryIdAlreadyExist((object) repoProvisionResult4.Repo.Id);
              }
              else
                throw;
            }
            provisionResult.ProvisionedConnection = new ExternalConnection()
            {
              Id = currentConnection.Id,
              Name = currentConnection.Name,
              ExternalGitRepos = (IEnumerable<ExternalGitRepo>) latestRepos,
              ServiceEndpoint = new ServiceEndpointViewModel(serviceEndpoint),
              ProjectId = projectId,
              ProviderKey = providerKey
            };
          }
          else
            provisionResult.ProvisionedConnection = currentConnection;
        }
        IEnumerable<ExternalGitRepoProvisionResult> values1 = source.Select<ExternalGitRepo, ExternalGitRepoProvisionResult>((Func<ExternalGitRepo, ExternalGitRepoProvisionResult>) (r => new ExternalGitRepoProvisionResult(r, false, false, "Repository is already connected to this team project")));
        provisionResult.RepoProvisionResult.AddRange<ExternalGitRepoProvisionResult, IList<ExternalGitRepoProvisionResult>>(values1);
        HashSet<string> externalNamesSet = repoExternal.Values.ToHashSet<string>();
        IEnumerable<ExternalGitRepoProvisionResult> values2 = repoExternalNames.Where<string>((Func<string, bool>) (r => !externalNamesSet.Contains(r))).Select<string, ExternalGitRepo>((Func<string, ExternalGitRepo>) (r => new ExternalGitRepo()
        {
          Name = r
        })).Select<ExternalGitRepo, ExternalGitRepoProvisionResult>((Func<ExternalGitRepo, ExternalGitRepoProvisionResult>) (r => new ExternalGitRepoProvisionResult(r, false, false, "Failed to find repository")));
        provisionResult.RepoProvisionResult.AddRange<ExternalGitRepoProvisionResult, IList<ExternalGitRepoProvisionResult>>(values2);
        provider.FinalizeProvision(requestContext, projectId, provisionResult);
        return provisionResult;
      }));
    }

    public ExternalConnectionProvisionResult RemoveExternalConnectionRepos(
      IVssRequestContext requestContext,
      Guid projectId,
      string providerKey,
      Guid connectionId,
      IEnumerable<string> repoExternalNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(providerKey, nameof (providerKey));
      ArgumentUtility.CheckForEmptyGuid(connectionId, nameof (connectionId));
      return requestContext.TraceBlock<ExternalConnectionProvisionResult>(this.TracePointStart + 25, this.TracePointStart + 26, this.Area, this.Layer, nameof (RemoveExternalConnectionRepos), (Func<ExternalConnectionProvisionResult>) (() =>
      {
        this.CheckPermission(requestContext, 2, new Guid?(projectId));
        IReadOnlyCollection<ExternalConnection> externalConnections = this.GetExternalConnections(requestContext, new Guid?(projectId), providerKey, false, true, false, false);
        ExternalConnection currentConnection = externalConnections.FirstOrDefault<ExternalConnection>((Func<ExternalConnection, bool>) (conn => conn.Id == connectionId));
        if (currentConnection == null)
          throw new GitHubBoardsConnectionDoesNotExistException(connectionId.ToString());
        ServiceEndpoint serviceEndpoint = currentConnection.ServiceEndpoint.RawServiceEndpoint;
        IExternalConnectionProvider connectionProvider;
        if (!this.m_endpointTypeToProviderLookup.TryGetValue(currentConnection.ServiceEndpoint.Type, out connectionProvider))
          throw new ExternalConnectionProviderNotFoundException(serviceEndpoint.Type);
        Dictionary<string, string> repoExternal = new Dictionary<string, string>();
        if (repoExternalNames != null && repoExternalNames.Any<string>())
        {
          List<string> list = repoExternalNames.Select<string, string>((Func<string, string>) (r => "repo:" + r)).ToList<string>();
          repoExternal = connectionProvider.GetRepoIdsByRepoNames(requestContext, projectId, serviceEndpoint, (IList<string>) list);
        }
        IEnumerable<ExternalGitRepo> source = externalConnections.Where<ExternalConnection>((Func<ExternalConnection, bool>) (c => c.Id == connectionId)).SelectMany<ExternalConnection, ExternalGitRepo>((Func<ExternalConnection, IEnumerable<ExternalGitRepo>>) (c => c.ExternalGitRepos)).Where<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => repoExternal.ContainsKey(r.Id)));
        HashSet<string> usedReposIdsSet = source.Select<ExternalGitRepo, string>((Func<ExternalGitRepo, string>) (r => r.Id)).ToHashSet<string>();
        HashSet<string> repoIdsToAdd = new HashSet<string>();
        HashSet<string> repoIdsToRemoveSet = repoExternal.Keys.Where<string>((Func<string, bool>) (r => usedReposIdsSet.Contains(r))).ToHashSet<string>();
        ExternalConnectionProvisionResult provisionResult = new ExternalConnectionProvisionResult()
        {
          ProvisionedConnection = currentConnection
        };
        if (repoIdsToRemoveSet.Any<string>())
        {
          provisionResult = connectionProvider.ProvisionIntegration(requestContext, projectId, serviceEndpoint, (IEnumerable<string>) repoIdsToAdd, (IEnumerable<string>) repoIdsToRemoveSet);
          IList<ExternalGitRepoProvisionResult> repoProvisionResult1 = provisionResult.RepoProvisionResult;
          if ((repoProvisionResult1 != null ? (repoProvisionResult1.Any<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => r.IsSuccess)) ? 1 : 0) : 0) != 0 || repoIdsToRemoveSet.Any<string>())
          {
            IList<ExternalGitRepoProvisionResult> repoProvisionResult2 = provisionResult.RepoProvisionResult;
            HashSet<string> succeededAddRepoIdsSet = new HashSet<string>((repoProvisionResult2 != null ? repoProvisionResult2.Where<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => r.IsSuccess && r.IsAdd)).Select<ExternalGitRepoProvisionResult, string>((Func<ExternalGitRepoProvisionResult, string>) (r => r.Repo.Id)) : (IEnumerable<string>) null) ?? Enumerable.Empty<string>());
            List<ExternalGitRepo> latestRepos = currentConnection.ExternalGitRepos.Where<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (repo => !repoIdsToRemoveSet.Contains(repo.Id))).Concat<ExternalGitRepo>(provisionResult.RepoProvisionResult.Where<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => succeededAddRepoIdsSet.Contains(r.Repo.Id))).Select<ExternalGitRepoProvisionResult, ExternalGitRepo>((Func<ExternalGitRepoProvisionResult, ExternalGitRepo>) (r => r.Repo))).ToList<ExternalGitRepo>();
            Guid createdBy = this.GetRequestContextUserVsid(requestContext);
            if (repoIdsToRemoveSet.Any<string>())
            {
              this.ResetRepositoryMetadataNoLongerIntegrated(requestContext, providerKey, projectId, repoIdsToRemoveSet.Select<string, Guid>((Func<string, Guid>) (externalId => currentConnection.GetRepoInternalId(externalId))));
              this.ResetConnectionMetadataForRepositoryNoLongerIntegrated(requestContext, currentConnection, (IEnumerable<string>) repoIdsToRemoveSet);
            }
            this.ExecuteSql<Guid>(requestContext, (Func<ExternalConnectionSqlComponent, Guid>) (component => component.SaveExternalConnection(projectId, providerKey, currentConnection.Name, serviceEndpoint.Id, (IEnumerable<ExternalGitRepo>) latestRepos, createdBy)));
            provisionResult.ProvisionedConnection = new ExternalConnection()
            {
              Id = currentConnection.Id,
              Name = currentConnection.Name,
              ExternalGitRepos = (IEnumerable<ExternalGitRepo>) latestRepos,
              ServiceEndpoint = new ServiceEndpointViewModel(serviceEndpoint),
              ProjectId = projectId,
              ProviderKey = providerKey
            };
          }
          else
            provisionResult.ProvisionedConnection = currentConnection;
        }
        IEnumerable<ExternalGitRepoProvisionResult> values = repoExternalNames.Except<string>(source.Select<ExternalGitRepo, string>((Func<ExternalGitRepo, string>) (r => r.Name))).Select<string, ExternalGitRepo>((Func<string, ExternalGitRepo>) (r => new ExternalGitRepo()
        {
          Name = r
        })).ToHashSet<ExternalGitRepo>().Select<ExternalGitRepo, ExternalGitRepoProvisionResult>((Func<ExternalGitRepo, ExternalGitRepoProvisionResult>) (r => new ExternalGitRepoProvisionResult(r, false, false, "Repository is not connected to this team project")));
        provisionResult.RepoProvisionResult.AddRange<ExternalGitRepoProvisionResult, IList<ExternalGitRepoProvisionResult>>(values);
        connectionProvider.FinalizeProvision(requestContext, projectId, provisionResult);
        return provisionResult;
      }));
    }

    public ExternalConnectionProvisionResult UpdateExternalConnectionRepos(
      IVssRequestContext requestContext,
      Guid projectId,
      string providerKey,
      Guid connectionId,
      IEnumerable<string> repoExternalIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(providerKey, nameof (providerKey));
      ArgumentUtility.CheckForEmptyGuid(connectionId, nameof (connectionId));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(repoExternalIds, nameof (repoExternalIds));
      requestContext.TraceEnter(this.TracePointStart + 25, this.Area, this.Layer, nameof (UpdateExternalConnectionRepos));
      try
      {
        this.CheckPermission(requestContext, 2, new Guid?(projectId));
        this.ValidateRepoCount(requestContext, repoExternalIds);
        IReadOnlyCollection<ExternalConnection> externalConnections = this.GetExternalConnections(requestContext, new Guid?(projectId), providerKey, false, true, false, false);
        ExternalConnection currentConnection = externalConnections.FirstOrDefault<ExternalConnection>((Func<ExternalConnection, bool>) (conn => conn.Id == connectionId));
        if (currentConnection == null)
          throw new GitHubBoardsConnectionDoesNotExistException(connectionId.ToString());
        ServiceEndpoint serviceEndpoint = currentConnection.ServiceEndpoint.RawServiceEndpoint;
        IExternalConnectionProvider connectionProvider;
        if (!this.m_endpointTypeToProviderLookup.TryGetValue(currentConnection.ServiceEndpoint.Type, out connectionProvider))
          throw new ExternalConnectionProviderNotFoundException(serviceEndpoint.Type);
        HashSet<string> currentConnectionRepoIdSet = new HashSet<string>(currentConnection.ExternalGitRepos.Select<ExternalGitRepo, string>((Func<ExternalGitRepo, string>) (r => r.Id)));
        HashSet<string> stringSet = new HashSet<string>(repoExternalIds.Where<string>((Func<string, bool>) (r => !currentConnectionRepoIdSet.Contains(r))));
        HashSet<string> requestedRepoIdSet = new HashSet<string>(repoExternalIds);
        HashSet<string> repoIdsToRemoveSet = new HashSet<string>(currentConnectionRepoIdSet.Where<string>((Func<string, bool>) (r => !requestedRepoIdSet.Contains(r))));
        if (!stringSet.Any<string>() && !repoIdsToRemoveSet.Any<string>())
          return new ExternalConnectionProvisionResult()
          {
            ProvisionedConnection = currentConnection
          };
        if (connectionProvider.ShouldMatchConnectionCreatorForAddOrUpdateRepo(serviceEndpoint))
        {
          Guid requestContextUserVsid = this.GetRequestContextUserVsid(requestContext);
          if (currentConnection.CreatedBy != requestContextUserVsid && stringSet.Any<string>())
            throw new ExternalConnectionPermissionDeniedException();
        }
        this.ValidateReposNotUsedByOtherConnection(new Guid?(), (IEnumerable<ExternalConnection>) externalConnections, stringSet);
        ExternalConnectionProvisionResult provisionResult = connectionProvider.ProvisionIntegration(requestContext, projectId, serviceEndpoint, (IEnumerable<string>) stringSet, (IEnumerable<string>) repoIdsToRemoveSet);
        IList<ExternalGitRepoProvisionResult> repoProvisionResult1 = provisionResult.RepoProvisionResult;
        if ((repoProvisionResult1 != null ? (repoProvisionResult1.Any<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => r.IsSuccess)) ? 1 : 0) : 0) != 0 || repoIdsToRemoveSet.Any<string>())
        {
          IList<ExternalGitRepoProvisionResult> repoProvisionResult2 = provisionResult.RepoProvisionResult;
          HashSet<string> succeededAddRepoIdsSet = new HashSet<string>((repoProvisionResult2 != null ? repoProvisionResult2.Where<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => r.IsSuccess && r.IsAdd)).Select<ExternalGitRepoProvisionResult, string>((Func<ExternalGitRepoProvisionResult, string>) (r => r.Repo?.Id)) : (IEnumerable<string>) null) ?? Enumerable.Empty<string>());
          List<ExternalGitRepo> latestRepos = currentConnection.ExternalGitRepos.Where<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (repo => !repoIdsToRemoveSet.Contains(repo.Id))).Concat<ExternalGitRepo>(provisionResult.RepoProvisionResult.Where<ExternalGitRepoProvisionResult>((Func<ExternalGitRepoProvisionResult, bool>) (r => succeededAddRepoIdsSet.Contains(r.Repo?.Id))).Select<ExternalGitRepoProvisionResult, ExternalGitRepo>((Func<ExternalGitRepoProvisionResult, ExternalGitRepo>) (r => r.Repo))).ToList<ExternalGitRepo>();
          Guid createdBy = this.GetRequestContextUserVsid(requestContext);
          if (repoIdsToRemoveSet.Any<string>())
          {
            this.ResetRepositoryMetadataNoLongerIntegrated(requestContext, providerKey, projectId, repoIdsToRemoveSet.Select<string, Guid>((Func<string, Guid>) (externalId => currentConnection.GetRepoInternalId(externalId))));
            this.ResetConnectionMetadataForRepositoryNoLongerIntegrated(requestContext, currentConnection, (IEnumerable<string>) repoIdsToRemoveSet);
          }
          this.ExecuteSql<Guid>(requestContext, (Func<ExternalConnectionSqlComponent, Guid>) (component => component.SaveExternalConnection(projectId, providerKey, currentConnection.Name, serviceEndpoint.Id, (IEnumerable<ExternalGitRepo>) latestRepos, createdBy)));
          provisionResult.ProvisionedConnection = new ExternalConnection()
          {
            Id = currentConnection.Id,
            Name = currentConnection.Name,
            ExternalGitRepos = (IEnumerable<ExternalGitRepo>) latestRepos,
            ServiceEndpoint = new ServiceEndpointViewModel(serviceEndpoint),
            ProjectId = projectId,
            ProviderKey = providerKey
          };
        }
        else
          provisionResult.ProvisionedConnection = currentConnection;
        connectionProvider.FinalizeProvision(requestContext, projectId, provisionResult);
        return provisionResult;
      }
      finally
      {
        requestContext.TraceEnter(this.TracePointStart + 26, this.Area, this.Layer, nameof (UpdateExternalConnectionRepos));
      }
    }

    public virtual void UpdateExternalConnectionMetadata(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid connectionId,
      ExternalConnectionMetadata connectionMetadata)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(connectionId, nameof (connectionId));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<ExternalConnectionMetadata>(connectionMetadata, nameof (connectionMetadata));
      requestContext.TraceEnter(this.TracePointStart + 33, this.Area, this.Layer, nameof (UpdateExternalConnectionMetadata));
      try
      {
        this.CheckPermission(requestContext, 2, new Guid?(projectId));
        this.ExecuteSql(requestContext, (Action<ExternalConnectionSqlComponent>) (component => component.UpdateConnectionMetadata(projectId, connectionId, connectionMetadata.Serialize<ExternalConnectionMetadata>())));
      }
      finally
      {
        requestContext.TraceEnter(this.TracePointStart + 34, this.Area, this.Layer, nameof (UpdateExternalConnectionMetadata));
      }
    }

    public ExternalConnectionProvisionResult DeleteExternalConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid connectionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      requestContext.TraceEnter(this.TracePointStart + 11, this.Area, this.Layer, nameof (DeleteExternalConnection));
      try
      {
        this.CheckPermission(requestContext, 2, new Guid?(projectId));
        ExternalConnection externalConnection = this.GetExternalConnection(requestContext, projectId, connectionId, false, true, false, true);
        if (externalConnection == null)
          throw new GitHubBoardsConnectionDoesNotExistException(connectionId.ToString());
        IExternalConnectionProvider connectionProvider;
        if (this.m_endpointTypeToProviderLookup.TryGetValue(externalConnection.ServiceEndpoint.Type ?? string.Empty, out connectionProvider))
        {
          ExternalConnectionProvisionResult connectionProvisionResult;
          try
          {
            connectionProvisionResult = connectionProvider.ProvisionIntegration(requestContext, projectId, externalConnection.ServiceEndpoint?.RawServiceEndpoint, Enumerable.Empty<string>(), externalConnection.ExternalGitRepos.Select<ExternalGitRepo, string>((Func<ExternalGitRepo, string>) (r => r.Id)));
            connectionProvisionResult.GlobalErrorMessage = (string) null;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(this.TracePointStart + 13, this.Area, this.Layer, ex);
            connectionProvisionResult = ExternalConnectionService.BuildDefaultDeleteConnectionProvisionResult(externalConnection);
          }
          this.ResetRepositoryMetadataNoLongerIntegrated(requestContext, externalConnection.ProviderKey, projectId, externalConnection.ExternalGitRepos.Select<ExternalGitRepo, Guid>((Func<ExternalGitRepo, Guid>) (r => r.GetRepoInternalId())));
          this.ResetConnectionMetadataForRepositoryNoLongerIntegrated(requestContext, externalConnection, externalConnection.ExternalGitRepos.Select<ExternalGitRepo, string>((Func<ExternalGitRepo, string>) (r => r.NodeId())));
          this.ExecuteSql(requestContext, (Action<ExternalConnectionSqlComponent>) (component => component.DeleteExternalConnection(projectId, connectionId)));
          return connectionProvisionResult;
        }
        requestContext.Trace(this.TracePointStart + 23, TraceLevel.Error, this.Area, this.Layer, string.Format("Cannot find provider for service endpoint type '{0}' during deletiong connection '{1}' ", (object) externalConnection.ServiceEndpoint.Type, (object) connectionId) + string.Format("with service endpoint '{0}, ", (object) externalConnection.ServiceEndpoint.Id) + "possibly because service endpoint is not found, continue remove connection without cleanup");
        ExternalConnectionProvisionResult connectionProvisionResult1 = ExternalConnectionService.BuildDefaultDeleteConnectionProvisionResult(externalConnection);
        this.ExecuteSql(requestContext, (Action<ExternalConnectionSqlComponent>) (component => component.DeleteExternalConnection(projectId, connectionId)));
        return connectionProvisionResult1;
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 12, this.Area, this.Layer, nameof (DeleteExternalConnection));
      }
    }

    public virtual void CheckReadPermissions(IVssRequestContext requestContext, Guid? projectId)
    {
      if (!projectId.HasValue && !requestContext.IsSystemContext)
        throw new ExternalConnectionPermissionDeniedException();
      if (projectId.HasValue && !this.HasPermission(requestContext, 1, projectId))
        throw new ExternalConnectionPermissionDeniedException();
    }

    public IEnumerable<ExternalProvider> GetExternalProviders(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(this.TracePointStart + 13, this.Area, this.Layer, nameof (GetExternalProviders));
      try
      {
        return this.ExecuteSql<IEnumerable<ExternalProvider>>(requestContext, (Func<ExternalConnectionSqlComponent, IEnumerable<ExternalProvider>>) (component => component.GetExternalProviders()));
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 14, this.Area, this.Layer, nameof (GetExternalProviders));
      }
    }

    public ExternalProvider SaveExternalProvider(
      IVssRequestContext requestContext,
      Guid? providerId,
      string providerKey,
      string providerType)
    {
      requestContext.TraceEnter(this.TracePointStart + 15, this.Area, this.Layer, nameof (SaveExternalProvider));
      try
      {
        return this.ExecuteSql<ExternalProvider>(requestContext, (Func<ExternalConnectionSqlComponent, ExternalProvider>) (component => component.SaveExternalProvider(providerId, providerKey, providerType)));
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 16, this.Area, this.Layer, nameof (SaveExternalProvider));
      }
    }

    public IEnumerable<ExternalConnectionRepository> GetExternalRepositories(
      IVssRequestContext requestContext,
      IEnumerable<Guid> repositoryIds = null)
    {
      requestContext.TraceEnter(this.TracePointStart + 17, this.Area, this.Layer, nameof (GetExternalRepositories));
      try
      {
        return this.ExecuteSql<IEnumerable<ExternalConnectionRepository>>(requestContext, (Func<ExternalConnectionSqlComponent, IEnumerable<ExternalConnectionRepository>>) (component => component.GetExternalRepositories(repositoryIds)));
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 18, this.Area, this.Layer, nameof (GetExternalRepositories));
      }
    }

    public IEnumerable<ExternalConnectionRepository> GetConnectedExternalRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      int limit,
      string fromRepositoryName,
      string filter)
    {
      requestContext.TraceEnter(this.TracePointStart + 39, this.Area, this.Layer, "GetExternalRepositories");
      try
      {
        return this.ExecuteSql<IEnumerable<ExternalConnectionRepository>>(requestContext, (Func<ExternalConnectionSqlComponent, IEnumerable<ExternalConnectionRepository>>) (component => component.GetConnectedExternalRepositories(projectId, limit, fromRepositoryName, filter)));
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 40, this.Area, this.Layer, "GetExternalRepositories");
      }
    }

    public virtual void SaveExternalRepositoryData(
      IVssRequestContext requestContext,
      IEnumerable<ExternalConnectionRepository> externalConnectionRepos)
    {
      requestContext.TraceEnter(this.TracePointStart + 19, this.Area, this.Layer, nameof (SaveExternalRepositoryData));
      try
      {
        this.ExecuteSql(requestContext, (Action<ExternalConnectionSqlComponent>) (component => component.SaveExternalRepositoryData(externalConnectionRepos)));
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 20, this.Area, this.Layer, nameof (SaveExternalRepositoryData));
      }
    }

    public virtual IEnumerable<string> TransformArtifactUriToStorageFormat(
      IVssRequestContext requestContext,
      IEnumerable<string> artifactUris,
      out IDictionary<string, string> rawToTransformedUriLookup)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(artifactUris, nameof (artifactUris));
      rawToTransformedUriLookup = (IDictionary<string, string>) new Dictionary<string, string>();
      if (!artifactUris.Any<string>((Func<string, bool>) (url => ExternalConnectionService.IsSupportedExternalArtifactLink(url))))
        return artifactUris;
      string[] array1 = artifactUris.ToArray<string>();
      try
      {
        Dictionary<(string, string), Guid> internalIdLookup = this.GetFriendlyRepoIdentiferToRepoInternalIdLookup(requestContext);
        for (int index = 0; index < array1.Length; ++index)
        {
          string str = array1[index];
          if (LinkingUtilities.IsUriWellFormed(str))
          {
            ArtifactId artifactId = LinkingUtilities.DecodeUri(str);
            if (ExternalConnectionService.IsSupportedExternalArtifactLink(artifactId))
            {
              string[] array2 = ((IEnumerable<string>) artifactId.ToolSpecificId.Split('/')).Select<string, string>((Func<string, string>) (s => Uri.UnescapeDataString(s))).ToArray<string>();
              Guid guid;
              if (array2.Length == 3 && !((IEnumerable<string>) array2).Any<string>((Func<string, bool>) (t => string.IsNullOrEmpty(t))) && internalIdLookup.TryGetValue((array2[0], array2[1]), out guid))
              {
                artifactId.ToolSpecificId = string.Format("{0}/{1}", (object) guid, (object) Uri.EscapeDataString(array2[2]));
                array1[index] = LinkingUtilities.EncodeUri(artifactId);
                rawToTransformedUriLookup[array1[index]] = str;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(this.TracePointStart + 21, TraceLevel.Info, this.Area, this.Layer, ex);
      }
      return (IEnumerable<string>) array1;
    }

    public virtual bool HasPermission(
      IVssRequestContext requestContext,
      int permission,
      Guid? projectId)
    {
      if (projectId.HasValue)
        this.EnsurePermissionProvisioning(requestContext, projectId.Value);
      return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, BoardsExternalConnectionSecurityConstants.NamespaceId).HasPermission(requestContext, this.GetSecurityToken(projectId), permission);
    }

    public void CheckCreatorPermission(
      IVssRequestContext requestContext,
      IExternalConnectionProvider provider,
      ExternalConnection externalConnection)
    {
      ServiceEndpoint rawServiceEndpoint = externalConnection.ServiceEndpoint.RawServiceEndpoint;
      if (!provider.ShouldMatchConnectionCreatorForAddOrUpdateRepo(rawServiceEndpoint))
        return;
      Guid requestContextUserVsid = this.GetRequestContextUserVsid(requestContext);
      if (externalConnection.CreatedBy != requestContextUserVsid)
        throw new ExternalConnectionPermissionDeniedException();
    }

    public void CheckPermission(IVssRequestContext requestContext, int permission, Guid? projectId)
    {
      if (!this.HasPermission(requestContext, permission, projectId))
        throw new ExternalConnectionPermissionDeniedException();
    }

    public static string GetUserFriendlyArtifactUrl(
      string toolName,
      string artifactType,
      string providerKey,
      string fullRepoName,
      string identifier)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(toolName, nameof (toolName));
      ArgumentUtility.CheckStringForNullOrEmpty(artifactType, nameof (artifactType));
      ArgumentUtility.CheckStringForNullOrEmpty(fullRepoName, nameof (fullRepoName));
      ArgumentUtility.CheckStringForNullOrEmpty(identifier, nameof (identifier));
      ArgumentUtility.CheckStringForNullOrEmpty(providerKey, nameof (providerKey));
      string specificId = Uri.EscapeDataString(providerKey) + "/" + Uri.EscapeDataString(fullRepoName) + "/" + Uri.EscapeDataString(identifier);
      return LinkingUtilities.EncodeUri(new ArtifactId(toolName, artifactType, specificId));
    }

    public ExternalConnectionTelemetryData CollectExternalConnectionTelemetry(
      IVssRequestContext requestContext,
      int dataCollectionTimeFrameInDays = 28)
    {
      requestContext.TraceEnter(this.TracePointStart + 30, this.Area, this.Layer, nameof (CollectExternalConnectionTelemetry));
      try
      {
        return this.ExecuteSql<ExternalConnectionTelemetryData>(requestContext, (Func<ExternalConnectionSqlComponent, ExternalConnectionTelemetryData>) (component => component.CollectectExternalConnectionTelemetry(dataCollectionTimeFrameInDays)));
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 31, this.Area, this.Layer, nameof (CollectExternalConnectionTelemetry));
      }
    }

    public void UpdateExternalRepositories(
      IVssRequestContext requestContext,
      string providerKey,
      IEnumerable<ExternalGitRepo> externalRepositories)
    {
      requestContext.TraceEnter(this.TracePointStart + 35, this.Area, this.Layer, nameof (UpdateExternalRepositories));
      try
      {
        this.ExecuteSql(requestContext, (Action<ExternalConnectionSqlComponent>) (component => component.UpdateExternalRepositories(providerKey, externalRepositories)));
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 36, this.Area, this.Layer, nameof (UpdateExternalRepositories));
      }
    }

    public void RemoveExternalRepositoriesFromConnections(
      IVssRequestContext requestContext,
      string providerKey,
      IEnumerable<string> externalRepositoryIds)
    {
      requestContext.TraceEnter(this.TracePointStart + 37, this.Area, this.Layer, "UpdateExternalRepositories");
      try
      {
        this.ExecuteSql(requestContext, (Action<ExternalConnectionSqlComponent>) (component => component.RemoveExternalRepositoriesFromConnections(providerKey, externalRepositoryIds)));
      }
      finally
      {
        requestContext.TraceLeave(this.TracePointStart + 38, this.Area, this.Layer, "UpdateExternalRepositories");
      }
    }

    protected virtual Dictionary<(string providerKey, string fullRepoName), Guid> GetFriendlyRepoIdentiferToRepoInternalIdLookup(
      IVssRequestContext requestContext)
    {
      IEnumerable<ExternalConnectionRepository> externalRepositories = requestContext.GetService<IExternalConnectionService>().GetExternalRepositories(requestContext);
      Dictionary<(string, string), Guid> internalIdLookup = new Dictionary<(string, string), Guid>((IEqualityComparer<(string, string)>) UserFriendlyExternalRepoIdentifierComparer.Instance);
      foreach (ExternalConnectionRepository connectionRepository in externalRepositories)
        internalIdLookup[(connectionRepository.ProviderKey, connectionRepository.RepositoryName)] = connectionRepository.RepositoryId;
      return internalIdLookup;
    }

    internal virtual T ExecuteSql<T>(
      IVssRequestContext requestContext,
      Func<ExternalConnectionSqlComponent, T> func)
    {
      using (ExternalConnectionSqlComponent component = requestContext.CreateComponent<ExternalConnectionSqlComponent>())
        return func(component);
    }

    internal virtual void ExecuteSql(
      IVssRequestContext requestContext,
      Action<ExternalConnectionSqlComponent> func)
    {
      using (ExternalConnectionSqlComponent component = requestContext.CreateComponent<ExternalConnectionSqlComponent>())
        func(component);
    }

    internal virtual List<T> HydrateWithServiceEndpointData<T>(
      IVssRequestContext requestContext,
      Dictionary<int, Guid> dataspaceIdToProjectIdMapping,
      IEnumerable<ExternalConnectionDataset> datasets,
      bool refreshMetadata = false,
      bool includeAuthorization = false)
      where T : ExternalConnection, new()
    {
      List<T> objList = new List<T>();
      foreach (int key in dataspaceIdToProjectIdMapping.Keys)
      {
        int dataspaceId = key;
        Guid guid = dataspaceIdToProjectIdMapping[dataspaceId];
        IEnumerable<ExternalConnectionDataset> source = datasets.Where<ExternalConnectionDataset>((Func<ExternalConnectionDataset, bool>) (d => d.DataspaceId == dataspaceId));
        Dictionary<Guid, Guid> dictionary1 = source.ToDictionary<ExternalConnectionDataset, Guid, Guid>((Func<ExternalConnectionDataset, Guid>) (p => p.ConnectionId), (Func<ExternalConnectionDataset, Guid>) (p => p.ServiceEndpointId));
        Dictionary<Guid, ServiceEndpoint> dictionary2 = requestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(refreshMetadata | includeAuthorization ? requestContext.Elevate() : requestContext, guid, (string) null, (IEnumerable<string>) null, dictionary1.Values.Distinct<Guid>(), (string) null, true, true).ToDictionary<ServiceEndpoint, Guid, ServiceEndpoint>((Func<ServiceEndpoint, Guid>) (s => s.Id), (Func<ServiceEndpoint, ServiceEndpoint>) (s => s));
        foreach (ExternalConnectionDataset connectionDataset in source)
        {
          ServiceEndpoint serviceEndpoint;
          if (!dictionary2.TryGetValue(dictionary1[connectionDataset.ConnectionId], out serviceEndpoint))
          {
            serviceEndpoint = new ServiceEndpoint()
            {
              Id = connectionDataset.ServiceEndpointId,
              CreatedBy = new IdentityRef()
            };
            requestContext.Trace(this.TracePointStart + 10, TraceLevel.Error, this.Area, this.Layer, string.Format("Service endpoint '{0}' for connection '{1}' does not exist.", (object) dictionary1[connectionDataset.ConnectionId], (object) connectionDataset.ConnectionName));
          }
          T obj = new T();
          obj.Id = connectionDataset.ConnectionId;
          obj.ProjectId = guid;
          obj.Name = connectionDataset.ConnectionName;
          obj.ProviderKey = connectionDataset.ProviderKey;
          obj.ServiceEndpoint = new ServiceEndpointViewModel(serviceEndpoint);
          obj.CreatedBy = connectionDataset.CreatedBy;
          obj.ConnectionMetadata = !string.IsNullOrEmpty(connectionDataset.ConnectionMetadata) ? JsonConvert.DeserializeObject<ExternalConnectionMetadata>(connectionDataset.ConnectionMetadata) : new ExternalConnectionMetadata();
          // ISSUE: variable of a boxed type
          __Boxed<T> local = (object) obj;
          IEnumerable<ExternalConnectionRepository> repos = connectionDataset.Repos;
          List<ExternalGitRepo> externalGitRepoList = (repos != null ? repos.Select<ExternalConnectionRepository, ExternalGitRepo>((Func<ExternalConnectionRepository, ExternalGitRepo>) (m => new ExternalGitRepo()
          {
            Id = m.ExternalId,
            Name = m.RepositoryName,
            IsPrivate = m.IsPrivate,
            Url = m.Url,
            WebUrl = m.WebUrl,
            AdditionalProperties = (m.AdditionalProperties != null ? (IDictionary<string, object>) JsonConvert.DeserializeObject<Dictionary<string, object>>(m.AdditionalProperties) : (IDictionary<string, object>) new Dictionary<string, object>())
          }.SetMetadata((object) m.Metadata).SetRepoInternalId(m.RepositoryId))).ToList<ExternalGitRepo>() : (List<ExternalGitRepo>) null) ?? new List<ExternalGitRepo>();
          local.ExternalGitRepos = (IEnumerable<ExternalGitRepo>) externalGitRepoList;
          T connection = obj;
          if (!connection.ExternalGitRepos.Any<ExternalGitRepo>())
            connection.ConnectionMetadata.ConnectionErrorType = new ConnectionErrorType?(ConnectionErrorType.NoRepositories);
          if (connection.ConnectionMetadata.RepositoriesWithMappingToDifferentOrganization.Any<string>())
            connection.ConnectionMetadata.ConnectionErrorType = new ConnectionErrorType?(ConnectionErrorType.RepositoriesMappedToDifferentOrganization);
          if (refreshMetadata)
          {
            if (!connection.ConnectionMetadata.ConnectionErrorType.HasValue)
            {
              try
              {
                this.PopulateMetadata(requestContext, guid, (ExternalConnection) connection);
              }
              catch (Exception ex)
              {
                requestContext.TraceException(this.TracePointStart + 22, this.Area, this.Layer, ex);
                connection.StatusMessage = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ExternalConnectionFailedToPopulateMetadata();
              }
            }
            else
              connection.StatusMessage = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ExternalConnectionIsInvalid();
          }
          objList.Add(connection);
        }
      }
      return objList;
    }

    private void ValidateRepoCount(
      IVssRequestContext requestContext,
      IEnumerable<string> repoExternalIds)
    {
      if (!repoExternalIds.Any<string>())
        throw new GitHubBoardsConnectionCannotBeEmptyException();
      int maxReposCountLimit = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ExternalConnectionMaxReposCountLimit;
      if (repoExternalIds.Count<string>() > maxReposCountLimit)
        throw new GitHubBoardsExceededReposLimitException(maxReposCountLimit);
    }

    private void ValidateReposNotUsedByOtherConnection(
      Guid? currentConnectionId,
      IEnumerable<ExternalConnection> existingConnectionsFromSameProvider,
      HashSet<string> repoExternalIdsToAdd)
    {
      ExternalGitRepo externalGitRepo = existingConnectionsFromSameProvider.Where<ExternalConnection>((Func<ExternalConnection, bool>) (conn =>
      {
        Guid id = conn.Id;
        Guid? nullable = currentConnectionId;
        return !nullable.HasValue || id != nullable.GetValueOrDefault();
      })).SelectMany<ExternalConnection, ExternalGitRepo>((Func<ExternalConnection, IEnumerable<ExternalGitRepo>>) (c => c.ExternalGitRepos)).FirstOrDefault<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => repoExternalIdsToAdd.Contains(r.Id)));
      if (externalGitRepo != null)
        throw new GitHubBoardsRepositoryAlreadyRegisteredException(externalGitRepo.Name);
    }

    private static void ValidateConnectionName(string name)
    {
      if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
        throw new ArgumentOutOfRangeException(nameof (name));
    }

    private void MapDataspaceIds(
      ExternalConnectionSqlComponent component,
      Guid? projectId,
      Dictionary<int, Guid> mapping,
      IEnumerable<ExternalConnectionDataset> datasets)
    {
      if (projectId.HasValue && datasets.Any<ExternalConnectionDataset>())
      {
        mapping.Add(datasets.First<ExternalConnectionDataset>().DataspaceId, projectId.Value);
      }
      else
      {
        foreach (ExternalConnectionDataset dataset in datasets)
        {
          if (!mapping.ContainsKey(dataset.DataspaceId))
          {
            try
            {
              Guid dataspaceIdentifier = component.GetDataspaceIdentifier(dataset.DataspaceId);
              mapping.Add(dataset.DataspaceId, dataspaceIdentifier);
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
    }

    private static ExternalConnectionProvisionResult BuildDefaultDeleteConnectionProvisionResult(
      ExternalConnection existingConnection)
    {
      return new ExternalConnectionProvisionResult()
      {
        RepoProvisionResult = (IList<ExternalGitRepoProvisionResult>) existingConnection.ExternalGitRepos.Select<ExternalGitRepo, ExternalGitRepoProvisionResult>((Func<ExternalGitRepo, ExternalGitRepoProvisionResult>) (repo => new ExternalGitRepoProvisionResult(repo, true, false, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FailedToDeprovisionRepo((object) repo.RepoNameWithOwner())))).ToList<ExternalGitRepoProvisionResult>()
      };
    }

    protected virtual Guid GetRequestContextUserVsid(IVssRequestContext requestContext) => requestContext.GetUserIdentity().Id;

    private void EnsurePermissionProvisioning(IVssRequestContext requestContext, Guid projectId)
    {
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      if (service1.GetValue<bool>(requestContext, (RegistryQuery) this.getExternalConnectionPermissionProvidionRegistryKey(projectId), false))
        return;
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, BoardsExternalConnectionSecurityConstants.NamespaceId);
      IdentityService service2 = requestContext.GetService<IdentityService>();
      IdentityDescriptor descriptor = service2.ReadIdentities(requestContext, IdentitySearchFilter.AdministratorsGroup, ProjectInfo.GetProjectUri(projectId), QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>()?.Descriptor;
      Microsoft.VisualStudio.Services.Identity.Identity identity = service2.GetGroups(requestContext, projectId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.EveryoneGroup
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (descriptor == (IdentityDescriptor) null || identity == null)
        throw new IdentityNotFoundException();
      accessControlEntryList.Add(new AccessControlEntry(descriptor, 3, 0));
      accessControlEntryList.Add(new AccessControlEntry(identity.Descriptor, 1, 0));
      if (securityNamespace.SetAccessControlEntries(requestContext, this.GetSecurityToken(new Guid?(projectId)), (IEnumerable<IAccessControlEntry>) accessControlEntryList, true, false).Count<IAccessControlEntry>() != 2)
        return;
      service1.SetValue<bool>(requestContext, this.getExternalConnectionPermissionProvidionRegistryKey(projectId), true);
    }

    private string GetSecurityToken(Guid? projectId) => !projectId.HasValue ? "$/" : string.Format("$/{0}", (object) projectId.Value);

    private string getExternalConnectionPermissionProvidionRegistryKey(Guid projectId) => string.Format("{0}/{1}/{2}", (object) "/Service/BoardsExternalConnection/Settings", (object) projectId, (object) "HasPermissionProvisioned");

    private void PopulateMetadata(
      IVssRequestContext requestContext,
      Guid projectId,
      ExternalConnection connection)
    {
      IExternalConnectionProvider connectionProvider;
      if (!this.m_endpointTypeToProviderLookup.TryGetValue(connection.ServiceEndpoint.Type, out connectionProvider))
        return;
      connectionProvider.PopulateMetadata(requestContext, projectId, connection);
    }

    protected virtual void ResetRepositoryMetadataNoLongerIntegrated(
      IVssRequestContext requestContext,
      string providerKey,
      Guid projectId,
      IEnumerable<Guid> repoInternalIdsToRemove)
    {
      IEnumerable<ExternalConnectionWithFilteredRepos> byRepoInternalIds = this.GetExternalConnectionsByRepoInternalIds(requestContext.Elevate(), new Guid?(), providerKey, repoInternalIdsToRemove, false, false);
      if (!byRepoInternalIds.Any<ExternalConnectionWithFilteredRepos>())
      {
        requestContext.Trace(this.TracePointStart + 32, TraceLevel.Error, this.Area, this.Layer, "No connection found containing repo '" + string.Join<Guid>(",", repoInternalIdsToRemove) + "' to remove.");
      }
      else
      {
        IEnumerable<ExternalConnectionRepository> connectionRepositories = byRepoInternalIds.SelectMany<ExternalConnectionWithFilteredRepos, ExternalGitRepo>((Func<ExternalConnectionWithFilteredRepos, IEnumerable<ExternalGitRepo>>) (ec => ec.ExternalGitRepos)).GroupBy<ExternalGitRepo, Guid>((Func<ExternalGitRepo, Guid>) (repo => repo.GetRepoInternalId())).Where<IGrouping<Guid, ExternalGitRepo>>((Func<IGrouping<Guid, ExternalGitRepo>, bool>) (g => g.Count<ExternalGitRepo>() == 1)).Select<IGrouping<Guid, ExternalGitRepo>, ExternalConnectionRepository>((Func<IGrouping<Guid, ExternalGitRepo>, ExternalConnectionRepository>) (g => new ExternalConnectionRepository()
        {
          ProviderKey = providerKey,
          RepositoryId = g.Key,
          ExternalId = g.First<ExternalGitRepo>().Id,
          RepositoryName = g.First<ExternalGitRepo>().Name,
          Url = g.First<ExternalGitRepo>().Url,
          WebUrl = g.First<ExternalGitRepo>().WebUrl,
          IsPrivate = g.First<ExternalGitRepo>().IsPrivate,
          Metadata = ExternalConnectionService.s_EmptyMetadata
        }));
        if (!connectionRepositories.Any<ExternalConnectionRepository>())
          return;
        this.SaveExternalRepositoryData(requestContext, connectionRepositories);
      }
    }

    protected virtual void ResetConnectionMetadataForRepositoryNoLongerIntegrated(
      IVssRequestContext requestContext,
      ExternalConnection connection,
      IEnumerable<string> repoExternalIdsToRemove)
    {
      if (!connection.ConnectionMetadata.RepositoriesWithMappingToDifferentOrganization.Any<string>())
        return;
      bool flag = false;
      foreach (string str in repoExternalIdsToRemove)
      {
        if (connection.ConnectionMetadata.RepositoriesWithMappingToDifferentOrganization.Contains(str))
        {
          connection.ConnectionMetadata.RepositoriesWithMappingToDifferentOrganization.Remove(str);
          flag = true;
        }
      }
      if (!flag)
        return;
      this.UpdateExternalConnectionMetadata(requestContext, connection.ProjectId, connection.Id, connection.ConnectionMetadata);
    }

    private static bool IsSupportedExternalArtifactLink(string url) => url != null && url.StartsWith("vstfs:///GitHub", StringComparison.CurrentCultureIgnoreCase);

    private static bool IsSupportedExternalArtifactLink(ArtifactId artifact) => VssStringComparer.ArtifactTool.Equals("GitHub", artifact.Tool) && (VssStringComparer.ArtifactType.Equals("PullRequest", artifact.ArtifactType) || VssStringComparer.ArtifactType.Equals("Commit", artifact.ArtifactType)) && !string.IsNullOrEmpty(artifact.ToolSpecificId);

    private string Layer => nameof (ExternalConnectionService);

    private string Area => "Services";

    private int TracePointStart => 919000;
  }
}
