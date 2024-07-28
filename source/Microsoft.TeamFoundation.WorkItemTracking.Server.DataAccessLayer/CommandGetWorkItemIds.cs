// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.CommandGetWorkItemIds
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class CommandGetWorkItemIds : Command
  {
    private const int MAX_CACHE_SIZE = 1048576;
    private StreamingCollection<WorkItemId> m_workItems;
    private DalSqlResourceComponent m_db;
    private WorkItemIdBinder m_itemBinder;
    private ResultCollection m_results;
    private int m_maxCacheSize;
    private bool m_fDestroyed;
    private int m_batchSize;
    private long m_offset;

    public CommandGetWorkItemIds(IVssRequestContext requestContext, bool destroyed)
      : base(requestContext)
    {
      this.m_maxCacheSize = 1048576;
      this.m_fDestroyed = destroyed;
      this.m_batchSize = requestContext.WitContext().ServerSettings.WorkItemSyncApiBatchSize;
      if (destroyed)
        return;
      this.m_offset = requestContext.WitContext().ServerSettings.WorkItemChangeWatermarkOffset;
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

    public void Execute(long rowVersion)
    {
      this.m_db = DalSqlResourceComponent.CreateComponent(this.RequestContext);
      this.m_db.GetWorkItemIds(rowVersion, this.m_fDestroyed, this.m_batchSize, this.m_offset, out this.m_results);
      this.m_itemBinder = (WorkItemIdBinder) this.m_results.GetCurrent<WorkItemId>();
      this.m_workItems = new StreamingCollection<WorkItemId>((Command) this);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      bool flag = false;
      while (!this.IsCacheFull)
      {
        flag = !this.m_itemBinder.MoveNext();
        if (!flag)
          this.m_workItems.Enqueue(this.m_itemBinder.Current);
        else
          break;
      }
      this.m_workItems.IsComplete = flag;
    }

    public StreamingCollection<WorkItemId> WorkItemIds => this.m_workItems;
  }
}
