// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent18
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent18 : WorkItemComponent17
  {
    public override WorkItemComment GetWorkItemComment(int workItemId, int revision)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemComment");
      this.BindInt("@workItemId", workItemId);
      this.BindInt("@revision", revision);
      IDataReader reader = this.ExecuteReader();
      return this.GetWorkItemCommentBinder().BindAll(reader).FirstOrDefault<WorkItemComment>();
    }

    public override WorkItemComments GetWorkItemComments(
      int workItemId,
      int fromRevision,
      int count,
      CommentSortOrder sort)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemComments");
      this.BindInt("@workItemId", workItemId);
      this.BindInt("@fromRevision", fromRevision);
      this.BindInt("@count", count);
      this.BindBoolean("@sort", sort == CommentSortOrder.Asc);
      IDataReader reader = this.ExecuteReader();
      WorkItemComment[] array = this.GetWorkItemCommentBinder().BindAll(reader).ToArray<WorkItemComment>();
      reader.NextResult();
      reader.Read();
      int int32_1 = reader.GetInt32(0);
      int int32_2 = reader.GetInt32(1);
      int totalCount = int32_1;
      int fromRevisionCount = int32_2;
      return new WorkItemComments((IEnumerable<WorkItemComment>) array, totalCount, fromRevisionCount);
    }

    protected virtual WorkItemComponent.WorkItemCommentBinder GetWorkItemCommentBinder() => new WorkItemComponent.WorkItemCommentBinder();
  }
}
