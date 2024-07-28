// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionedItemComponent24
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class VersionedItemComponent24 : VersionedItemComponent23
  {
    public override ResultCollection QueryHistoryScore(
      ItemPathPair targetItemPathPair,
      VersionSpec versionItem,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      Guid teamFoundationId,
      RecursionType recursive,
      bool includeFiles,
      bool slotMode,
      int maxChangesets,
      bool sortAscending,
      int maxChangesPerChangeset,
      int acceptableSeconds)
    {
      this.PrepareStoredProcedure("Tfvc.prc_QueryHistoryScore", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindServerItem("@targetServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(targetItemPathPair), false);
      this.PrepareAndBindVersionSpec("@versionSpecItem", versionItem, false);
      this.PrepareAndBindVersionSpec("@versionSpecFrom", versionFrom, false);
      this.PrepareAndBindVersionSpec("@versionSpecTo", versionTo, false);
      this.BindNullableGuid("@teamFoundationId", teamFoundationId);
      this.BindInt("@depth", (int) recursive);
      this.BindBoolean("@includeFiles", includeFiles);
      this.BindBoolean("@slotMode", slotMode);
      this.BindInt("@maxChangeSets", maxChangesets);
      this.BindBoolean("@sortAscending", sortAscending);
      this.BindInt("@maxChangesPerChangeSet", maxChangesPerChangeset);
      this.BindInt("@acceptableSeconds", acceptableSeconds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) this.CreateDetermineItemTypeColumns());
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      resultCollection.AddBinder<PolicyFailureInfo>((ObjectBinder<PolicyFailureInfo>) new PolicyFailureColumns());
      resultCollection.AddBinder<CheckinNoteFieldValue>((ObjectBinder<CheckinNoteFieldValue>) new CheckinNoteColumns());
      if (includeFiles)
        resultCollection.AddBinder<Change>((ObjectBinder<Change>) this.CreateChangeColumns());
      return resultCollection;
    }

    public override ResultCollection QueryHistoryScore(
      ItemPathPair targetItemPathPair,
      VersionSpec versionItem,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      Guid teamFoundationId,
      RecursionType recursive,
      bool includeFiles,
      bool slotMode,
      int maxChangesets,
      bool sortAscending,
      int maxChangesPerChangeset,
      int acceptableSeconds,
      bool isWildCard)
    {
      return isWildCard ? this.QueryHistory(targetItemPathPair, versionItem, versionFrom, versionTo, teamFoundationId, recursive, includeFiles, slotMode, maxChangesets, sortAscending, maxChangesPerChangeset) : this.QueryHistoryScore(targetItemPathPair, versionItem, versionFrom, versionTo, teamFoundationId, recursive, includeFiles, slotMode, maxChangesets, sortAscending, maxChangesPerChangeset, acceptableSeconds);
    }
  }
}
