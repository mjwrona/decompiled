// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.TFSVersionControlDBClient
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B5DEEFA-3C5E-4BFB-92E2-3ADDA47952C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.CodeSense.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.CodeSense.Platform.OnPrem
{
  [Export(typeof (ITfsVersionControlClient))]
  public class TFSVersionControlDBClient : ITfsVersionControlClient
  {
    private const int DefaultChangesPageSize = 20000;
    private const string PageSizeRegistryKey = "/Service/CodeSense/Settings/ChangesetPageSize";
    private ITfvcConverter _tfvcConverter;

    public TFSVersionControlDBClient()
      : this((ITfvcConverter) new TfvcConverter())
    {
    }

    public TFSVersionControlDBClient(ITfvcConverter tfvcConverter) => this._tfvcConverter = tfvcConverter;

    public string DownloadItem(
      IVssRequestContext requestContext,
      TfvcItem tfvcItem,
      TfvcVersionDescriptor versionDescriptor,
      CancellationToken cancellationToken,
      object userState = null)
    {
      try
      {
        TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
        Item serverItem = (Item) null;
        using (TeamFoundationDataReader foundationDataReader = service.QueryItems(requestContext, (string) null, (string) null, new ItemSpec[1]
        {
          new ItemSpec(tfvcItem.Path, RecursionType.None)
        }, (VersionSpec) new ChangesetVersionSpec(versionDescriptor.Version), DeletedState.NonDeleted, ItemType.File, false, 0))
          serverItem = foundationDataReader.CurrentEnumerable<ItemSet>().Single<ItemSet>().Items.SingleOrDefault<Item>();
        using (TeamFoundationDataReader foundationDataReader = service.QueryFileContents(requestContext, serverItem))
        {
          using (StreamReader streamReader = new StreamReader(foundationDataReader.Current<Stream>()))
            return streamReader.ReadToEnd();
        }
      }
      catch (Exception ex)
      {
        requestContext.LogKPI(CodeLensKpiArea.CodeLensService, CodeLensKpiName.DownloadItemFailed);
        throw;
      }
    }

    public TfvcBranch GetBranch(
      IVssRequestContext requestContext,
      string path,
      bool includeParent = false,
      bool includeChildren = false,
      object userState = null)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<TfvcChange> GetChangesetChanges(
      IVssRequestContext requestContext,
      int changesetId,
      int? top = 0,
      int? skip = 0,
      object userState = null)
    {
      ItemSpec lastItem = (ItemSpec) null;
      int pagesCount = 0;
      IVssRegistryService registryService = requestContext.GetService<IVssRegistryService>();
      TeamFoundationVersionControlService versionControlService = requestContext.GetService<TeamFoundationVersionControlService>();
      do
      {
        ++pagesCount;
        int pageSize = registryService.GetValue<int>(requestContext, (RegistryQuery) "/Service/CodeSense/Settings/ChangesetPageSize", 20000);
        pageSize = Math.Max(1, pageSize);
        Change[] changes;
        using (TeamFoundationDataReader source = versionControlService.QueryChangesForChangeset(requestContext, changesetId, false, pageSize, lastItem, new string[0], true))
          changes = source.OfType<Change>().ToArray<Change>();
        Change[] changeArray = changes;
        for (int index = 0; index < changeArray.Length; ++index)
          yield return this._tfvcConverter.GetTfvcChange(changeArray[index]);
        changeArray = (Change[]) null;
        if (pageSize <= changes.Length)
        {
          Change change = changes[changes.Length - 1];
          lastItem = new ItemSpec(change.Item.ServerItem, RecursionType.None, change.Item.DeletionId);
        }
        else
          lastItem = (ItemSpec) null;
        changes = (Change[]) null;
      }
      while (lastItem != null);
    }

    public List<TfvcChangesetRef> GetChangesets(
      IVssRequestContext requestContext,
      TfvcChangesetSearchCriteria searchCriteriaObject,
      int? top = 0,
      int? skip = 0,
      object userState = null)
    {
      throw new NotImplementedException();
    }

    public TfvcChangeset GetChangeset(
      IVssRequestContext requestContext,
      int changesetId,
      bool includeDetails,
      bool includeWorkItems,
      bool includeSourceRenames,
      int maxChangeCount,
      object userState = null)
    {
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryChangeset(requestContext, changesetId, false, false, false))
        return this._tfvcConverter.GetTfvcChangeset(foundationDataReader.Current<Changeset>());
    }

    public IEnumerable<WorkItemData> GetChangesetWorkItems(
      IVssRequestContext requestContext,
      TfvcChangeset tfvcChangeset)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<WorkItemData> GetChangesetWorkItems(
      IVssRequestContext requestContext,
      int changesetId)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<WorkItemData> GetChangesetWorkItems(
      IVssRequestContext requestContext,
      TfvcChangeset changeset,
      IEnumerable<int> workItemLookupEntries)
    {
      workItemLookupEntries = workItemLookupEntries.Where<int>((Func<int, bool>) (entry => entry != -1));
      return TfWorkItemFactory.Create(requestContext, workItemLookupEntries, new DateTime?(changeset.CreatedDate));
    }

    public IEnumerable<WorkItemData> GetChangesetWorkItems(
      IVssRequestContext requestContext,
      TfvcChangeset changeset,
      DateTime workItemAssociationDate)
    {
      IEnumerable<int> associatedWorkItemUris = this.GetAssociatedWorkItemUris(requestContext, changeset, workItemAssociationDate);
      return TfWorkItemFactory.Create(requestContext, associatedWorkItemUris, new DateTime?(workItemAssociationDate));
    }

    public List<List<TfvcItem>> GetItemBatch(
      IVssRequestContext requestContext,
      TfvcItemRequestData requestData,
      object userState = null)
    {
      throw new NotImplementedException();
    }

    public List<TfvcItem> GetItems(
      IVssRequestContext requestContext,
      string scopePath = "$/",
      TfvcVersionDescriptor versionDescriptor = null,
      VersionControlRecursionType recursionLevel = VersionControlRecursionType.OneLevel,
      object userState = null)
    {
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryItems(requestContext, (string) null, (string) null, new ItemSpec[1]
      {
        new ItemSpec(scopePath, RecursionType.None)
      }, (VersionSpec) new ChangesetVersionSpec(versionDescriptor.Version), DeletedState.NonDeleted, ItemType.File, false, 0))
        return (List<TfvcItem>) this._tfvcConverter.GetTfvcItem(foundationDataReader.CurrentEnumerable<ItemSet>().Single<ItemSet>().Items.SingleOrDefault<Item>());
    }

    public int GetLatestChangesetNumber(IVssRequestContext requestContext) => new LatestVersionSpec().ToChangeset(requestContext);

    private IEnumerable<int> GetAssociatedWorkItemUris(
      IVssRequestContext requestContext,
      TfvcChangeset changeset,
      DateTime workItemAssociationDate)
    {
      string absoluteUri = new ChangesetUri(changeset.ChangesetId, UriType.Normal).Uri.AbsoluteUri;
      return requestContext.GetService<TeamFoundationWorkItemService>().GetWorkItemIdsForArtifactUris(requestContext, (IEnumerable<string>) new string[1]
      {
        absoluteUri
      }, new DateTime?(workItemAssociationDate), new Guid?()).First<ArtifactUriQueryResult>().WorkItemIds;
    }

    public bool IsItemPathValid(string itemPath) => VersionControlPath.IsServerItem(itemPath) && VersionControlPath.IsValidPath(itemPath);

    private ICodeLensKpiLoggerService GetCodeLensKpiLoggerService(IVssRequestContext requestContext) => requestContext.GetService<ICodeLensKpiLoggerService>();
  }
}
