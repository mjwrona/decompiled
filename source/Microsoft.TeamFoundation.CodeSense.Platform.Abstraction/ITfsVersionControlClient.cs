// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.ITfsVersionControlClient
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using Microsoft.TeamFoundation.CodeSense.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public interface ITfsVersionControlClient
  {
    List<TfvcItem> GetItems(
      IVssRequestContext requestContext,
      string scopePath = "$/",
      TfvcVersionDescriptor versionDescriptor = null,
      VersionControlRecursionType recursionLevel = VersionControlRecursionType.OneLevel,
      object userState = null);

    List<List<TfvcItem>> GetItemBatch(
      IVssRequestContext requestContext,
      TfvcItemRequestData requestData,
      object userState = null);

    string DownloadItem(
      IVssRequestContext requestContext,
      TfvcItem item,
      TfvcVersionDescriptor versionDescriptor,
      CancellationToken cancellationToken,
      object userState = null);

    TfvcChangeset GetChangeset(
      IVssRequestContext requestContext,
      int changesetId,
      bool includeDetails,
      bool includeWorkItems,
      bool includeSourceRenames,
      int maxChangeCount,
      object userState = null);

    List<TfvcChangesetRef> GetChangesets(
      IVssRequestContext requestContext,
      TfvcChangesetSearchCriteria searchCriteriaObject,
      int? top = null,
      int? skip = null,
      object userState = null);

    IEnumerable<TfvcChange> GetChangesetChanges(
      IVssRequestContext requestContext,
      int changesetId,
      int? top = null,
      int? skip = null,
      object userState = null);

    IEnumerable<WorkItemData> GetChangesetWorkItems(
      IVssRequestContext requestContext,
      int changesetId);

    IEnumerable<WorkItemData> GetChangesetWorkItems(
      IVssRequestContext requestContext,
      TfvcChangeset tfvcChangeset);

    IEnumerable<WorkItemData> GetChangesetWorkItems(
      IVssRequestContext requestContext,
      TfvcChangeset changeset,
      IEnumerable<int> workItemLookupEntries);

    IEnumerable<WorkItemData> GetChangesetWorkItems(
      IVssRequestContext requestContext,
      TfvcChangeset changeset,
      DateTime workItemAssociationDate);

    TfvcBranch GetBranch(
      IVssRequestContext requestContext,
      string path,
      bool includeParent = false,
      bool includeChildren = false,
      object userState = null);

    int GetLatestChangesetNumber(IVssRequestContext requestContext);

    bool IsItemPathValid(string itemPath);
  }
}
