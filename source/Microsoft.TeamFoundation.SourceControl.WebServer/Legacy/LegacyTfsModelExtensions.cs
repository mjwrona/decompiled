// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Legacy.LegacyTfsModelExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Legacy
{
  public static class LegacyTfsModelExtensions
  {
    public const string c_symbolicLinkPropertyName = "Microsoft.TeamFoundation.VersionControl.SymbolicLink";

    public static TfsItem ToWebApiItem(this Item item)
    {
      TfsItem tfsItem1 = new TfsItem();
      tfsItem1.Id = item.ItemId;
      tfsItem1.ServerItem = item.ServerItem;
      tfsItem1.IsFolder = item.ItemType == ItemType.Folder;
      tfsItem1.ChangesetVersion = item.ChangesetId;
      tfsItem1.DeletionId = item.DeletionId;
      tfsItem1.Encoding = item.Encoding;
      tfsItem1.FileId = item.GetFileId(0);
      tfsItem1.ChangeDate = item.CheckinDate;
      tfsItem1.IsBranch = item.IsBranch;
      tfsItem1.IsPendingChange = false;
      TfsItem tfsItem2 = tfsItem1;
      if (item.ChangesetId > -1)
      {
        VersionSpec versionSpec = (VersionSpec) new ChangesetVersionSpec(item.ChangesetId);
        tfsItem2.VersionString = versionSpec.ToString();
      }
      tfsItem2.ParsePropertyValues((IEnumerable<PropertyValue>) item.PropertyValues);
      return tfsItem2;
    }

    public static TfsItem CreateWebApiItem(PendingChange change, ShelvesetVersionSpec shelvesetSpec)
    {
      TfsItem tfsItem = new TfsItem();
      tfsItem.ServerItem = change.ServerItem;
      tfsItem.IsFolder = change.ItemType == ItemType.Folder;
      tfsItem.DeletionId = change.DeletionId;
      tfsItem.Encoding = change.Encoding;
      tfsItem.IsPendingChange = true;
      tfsItem.FileId = change.GetFileId(0);
      tfsItem.VersionString = shelvesetSpec == null ? (string) null : shelvesetSpec.ToString();
      tfsItem.ChangesetVersion = change.Version;
      tfsItem.IsBranch = (change.ChangeType & ChangeType.Branch) != 0;
      tfsItem.ParsePropertyValues((IEnumerable<PropertyValue>) change.PropertyValues);
      return tfsItem;
    }

    public static TfsItem ToWebApiItem(
      this Item mergedItem,
      string mergeSourceServerItem,
      VersionSpec version)
    {
      TfsItem webApiItem = mergedItem.ToWebApiItem();
      webApiItem.ServerItem = mergeSourceServerItem;
      webApiItem.VersionString = version.ToString();
      return webApiItem;
    }

    public static TfsItem CreateWebApiItem(string serverItem, VersionSpec version)
    {
      TfsItem webApiItem = new TfsItem();
      webApiItem.ServerItem = serverItem;
      webApiItem.IsPendingChange = version is ShelvesetVersionSpec;
      webApiItem.VersionString = version.ToString();
      return webApiItem;
    }

    public static void ParsePropertyValues(
      this TfsItem tfsItem,
      IEnumerable<PropertyValue> propertyValuesCollection)
    {
      if (propertyValuesCollection == null)
        return;
      foreach (PropertyValue propertyValues in propertyValuesCollection)
      {
        if (string.Equals(propertyValues.PropertyName, "Microsoft.TeamFoundation.VersionControl.SymbolicLink", StringComparison.Ordinal) && propertyValues.Value != null && string.Equals(propertyValues.Value.ToString(), "true", StringComparison.OrdinalIgnoreCase))
          tfsItem.IsSymbolicLink = true;
      }
    }

    public static TfsChange ToWebApiChangeModel(this Microsoft.TeamFoundation.VersionControl.Server.Change change)
    {
      TfsChange webApiChangeModel = new TfsChange(change.Item.ToWebApiItem(), (VersionControlChangeType) change.ChangeType);
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
      return webApiChangeModel;
    }

    public static TfsChange CreateWebApiChangeModel(
      PendingChange pendingChange,
      ShelvesetVersionSpec shelvesetVersionSpec)
    {
      TfsChange webApiChangeModel = new TfsChange(LegacyTfsModelExtensions.CreateWebApiItem(pendingChange, shelvesetVersionSpec), (VersionControlChangeType) pendingChange.ChangeType);
      webApiChangeModel.PendingVersion = pendingChange.Version;
      if (!string.IsNullOrEmpty(pendingChange.SourceServerItem) && !string.Equals(pendingChange.ServerItem, pendingChange.SourceServerItem))
      {
        webApiChangeModel.SourceServerItem = pendingChange.SourceServerItem;
        if (pendingChange.Version == 0)
          webApiChangeModel.PendingVersion = pendingChange.SourceVersionFrom;
      }
      return webApiChangeModel;
    }

    public static TfsPolicyFailureInfo ToWebApiPolicyFailureInfo(
      this PolicyFailureInfo policyFailure)
    {
      return new TfsPolicyFailureInfo()
      {
        Message = policyFailure.Message,
        PolicyName = policyFailure.PolicyName
      };
    }

    public static TfsPolicyOverrideInfo ToWebApiPolicyOverrideInfo(
      this PolicyOverrideInfo policyOverride)
    {
      string comment = (string) null;
      IEnumerable<TfsPolicyFailureInfo> policyFailures = (IEnumerable<TfsPolicyFailureInfo>) null;
      if (policyOverride != null)
      {
        comment = policyOverride.Comment;
        if (policyOverride.PolicyFailures != null)
          policyFailures = ((IEnumerable<PolicyFailureInfo>) policyOverride.PolicyFailures).Select<PolicyFailureInfo, TfsPolicyFailureInfo>((Func<PolicyFailureInfo, TfsPolicyFailureInfo>) (pf => pf.ToWebApiPolicyFailureInfo()));
      }
      return new TfsPolicyOverrideInfo(comment, policyFailures);
    }

    public static CheckinNoteFieldValue ToWebApiCheckinNoteFieldValue(this Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote model) => new CheckinNoteFieldValue()
    {
      Name = model.Name,
      Value = model.Value
    };

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote ToWebApiCheckinNote(
      this CheckinNoteFieldValue checkinNoteFieldValue)
    {
      return new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote()
      {
        Name = checkinNoteFieldValue.Name,
        Value = checkinNoteFieldValue.Value
      };
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote[] ToWebApiCheckInNotes(
      this Microsoft.TeamFoundation.VersionControl.Server.CheckinNote obj)
    {
      return obj == null || obj.Values == null ? (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote[]) null : Array.ConvertAll<CheckinNoteFieldValue, Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote>(obj.Values, (Converter<CheckinNoteFieldValue, Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote>) (x => x.ToWebApiCheckinNote()));
    }

    public static TfsChangeList ToTfsChangeList(this Changeset changeset, bool includeDetails)
    {
      TfsChangeList tfsChangeList1 = new TfsChangeList();
      tfsChangeList1.ChangesetId = changeset.ChangesetId;
      tfsChangeList1.Owner = changeset.Owner;
      tfsChangeList1.OwnerId = changeset.ownerId;
      tfsChangeList1.OwnerDisplayName = changeset.OwnerDisplayName;
      tfsChangeList1.SortDate = changeset.CreationDate;
      tfsChangeList1.CreationDate = changeset.CreationDate;
      tfsChangeList1.Version = new ChangesetVersionSpec(changeset.ChangesetId).ToString();
      TfsChangeList tfsChangeList2 = tfsChangeList1;
      if (includeDetails)
      {
        tfsChangeList2.Comment = changeset.Comment;
        tfsChangeList2.PolicyOverride = changeset.PolicyOverride.ToWebApiPolicyOverrideInfo();
        tfsChangeList2.Notes = changeset.CheckinNote.ToWebApiCheckInNotes();
      }
      else if (changeset.Comment != null)
      {
        if (changeset.Comment.Length > 100)
        {
          tfsChangeList2.Comment = StringUtil.Truncate(changeset.Comment, 100, false);
          tfsChangeList2.CommentTruncated = true;
        }
        else
          tfsChangeList2.Comment = changeset.Comment;
      }
      return tfsChangeList2;
    }

    public static TfsChangeList ToWebApiChangeList(
      this Changeset changeset,
      IEnumerable<TfsChange> changes,
      bool allChangesIncluded,
      bool includeDetails)
    {
      TfsChangeList tfsChangeList = changeset.ToTfsChangeList(includeDetails);
      tfsChangeList.Changes = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) changes;
      tfsChangeList.AllChangesIncluded = allChangesIncluded;
      if (changes != null && changes.Any<TfsChange>())
        tfsChangeList.ChangeCounts = ChangeListHelpers.ComputeChangeCounts((IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) changes);
      return tfsChangeList;
    }

    public static TfsChangeList ToWebApiChangeList(
      this Shelveset shelveset,
      IEnumerable<TfsChange> changes,
      bool allChangesIncluded)
    {
      TfsChangeList tfsChangeList = new TfsChangeList(true);
      tfsChangeList.Comment = shelveset.Comment;
      tfsChangeList.PolicyOverride = new TfsPolicyOverrideInfo(shelveset.PolicyOverrideComment, (IEnumerable<TfsPolicyFailureInfo>) null);
      tfsChangeList.Notes = shelveset.CheckinNote.ToWebApiCheckInNotes();
      tfsChangeList.ShelvesetName = shelveset.Name;
      tfsChangeList.Owner = shelveset.Owner;
      tfsChangeList.OwnerId = shelveset.ownerId;
      tfsChangeList.OwnerDisplayName = shelveset.OwnerDisplayName;
      tfsChangeList.SortDate = shelveset.CreationDate;
      tfsChangeList.CreationDate = shelveset.CreationDate;
      tfsChangeList.Version = new ShelvesetVersionSpec(shelveset.Name, shelveset.Owner).ToString();
      tfsChangeList.AllChangesIncluded = allChangesIncluded;
      tfsChangeList.Changes = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) changes;
      TfsChangeList webApiChangeList = tfsChangeList;
      if (changes != null && changes.Any<TfsChange>())
        webApiChangeList.ChangeCounts = ChangeListHelpers.ComputeChangeCounts((IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) changes);
      return webApiChangeList;
    }

    public static TfsIdentityReference ToWebApiIdentityReference(this Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      TfsIdentityReference identityReference = new TfsIdentityReference();
      identityReference.DisplayName = identity.DisplayName;
      identityReference.TeamFoundationId = identity.Id;
      identityReference.AccountName = identity.GetProperty<string>("Account", (string) null);
      return identityReference;
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
  }
}
