// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.ITeamFoundationTaggingService
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [DefaultServiceImplementation(typeof (TaggingService))]
  public interface ITeamFoundationTaggingService : IVssFrameworkService
  {
    IEnumerable<ArtifactKind> GetAvailableArtifactKinds(IVssRequestContext requestContext);

    void CleanupUnusedTagDefinitions(IVssRequestContext requestContext, DateTime cutoffTime);

    TagDefinition CreateTagDefinition(
      IVssRequestContext requestContext,
      string name,
      IEnumerable<Guid> applicableArtifactKindIds);

    TagDefinition CreateTagDefinition(
      IVssRequestContext requestContext,
      string name,
      IEnumerable<Guid> applicableArtifactKindIds,
      Guid scope);

    TagDefinition CreateTagDefinition(
      IVssRequestContext requestContext,
      string name,
      IEnumerable<Guid> applicableArtifactKindIds,
      Guid scope,
      TagDefinitionStatus status);

    TagDefinition CreateTagDefinition(IVssRequestContext requestContext, string name);

    TagDefinition CreateTagDefinition(IVssRequestContext requestContext, string name, Guid scope);

    IEnumerable<TagDefinition> EnsureTagDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<string> tagNames,
      IEnumerable<Guid> applicableKinds,
      Guid scope);

    IEnumerable<TagDefinition> GetTagDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds);

    IEnumerable<TagDefinition> GetTagDefinitions(IVssRequestContext requestContext);

    IEnumerable<TagDefinition> GetTagDefinitions(IVssRequestContext requestContext, Guid scope);

    TagDefinition GetTagDefinition(IVssRequestContext requestContext, string name);

    TagDefinition GetTagDefinition(IVssRequestContext requestContext, string name, Guid scope);

    TagDefinition GetTagDefinition(IVssRequestContext requestContext, Guid tagId);

    IEnumerable<TagDefinition> QueryTagDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> applicableKinds);

    IEnumerable<TagDefinition> QueryTagDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> applicableKinds,
      Guid scope);

    TagDefinition UpdateTagDefinition(IVssRequestContext requestContext, TagDefinition tag);

    bool DeleteTagDefinition(IVssRequestContext requestContext, Guid tagId);

    void DeleteTagDefinitionsInScope(IVssRequestContext requestContext, Guid scope);

    TagHistoryEntry<T>[] GetTagsHistoryForArtifact<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      TagArtifact<T> artifact);

    ArtifactTags<T> GetTagsForArtifact<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      TagArtifact<T> artifact);

    IEnumerable<ArtifactTags<T>> GetTagsForArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<TagArtifact<T>> artifacts);

    IEnumerable<ArtifactTags<T>> GetTagsForVersionedArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      ICollection<VersionedTagArtifact<T>> artifacts);

    void UpdateTagsForArtifact<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      TagArtifact<T> artifact,
      IEnumerable<Guid> addedTagIds,
      IEnumerable<Guid> removedTagIds,
      Guid changedBy,
      int? version = null);

    void UpdateTagsForArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<ArtifactTagUpdate<T>> tagUpdates,
      DateTime? changedDate,
      Guid? changedBy);

    IEnumerable<VersionedTagArtifact<T>> QueryArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<Guid> tagIds);

    void DeleteTagHistoryForArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<TagArtifact<T>> artifacts);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool DatabaseSupportsGetTaggedArtifactsFunctions(IVssRequestContext requestContext);
  }
}
