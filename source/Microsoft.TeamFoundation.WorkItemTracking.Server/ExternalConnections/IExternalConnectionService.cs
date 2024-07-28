// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.IExternalConnectionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  [DefaultServiceImplementation(typeof (ExternalConnectionService))]
  public interface IExternalConnectionService : IVssFrameworkService
  {
    ExternalConnection GetExternalConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid connectionId,
      bool refreshMetadata = false,
      bool includeAuthorization = false,
      bool includeRepoProviderData = false,
      bool includeConnectionWithInvalidEndpoint = false);

    IReadOnlyCollection<ExternalConnection> GetExternalConnections(
      IVssRequestContext requestContext,
      Guid? projectId,
      string providerKey,
      bool refreshMetadata = false,
      bool includeAuthorization = false,
      bool includeRepoProviderData = false,
      bool includeInvalidConnections = false);

    IEnumerable<ExternalConnectionWithFilteredRepos> GetExternalConnectionsByRepoExternalIds(
      IVssRequestContext requestContext,
      Guid? projectId,
      string providerKey,
      IEnumerable<string> externalIds,
      bool refreshMetadata = false,
      bool includeAuthorization = false);

    IEnumerable<ExternalConnectionWithFilteredRepos> GetExternalConnectionsByRepoInternalIds(
      IVssRequestContext requestContext,
      Guid? projectId,
      string providerKey,
      IEnumerable<Guid> internalIds,
      bool refreshMetadata = false,
      bool includeAuthorization = false);

    ExternalConnectionProvisionResult CreateExternalConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint serviceEndpoint,
      IEnumerable<string> repoExternalIds);

    IEnumerable<ExternalGitRepo> GetExternalConnectionRepos(
      IVssRequestContext requestContext,
      Guid projectId,
      string providerKey,
      Guid connectionId,
      IEnumerable<string> repoExternalIds);

    ExternalConnectionProvisionResult AddExternalConnectionRepos(
      IVssRequestContext requestContext,
      Guid projectId,
      string providerKey,
      Guid connectionId,
      IEnumerable<string> repoExternalNames);

    ExternalConnectionProvisionResult RemoveExternalConnectionRepos(
      IVssRequestContext requestContext,
      Guid projectId,
      string providerKey,
      Guid connectionId,
      IEnumerable<string> repoExternalNames);

    ExternalConnectionProvisionResult UpdateExternalConnectionRepos(
      IVssRequestContext requestContext,
      Guid projectId,
      string providerKey,
      Guid connectionId,
      IEnumerable<string> repoExternalIds);

    void UpdateExternalConnectionMetadata(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid connectionId,
      ExternalConnectionMetadata connectionMetadata);

    ExternalConnectionProvisionResult DeleteExternalConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid connectionId);

    IEnumerable<ExternalProvider> GetExternalProviders(IVssRequestContext requestContext);

    IEnumerable<ExternalConnectionRepository> GetExternalRepositories(
      IVssRequestContext requestContext,
      IEnumerable<Guid> repositoryIds = null);

    IEnumerable<ExternalConnectionRepository> GetConnectedExternalRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      int limit,
      string fromRepositoryName,
      string filter);

    void SaveExternalRepositoryData(
      IVssRequestContext requestContext,
      IEnumerable<ExternalConnectionRepository> externalConnectionRepos);

    ExternalProvider SaveExternalProvider(
      IVssRequestContext requestContext,
      Guid? providerId,
      string providerKey,
      string providerType);

    IEnumerable<string> TransformArtifactUriToStorageFormat(
      IVssRequestContext requestContext,
      IEnumerable<string> artifactUris,
      out IDictionary<string, string> rawToTransformedUriLookup);

    bool HasPermission(IVssRequestContext requestContext, int permission, Guid? projectId);

    void CheckPermission(IVssRequestContext requestContext, int permission, Guid? projectId);

    ExternalConnectionTelemetryData CollectExternalConnectionTelemetry(
      IVssRequestContext requestContext,
      int dataCollectionTimeFrameInDays = 28);

    void UpdateExternalRepositories(
      IVssRequestContext requestContext,
      string providerKey,
      IEnumerable<ExternalGitRepo> externalRepositories);

    void RemoveExternalRepositoriesFromConnections(
      IVssRequestContext requestContext,
      string providerKey,
      IEnumerable<string> externalRepositoryIds);
  }
}
