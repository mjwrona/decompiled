// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.CommandGetWorkItemLinkChanges
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class CommandGetWorkItemLinkChanges : Command
  {
    private const int MAX_CACHE_SIZE = 1048576;
    private StreamingCollection<WorkItemLinkChange> m_workItemLinkChanges;
    private DalSqlResourceComponent m_db;
    private WorkItemLinkChangeBinder m_itemBinder;
    private ResultCollection m_results;
    private int m_maxCacheSize;
    private bool m_bypassPermissions;
    private readonly int m_defaultBatchSize;
    private int m_rawResultCount;
    private long m_rawMaxReadRowVersion;

    public CommandGetWorkItemLinkChanges(IVssRequestContext requestContext, bool bypassPermissions)
      : base(requestContext)
    {
      this.m_maxCacheSize = 1048576;
      this.m_bypassPermissions = bypassPermissions;
      this.m_defaultBatchSize = requestContext.WitContext().ServerSettings.WorkItemSyncApiBatchSize;
      this.MaxCreatedDateWatermark = SqlDateTime.MinValue.Value;
      this.MaxRemovedDateWatermark = SqlDateTime.MinValue.Value;
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_results != null)
        this.m_results.Dispose();
      if (this.m_db == null)
        return;
      this.m_db.Dispose();
      this.m_db = (DalSqlResourceComponent) null;
    }

    public override int MaxCacheSize
    {
      get => this.m_maxCacheSize;
      set => this.m_maxCacheSize = value;
    }

    public void Execute(
      long rowVersion,
      int batchSize = 2147483647,
      Guid? projectId = null,
      IEnumerable<string> types = null,
      IEnumerable<string> linkTypes = null,
      DateTime? createdDateWatermark = null,
      DateTime? removedDateWatermark = null)
    {
      this.m_db = this.RequestContext.CreateReadReplicaAwareComponent<DalSqlResourceComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica")));
      this.m_rawMaxReadRowVersion = rowVersion;
      this.m_rawResultCount = 0;
      if (batchSize == int.MaxValue)
        batchSize = this.m_defaultBatchSize;
      DateTime? nullable = createdDateWatermark;
      this.MaxCreatedDateWatermark = nullable ?? this.MaxCreatedDateWatermark;
      nullable = removedDateWatermark;
      this.MaxRemovedDateWatermark = nullable ?? this.MaxRemovedDateWatermark;
      int lookbackWindowInSeconds = this.RequestContext.WitContext().ServerSettings.UncommittedLinkChangesLookbackWindowInSeconds;
      this.m_db.GetWorkItemLinkChanges(projectId, types, linkTypes, rowVersion, this.m_bypassPermissions, batchSize, createdDateWatermark, removedDateWatermark, lookbackWindowInSeconds, out this.m_results);
      this.m_itemBinder = (WorkItemLinkChangeBinder) this.m_results.GetCurrent<WorkItemLinkChange>();
      this.m_workItemLinkChanges = new StreamingCollection<WorkItemLinkChange>((Command) this);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      bool flag = false;
      WorkItemTrackingLinkService linkService = this.RequestContext.WitContext().LinkService;
      while (!this.IsCacheFull)
      {
        flag = !this.m_itemBinder.MoveNext();
        if (!flag)
        {
          WorkItemLinkChange current = this.m_itemBinder.Current;
          if (current != null)
          {
            ++this.m_rawResultCount;
            this.m_rawMaxReadRowVersion = Math.Max(this.m_rawMaxReadRowVersion, current.RowVersion);
            DateTime dateTime;
            if (current.IsActive)
            {
              dateTime = current.ChangedDate;
              long ticks1 = dateTime.Ticks;
              dateTime = this.MaxCreatedDateWatermark;
              long ticks2 = dateTime.Ticks;
              if (ticks1 > ticks2)
                this.MaxCreatedDateWatermark = current.ChangedDate;
            }
            if (!current.IsActive)
            {
              dateTime = current.ChangedDate;
              long ticks3 = dateTime.Ticks;
              dateTime = this.MaxRemovedDateWatermark;
              long ticks4 = dateTime.Ticks;
              if (ticks3 > ticks4)
                this.MaxRemovedDateWatermark = current.ChangedDate;
            }
            if (current.PassedPermissionCheck && linkService.TryGetLinkTypeByReferenceName(this.RequestContext, current.LinkType, out MDWorkItemLinkType _))
              this.m_workItemLinkChanges.Enqueue(current);
          }
        }
        else
          break;
      }
      this.m_workItemLinkChanges.IsComplete = flag;
    }

    public StreamingCollection<WorkItemLinkChange> WorkItemLinkChanges => this.m_workItemLinkChanges;

    public long MaxReadRowVersionUnfiltered => this.m_rawMaxReadRowVersion;

    public int TotalResultCountUnfiltered => this.m_rawResultCount;

    public DateTime MaxCreatedDateWatermark { get; set; }

    public DateTime MaxRemovedDateWatermark { get; set; }
  }
}
