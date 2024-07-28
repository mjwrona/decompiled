// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcToGitImportClient
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class TfvcToGitImportClient : ITfvcToGitImportClient
  {
    private IVssRequestContext m_userRequestContext;
    private TeamFoundationVersionControlService m_versionControlService;

    public TfvcToGitImportClient(IVssRequestContext userUserRequestContext)
    {
      this.m_userRequestContext = userUserRequestContext.IsUserContext ? userUserRequestContext : throw new ArgumentException("request context has to be a user request context");
      this.m_versionControlService = this.m_userRequestContext.GetService<TeamFoundationVersionControlService>();
    }

    public IEnumerable<TfvcBranch> GetTfvcBranches(string scopePath)
    {
      TfvcBranchesCollection tfvcBranches = new TfvcBranchesCollection();
      using (TeamFoundationDataReader foundationDataReader = this.m_versionControlService.QueryBranchObjects(this.m_userRequestContext, (ItemIdentifier) null, RecursionType.Full))
      {
        foreach (BranchObject branchObject in foundationDataReader.Current<StreamingCollection<BranchObject>>())
        {
          if (branchObject.Properties.RootItem.DeletionId == 0)
          {
            TfvcBranch webApiTfvcBranch = TfsModelExtensions.ToWebApiTfvcBranch(this.m_userRequestContext, branchObject, false, false);
            if (webApiTfvcBranch.Path.ToLower().StartsWith(scopePath.ToLower()))
              tfvcBranches.Add(webApiTfvcBranch);
          }
        }
      }
      return (IEnumerable<TfvcBranch>) tfvcBranches;
    }

    public TeamFoundationDataReader QueryTfvcItems(
      string scopePath,
      TfvcVersionDescriptor versionDescriptor)
    {
      return this.m_versionControlService.QueryItems(this.m_userRequestContext, (string) null, (string) null, new ItemSpec[1]
      {
        new ItemSpec(scopePath, RecursionType.Full)
      }, TfvcVersionSpecUtility.GetVersionSpec(this.m_userRequestContext, versionDescriptor), DeletedState.NonDeleted, ItemType.Any, false, 12, new string[1]
      {
        "Microsoft.TeamFoundation.VersionControl.SymbolicLink"
      }, (string[]) null);
    }

    public IEnumerable<int> QueryTfvcChangesetIds(string itemPath, DateTime fromDate)
    {
      List<int> source = new List<int>();
      List<TfvcChangesetRef> list = this.QueryTfvcChangesetRefs(itemPath, 0, 250).ToList<TfvcChangesetRef>();
      source.AddRange(list.Select<TfvcChangesetRef, int>((Func<TfvcChangesetRef, int>) (x => x.ChangesetId)));
      bool flag = list.Min<TfvcChangesetRef, DateTime>((Func<TfvcChangesetRef, DateTime>) (x => x.CreatedDate)) < fromDate;
      while (list.Count == 250 && !flag)
      {
        int toId = list.Min<TfvcChangesetRef>((Func<TfvcChangesetRef, int>) (x => x.ChangesetId)) - 1;
        list = this.QueryTfvcChangesetRefs(itemPath, toId, 250).ToList<TfvcChangesetRef>();
        flag = list.Min<TfvcChangesetRef, DateTime>((Func<TfvcChangesetRef, DateTime>) (x => x.CreatedDate)) < fromDate;
        source.AddRange(list.Select<TfvcChangesetRef, int>((Func<TfvcChangesetRef, int>) (x => x.ChangesetId)));
      }
      if (flag)
      {
        foreach (TfvcChangesetRef tfvcChangesetRef in list.Where<TfvcChangesetRef>((Func<TfvcChangesetRef, bool>) (x => x.CreatedDate < fromDate)))
          source.Remove(tfvcChangesetRef.ChangesetId);
      }
      return (IEnumerable<int>) source.OrderBy<int, int>((Func<int, int>) (x => x)).ToList<int>();
    }

    public IEnumerable<int> QueryTfvcChangesetIds(string itemPath, int toId, int top) => this.QueryTfvcChangesetRefs(itemPath, toId, top).Select<TfvcChangesetRef, int>((Func<TfvcChangesetRef, int>) (x => x.ChangesetId));

    public TfvcChangesetRef QueryChangesetRef(int changeSetId)
    {
      using (TeamFoundationDataReader foundationDataReader = this.m_versionControlService.QueryChangeset(this.m_userRequestContext, changeSetId, false, false, false))
        return (TfvcChangesetRef) foundationDataReader.Current<Changeset>().ToWebApiChangeset(this.m_userRequestContext, false, int.MaxValue);
    }

    public IEnumerable<TfvcChange> QueryChangeSetChanges(
      int changesetId,
      int pageSize,
      ItemSpec lastItem)
    {
      List<TfvcChange> tfvcChangeList = new List<TfvcChange>();
      using (TeamFoundationDataReader foundationDataReader = this.m_versionControlService.QueryChangesForChangeset(this.m_userRequestContext, changesetId, false, pageSize, lastItem, (string[]) null, true))
      {
        foreach (Change current in foundationDataReader.CurrentEnumerable<Change>())
        {
          TfvcChange webApiChangeModel = current.ToWebApiChangeModel();
          webApiChangeModel.Item.FileId = current.Item.GetFileId(0);
          tfvcChangeList.Add(webApiChangeModel);
        }
      }
      return (IEnumerable<TfvcChange>) tfvcChangeList;
    }

    private IEnumerable<TfvcChangesetRef> QueryTfvcChangesetRefs(
      string itemPath,
      int toId,
      int top)
    {
      ItemSpec itemSpec = new ItemSpec(itemPath, RecursionType.Full);
      VersionSpec versionTo = (VersionSpec) null;
      if (toId > 0)
        versionTo = (VersionSpec) new ChangesetVersionSpec(toId);
      TfvcChangesetsCollection changesetsCollection = new TfvcChangesetsCollection();
      using (TeamFoundationDataReader foundationDataReader = this.m_versionControlService.QueryHistory(this.m_userRequestContext, (string) null, (string) null, itemSpec, (VersionSpec) new LatestVersionSpec(), (string) null, (VersionSpec) null, versionTo, top, false, false, true, false))
      {
        IEnumerable<Changeset> changesets = foundationDataReader.CurrentEnumerable<Changeset>();
        if (changesets != null)
        {
          foreach (Changeset changeset in changesets)
          {
            if (changesetsCollection.Count != top)
            {
              string comment = changeset.Comment;
              int length = comment != null ? comment.Length : 0;
              TfvcChangesetRef webApiChangeset = changeset.ToWebApiChangeset(this.m_userRequestContext, length);
              changesetsCollection.Add(webApiChangeset);
            }
            else
              break;
          }
        }
      }
      return (IEnumerable<TfvcChangesetRef>) changesetsCollection;
    }
  }
}
