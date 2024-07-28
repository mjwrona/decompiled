// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryItems`2
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryItems<ItemSetT, ItemT> : VersionControlCommand
    where ItemSetT : BaseItemSet<ItemT>, new()
    where ItemT : Item
  {
    protected StreamingCollection<ItemSetT> m_itemSets;
    private VersionedItemComponent m_db;
    private UrlSigner m_urlSigner;
    private bool m_hasAdminConfiguration;
    private List<DeferredQuery> m_queries;
    private int m_queryIndex;
    private ItemSetT m_currentItemSet;
    private VersionedItemPermissions m_requiredPermission;
    private ContinueExecutionCompletedCallback m_continueExecutionCompletedCallback;
    private PropertyMerger<ItemT> m_propertyMerger;
    private PropertyMerger<ItemT> m_attributeMerger;
    private List<StreamingCollection<ItemT>> m_itemSetsForProperties;
    private CommandQueryItems<ItemSetT, ItemT>.State m_state;
    private bool m_hasMoreData = true;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;
    private Workspace m_workspace;
    private DeletedState m_deleted;
    private bool m_generateDownloadUrls;
    private int m_totalItems;
    private int m_requiredPermissionAllowed;
    private int m_readAllowed;

    public CommandQueryItems(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(
      Workspace workspace,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deleted,
      ItemType itemType,
      bool generateDownloadUrls,
      int options)
    {
      this.Execute(workspace, itemSpecs, versionSpec, deleted, itemType, generateDownloadUrls, options, VersionedItemPermissions.Read);
    }

    public void Execute(
      Workspace workspace,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deleted,
      ItemType itemType,
      bool generateDownloadUrls,
      int options,
      VersionedItemPermissions requiredPermission)
    {
      this.Execute(workspace, itemSpecs, versionSpec, deleted, itemType, generateDownloadUrls, options, VersionedItemPermissions.Read, (ContinueExecutionCompletedCallback) null, (string[]) null, (string[]) null);
    }

    public void Execute(
      Workspace workspace,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deleted,
      ItemType itemType,
      bool generateDownloadUrls,
      int options,
      VersionedItemPermissions requiredPermission,
      ContinueExecutionCompletedCallback continueExecutionCompletedCallback,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters)
    {
      this.m_workspace = workspace;
      this.m_deleted = deleted;
      this.m_generateDownloadUrls = generateDownloadUrls;
      this.m_requiredPermission = requiredPermission;
      this.m_continueExecutionCompletedCallback = continueExecutionCompletedCallback;
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add(nameof (versionSpec), (object) versionSpec);
      ctData.Add(nameof (deleted), (object) deleted);
      ctData.Add(nameof (itemType), (object) itemType);
      ctData.Add(nameof (generateDownloadUrls), (object) generateDownloadUrls);
      ctData.Add(nameof (options), (object) options);
      ctData.Add(nameof (requiredPermission), (object) requiredPermission);
      if (itemSpecs != null)
      {
        ctData.Add("length", (object) itemSpecs.Length);
        ctData.Add("items", (object) ((IEnumerable<ItemSpec>) itemSpecs).Take<ItemSpec>(10).Select<ItemSpec, string>((Func<ItemSpec, string>) (x => string.Format("{0};{1}", (object) x.Item, (object) x.RecursionType))).ToList<string>());
      }
      ClientTrace.Publish(this.RequestContext, "QueryItems", ctData);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      if (this.m_generateDownloadUrls)
        this.m_urlSigner = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_itemSets = new StreamingCollection<ItemSetT>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_itemSetsForProperties = new List<StreamingCollection<ItemT>>();
      this.m_hasAdminConfiguration = this.m_versionControlRequestContext.GetPrivilegeSecurity().HasPermission(this.RequestContext, SecurityConstants.GlobalSecurityResource, 32, false);
      this.m_queries = new List<DeferredQuery>(itemSpecs.Length);
      this.m_queryIndex = 0;
      for (int index = 0; index < itemSpecs.Length; ++index)
      {
        ItemSpec itemSpec = itemSpecs[index];
        if (!itemSpec.isServerItem)
        {
          if (!(versionSpec is WorkspaceVersionSpec))
          {
            try
            {
              itemSpec.ItemPathPair = itemSpec.toServerItemWithoutMappingRenames(this.m_versionControlRequestContext, this.m_workspace, true);
            }
            catch (IllegalServerItemException ex)
            {
            }
          }
        }
        this.m_queries.Add(new DeferredQuery(this.m_versionControlRequestContext, itemSpec, this.m_deleted, itemType, versionSpec, false, options));
      }
      if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
        this.m_propertyMerger = new PropertyMerger<ItemT>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
      if (itemAttributeFilters != null && itemAttributeFilters.Length != 0)
        this.m_attributeMerger = new PropertyMerger<ItemT>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
      DeferredQuery.ExecuteOptimizedQueries(this.m_queries, this.m_workspace, this.m_db);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandQueryItems<ItemSetT, ItemT>.State.Items)
      {
        for (; this.m_queryIndex < this.m_queries.Count; ++this.m_queryIndex)
        {
          DeferredQuery query = this.m_queries[this.m_queryIndex];
          if (!query.Executed)
          {
            query.Execute(this.m_workspace, this.m_db);
            if (query.Exception != null)
              throw query.Exception;
          }
          if ((object) this.m_currentItemSet == null)
          {
            this.m_currentItemSet = new ItemSetT();
            this.m_currentItemSet.Items = new StreamingCollection<ItemT>((Command) this)
            {
              HandleExceptions = false
            };
            if (query.QueryPath != null)
            {
              this.m_currentItemSet.QueryPath = VersionControlPath.IsServerItem(query.QueryPath) ? query.QueryPath : this.m_workspace.LocalToServerItem(this.RequestContext, query.QueryPath, true);
              this.m_currentItemSet.Pattern = query.FilePattern;
            }
            this.m_itemSets.Enqueue(this.m_currentItemSet);
            this.m_itemSetsForProperties.Add(this.m_currentItemSet.Items);
          }
          bool flag1 = false;
          Item o;
          while (!this.IsCacheFull && (flag1 = query.TryGetNextItem(out o)))
          {
            if (query.ItemSpec.postMatch(o.ServerItem) && (this.m_deleted != DeletedState.Deleted || o.DeletionId != 0))
            {
              ++this.m_totalItems;
              bool flag2 = o.HasPermission(this.m_versionControlRequestContext, this.m_requiredPermission);
              if (!flag2 && this.m_requiredPermission == VersionedItemPermissions.Checkin)
              {
                if (this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, o.ItemPathPair))
                  ++this.m_readAllowed;
              }
              else
              {
                if (!flag2 && !this.m_hasAdminConfiguration)
                {
                  if (o.ItemType == ItemType.Folder && (this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, this.m_requiredPermission, o.ItemPathPair) || this.SecurityWrapper.HasItemPermissionForAnyChildren(this.m_versionControlRequestContext, this.m_requiredPermission, o.ItemPathPair)))
                    flag2 = true;
                  else
                    continue;
                }
                if (o is WorkspaceItem && typeof (ItemT) == typeof (Item))
                  o = new Item((WorkspaceItem) o);
                if (flag2 && this.m_generateDownloadUrls)
                  this.m_urlSigner.SignObject((ISignable) o);
                ++this.m_requiredPermissionAllowed;
                this.m_currentItemSet.Items.Enqueue((ItemT) o);
              }
            }
          }
          if (!this.IsCacheFull)
          {
            if (!flag1)
              this.m_currentItemSet.Items.IsComplete = true;
            this.m_currentItemSet = default (ItemSetT);
          }
          else
            break;
        }
        if (this.m_urlSigner != null)
          this.m_urlSigner.FlushDeferredSignatures();
        if (this.m_queryIndex == this.m_queries.Count)
        {
          this.m_hasMoreData = false;
          this.m_itemSets.IsComplete = true;
          if (this.m_continueExecutionCompletedCallback != null)
            this.m_continueExecutionCompletedCallback((Command) this);
        }
        this.m_state = CommandQueryItems<ItemSetT, ItemT>.State.Properties;
      }
      if (this.m_state == CommandQueryItems<ItemSetT, ItemT>.State.Properties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_itemSetsForProperties);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandQueryItems<ItemSetT, ItemT>.State.Attributes;
      }
      if (this.m_state != CommandQueryItems<ItemSetT, ItemT>.State.Attributes)
        return;
      if (this.m_attributeMerger != null)
      {
        if (!this.m_hasMoreAttributes)
          this.m_attributeMerger.Execute(this.m_itemSetsForProperties);
        this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
      }
      this.m_state = this.m_hasMoreProperties ? CommandQueryItems<ItemSetT, ItemT>.State.Properties : (this.m_hasMoreAttributes ? CommandQueryItems<ItemSetT, ItemT>.State.Attributes : (this.m_hasMoreData ? CommandQueryItems<ItemSetT, ItemT>.State.Items : CommandQueryItems<ItemSetT, ItemT>.State.Complete));
    }

    public StreamingCollection<ItemSetT> ItemSets => this.m_itemSets;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (VersionedItemComponent) null;
      }
      if (this.m_urlSigner != null)
      {
        this.m_urlSigner.Dispose();
        this.m_urlSigner = (UrlSigner) null;
      }
      if (this.m_propertyMerger != null)
      {
        this.m_propertyMerger.Dispose();
        this.m_propertyMerger = (PropertyMerger<ItemT>) null;
      }
      if (this.m_attributeMerger == null)
        return;
      this.m_attributeMerger.Dispose();
      this.m_attributeMerger = (PropertyMerger<ItemT>) null;
    }

    internal bool HasItems => this.m_totalItems > 0;

    internal bool CanAccessAllItems => this.HasItems && this.m_requiredPermissionAllowed == this.m_totalItems;

    internal bool CanReadAtLeastOneItem => this.HasItems && this.m_readAllowed > 0;

    private enum State
    {
      Items,
      Properties,
      Attributes,
      Complete,
    }
  }
}
