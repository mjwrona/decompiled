// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryItemsExtended
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryItemsExtended : VersionControlCommand
  {
    private List<StreamingCollection<ExtendedItem>> m_extendedItems;
    private VersionedItemComponent m_db;
    private int m_itemSpecIndex;
    private ObjectBinder<ExtendedItem> m_extendedItemBinder;
    private ResultCollection m_results;
    private PropertyMerger<ExtendedItem> m_merger;
    private int m_timeoutMinutes;
    private Workspace m_workspace;
    private ItemSpec[] m_itemSpecs;
    private DeletedState m_deletedState;
    private ItemType m_itemType;
    private int m_options;
    private CommandQueryItemsExtended.State m_state;

    public CommandQueryItemsExtended(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(
      Workspace localWorkspace,
      ItemSpec[] itemSpecs,
      DeletedState deletedState,
      ItemType itemType,
      int options,
      string[] itemPropertyFilters,
      int timeoutMinutes)
    {
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add(nameof (deletedState), (object) deletedState);
      ctData.Add(nameof (itemType), (object) itemType);
      ctData.Add(nameof (options), (object) options);
      if (itemSpecs != null)
      {
        ctData.Add("length", (object) itemSpecs.Length);
        ctData.Add("items", (object) ((IEnumerable<ItemSpec>) itemSpecs).Take<ItemSpec>(10).Select<ItemSpec, string>((Func<ItemSpec, string>) (x => string.Format("{0};{1}", (object) x.Item, (object) x.RecursionType))).ToList<string>());
      }
      ClientTrace.Publish(this.RequestContext, "QueryItemsExtended", ctData);
      if (this.RequestContext.IsFeatureEnabled("SourceControl.BlockQueryItemsExtendedFullRecursion"))
      {
        foreach (ItemSpec itemSpec in itemSpecs)
        {
          if (itemSpec.RecursionType == RecursionType.Full)
            throw new ArgumentException("QueryItemsExtended with RecursionType.Full has been disabled");
        }
      }
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_timeoutMinutes = timeoutMinutes;
      this.m_workspace = localWorkspace;
      this.m_itemSpecs = itemSpecs;
      this.m_deletedState = deletedState;
      this.m_itemType = itemType;
      this.m_itemSpecIndex = 0;
      this.m_options = options;
      this.m_extendedItems = new List<StreamingCollection<ExtendedItem>>(this.m_itemSpecs.Length);
      for (int index = 0; index < itemSpecs.Length; ++index)
        this.m_extendedItems.Add(new StreamingCollection<ExtendedItem>((Command) this)
        {
          HandleExceptions = false
        });
      if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
        this.m_merger = new PropertyMerger<ExtendedItem>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandQueryItemsExtended.State.ExtendedItems)
      {
        for (; this.m_itemSpecIndex < this.m_itemSpecs.Length; ++this.m_itemSpecIndex)
        {
          ItemSpec itemSpec = this.m_itemSpecs[this.m_itemSpecIndex];
          if (this.m_extendedItemBinder == null)
          {
            this.RequestContext.Trace(700332, TraceLevel.Verbose, TraceArea.General, TraceLayer.Command, "QueryItemsExtended isServer:{0}, Recursion:{1}, workspaceId:{2}, item:{3}, deleted:{4} type:{5} options:{6}", (object) itemSpec.isServerItem, (object) itemSpec.RecursionType, (object) this.m_workspace?.Id, (object) itemSpec.Item, (object) this.m_deletedState, (object) this.m_itemType, (object) this.m_options);
            this.m_results = !itemSpec.isServerItem ? this.m_db.QueryItemsExtendedLocal(this.m_workspace, itemSpec.Item, itemSpec.RecursionType, this.m_deletedState, this.m_itemType, this.m_options, this.m_versionControlRequestContext.MaxSupportedServerPathLength, this.m_timeoutMinutes) : this.m_db.QueryItemsExtended(this.m_workspace, itemSpec.ItemPathPair, itemSpec.RecursionType, this.m_deletedState, this.m_itemType, this.m_options, this.m_timeoutMinutes);
            this.m_extendedItemBinder = this.m_results.GetCurrent<ExtendedItem>();
          }
          while (!this.IsCacheFull && this.m_extendedItemBinder.MoveNext())
          {
            ExtendedItem current = this.m_extendedItemBinder.Current;
            bool flag = false;
            if (itemSpec.postMatch(current.TargetServerItem))
            {
              if (this.m_deletedState == DeletedState.NonDeleted)
              {
                if (current.DeletionId > 0 && current.VersionLocal == 0 && current.ChangeType == ChangeType.None)
                  continue;
              }
              else if (this.m_deletedState == DeletedState.Deleted && current.DeletionId == 0)
                continue;
              if (this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.TargetItemPathPair) && (current.TargetServerItem.Equals(current.SourceServerItem) || current.SourceServerItem == null || this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.SourceItemPathPair)))
                flag = true;
              else if (this.m_versionControlRequestContext.GetPrivilegeSecurity().HasPermission(this.RequestContext, SecurityConstants.GlobalSecurityResource, 32, false))
                flag = true;
              else if (current.ItemType == ItemType.Folder && this.SecurityWrapper.HasItemPermissionForAnyChildren(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.TargetItemPathPair) && (current.TargetServerItem.Equals(current.SourceServerItem) || current.SourceServerItem == null || this.SecurityWrapper.HasItemPermissionForAnyChildren(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.SourceItemPathPair)))
                flag = true;
              if (flag)
              {
                if (!current.lockOwnerId.Equals(Guid.Empty))
                {
                  string identityName;
                  string displayName;
                  this.SecurityWrapper.FindIdentityNames(this.RequestContext, current.lockOwnerId, out identityName, out displayName);
                  current.LockOwner = identityName;
                  current.LockOwnerDisplayName = displayName;
                }
                if (itemSpec.isServerItem && itemSpec.Item == "$/" && itemSpec.RecursionType == RecursionType.OneLevel)
                {
                  if (current.SourceServerItem != "$/")
                  {
                    try
                    {
                      ProjectUtility.GetProjectId(this.RequestContext, VersionControlPath.GetTeamProjectNameAndRemainingPath(current.SourceItemPathPair.ProjectNamePath, out string _));
                    }
                    catch (ProjectDoesNotExistWithNameException ex)
                    {
                      continue;
                    }
                  }
                }
                this.m_extendedItems[this.m_itemSpecIndex].Enqueue(current);
              }
            }
          }
          if (!this.IsCacheFull)
          {
            this.m_results.Dispose();
            this.m_results = (ResultCollection) null;
            this.m_extendedItemBinder = (ObjectBinder<ExtendedItem>) null;
            this.m_extendedItems[this.m_itemSpecIndex].IsComplete = true;
          }
          else
            break;
        }
        if (this.m_merger != null)
        {
          this.m_merger.Execute(this.m_extendedItems);
          this.m_state = CommandQueryItemsExtended.State.Properties;
        }
      }
      if (this.m_state != CommandQueryItemsExtended.State.Properties || this.m_merger.TryMergeNextPage())
        return;
      this.m_state = CommandQueryItemsExtended.State.ExtendedItems;
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (VersionedItemComponent) null;
      }
      if (this.m_results == null)
        return;
      this.m_results.Dispose();
      this.m_results = (ResultCollection) null;
    }

    public List<StreamingCollection<ExtendedItem>> ExtendedItems => this.m_extendedItems;

    private enum State
    {
      ExtendedItems,
      Properties,
    }
  }
}
