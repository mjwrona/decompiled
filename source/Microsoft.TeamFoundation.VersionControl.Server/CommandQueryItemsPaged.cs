// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryItemsPaged
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryItemsPaged : VersionControlCommand
  {
    private int m_pageSize;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private ObjectBinder<Item> m_itemBinder;
    private int m_itemsEnqueued;
    private string m_lastItemInPage;
    private bool m_hasAdminConfiguration;
    private static readonly VersionedItemPermissions s_requiredPermissions = VersionedItemPermissions.Read;

    public CommandQueryItemsPaged(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(
      ItemSpec scopeItem,
      int changesetId,
      int pageSize,
      ItemSpec lastItem,
      int options,
      out string lastItemPaged)
    {
      this.m_pageSize = pageSize;
      Validation validation = this.m_versionControlRequestContext.Validation;
      validation.check((IValidatable) scopeItem, nameof (scopeItem), false);
      scopeItem.requireServerItem();
      if (changesetId <= 0 || changesetId > this.m_versionControlRequestContext.VersionControlService.GetLatestChangeset(this.m_versionControlRequestContext))
        throw new ArgumentOutOfRangeException(nameof (changesetId));
      if (pageSize < 1 || pageSize > 10000)
        throw new ArgumentOutOfRangeException(nameof (pageSize));
      if (lastItem != null)
      {
        validation.check((IValidatable) lastItem, nameof (lastItem), false);
        lastItem.requireServerItem();
      }
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_hasAdminConfiguration = this.m_versionControlRequestContext.GetPrivilegeSecurity().HasPermission(this.RequestContext, SecurityConstants.GlobalSecurityResource, 32, false);
      this.Items = new StreamingCollection<Item>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_results = this.m_db.QueryItemsPaged(scopeItem, changesetId, lastItem, this.m_pageSize, options);
      this.m_itemBinder = this.m_results.GetCurrent<Item>();
      this.ContinueExecution();
      if (this.IsCacheFull)
        this.RequestContext.PartialResultsReady();
      lastItemPaged = this.m_lastItemInPage;
    }

    public override void ContinueExecution()
    {
      bool flag = true;
      while (!this.IsCacheFull && this.m_itemsEnqueued < this.m_pageSize && (flag = this.m_itemBinder.MoveNext()))
      {
        Item current = this.m_itemBinder.Current;
        if (current.HasPermission(this.m_versionControlRequestContext, CommandQueryItemsPaged.s_requiredPermissions) || this.m_hasAdminConfiguration || current.ItemType == ItemType.Folder && (this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, CommandQueryItemsPaged.s_requiredPermissions, current.ItemPathPair) || this.SecurityWrapper.HasItemPermissionForAnyChildren(this.m_versionControlRequestContext, CommandQueryItemsPaged.s_requiredPermissions, current.ItemPathPair)))
        {
          ++this.m_itemsEnqueued;
          this.Items.Enqueue(current);
          if (this.m_itemsEnqueued == this.m_pageSize)
            this.m_lastItemInPage = current.ItemPathPair.ProjectNamePath;
        }
      }
      if (!flag)
        this.m_lastItemInPage = (string) null;
      if (this.IsCacheFull && this.m_itemsEnqueued != this.m_pageSize && flag)
        return;
      this.Items.IsComplete = true;
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

    public StreamingCollection<Item> Items { get; private set; }
  }
}
