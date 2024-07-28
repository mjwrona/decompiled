// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobQueueEntryColumns2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationJobQueueEntryColumns2 : TeamFoundationJobQueueEntryColumns
  {
    protected override TeamFoundationJobQueueEntry Bind()
    {
      if (this.PriorityColumn.IsNull((IDataReader) this.Reader))
        return (TeamFoundationJobQueueEntry) null;
      TeamFoundationJobResult foundationJobResult = TeamFoundationJobResult.Succeeded;
      try
      {
        if (!this.JobLastResultColumn.IsNull((IDataReader) this.Reader))
          foundationJobResult = (TeamFoundationJobResult) this.JobLastResultColumn.GetInt32((IDataReader) this.Reader);
      }
      catch (Exception ex)
      {
      }
      TeamFoundationJobQueueEntry foundationJobQueueEntry = new TeamFoundationJobQueueEntry();
      foundationJobQueueEntry.Priority = this.PriorityColumn.GetInt32((IDataReader) this.Reader);
      foundationJobQueueEntry.QueueTime = this.QueueTimeColumn.GetDateTime((IDataReader) this.Reader);
      foundationJobQueueEntry.NextRun = this.NextRunColumn.GetInt32((IDataReader) this.Reader);
      foundationJobQueueEntry.JobSource = this.JobSourceColumn.GetGuid((IDataReader) this.Reader);
      foundationJobQueueEntry.JobId = this.JobIdColumn.GetGuid((IDataReader) this.Reader);
      foundationJobQueueEntry.State = (TeamFoundationJobState) this.StateColumn.GetByte((IDataReader) this.Reader);
      foundationJobQueueEntry.AgentId = this.AgentColumn.GetGuid((IDataReader) this.Reader, true);
      foundationJobQueueEntry.ExecutionStartTime = this.StartTimeColumn.GetDateTime((IDataReader) this.Reader);
      foundationJobQueueEntry.StateChangeTime = this.StateChangeTimeColumn.GetDateTime((IDataReader) this.Reader);
      foundationJobQueueEntry.QueuedReasons = (TeamFoundationJobQueuedReasons) this.QueuedReasonsColumn.GetInt32((IDataReader) this.Reader);
      foundationJobQueueEntry.QueueFlagsValue = this.QueueFlagsColumn.GetInt32((IDataReader) this.Reader);
      foundationJobQueueEntry.JobLastResult = foundationJobResult;
      return foundationJobQueueEntry;
    }
  }
}
