// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfsModelExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TfsModelExtensions
  {
    public const string c_symbolicLinkPropertyName = "Microsoft.TeamFoundation.VersionControl.SymbolicLink";
    internal const string c_plusSign = "+";
    internal const string c_plusSignEncoded = "%2b";
    private const string c_items = "/items/$/";
    private const string c_itemsPath = "/items?path=$/";
    private const string c_webconfig = "/web.config";
    private const string c_cshtmlExtension = ".cshtml";

    public static TfvcItem ToWebApiItem(
      this Item item,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper,
      bool shallowItem = false,
      bool includeLinks = false)
    {
      TfvcItem tfvcItem = new TfvcItem();
      tfvcItem.ChangesetVersion = item.ChangesetId;
      tfvcItem.Path = item.ServerItem;
      tfvcItem.IsFolder = item.ItemType == ItemType.Folder;
      tfvcItem.DeletionId = item.DeletionId;
      if (!tfvcItem.IsFolder && item.HashValue != null)
      {
        tfvcItem.HashValue = Convert.ToBase64String(item.HashValue);
        tfvcItem.Size = item.FileLength;
      }
      if (!shallowItem)
      {
        tfvcItem.Id = item.ItemId;
        tfvcItem.Encoding = item.Encoding;
        tfvcItem.FileId = item.GetFileId(0);
        tfvcItem.ChangeDate = item.CheckinDate;
        tfvcItem.IsBranch = item.IsBranch;
        tfvcItem.IsPendingChange = false;
        tfvcItem.ParsePropertyValues((IEnumerable<PropertyValue>) item.PropertyValues);
      }
      if (urlHelper != null && tfsRequestContext != null)
        tfvcItem.Url = urlHelper.RestLink(tfsRequestContext, TfvcConstants.TfvcItemsLocationId, (object) new
        {
          path = item.ServerItem,
          versionType = TfvcVersionType.Changeset,
          version = item.ChangesetId
        });
      string url1 = tfvcItem.Url;
      if ((url1 != null ? (url1.IndexOf("+", StringComparison.OrdinalIgnoreCase) > 0 ? 1 : 0) : 0) == 0)
      {
        string url2 = tfvcItem.Url;
        if ((url2 != null ? (url2.IndexOf("%2b", StringComparison.OrdinalIgnoreCase) > 0 ? 1 : 0) : 0) == 0)
        {
          if (!tfvcItem.IsFolder)
          {
            string url3 = tfvcItem.Url;
            if ((url3 != null ? (url3.Length > 0 ? 1 : 0) : 0) == 0)
              goto label_12;
          }
          else
            goto label_12;
        }
      }
      if (tfsRequestContext.IsFeatureEnabled("Tfvc.ItemUriUsePath"))
        tfvcItem.Url = TfsModelExtensions.ParameterizeUnsafeUrlPaths(tfvcItem.Url);
label_12:
      tfvcItem.Links = includeLinks ? tfvcItem.GetItemsReferenceLinks(tfsRequestContext, urlHelper) : (ReferenceLinks) null;
      return tfvcItem;
    }

    public static TfvcItemPreviousHash ToWebApiItem(
      this PreviousHashItem item,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper,
      bool shallowItem = false,
      bool includeLinks = false)
    {
      TfvcItemPreviousHash tfvcItem = new TfvcItemPreviousHash();
      tfvcItem.ChangesetVersion = item.ChangesetId;
      tfvcItem.Path = item.ServerItem;
      tfvcItem.IsFolder = item.ItemType == ItemType.Folder;
      tfvcItem.DeletionId = item.DeletionId;
      if (!tfvcItem.IsFolder && item.HashValue != null)
      {
        tfvcItem.HashValue = Convert.ToBase64String(item.HashValue);
        tfvcItem.Size = item.FileLength;
      }
      if (!tfvcItem.IsFolder && item.PreviousHashValue != null)
        tfvcItem.PreviousHashValue = Convert.ToBase64String(item.PreviousHashValue);
      if (!shallowItem)
      {
        tfvcItem.Id = item.ItemId;
        tfvcItem.Encoding = item.Encoding;
        tfvcItem.FileId = item.GetFileId(0);
        tfvcItem.ChangeDate = item.CheckinDate;
        tfvcItem.IsBranch = item.IsBranch;
        tfvcItem.IsPendingChange = false;
        tfvcItem.ParsePropertyValues((IEnumerable<PropertyValue>) item.PropertyValues);
      }
      if (urlHelper != null && tfsRequestContext != null)
        tfvcItem.Url = urlHelper.RestLink(tfsRequestContext, TfvcConstants.TfvcItemsLocationId, (object) new
        {
          path = item.ServerItem,
          versionType = TfvcVersionType.Changeset,
          version = item.ChangesetId
        });
      tfvcItem.Links = includeLinks ? tfvcItem.GetItemsReferenceLinks(tfsRequestContext, urlHelper) : (ReferenceLinks) null;
      return tfvcItem;
    }

    public static TfvcItem CreateWebApiItem(
      PendingChange change,
      ShelvesetVersionSpec shelvesetSpec,
      bool shallowItem,
      UrlHelper urlHelper,
      IVssRequestContext requestContext,
      bool includeLinks = false)
    {
      TfvcItem tfvcItem1 = new TfvcItem();
      tfvcItem1.ChangesetVersion = change.Version == 0 ? change.SourceVersionFrom : change.Version;
      tfvcItem1.Path = change.ServerItem;
      tfvcItem1.IsFolder = change.ItemType == ItemType.Folder;
      tfvcItem1.DeletionId = change.DeletionId;
      TfvcItem tfvcItem2 = tfvcItem1;
      if (!tfvcItem2.IsFolder && change.UploadHashValue != null)
        tfvcItem2.HashValue = Convert.ToBase64String(change.UploadHashValue);
      if (!shallowItem)
      {
        tfvcItem2.Encoding = change.Encoding;
        tfvcItem2.IsPendingChange = true;
        tfvcItem2.FileId = change.GetFileId(0);
        tfvcItem2.IsBranch = (change.ChangeType & ChangeType.Branch) != 0;
        tfvcItem2.ParsePropertyValues((IEnumerable<PropertyValue>) change.PropertyValues);
      }
      if (urlHelper != null && requestContext != null)
        tfvcItem2.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcItemsLocationId, (object) new
        {
          path = change.ServerItem,
          versionType = TfvcVersionType.Shelveset,
          version = shelvesetSpec.ToShelvesetVersion()
        });
      tfvcItem2.Links = includeLinks ? tfvcItem2.GetItemsReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
      return tfvcItem2;
    }

    public static TfvcItem CreateWebApiItem(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string serverItem,
      VersionSpec version,
      bool includeLinks = false)
    {
      TfvcItem tfvcItem1 = new TfvcItem();
      tfvcItem1.Path = serverItem;
      tfvcItem1.IsPendingChange = version is ShelvesetVersionSpec;
      tfvcItem1.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcItemsLocationId, (object) new
      {
        path = serverItem,
        versionType = TfvcVersionType.Changeset,
        version = version.ToChangeset(requestContext)
      });
      TfvcItem tfvcItem2 = tfvcItem1;
      tfvcItem2.Links = includeLinks ? tfvcItem2.GetItemsReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
      return tfvcItem2;
    }

    public static void ParsePropertyValues(
      this TfvcItem tfvcItem,
      IEnumerable<PropertyValue> propertyValuesCollection)
    {
      if (propertyValuesCollection == null)
        return;
      foreach (PropertyValue propertyValues in propertyValuesCollection)
      {
        if (string.Equals(propertyValues.PropertyName, "Microsoft.TeamFoundation.VersionControl.SymbolicLink", StringComparison.Ordinal) && propertyValues.Value != null && string.Equals(propertyValues.Value.ToString(), "true", StringComparison.OrdinalIgnoreCase))
          tfvcItem.IsSymbolicLink = true;
      }
    }

    public static TfvcChange CreateWebApiChangeModel(
      PendingChange pendingChange,
      ShelvesetVersionSpec shelvesetVersionSpec,
      bool shallowItem = false,
      UrlHelper url = null,
      IVssRequestContext requestContext = null,
      bool includeLinks = false)
    {
      TfvcChange webApiChangeModel = new TfvcChange(TfsModelExtensions.CreateWebApiItem(pendingChange, shelvesetVersionSpec, shallowItem, url, requestContext, includeLinks), (VersionControlChangeType) pendingChange.ChangeType);
      if (!shallowItem)
        webApiChangeModel.PendingVersion = pendingChange.Version;
      if (!string.IsNullOrEmpty(pendingChange.SourceServerItem) && !string.Equals(pendingChange.ServerItem, pendingChange.SourceServerItem))
      {
        webApiChangeModel.SourceServerItem = pendingChange.SourceServerItem;
        if (pendingChange.Version == 0 && !shallowItem)
          webApiChangeModel.PendingVersion = pendingChange.SourceVersionFrom;
      }
      return webApiChangeModel;
    }

    public static VersionSpec ParseVersionSpecString(string version, VersionSpec defaultSpec)
    {
      VersionSpec versionSpecString = (VersionSpec) null;
      if (!string.IsNullOrEmpty(version))
      {
        char upperInvariant = char.ToUpperInvariant(version[0]);
        if ((int) upperInvariant == (int) TipVersionSpec.Identifier)
        {
          if (version.Length > 1)
          {
            TipVersionSpec tipVersionSpec = new TipVersionSpec(version.Substring(1));
            versionSpecString = !(tipVersionSpec.Version is LatestVersionSpec) ? (VersionSpec) tipVersionSpec : (VersionSpec) new LatestVersionSpec();
          }
          else
            versionSpecString = (VersionSpec) new LatestVersionSpec();
        }
        else if ((int) upperInvariant == (int) ShelvesetVersionSpec.Identifier)
        {
          if (version.Length > 1)
          {
            string shelvesetName = (string) null;
            string shelvesetOwner = (string) null;
            string[] strArray = version.Substring(1).Split(new char[1]
            {
              ';'
            }, StringSplitOptions.None);
            if (strArray.Length != 0)
              shelvesetName = strArray[0];
            if (!string.IsNullOrEmpty(shelvesetName))
            {
              if (strArray.Length > 1)
                shelvesetOwner = strArray[1];
              if (!string.IsNullOrEmpty(shelvesetOwner))
                versionSpecString = (VersionSpec) new ShelvesetVersionSpec(shelvesetName, shelvesetOwner);
            }
          }
        }
        else if ((int) upperInvariant == (int) PreviousVersionSpec.Identifier)
          versionSpecString = version.Length <= 1 ? (VersionSpec) new PreviousVersionSpec((VersionSpec) new LatestVersionSpec()) : (VersionSpec) new PreviousVersionSpec(version.Substring(1));
        else if ((int) upperInvariant == (int) ChangeVersionSpec.Identifier)
        {
          int result;
          if (version.Length > 1 && int.TryParse(version.Substring(1), out result))
            versionSpecString = (VersionSpec) new ChangeVersionSpec(result);
        }
        else if ((int) upperInvariant == (int) MergeSourceVersionSpec.Identifier)
        {
          if (version.Length > 1)
          {
            string s = version.Substring(1);
            bool useRenameSource = false;
            if (s.Length > 1 && (s[0] == 'R' || s[0] == 'r'))
            {
              useRenameSource = true;
              s = s.Substring(1);
            }
            int result;
            if (s.Length > 0 && int.TryParse(s, out result))
              versionSpecString = (VersionSpec) new MergeSourceVersionSpec(result, useRenameSource);
          }
        }
        else
        {
          int result;
          if (int.TryParse(version, out result))
          {
            if (result > 0)
              versionSpecString = (VersionSpec) new ChangesetVersionSpec(result);
          }
          else
            versionSpecString = VersionSpec.ParseSingleSpec(version, "*");
        }
      }
      if (versionSpecString == null)
        versionSpecString = defaultSpec;
      return versionSpecString;
    }

    public static IEnumerable<TfvcChange> GetShelvedChanges(
      IVssRequestContext requestContext,
      string itemPath,
      RecursionType recursion,
      int deletionId,
      ShelvesetVersionSpec versionSpec,
      int top,
      int skip,
      string[] propertyFilters,
      out bool allChangesIncluded,
      bool shallowItems = false,
      UrlHelper url = null)
    {
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
          int num = 0;
          foreach (PendingSet current in foundationDataReader.CurrentEnumerable<PendingSet>())
          {
            foreach (PendingChange pendingChange in current.PendingChanges)
            {
              if (num < skip)
              {
                ++num;
              }
              else
              {
                if (shelvedChanges.Count == top)
                {
                  allChangesIncluded = false;
                  break;
                }
                shelvedChanges.Add(TfsModelExtensions.CreateWebApiChangeModel(pendingChange, versionSpec, shallowItems, url, requestContext));
              }
            }
            if (!allChangesIncluded)
              break;
          }
          return (IEnumerable<TfvcChange>) shelvedChanges;
        }
      }
      finally
      {
        requestContext.TraceLeave(513120, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetShelvedChanges));
      }
    }

    public static IEnumerable<AssociatedWorkItem> ToWebApiWorkItems(
      this CheckinWorkItemInfo[] workItems)
    {
      return workItems != null ? ((IEnumerable<CheckinWorkItemInfo>) workItems).Where<CheckinWorkItemInfo>((Func<CheckinWorkItemInfo, bool>) (x => x != null)).Select<CheckinWorkItemInfo, AssociatedWorkItem>((Func<CheckinWorkItemInfo, AssociatedWorkItem>) (y => y.ToWebApiWorkItem())) : (IEnumerable<AssociatedWorkItem>) new List<AssociatedWorkItem>();
    }

    public static IEnumerable<AssociatedWorkItem> ToWebApiWorkItemsWithUrl(
      this CheckinWorkItemInfo[] workItems,
      IVssRequestContext TfsRequestContext)
    {
      ILocationService locationService = (ILocationService) null;
      if (TfsRequestContext != null)
        locationService = TfsRequestContext.GetService<ILocationService>();
      return workItems != null ? ((IEnumerable<CheckinWorkItemInfo>) workItems).Where<CheckinWorkItemInfo>((Func<CheckinWorkItemInfo, bool>) (x => x != null)).Select<CheckinWorkItemInfo, AssociatedWorkItem>((Func<CheckinWorkItemInfo, AssociatedWorkItem>) (y => y.ToWebApiWorkItem(locationService, TfsRequestContext))) : (IEnumerable<AssociatedWorkItem>) new List<AssociatedWorkItem>();
    }

    public static AssociatedWorkItem ToWebApiWorkItem(this CheckinWorkItemInfo workItem)
    {
      if (workItem == null)
        return (AssociatedWorkItem) null;
      return new AssociatedWorkItem()
      {
        WebUrl = workItem.WorkItemUrl,
        Id = workItem.Id,
        Title = workItem.Title,
        WorkItemType = workItem.Type,
        State = workItem.State,
        AssignedTo = workItem.AssignedTo
      };
    }

    public static AssociatedWorkItem ToWebApiWorkItem(
      this CheckinWorkItemInfo workItem,
      ILocationService locationService,
      IVssRequestContext TfsRequestContext)
    {
      AssociatedWorkItem webApiWorkItem = (AssociatedWorkItem) null;
      if (workItem != null)
      {
        webApiWorkItem = workItem.ToWebApiWorkItem();
        if (locationService != null && TfsRequestContext != null)
          webApiWorkItem.Url = locationService.GetResourceUri(TfsRequestContext, "wit", WitConstants.WorkItemTrackingLocationIds.WorkItems, (object) new
          {
            id = workItem.Id
          }).AbsoluteUri;
      }
      return webApiWorkItem;
    }

    public static string ParseOwnerId(IVssRequestContext requestContext, string ownerId)
    {
      string ownerId1 = ownerId;
      Guid result;
      if (Guid.TryParse(ownerId, out result))
      {
        ownerId1 = IdentityHelper.GetUniqueName(TfvcIdentityHelper.FindIdentity(requestContext, result, false));
        if (string.IsNullOrEmpty(ownerId1))
          ownerId1 = ownerId;
      }
      return ownerId1;
    }

    public static TfvcBranch ToWebApiTfvcBranch(
      IVssRequestContext requestContext,
      BranchObject obj,
      bool includeParent,
      bool includeDeleted)
    {
      TfvcBranch webApiTfvcBranch = new TfvcBranch(TfsModelExtensions.ToWebApiTfvcBranchRef(requestContext, obj));
      if (obj.Properties.ParentBranch != null & includeParent)
        webApiTfvcBranch.Parent = new TfvcShallowBranchRef(obj.Properties.ParentBranch.Item);
      if (obj.RelatedBranches != null)
      {
        webApiTfvcBranch.RelatedBranches = new List<TfvcShallowBranchRef>();
        foreach (ItemIdentifier relatedBranch in obj.RelatedBranches)
        {
          if (includeDeleted || relatedBranch.DeletionId == 0)
            webApiTfvcBranch.RelatedBranches.Add(new TfvcShallowBranchRef(relatedBranch.Item));
        }
      }
      if (obj.Properties.BranchMappings != null)
      {
        webApiTfvcBranch.Mappings = new List<TfvcBranchMapping>();
        foreach (Mapping branchMapping in obj.Properties.BranchMappings)
          webApiTfvcBranch.Mappings.Add(new TfvcBranchMapping()
          {
            ServerItem = branchMapping.ServerItem,
            Type = branchMapping.Type.ToString() == "Map" ? "Active" : "Cloaked",
            Depth = ((VersionControlRecursionType) branchMapping.Depth).ToString()
          });
      }
      return webApiTfvcBranch;
    }

    public static TfvcBranchRef ToWebApiTfvcBranchRef(
      IVssRequestContext requestContext,
      BranchObject obj)
    {
      TfvcBranchRef apiTfvcBranchRef = new TfvcBranchRef();
      apiTfvcBranchRef.Path = obj.Properties.RootItem.Item;
      Guid ownerId = obj.Properties.OwnerId;
      apiTfvcBranchRef.Owner = new IdentityRef()
      {
        Id = ownerId.ToString(),
        DisplayName = obj.Properties.OwnerDisplayName,
        UniqueName = TfsModelExtensions.ParseOwnerId(requestContext, obj.Properties.OwnerUniqueName)
      };
      if (ownerId != Guid.Empty)
      {
        apiTfvcBranchRef.Owner.Url = IdentityHelper.GetIdentityResourceUriString(requestContext, ownerId);
        apiTfvcBranchRef.Owner.ImageUrl = IdentityHelper.GetImageResourceUrl(requestContext, ownerId);
      }
      apiTfvcBranchRef.Description = obj.Properties.Description;
      apiTfvcBranchRef.CreatedDate = obj.DateCreated;
      apiTfvcBranchRef.IsDeleted = obj.Properties.RootItem.DeletionId != 0;
      return apiTfvcBranchRef;
    }

    public static TfvcChangeset ToWebApiChangeset(
      this Changeset changeset,
      IVssRequestContext requestContext,
      IEnumerable<TfvcChange> changes,
      bool allChangesIncluded,
      bool includeDetails,
      int commentLength)
    {
      TfvcChangeset webApiChangeset = changeset.ToWebApiChangeset(requestContext, includeDetails, commentLength);
      webApiChangeset.Changes = changes;
      webApiChangeset.HasMoreChanges = allChangesIncluded;
      return webApiChangeset;
    }

    public static TfvcChangeset ToWebApiChangeset(
      this Changeset changeset,
      IVssRequestContext requestContext,
      bool includeDetails,
      int maxCommentLength)
    {
      TfvcChangeset webApiChangeset = new TfvcChangeset(changeset.ToWebApiChangeset(requestContext, maxCommentLength));
      if (includeDetails)
      {
        webApiChangeset.PolicyOverride = changeset.PolicyOverride.ToWebApiPolicyOverrideInfo();
        webApiChangeset.CheckinNotes = changeset.CheckinNote.ToWebApiCheckInNotes();
      }
      return webApiChangeset;
    }

    public static TfvcChangesetRef ToWebApiChangeset(
      this Changeset changeset,
      IVssRequestContext requestContext,
      int maxCommentLength)
    {
      TfvcChangesetRef webApiChangeset = new TfvcChangesetRef()
      {
        ChangesetId = changeset.ChangesetId,
        Author = new IdentityRef()
        {
          Id = changeset.ownerId.ToString(),
          DisplayName = changeset.OwnerDisplayName,
          UniqueName = TfsModelExtensions.ParseOwnerId(requestContext, changeset.Owner)
        },
        CheckedInBy = new IdentityRef()
        {
          Id = changeset.committerId.ToString(),
          DisplayName = changeset.CommitterDisplayName,
          UniqueName = TfsModelExtensions.ParseOwnerId(requestContext, changeset.Committer)
        },
        CreatedDate = changeset.CreationDate
      };
      if (changeset.ownerId != Guid.Empty)
      {
        webApiChangeset.Author.Url = IdentityHelper.GetIdentityResourceUriString(requestContext, changeset.ownerId);
        webApiChangeset.Author.ImageUrl = IdentityHelper.GetImageResourceUrl(requestContext, changeset.ownerId);
      }
      if (changeset.committerId != Guid.Empty)
      {
        webApiChangeset.CheckedInBy.Url = IdentityHelper.GetIdentityResourceUriString(requestContext, changeset.committerId);
        webApiChangeset.CheckedInBy.ImageUrl = IdentityHelper.GetImageResourceUrl(requestContext, changeset.committerId);
      }
      if (!string.IsNullOrWhiteSpace(changeset.Comment))
      {
        if (changeset.Comment.Length > maxCommentLength)
        {
          webApiChangeset.Comment = StringUtil.Truncate(changeset.Comment, maxCommentLength, false);
          webApiChangeset.CommentTruncated = true;
        }
        else
          webApiChangeset.Comment = changeset.Comment;
      }
      return webApiChangeset;
    }

    public static TfvcChange ToWebApiChangeModel(this Change change)
    {
      TfvcChange webApiChangeModel = new TfvcChange(change.Item.ToWebApiChangesetChangeItem(), (VersionControlChangeType) change.ChangeType);
      MergeSource mergeSource = (MergeSource) null;
      if ((change.ChangeType & ChangeType.Rename) != ChangeType.None)
      {
        if (change.MergeSources != null)
          mergeSource = change.MergeSources.FirstOrDefault<MergeSource>((Func<MergeSource, bool>) (ms => ms.IsRename));
      }
      else if ((change.ChangeType & ChangeType.Branch) != ChangeType.None && change.MergeSources != null)
        mergeSource = change.MergeSources.FirstOrDefault<MergeSource>();
      if (mergeSource != null)
        webApiChangeModel.SourceServerItem = mergeSource.ServerItem;
      if (change.MergeSources != null)
        webApiChangeModel.MergeSources = change.MergeSources.ToWebApiMergeSources();
      return webApiChangeModel;
    }

    public static IEnumerable<TfvcMergeSource> ToWebApiMergeSources(
      this List<MergeSource> mergeSources)
    {
      return mergeSources != null ? mergeSources.Where<MergeSource>((Func<MergeSource, bool>) (x => x != null)).Select<MergeSource, TfvcMergeSource>((Func<MergeSource, TfvcMergeSource>) (y => y.ToWebApiMergeSource())) : (IEnumerable<TfvcMergeSource>) null;
    }

    public static TfvcMergeSource ToWebApiMergeSource(this MergeSource mergeSource) => mergeSource != null ? new TfvcMergeSource(mergeSource.ServerItem, mergeSource.VersionFrom, mergeSource.VersionTo, mergeSource.IsRename) : (TfvcMergeSource) null;

    public static TfvcItem ToWebApiChangesetChangeItem(this Item item)
    {
      TfvcItem tfvcItem = new TfvcItem();
      tfvcItem.ChangesetVersion = item.ChangesetId;
      tfvcItem.Path = item.ServerItem;
      tfvcItem.IsFolder = item.IsFolder;
      TfvcItem changesetChangeItem = tfvcItem;
      if (!changesetChangeItem.IsFolder && item.HashValue != null)
      {
        changesetChangeItem.HashValue = Convert.ToBase64String(item.HashValue);
        changesetChangeItem.Size = item.FileLength;
      }
      return changesetChangeItem;
    }

    public static TfvcPolicyFailureInfo ToWebApiPolicyFailureInfo(
      this PolicyFailureInfo policyFailure)
    {
      return new TfvcPolicyFailureInfo()
      {
        Message = policyFailure.Message,
        PolicyName = policyFailure.PolicyName
      };
    }

    public static TfvcPolicyOverrideInfo ToWebApiPolicyOverrideInfo(
      this PolicyOverrideInfo policyOverride)
    {
      string comment = (string) null;
      IEnumerable<TfvcPolicyFailureInfo> policyFailures = (IEnumerable<TfvcPolicyFailureInfo>) null;
      if (policyOverride != null)
      {
        comment = policyOverride.Comment;
        if (policyOverride.PolicyFailures != null)
          policyFailures = ((IEnumerable<PolicyFailureInfo>) policyOverride.PolicyFailures).Select<PolicyFailureInfo, TfvcPolicyFailureInfo>((Func<PolicyFailureInfo, TfvcPolicyFailureInfo>) (pf => pf.ToWebApiPolicyFailureInfo()));
      }
      return new TfvcPolicyOverrideInfo(comment, policyFailures);
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote ToWebApiCheckinNote(
      this CheckinNoteFieldValue checkinNoteFieldValue)
    {
      return new Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote()
      {
        Name = checkinNoteFieldValue.Name,
        Value = checkinNoteFieldValue.Value
      };
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote[] ToWebApiCheckInNotes(
      this Microsoft.TeamFoundation.VersionControl.Server.CheckinNote obj)
    {
      return obj == null || obj.Values == null ? Array.Empty<Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote>() : Array.ConvertAll<CheckinNoteFieldValue, Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote>(obj.Values, (Converter<CheckinNoteFieldValue, Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote>) (x => x.ToWebApiCheckinNote()));
    }

    public static TfvcLabelRef ToWebApiShallowLabel(
      this VersionControlLabel label,
      IVssRequestContext requestContext)
    {
      string comment = label.Comment;
      TfvcLabelRef webApiShallowLabel = new TfvcLabelRef()
      {
        Id = label.LabelId,
        Name = label.Name,
        LabelScope = label.Scope,
        ModifiedDate = label.LastModifiedDate,
        Owner = new IdentityRef()
        {
          Id = label.ownerId.ToString(),
          DisplayName = label.OwnerDisplayName,
          UniqueName = TfsModelExtensions.ParseOwnerId(requestContext, label.OwnerUniqueName)
        },
        Description = comment
      };
      if (label.ownerId != Guid.Empty)
      {
        webApiShallowLabel.Owner.Url = IdentityHelper.GetIdentityResourceUriString(requestContext, label.ownerId);
        webApiShallowLabel.Owner.ImageUrl = IdentityHelper.GetImageResourceUrl(requestContext, label.ownerId);
      }
      return webApiShallowLabel;
    }

    public static TfvcLabel ToWebApiLabel(
      this VersionControlLabel label,
      IVssRequestContext requestContext,
      IEnumerable<TfvcItem> items)
    {
      return new TfvcLabel(label.ToWebApiShallowLabel(requestContext))
      {
        Items = items
      };
    }

    public static TfvcStatistics ToWebApiTfvcFileStats(this TfvcFileStats tfvcStats) => new TfvcStatistics()
    {
      ChangesetId = tfvcStats.ChangesetId,
      FileCountTotal = tfvcStats.FileCountTotal
    };

    public static string ParameterizeUnsafeUrlPaths(string url)
    {
      if (url.IndexOf("+", StringComparison.Ordinal) > 0)
        url = url.Replace("+", "%2b");
      if (url.IndexOf("/web.config", StringComparison.OrdinalIgnoreCase) > 0 || url.IndexOf(".cshtml", StringComparison.OrdinalIgnoreCase) > 0 || url.IndexOf("%2b", StringComparison.OrdinalIgnoreCase) > 0)
      {
        url = url.Replace('?', '&').Replace("/items/$/", "/items?path=$/");
        url = url.Replace("/items/$/", "/items?path=$/");
      }
      return url;
    }
  }
}
