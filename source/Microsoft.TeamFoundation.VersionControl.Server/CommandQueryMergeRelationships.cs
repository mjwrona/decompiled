// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryMergeRelationships
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryMergeRelationships : VersionControlCommand
  {
    private StreamingCollection<ItemIdentifier> m_mergeRelationships;
    private ResultCollection m_results;
    private VersionedItemComponent m_db;

    public CommandQueryMergeRelationships(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(string serverItem)
    {
      PathLength serverPathLength = this.m_versionControlRequestContext.MaxSupportedServerPathLength;
      this.m_versionControlRequestContext.Validation.checkServerItem(ref serverItem, nameof (serverItem), false, false, true, true, serverPathLength);
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add(nameof (serverItem), (object) serverItem);
      ClientTrace.Publish(this.RequestContext, "QueryMergeRelationships", ctData);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_mergeRelationships = new StreamingCollection<ItemIdentifier>((Command) this)
      {
        HandleExceptions = false
      };
      if (!this.RepositorySecurity.HasPermission(this.RequestContext, serverItem, 1, false))
      {
        this.m_mergeRelationships.IsComplete = true;
      }
      else
      {
        this.m_results = this.m_db.QueryMergeRelationships(serverItem);
        this.ContinueExecution();
        if (!this.IsCacheFull)
          return;
        this.RequestContext.PartialResultsReady();
      }
    }

    public override void ContinueExecution()
    {
      bool flag;
      while (flag = this.m_results.GetCurrent<ItemIdentifier>().MoveNext())
      {
        ItemIdentifier current = this.m_results.GetCurrent<ItemIdentifier>().Current;
        if (this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.ItemPathPair))
        {
          this.m_mergeRelationships.Enqueue(current);
          if (this.IsCacheFull)
            break;
        }
      }
      if (flag)
        return;
      this.m_mergeRelationships.IsComplete = true;
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

    public StreamingCollection<ItemIdentifier> MergeRelationships => this.m_mergeRelationships;
  }
}
