// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildServiceInternal
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildService))]
  internal interface IBuildServiceInternal : IVssFrameworkService
  {
    IEnumerable<BuildData> GetBuildsByIds(
      IVssRequestContext requestContext,
      IEnumerable<int> buildIds,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false);

    GetChangesResult GetChanges(
      IVssRequestContext requestContext,
      int buildId,
      bool includeSourceChange = false,
      int startId = 0,
      int maxChanges = 50,
      Guid? projectId = null);

    IEnumerable<Change> GetChangesBetweenBuilds(
      IVssRequestContext requestContext,
      int fromBuildId,
      int toBuildId,
      int maxChanges,
      Guid? projectId = null);

    IList<ResourceRef> GetWorkItemsBetweenBuilds(
      IVssRequestContext requestContext,
      int fromBuildId,
      int toBuildId,
      IEnumerable<string> commitIds,
      int maxItems,
      Guid? projectId = null);

    IList<ResourceRef> GetBuildWorkItemRefs(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      IEnumerable<string> commitIds,
      int maxItems);

    Task<int> PurgeArtifactsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int daysOld,
      int batchSize);

    Task<int> PurgeBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int daysOld,
      int batchSize,
      string branchPrefix);

    Task<IList<BuildArtifact>> GetArtifactsBySourceAsync(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      string source);

    long CreateBuildContainer(IVssRequestContext requestContext, BuildData buildData);

    Task<IList<ArtifactCleanupRecord>> CleanUpArtifactsAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requestedBy,
      IList<ArtifactCleanupRecord> artifactCleanupRecords);
  }
}
