// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryLabels
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal sealed class CommandQueryLabels : VersionControlCommand
  {
    private StreamingCollection<VersionControlLabel> m_labels;
    private VersionControlLabel m_currentLabel;
    private LabelComponent m_db;
    private ObjectBinder<VersionControlLabel> m_versionControlLabelBinder;
    private CommandQueryLabels.State m_state;
    private StreamingCollection<Item> m_items;
    private readonly ItemSpec[] m_itemSpecs = new ItemSpec[1]
    {
      new ItemSpec("$/", RecursionType.Full)
    };
    private CommandQueryItems<ItemSet, Item> m_commandQueryItems;
    private string m_filterServerItem;
    private bool m_includeItems;
    private bool m_generateDownloadUrls;

    public CommandQueryLabels(VersionControlRequestContext context)
      : base(context)
    {
      this.m_labels = new StreamingCollection<VersionControlLabel>((Command) this)
      {
        HandleExceptions = false
      };
    }

    public void Execute(
      string workspaceName,
      string workspaceOwner,
      string labelName,
      string labelScope,
      string ownerName,
      string itemFilter,
      VersionSpec versionFilterItem,
      bool includeItems,
      bool generateDownloadUrls)
    {
      Workspace localWorkspace = Workspace.QueryWorkspace(this.m_versionControlRequestContext, workspaceName, workspaceOwner);
      this.m_includeItems = includeItems;
      this.m_generateDownloadUrls = generateDownloadUrls;
      Microsoft.VisualStudio.Services.Identity.Identity owner = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (!string.IsNullOrEmpty(ownerName))
        owner = TfvcIdentityHelper.FindIdentity(this.m_versionControlRequestContext.RequestContext, ownerName);
      this.m_filterServerItem = (string) null;
      ItemPathPair serverItem = new ItemPathPair();
      int versionItem = 0;
      if (!string.IsNullOrEmpty(itemFilter))
      {
        using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
        {
          versionFilterItem.QueryItems(this.m_versionControlRequestContext, new ItemSpec(itemFilter, RecursionType.None), localWorkspace, versionedItemComponent, DeletedState.Any, ItemType.Any, out string _, out string _, 8);
          Item obj;
          if (!versionFilterItem.TryGetNextItem(out obj) || !obj.HasPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read))
            throw new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, new ItemSpec(itemFilter, RecursionType.None), versionFilterItem);
          serverItem = obj is WorkspaceItem workspaceItem ? workspaceItem.CommittedItemPathPair : obj.ItemPathPair;
          this.m_filterServerItem = serverItem.ProjectNamePath;
          versionItem = obj.ChangesetId;
        }
      }
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetLabelComponent(this.m_versionControlRequestContext);
      this.m_versionControlLabelBinder = this.m_db.QueryLabels(labelName, ItemPathPair.FromServerItem(labelScope), owner, serverItem, versionItem).GetCurrent<VersionControlLabel>();
      this.m_state = CommandQueryLabels.State.Label;
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      bool flag1 = true;
      while (!this.IsCacheFull & flag1)
      {
        if (CommandQueryLabels.State.Label == this.m_state)
        {
          bool flag2 = true;
          while (!this.IsCacheFull && (flag2 = this.m_versionControlLabelBinder.MoveNext()))
          {
            this.m_currentLabel = this.m_versionControlLabelBinder.Current;
            string identityName;
            string displayName;
            this.SecurityWrapper.FindIdentityNames(this.m_versionControlRequestContext.RequestContext, this.m_currentLabel.ownerId, out identityName, out displayName);
            this.m_currentLabel.OwnerName = identityName;
            this.m_currentLabel.OwnerDisplayName = displayName;
            if (this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, this.m_currentLabel.ScopePair))
            {
              this.m_labels.Enqueue(this.m_currentLabel);
              if (this.m_includeItems)
              {
                this.GetItemsForLabel();
                this.m_currentLabel.Items = new StreamingCollection<Item>((Command) this)
                {
                  HandleExceptions = false
                };
                this.m_state = CommandQueryLabels.State.Item;
                break;
              }
              if (this.m_filterServerItem != null)
              {
                this.m_currentLabel.Items = new StreamingCollection<Item>((Command) this)
                {
                  HandleExceptions = false
                };
                if (this.m_currentLabel.filterItem != null)
                {
                  this.m_currentLabel.filterItem.ServerItem = this.m_filterServerItem;
                  this.m_currentLabel.Items.Enqueue(this.m_currentLabel.filterItem);
                }
                this.m_currentLabel.Items.IsComplete = true;
              }
            }
          }
          flag1 = flag2;
        }
        if (CommandQueryLabels.State.Item == this.m_state)
        {
          bool flag3 = true;
          while (!this.IsCacheFull && this.m_items != null && (flag3 = this.m_items.MoveNext()))
          {
            if ((PathLength) this.m_items.Current.ServerItem.Length > this.m_versionControlRequestContext.MaxSupportedServerPathLength + 1)
              throw new RepositoryPathTooLongDetailedException(this.m_items.Current.ServerItem, (int) this.m_versionControlRequestContext.MaxSupportedServerPathLength);
            this.m_currentLabel.Items.Enqueue(this.m_items.Current);
          }
          if (!flag3)
          {
            this.m_currentLabel.Items.IsComplete = true;
            this.m_commandQueryItems.Dispose();
            this.m_commandQueryItems = (CommandQueryItems<ItemSet, Item>) null;
            this.m_state = CommandQueryLabels.State.Label;
          }
          flag1 |= flag3;
        }
      }
      if (flag1)
        return;
      this.m_labels.IsComplete = true;
      this.m_db.Dispose();
      this.m_db = (LabelComponent) null;
    }

    public StreamingCollection<VersionControlLabel> Labels => this.m_labels;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (LabelComponent) null;
      }
      if (this.m_commandQueryItems == null)
        return;
      this.m_commandQueryItems.Dispose();
      this.m_commandQueryItems = (CommandQueryItems<ItemSet, Item>) null;
    }

    private void GetItemsForLabel()
    {
      this.m_items = (StreamingCollection<Item>) null;
      if (this.m_commandQueryItems != null)
        this.m_commandQueryItems.Dispose();
      this.m_commandQueryItems = new CommandQueryItems<ItemSet, Item>(this.m_versionControlRequestContext);
      this.m_commandQueryItems.Execute((Workspace) null, this.m_itemSpecs, (VersionSpec) new LabelVersionSpec(this.m_currentLabel.Name, this.m_currentLabel.Scope), DeletedState.Any, ItemType.Any, this.m_generateDownloadUrls, 4);
      StreamingCollection<ItemSet> itemSets = this.m_commandQueryItems.ItemSets;
      if (!itemSets.MoveNext())
        return;
      this.m_items = itemSets.Current.Items;
    }

    private enum State
    {
      Label = 1,
      Item = 2,
    }
  }
}
