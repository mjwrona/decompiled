// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent42
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent42 : TaskTrackingComponent41
  {
    public override async Task<TimelineRecordReference> GetTimelineRecordReferenceAsync(
      Guid scopeId,
      Guid planId,
      Guid timeLineRecordId)
    {
      TaskTrackingComponent42 component = this;
      TimelineRecordReference recordReferenceAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetTimelineRecordReferenceAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetTimelineRecordReferences");
        component.BindDataspaceId(scopeId);
        component.BindGuid("@planId", planId);
        component.BindGuid("@timeLineRecordId", timeLineRecordId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TimelineRecordReference>((ObjectBinder<TimelineRecordReference>) new TimelineRecordReferenceBinder());
          recordReferenceAsync = resultCollection.GetCurrent<TimelineRecordReference>().FirstOrDefault<TimelineRecordReference>();
        }
      }
      return recordReferenceAsync;
    }
  }
}
