// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ITeamFoundationBuildService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationBuildService))]
  public interface ITeamFoundationBuildService : IVssFrameworkService
  {
    void CancelBuilds(IVssRequestContext requestContext, int[] queueIds, Guid projectId = default (Guid));

    List<BuildDeletionResult> DeleteBuilds(
      IVssRequestContext requestContext,
      IList<string> uris,
      DeleteOptions options,
      bool throwIfInvalidUri,
      Guid projectId = default (Guid),
      bool deleteKeepForever = false);

    IDictionary<int, List<int>> GetQueueIdsByBuildIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> buildIds);

    BuildDefinitionQueryResult QueryBuildDefinitionsByUri(
      IVssRequestContext requestContext,
      IList<string> uris,
      IList<string> propertyNames,
      QueryOptions options,
      Guid projectId = default (Guid));

    TeamFoundationDataReader QueryBuilds(
      IVssRequestContext requestContext,
      IList<BuildDetailSpec> specs,
      Guid projectId = default (Guid));

    TeamFoundationDataReader QueryBuildsByUri(
      IVssRequestContext requestContext,
      IList<string> uris,
      IList<string> informationTypes,
      QueryOptions options,
      QueryDeletedOption deletedOption,
      Guid projectId = default (Guid),
      bool xamlBuildsOnly = false);

    TeamFoundationDataReader QueryQueuedBuilds(
      IVssRequestContext requestContext,
      IList<BuildQueueSpec> specs,
      Guid projectId = default (Guid));

    TeamFoundationDataReader QueryQueuedBuildsById(
      IVssRequestContext requestContext,
      IList<int> ids,
      IList<string> informationTypes,
      QueryOptions options,
      bool includeNewBuilds,
      Guid projectId = default (Guid));

    BuildQueueQueryResult QueueBuilds(
      IVssRequestContext requestContext,
      IList<BuildRequest> requests,
      QueueOptions options,
      Guid projectId = default (Guid));

    BuildQueueQueryResult StartQueuedBuildsNow(
      IVssRequestContext requestContext,
      int[] queueIds,
      Guid projectId = default (Guid));

    void StopBuilds(IVssRequestContext requestContext, IList<string> uris, Guid projectId = default (Guid));

    List<BuildDetail> UpdateBuilds(
      IVssRequestContext requestContext,
      IList<BuildUpdateOptions> updateOptions,
      Guid projectId = default (Guid));

    BuildQueueQueryResult UpdateQueuedBuilds(
      IVssRequestContext requestContext,
      IList<QueuedBuildUpdateOptions> updates,
      Guid projectId = default (Guid));
  }
}
