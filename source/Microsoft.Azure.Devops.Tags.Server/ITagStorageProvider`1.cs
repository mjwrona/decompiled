// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.ITagStorageProvider`1
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [InheritedExport]
  public interface ITagStorageProvider<T>
  {
    TagIdsHistoryEntry<T>[] GetTagsHistoryForArtifact(
      IVssRequestContext requestContext,
      TagArtifact<T> artifactId);

    void DeleteTagHistoryForArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<TagArtifact<T>> artifacts);

    IEnumerable<ArtifactTagIds<T>> GetTagsForArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<TagArtifact<T>> artifacts);

    IEnumerable<ArtifactTagIds<T>> GetTagsForVersionedArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<VersionedTagArtifact<T>> artifacts);

    void UpdateTagsForArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactTagUpdate<T>> tagUpdates,
      DateTime? changedDate,
      Guid? changedBy);

    IEnumerable<VersionedTagArtifact<T>> QueryArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds);

    IEnumerable<Guid> DeleteHistoryForTags(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds);
  }
}
