// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemPickListCacheService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemPickListCacheService : VssVersionedCacheService<PicklistCacheData>
  {
    private static readonly string s_area = nameof (WorkItemPickListCacheService);

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(systemRequestContext, "Default", PicklistNotifications.PicklistChanged, new SqlNotificationCallback(this.OnPicklistChanged), true);
      service.RegisterNotification(systemRequestContext, "Default", PicklistNotifications.PicklistDeleted, new SqlNotificationCallback(this.OnPicklistDeleted), true);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", PicklistNotifications.PicklistChanged, new SqlNotificationCallback(this.OnPicklistChanged), true);
      service.UnregisterNotification(systemRequestContext, "Default", PicklistNotifications.PicklistDeleted, new SqlNotificationCallback(this.OnPicklistDeleted), true);
      base.ServiceEnd(systemRequestContext);
    }

    public virtual bool TryGetPicklist(
      IVssRequestContext requestContext,
      Guid picklistId,
      out WorkItemPickList picklist)
    {
      WorkItemPickList picklistToReturn = (WorkItemPickList) null;
      if (!this.TryRead(requestContext, (Func<PicklistCacheData, bool>) (cache => cache.TryGetPicklist(picklistId, out picklistToReturn))))
        picklistToReturn = this.RefreshPickList(requestContext, picklistId);
      picklist = picklistToReturn;
      return picklist != null;
    }

    public virtual bool TryGetPicklistMetadata(
      IVssRequestContext requestContext,
      out IReadOnlyCollection<WorkItemPickListMetadata> metadata)
    {
      IReadOnlyCollection<WorkItemPickListMetadata> metadataToReturn = (IReadOnlyCollection<WorkItemPickListMetadata>) null;
      if (!this.TryRead(requestContext, (Func<PicklistCacheData, bool>) (cache => cache.TryGetPicklistMetadata(out metadataToReturn))))
        metadataToReturn = this.RefreshPickListMetadata(requestContext);
      metadata = metadataToReturn;
      return metadata != null;
    }

    protected override PicklistCacheData InitializeCache(IVssRequestContext requestContext)
    {
      PicklistCacheData data = new PicklistCacheData((VssCacheBase) this);
      this.InitializePickLists(requestContext, data);
      return data;
    }

    private void OnPicklistChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceBlock(909918, 909919, WorkItemPickListCacheService.s_area, nameof (OnPicklistChanged), nameof (OnPicklistChanged), (Action) (() =>
      {
        Guid result;
        if (!string.IsNullOrEmpty(eventData) && Guid.TryParse(eventData, out result))
        {
          this.RefreshPickList(requestContext, result);
          this.RefreshPickListMetadata(requestContext);
        }
        else
        {
          requestContext.Trace(909920, TraceLevel.Error, WorkItemPickListCacheService.s_area, nameof (OnPicklistChanged), "Invalid event payload: " + (eventData ?? string.Empty));
          this.Reset(requestContext);
        }
      }));
    }

    private void OnPicklistDeleted(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceBlock(909915, 909916, WorkItemPickListCacheService.s_area, nameof (OnPicklistDeleted), nameof (OnPicklistDeleted), (Action) (() =>
      {
        Guid picklistId;
        if (!string.IsNullOrEmpty(eventData) && Guid.TryParse(eventData, out picklistId))
        {
          WorkItemPickList picklist = (WorkItemPickList) null;
          if (this.TryRead(requestContext, (Func<PicklistCacheData, bool>) (cache => cache.TryGetPicklist(picklistId, out picklist))))
            this.Invalidate<bool>(requestContext, (Func<PicklistCacheData, bool>) (cache => cache.Remove(picklist)));
          this.RefreshPickListMetadata(requestContext);
        }
        else
        {
          requestContext.Trace(909917, TraceLevel.Error, WorkItemPickListCacheService.s_area, nameof (OnPicklistDeleted), "Invalid event payload: " + (eventData ?? string.Empty));
          this.Reset(requestContext);
        }
      }));
    }

    private WorkItemPickList RefreshPickList(IVssRequestContext requestContext, Guid picklistId) => requestContext.TraceBlock<WorkItemPickList>(909913, 909914, WorkItemPickListCacheService.s_area, "OnPicklistChanged", nameof (RefreshPickList), (Func<WorkItemPickList>) (() => this.Synchronize<WorkItemPickList>(requestContext, (Func<WorkItemPickList>) (() => this.GetPicklistFromDB(requestContext, picklistId)), (Action<PicklistCacheData, WorkItemPickList>) ((cache, picklist) => cache.Update(picklist)))));

    private void InitializePickLists(IVssRequestContext requestContext, PicklistCacheData data) => requestContext.TraceBlock(909923, 909924, WorkItemPickListCacheService.s_area, "OnPicklistChanged", nameof (InitializePickLists), (Action) (() =>
    {
      foreach (WorkItemPickList picklist in (IEnumerable<WorkItemPickList>) this.GetPicklistsFromDB(requestContext))
        data.Update(picklist);
    }));

    private IReadOnlyCollection<WorkItemPickListMetadata> RefreshPickListMetadata(
      IVssRequestContext requestContext)
    {
      return requestContext.TraceBlock<IReadOnlyCollection<WorkItemPickListMetadata>>(909921, 909922, WorkItemPickListCacheService.s_area, nameof (RefreshPickListMetadata), nameof (RefreshPickListMetadata), (Func<IReadOnlyCollection<WorkItemPickListMetadata>>) (() => this.Synchronize<IReadOnlyCollection<WorkItemPickListMetadata>>(requestContext, (Func<IReadOnlyCollection<WorkItemPickListMetadata>>) (() => this.GetPicklistMetadataFromDB(requestContext)), (Action<PicklistCacheData, IReadOnlyCollection<WorkItemPickListMetadata>>) ((cache, metadata) => cache.Update(metadata)))));
    }

    private IReadOnlyCollection<WorkItemPickList> GetPicklistsFromDB(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<WorkItemPickList>) requestContext.TraceBlock<List<WorkItemPickList>>(909925, 909926, WorkItemPickListCacheService.s_area, "WorkItemPickListComponent", nameof (GetPicklistsFromDB), (Func<List<WorkItemPickList>>) (() =>
      {
        IEnumerable<WorkItemPickListRecord> source = (IEnumerable<WorkItemPickListRecord>) null;
        using (WorkItemPickListComponent component = requestContext.CreateComponent<WorkItemPickListComponent>())
          source = (IEnumerable<WorkItemPickListRecord>) component.GetLists();
        if (source == null)
          return (List<WorkItemPickList>) null;
        return source.Select<WorkItemPickListRecord, WorkItemPickList>((Func<WorkItemPickListRecord, WorkItemPickList>) (r => WorkItemPickList.Create(r))).ToList<WorkItemPickList>();
      }));
    }

    internal WorkItemPickList GetPicklistFromDB(IVssRequestContext requestContext, Guid listId) => requestContext.TraceBlock<WorkItemPickList>(909929, 909930, WorkItemPickListCacheService.s_area, "WorkItemPickListComponent", nameof (GetPicklistFromDB), (Func<WorkItemPickList>) (() =>
    {
      WorkItemPickListRecord picklist = (WorkItemPickListRecord) null;
      using (WorkItemPickListComponent component = requestContext.CreateComponent<WorkItemPickListComponent>())
        picklist = component.GetList(listId);
      if (picklist == null)
        return (WorkItemPickList) null;
      return WorkItemPickList.Create(picklist);
    }));

    private IReadOnlyCollection<WorkItemPickListMetadata> GetPicklistMetadataFromDB(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<WorkItemPickListMetadata>) requestContext.TraceBlock<List<WorkItemPickListMetadata>>(909927, 909928, WorkItemPickListCacheService.s_area, "WorkItemPickListComponent", nameof (GetPicklistMetadataFromDB), (Func<List<WorkItemPickListMetadata>>) (() =>
      {
        IReadOnlyCollection<WorkItemPickListMetadataRecord> source = (IReadOnlyCollection<WorkItemPickListMetadataRecord>) null;
        using (WorkItemPickListComponent component = requestContext.CreateComponent<WorkItemPickListComponent>())
          source = component.GetListsMetadata();
        if (source == null)
          return (List<WorkItemPickListMetadata>) null;
        return source.Select<WorkItemPickListMetadataRecord, WorkItemPickListMetadata>((Func<WorkItemPickListMetadataRecord, WorkItemPickListMetadata>) (r => WorkItemPickListMetadata.Create(r))).ToList<WorkItemPickListMetadata>();
      }));
    }
  }
}
