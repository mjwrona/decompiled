// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents.IBoardsGitHubAppHostIdMappingService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents
{
  [DefaultServiceImplementation(typeof (BoardsGitHubAppHostIdMappingService))]
  public interface IBoardsGitHubAppHostIdMappingService : IHostIdMappingService, IVssFrameworkService
  {
    Guid? GetHostMappingForInstallation(IVssRequestContext requestContext, string installationId);

    IList<Guid> GetActiveHostIds(IVssRequestContext requestContext, IList<Guid> collectionIds);

    IDictionary<string, Guid?> GetHostMappingsForRepositories(
      IVssRequestContext requestContext,
      string installationId,
      List<string> repoNodeIds);

    bool AddHostMappingForRepository(
      IVssRequestContext requestContext,
      string installationId,
      string repoNodeId);

    IEnumerable<string> AddHostMappingForRepositories(
      IVssRequestContext requestContext,
      string installationId,
      IReadOnlyList<string> repoNodeIds);

    void RemoveHostMappingForRepositories(
      IVssRequestContext requestContext,
      string installationId,
      IReadOnlyList<string> repoNodeIds,
      Guid currentProjectId,
      string providerKey);

    void RemoveHostMappingForRepositoriesWithoutChecking(
      IVssRequestContext requestContext,
      string installationId,
      IReadOnlyList<string> repoNodeIds);

    void RemoveHostMappingForInstallation(IVssRequestContext requestContext, string installationId);

    void AddHostMappingForInstallation(IVssRequestContext requestContext, string installationId);
  }
}
