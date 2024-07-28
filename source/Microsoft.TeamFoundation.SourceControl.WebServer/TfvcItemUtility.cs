// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcItemUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TfvcItemUtility
  {
    internal static readonly string[] s_defaultPropertyFilters = new string[1]
    {
      "Microsoft.TeamFoundation.VersionControl.SymbolicLink"
    };
    private static readonly int s_queryItemOptions = 12;

    public static TfvcItem GetItem(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string path,
      VersionSpec version,
      DeletedState deletedState)
    {
      return TfvcItemUtility.GetItems(requestContext, urlHelper, path, version, RecursionType.None, deletedState, false).FirstOrDefault<TfvcItem>() ?? throw new ItemNotFoundException(requestContext, new ItemSpec(path, RecursionType.None), version);
    }

    public static TfvcItemsCollection GetItemsCollection(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string scopePath,
      TfvcVersionDescriptor versionDescriptor,
      VersionControlRecursionType recursionType,
      bool includeContentMetadata,
      bool includeLinks,
      long scanBytesForEncoding = 0)
    {
      VersionControlPath.ValidatePath(scopePath);
      VersionSpec versionSpec = TfvcVersionSpecUtility.GetVersionSpec(requestContext, versionDescriptor);
      List<TfvcItem> list = TfvcItemUtility.GetItems(requestContext, urlHelper, scopePath, versionSpec, TfvcCommonUtility.ConvertVersionControlRecursionType(recursionType), DeletedState.NonDeleted, includeLinks).ToList<TfvcItem>();
      if (list == null || !list.Any<TfvcItem>())
        throw new ItemNotFoundException(requestContext, new ItemSpec(scopePath, RecursionType.None), versionSpec);
      if (includeContentMetadata)
      {
        int encoding = 0;
        bool containsByteOrderMark = false;
        TfvcItem file = list.First<TfvcItem>();
        if (!file.IsFolder)
          encoding = file.Encoding != -1 ? TfvcFileUtility.TryDetectFileEncoding(requestContext, (ItemModel) file, file.Encoding, scanBytesForEncoding, out containsByteOrderMark) : (!file.IsSymbolicLink ? -1 : 0);
        file.ContentMetadata = TfvcFileUtility.GetFileContentMetadata(requestContext, scopePath, file.IsFolder, encoding);
      }
      TfvcItemsCollection itemsCollection = new TfvcItemsCollection();
      itemsCollection.AddRange((IEnumerable<TfvcItem>) list);
      return itemsCollection;
    }

    public static IEnumerable<TfvcItem> GetItems(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string path,
      VersionSpec version,
      RecursionType recursion,
      DeletedState deletedState,
      bool includeLinks)
    {
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      path = VersionControlPath.GetFullPath(path);
      ItemSpec itemSpec1 = new ItemSpec(path, RecursionType.None, 0);
      if (version is TipVersionSpec && ((TipVersionSpec) version).Version is LatestVersionSpec)
        version = (VersionSpec) new LatestVersionSpec();
      if (version is TipVersionSpec)
      {
        TipVersionSpec tipVersionSpec = (TipVersionSpec) version;
        VersionSpec versionSpec = (VersionSpec) new ChangesetVersionSpec(TfvcItemUtility.GetItem(requestContext, urlHelper, path, tipVersionSpec.Version, DeletedState.Any).ChangesetVersion);
        TfvcItem tfvcItem = (TfvcItem) null;
        using (TeamFoundationDataReader foundationDataReader = service.QueryHistory(requestContext, (string) null, (string) null, itemSpec1, versionSpec, string.Empty, versionSpec, (VersionSpec) new LatestVersionSpec(), 1, true, false, true, false))
        {
          Changeset changeset = foundationDataReader.CurrentEnumerable<Changeset>().FirstOrDefault<Changeset>();
          tfvcItem = TfvcItemUtility.GetItemFromChangeset(requestContext, urlHelper, service, changeset, TfvcItemUtility.s_defaultPropertyFilters);
        }
        if (tfvcItem == null)
          throw new ItemNotFoundException(requestContext, itemSpec1, version);
        if (tfvcItem.IsFolder && recursion != RecursionType.None)
          return TfvcItemUtility.GetItems(requestContext, urlHelper, tfvcItem.Path, (VersionSpec) new LatestVersionSpec(), recursion, deletedState, includeLinks);
        tfvcItem.Links = includeLinks ? tfvcItem.GetItemsReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
        return (IEnumerable<TfvcItem>) new TfvcItem[1]
        {
          tfvcItem
        };
      }
      if (version is PreviousVersionSpec)
      {
        PreviousVersionSpec previousVersionSpec = (PreviousVersionSpec) version;
        if (previousVersionSpec.Version is ShelvesetVersionSpec)
        {
          ShelvesetVersionSpec version1 = (ShelvesetVersionSpec) previousVersionSpec.Version;
          TfvcChange tfvcChange = TfvcItemUtility.GetShelvedChanges(requestContext, urlHelper, path, RecursionType.Full, 0, version1, int.MaxValue, 0, (string[]) null, out bool _).FirstOrDefault<TfvcChange>();
          if (tfvcChange == null)
            throw new ItemNotFoundException(requestContext, itemSpec1, version);
          VersionSpec version2 = tfvcChange.PendingVersion != 0 ? (VersionSpec) new ChangesetVersionSpec(tfvcChange.PendingVersion) : (VersionSpec) version1;
          string path1 = tfvcChange.Item.Path;
          if (!string.IsNullOrEmpty(tfvcChange.SourceServerItem))
            path1 = tfvcChange.SourceServerItem;
          return TfvcItemUtility.GetItems(requestContext, urlHelper, path1, version2, recursion, deletedState, includeLinks);
        }
        TfvcItem tfvcItem = (TfvcItem) null;
        using (TeamFoundationDataReader foundationDataReader = service.QueryHistory(requestContext, (string) null, (string) null, itemSpec1, previousVersionSpec.Version, string.Empty, (VersionSpec) new ChangesetVersionSpec(1), previousVersionSpec.Version, 2, true, false, true, false))
        {
          List<Changeset> list = foundationDataReader.CurrentEnumerable<Changeset>().ToList<Changeset>();
          Changeset changeset = list.Count == 2 ? list.LastOrDefault<Changeset>() : throw new ItemNotFoundException(requestContext, itemSpec1, (VersionSpec) previousVersionSpec);
          tfvcItem = TfvcItemUtility.GetItemFromChangeset(requestContext, urlHelper, service, changeset, TfvcItemUtility.s_defaultPropertyFilters);
        }
        if (tfvcItem == null)
          throw new ItemNotFoundException(requestContext, itemSpec1, version);
        if (tfvcItem.IsFolder && recursion != RecursionType.None)
          return TfvcItemUtility.GetItems(requestContext, urlHelper, tfvcItem.Path, (VersionSpec) new ChangesetVersionSpec(tfvcItem.ChangesetVersion), recursion, deletedState, includeLinks);
        tfvcItem.Links = includeLinks ? tfvcItem.GetItemsReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
        return (IEnumerable<TfvcItem>) new TfvcItem[1]
        {
          tfvcItem
        };
      }
      if (version is ShelvesetVersionSpec)
      {
        ShelvesetVersionSpec shelvesetVerSpec = version as ShelvesetVersionSpec;
        requestContext.Trace(513075, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Retrieving shelveset version of item. {0};{1}", (object) shelvesetVerSpec.Name, (object) shelvesetVerSpec.Owner);
        RecursionType recursionType = recursion;
        if (recursion == RecursionType.OneLevel)
          recursionType = RecursionType.Full;
        if (recursion == RecursionType.OneLevel)
        {
          IList<TfvcChange> shelvedChanges = TfvcItemUtility.GetShelvedChanges(requestContext, urlHelper, path, recursionType, 0, shelvesetVerSpec, int.MaxValue, 0, TfvcItemUtility.s_defaultPropertyFilters, out bool _);
          return TfvcItemUtility.NormalizeSparseTree(requestContext, urlHelper, path, (VersionSpec) shelvesetVerSpec, (IEnumerable<TfvcChange>) shelvedChanges, includeLinks).Select<TfvcChange, TfvcItem>((Func<TfvcChange, TfvcItem>) (c => c.Item));
        }
        ItemSpec itemSpec2 = new ItemSpec(path, recursionType, 0);
        using (TeamFoundationDataReader foundationDataReader = service.QueryShelvedChanges(requestContext, (string) null, (string) null, shelvesetVerSpec.Name, shelvesetVerSpec.Owner, new ItemSpec[1]
        {
          itemSpec2
        }, false, TfvcItemUtility.s_defaultPropertyFilters))
          return (IEnumerable<TfvcItem>) foundationDataReader.CurrentEnumerable<PendingSet>().SelectMany<PendingSet, PendingChange, TfvcItem>((Func<PendingSet, IEnumerable<PendingChange>>) (pendingSet => (IEnumerable<PendingChange>) pendingSet.PendingChanges), (Func<PendingSet, PendingChange, TfvcItem>) ((pendingSet, pendingChange) => TfsModelExtensions.CreateWebApiChangeModel(pendingChange, shelvesetVerSpec, url: urlHelper, requestContext: requestContext, includeLinks: includeLinks).Item)).ToList<TfvcItem>();
      }
      else if (version is ChangeVersionSpec)
      {
        ChangeVersionSpec version3 = version as ChangeVersionSpec;
        requestContext.Trace(513075, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Retrieving changeset version of item. Changeset: {0}", (object) version3.ChangesetId);
        using (TeamFoundationDataReader foundationDataReader = service.QueryChangesForChangeset(requestContext, version3.ChangesetId, false, int.MaxValue, (ItemSpec) null, TfvcItemUtility.s_defaultPropertyFilters, false))
        {
          if (recursion != RecursionType.OneLevel && recursion != RecursionType.None)
            return foundationDataReader.CurrentEnumerable<Change>().Where<Change>((Func<Change, bool>) (change => VersionControlPath.IsSubItem(change.Item.ServerItem, path))).Select<Change, TfvcItem>((Func<Change, TfvcItem>) (change => change.Item.ToWebApiItem(requestContext, urlHelper, includeLinks: includeLinks)));
          TfvcChange[] array = foundationDataReader.CurrentEnumerable<Change>().Where<Change>((Func<Change, bool>) (change => VersionControlPath.IsSubItem(change.Item.ServerItem, path))).Select<Change, TfvcChange>((Func<Change, TfvcChange>) (change => change.ToWebApiChangeModel())).ToArray<TfvcChange>();
          if (array.Length == 0)
            throw new ItemNotFoundException(requestContext, itemSpec1, version);
          return TfvcItemUtility.NormalizeSparseTree(requestContext, urlHelper, path, (VersionSpec) version3, (IEnumerable<TfvcChange>) array, includeLinks).Select<TfvcChange, TfvcItem>((Func<TfvcChange, TfvcItem>) (c => c.Item));
        }
      }
      else
      {
        if (version is MergeSourceVersionSpec)
          return (IEnumerable<TfvcItem>) TfvcItemUtility.GetMergeSourceChanges<TfvcItem>(requestContext, version as MergeSourceVersionSpec, path, (Func<ExtendedMerge, TfvcItem>) (merge => merge.SourceItem.Item.ToWebApiItem(requestContext, urlHelper)));
        ItemSpec itemSpec3 = new ItemSpec(path, recursion);
        using (TeamFoundationDataReader foundationDataReader = service.QueryItems(requestContext, (string) null, (string) null, new ItemSpec[1]
        {
          itemSpec3
        }, version, deletedState, ItemType.Any, false, TfvcItemUtility.s_queryItemOptions, TfvcItemUtility.s_defaultPropertyFilters, (string[]) null))
          return (IEnumerable<TfvcItem>) foundationDataReader.CurrentEnumerable<ItemSet>().SelectMany<ItemSet, Item, TfvcItem>((Func<ItemSet, IEnumerable<Item>>) (itemset => (IEnumerable<Item>) itemset.Items), (Func<ItemSet, Item, TfvcItem>) ((itemset, item) => item.ToWebApiItem(requestContext, urlHelper, includeLinks: includeLinks))).ToList<TfvcItem>();
      }
    }

    internal static IEnumerable<TfvcItem> GetItemsPaged(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string scopePath,
      int changesetId,
      int top,
      string continuationPath,
      out string lastItemPaged)
    {
      scopePath = VersionControlPath.GetFullPath(scopePath);
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      ItemSpec itemSpec1 = new ItemSpec(scopePath, RecursionType.Full);
      ItemSpec itemSpec2 = string.IsNullOrWhiteSpace(continuationPath) ? (ItemSpec) null : new ItemSpec(continuationPath, RecursionType.Full);
      IVssRequestContext requestContext1 = requestContext;
      ItemSpec scopeItem = itemSpec1;
      int changesetId1 = changesetId;
      int pageSize = top;
      ItemSpec continuationItem = itemSpec2;
      int queryItemOptions = TfvcItemUtility.s_queryItemOptions;
      ref string local = ref lastItemPaged;
      using (TeamFoundationDataReader foundationDataReader = service.QueryItemsPaged(requestContext1, scopeItem, changesetId1, pageSize, continuationItem, queryItemOptions, out local))
        return (IEnumerable<TfvcItem>) foundationDataReader.CurrentEnumerable<Item>().Select<Item, TfvcItem>((Func<Item, TfvcItem>) (item => item.ToWebApiItem(requestContext, urlHelper))).ToList<TfvcItem>();
    }

    internal static IEnumerable<TfvcItemPreviousHash> GetItemsByChangesetPaged(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string scopePath,
      int baseChangesetId,
      int targetChangesetId,
      int top,
      string continuationPath,
      out string lastItemPaged)
    {
      scopePath = VersionControlPath.GetFullPath(scopePath);
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      ItemSpec itemSpec1 = new ItemSpec(scopePath, RecursionType.Full);
      ItemSpec itemSpec2 = string.IsNullOrWhiteSpace(continuationPath) ? (ItemSpec) null : new ItemSpec(continuationPath, RecursionType.Full);
      IVssRequestContext requestContext1 = requestContext;
      ItemSpec scopeItem = itemSpec1;
      int baseChangesetId1 = baseChangesetId;
      int targetChangesetId1 = targetChangesetId;
      int pageSize = top;
      ItemSpec continuationItem = itemSpec2;
      ref string local = ref lastItemPaged;
      using (TeamFoundationDataReader foundationDataReader = service.QueryItemsByChangesetPaged(requestContext1, scopeItem, baseChangesetId1, targetChangesetId1, pageSize, continuationItem, out local))
        return (IEnumerable<TfvcItemPreviousHash>) foundationDataReader.CurrentEnumerable<PreviousHashItem>().Select<PreviousHashItem, TfvcItemPreviousHash>((Func<PreviousHashItem, TfvcItemPreviousHash>) (item => TfsModelExtensions.ToWebApiItem(item, requestContext, urlHelper, false, false))).ToList<TfvcItemPreviousHash>();
    }

    public static IList<WebApiType> GetMergeSourceChanges<WebApiType>(
      IVssRequestContext rc,
      MergeSourceVersionSpec mergeSource,
      string path,
      Func<ExtendedMerge, WebApiType> translateFunction)
    {
      ChangesetVersionSpec changesetVersionSpec = new ChangesetVersionSpec(mergeSource.ChangesetId);
      using (TeamFoundationDataReader foundationDataReader = rc.GetService<TeamFoundationVersionControlService>().QueryMergesExtended(rc, (string) null, (string) null, new ItemSpec(path, RecursionType.None), (VersionSpec) changesetVersionSpec, (VersionSpec) changesetVersionSpec, (VersionSpec) changesetVersionSpec, mergeSource.UseRenameSource ? QueryMergesExtendedOptions.QueryRenames : QueryMergesExtendedOptions.None))
        return (IList<WebApiType>) foundationDataReader.Current<StreamingCollection<ExtendedMerge>>().Select<ExtendedMerge, WebApiType>((Func<ExtendedMerge, WebApiType>) (m => translateFunction(m))).ToList<WebApiType>();
    }

    internal static IList<TfvcChange> GetShelvedChanges(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string itemPath,
      RecursionType recursion,
      int deletionId,
      ShelvesetVersionSpec versionSpec,
      int maxCount,
      int skipCount,
      string[] propertyFilters,
      out bool allChangesIncluded)
    {
      int num = 0;
      try
      {
        requestContext.TraceEnter(513115, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetShelvedChanges));
        ItemSpec itemSpec = new ItemSpec(itemPath, recursion, deletionId);
        TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
        allChangesIncluded = true;
        IVssRequestContext requestContext1 = requestContext;
        string name = versionSpec.Name;
        string owner = versionSpec.Owner;
        ItemSpec[] itemSpecs = new ItemSpec[1]{ itemSpec };
        string[] itemPropertyFilters = propertyFilters;
        using (TeamFoundationDataReader foundationDataReader = service.QueryShelvedChanges(requestContext1, (string) null, (string) null, name, owner, itemSpecs, false, itemPropertyFilters))
        {
          List<TfvcChange> shelvedChanges = new List<TfvcChange>();
          foreach (PendingSet current in foundationDataReader.CurrentEnumerable<PendingSet>())
          {
            foreach (PendingChange pendingChange in current.PendingChanges)
            {
              if (num < skipCount)
              {
                ++num;
              }
              else
              {
                if (shelvedChanges.Count == maxCount)
                {
                  allChangesIncluded = false;
                  break;
                }
                shelvedChanges.Add(TfsModelExtensions.CreateWebApiChangeModel(pendingChange, versionSpec, url: urlHelper, requestContext: requestContext));
              }
            }
            if (!allChangesIncluded)
              break;
          }
          return (IList<TfvcChange>) shelvedChanges;
        }
      }
      finally
      {
        requestContext.TraceLeave(513120, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetShelvedChanges));
      }
    }

    private static TfvcItem GetItemFromChangeset(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      TeamFoundationVersionControlService vcService,
      Changeset changeset,
      string[] propertyFilters)
    {
      TfvcItem tfvcItem = (TfvcItem) null;
      if (changeset != null)
      {
        Change change = changeset.Changes.FirstOrDefault<Change>();
        if (change != null)
        {
          tfvcItem = change.Item.ToWebApiItem(requestContext, urlHelper);
          tfvcItem.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcItemsLocationId, (object) new
          {
            path = tfvcItem.Path,
            versionType = TfvcVersionType.Changeset,
            version = changeset.ChangesetId
          });
          if (propertyFilters != null)
          {
            using (TeamFoundationDataReader foundationDataReader = vcService.QueryItems(requestContext, (string) null, (string) null, new ItemSpec[1]
            {
              new ItemSpec(tfvcItem.Path, RecursionType.None)
            }, (VersionSpec) new ChangesetVersionSpec(changeset.ChangesetId), DeletedState.Any, ItemType.Any, false, 0, propertyFilters, (string[]) null))
            {
              Item obj = foundationDataReader.CurrentEnumerable<ItemSet>().SelectMany<ItemSet, Item>((Func<ItemSet, IEnumerable<Item>>) (set => (IEnumerable<Item>) set.Items)).FirstOrDefault<Item>();
              if (obj != null)
                tfvcItem.ParsePropertyValues((IEnumerable<PropertyValue>) obj.PropertyValues);
            }
          }
        }
      }
      return tfvcItem;
    }

    private static IList<TfvcChange> NormalizeSparseTree(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string path,
      VersionSpec version,
      IEnumerable<TfvcChange> changes,
      bool includeLinks)
    {
      List<TfvcChange> list = changes.GroupBy<TfvcChange, string>((Func<TfvcChange, string>) (change =>
      {
        string str = VersionControlPath.MakeRelative(change.Item.Path, path);
        if (str.Length > 0)
        {
          int length = str.IndexOf('/');
          if (length >= 0)
            str = str.Substring(0, length);
        }
        return str;
      })).Select<IGrouping<string, TfvcChange>, TfvcChange>((Func<IGrouping<string, TfvcChange>, TfvcChange>) (g =>
      {
        string str = VersionControlPath.Combine(path, g.Key);
        VersionControlChangeType changeType = VersionControlChangeType.None;
        int val1_1 = int.MaxValue;
        DateTime dateTime = DateTime.MaxValue;
        bool flag = true;
        int val1_2 = int.MaxValue;
        foreach (TfvcChange tfvcChange in (IEnumerable<TfvcChange>) g)
        {
          changeType |= tfvcChange.ChangeType;
          val1_1 = Math.Min(val1_1, tfvcChange.Item.ChangesetVersion);
          dateTime = new DateTime(Math.Min(dateTime.Ticks, tfvcChange.Item.ChangeDate.Ticks));
          if (StringComparer.OrdinalIgnoreCase.Equals(str, tfvcChange.Item.Path))
            flag = tfvcChange.Item.IsFolder;
          val1_2 = Math.Min(val1_2, tfvcChange.Item.DeletionId);
        }
        TfvcItem webApiItem = TfsModelExtensions.CreateWebApiItem(requestContext, urlHelper, str, version, includeLinks);
        webApiItem.ChangesetVersion = val1_1;
        webApiItem.IsFolder = flag;
        webApiItem.DeletionId = val1_2;
        webApiItem.ChangeDate = dateTime;
        return new TfvcChange(webApiItem, changeType, (string) null);
      })).ToList<TfvcChange>();
      if (list.Count > 0 && !StringComparer.OrdinalIgnoreCase.Equals(path, list[0].Item.Path))
      {
        TfvcItem webApiItem = TfsModelExtensions.CreateWebApiItem(requestContext, urlHelper, path, version, includeLinks);
        webApiItem.IsFolder = true;
        webApiItem.ChangesetVersion = int.MaxValue;
        webApiItem.ChangeDate = DateTime.MaxValue;
        webApiItem.DeletionId = int.MaxValue;
        TfvcChange tfvcChange1 = new TfvcChange(webApiItem, VersionControlChangeType.None, (string) null);
        foreach (TfvcChange tfvcChange2 in list)
        {
          tfvcChange1.ChangeType |= tfvcChange2.ChangeType;
          webApiItem.ChangesetVersion = Math.Min(webApiItem.ChangesetVersion, tfvcChange2.Item.ChangesetVersion);
          webApiItem.ChangeDate = new DateTime(Math.Min(webApiItem.ChangeDate.Ticks, tfvcChange2.Item.ChangeDate.Ticks));
          webApiItem.DeletionId = Math.Min(webApiItem.DeletionId, tfvcChange2.Item.DeletionId);
        }
        list.Insert(0, tfvcChange1);
      }
      return (IList<TfvcChange>) list;
    }
  }
}
