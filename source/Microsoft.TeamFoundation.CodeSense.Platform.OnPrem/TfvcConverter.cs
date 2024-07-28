// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.TfvcConverter
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B5DEEFA-3C5E-4BFB-92E2-3ADDA47952C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Platform.OnPrem
{
  public class TfvcConverter : ITfvcConverter
  {
    public Item GetItem(
      string itemPath,
      TfvcVersionDescriptor versionDescriptor,
      TfvcItem tfvcItem)
    {
      return new Item()
      {
        ChangesetId = tfvcItem.ChangesetVersion,
        CheckinDate = tfvcItem.ChangeDate,
        ItemId = tfvcItem.Id,
        DownloadUrl = tfvcItem.Url,
        IsBranch = tfvcItem.IsBranch,
        SequenceId = tfvcItem.ChangesetVersion,
        ServerItem = itemPath,
        DeletionId = tfvcItem.DeletionId
      };
    }

    public TfvcChange GetTfvcChange(Change change)
    {
      if (change == null)
        return (TfvcChange) null;
      TfvcChange tfvcChange = new TfvcChange();
      tfvcChange.ChangeType = (VersionControlChangeType) change.ChangeType;
      tfvcChange.Item = this.GetItem(change.Item);
      if (change.MergeSources != null)
        tfvcChange.MergeSources = (IEnumerable<TfvcMergeSource>) change.MergeSources.ConvertAll<TfvcMergeSource>((Converter<MergeSource, TfvcMergeSource>) (x => new TfvcMergeSource(x.ServerItem, x.VersionFrom, x.VersionTo, x.IsRename)));
      return tfvcChange;
    }

    public TfvcChangeset GetTfvcChangeset(Changeset changeset)
    {
      TfvcChangeset tfvcChangeset = new TfvcChangeset();
      tfvcChangeset.Author = this.GetAuthor(changeset);
      tfvcChangeset.ChangesetId = changeset.ChangesetId;
      tfvcChangeset.CheckedInBy = this.GetCheckedInBy(changeset);
      tfvcChangeset.CheckinNotes = this.GetCheckinNote(changeset.CheckinNote);
      tfvcChangeset.Comment = changeset.Comment;
      tfvcChangeset.CreatedDate = changeset.CreationDate;
      tfvcChangeset.Changes = this.GetTfvcChange(changeset.Changes);
      return tfvcChangeset;
    }

    public TfvcItemsCollection GetTfvcItem(Item item)
    {
      TfvcItemsCollection tfvcItem = new TfvcItemsCollection();
      tfvcItem.Add(this.GetItem(item));
      return tfvcItem;
    }

    private Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote[] GetCheckinNote(
      Microsoft.TeamFoundation.VersionControl.Server.CheckinNote checkinNote)
    {
      List<Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote> checkinNoteList = new List<Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote>();
      if (checkinNote != null && checkinNote.Values != null)
      {
        foreach (CheckinNoteFieldValue checkinNoteFieldValue in checkinNote.Values)
        {
          Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote checkinNote1 = new Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote()
          {
            Name = checkinNoteFieldValue.Name,
            Value = checkinNoteFieldValue.Value
          };
          checkinNoteList.Add(checkinNote1);
        }
      }
      return checkinNoteList.ToArray();
    }

    private IdentityRef GetCheckedInBy(Changeset changeset)
    {
      if (changeset == null || string.IsNullOrEmpty(changeset.Committer))
        return new IdentityRef();
      return new IdentityRef()
      {
        Id = changeset.Committer,
        UniqueName = changeset.Committer,
        DisplayName = changeset.CommitterDisplayName
      };
    }

    private IEnumerable<TfvcChange> GetTfvcChange(StreamingCollection<Change> changes)
    {
      List<TfvcChange> tfvcChange1 = new List<TfvcChange>();
      if (changes != null)
      {
        foreach (Change change in changes)
        {
          TfvcChange tfvcChange2 = new TfvcChange();
          tfvcChange2.ChangeType = (VersionControlChangeType) change.ChangeType;
          tfvcChange2.Item = this.GetItem(change.Item);
          TfvcChange tfvcChange3 = tfvcChange2;
          tfvcChange1.Add(tfvcChange3);
        }
      }
      return (IEnumerable<TfvcChange>) tfvcChange1;
    }

    private IdentityRef GetAuthor(Changeset changeset)
    {
      if (changeset == null || string.IsNullOrEmpty(changeset.Owner))
        return new IdentityRef();
      return new IdentityRef()
      {
        Id = changeset.Owner,
        UniqueName = changeset.Owner,
        DisplayName = changeset.OwnerDisplayName
      };
    }

    private TfvcItem GetItem(Item item)
    {
      TfvcItem tfvcItem = new TfvcItem();
      if (item != null)
      {
        tfvcItem.ChangeDate = item.CheckinDate;
        tfvcItem.ChangesetVersion = item.ChangesetId;
        tfvcItem.DeletionId = item.DeletionId;
        tfvcItem.Id = item.ItemId;
        tfvcItem.IsBranch = item.IsBranch;
        tfvcItem.IsFolder = item.IsFolder;
        tfvcItem.Path = item.ServerItem;
        tfvcItem.Url = item.DownloadUrl;
        tfvcItem.Encoding = item.Encoding;
      }
      return tfvcItem;
    }
  }
}
